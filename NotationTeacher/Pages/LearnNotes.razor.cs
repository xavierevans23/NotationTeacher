using NotationTeacher.Shared;
using NotesAndScales;

namespace NotationTeacher.Pages;

public partial class LearnNotes : IDisposable
{
    private NoteData currentNote = new();
    private Note? inputNote = null;
    private DateTime timeRecieved = DateTime.Now;
    // The text telling the user if they got a note right or wrong.
    // Prepopulated feedback queue with invisible unicode.
    private readonly Queue<string> feedback = new(new[]{"⠀", "⠀", "⠀", "⠀", "⠀", "⠀", "⠀"});
    private readonly Random random = new();
    // Variables bound to inputs options.
    private bool useTimer = false;
    private string timerInputText = "2";
    private float TimerLength => float.TryParse(timerInputText, out float time) ? time : 2;
    private string cleffSelectionString = "both";
    public void GetNote(bool requireNew)
    {
        NoteData oldNote = currentNote;
        // If requireNew is on, it will try 5 times to give a different note.
        int attempts = 5;
        while (currentNote == oldNote && attempts > 0 && requireNew)
        {
            bool useTrebleCleff;
            if (cleffSelectionString == "treble")
            {
                useTrebleCleff = true;
            }
            else if (cleffSelectionString == "bass")
            {
                useTrebleCleff = false;
            }
            else
            {
                useTrebleCleff = random.Next(2) == 0;
            }

            currentNote = DataService.DataHolder.NoteLearningData.GetNextNote(useTrebleCleff);
            attempts--;
        }

        inputNote = null;
        timeRecieved = DateTime.Now;
    }

    public void GiveNote()
    {
        bool correct = inputNote is not null && inputNote.Number == currentNote.Note;
        bool inTime = (DateTime.Now - timeRecieved).TotalSeconds < TimerLength;
        if (correct)
        {
            if (inTime || !useTimer)
            {
                feedback.Enqueue($"{new Note(currentNote.Note).Name} ✔️");
            }
            else
            {
                feedback.Enqueue($"{new Note(currentNote.Note).Name} ⏱");
            }
        }
        else
        {
            feedback.Enqueue($"{new Note(currentNote.Note).Name} ❌ (You played {(inputNote is not null ? inputNote.Name : "null")})");
        }

        while (feedback.Count > 7)
        {
            feedback.Dequeue();
        }

        DataService.DataHolder.NoteLearningData.GiveResult(currentNote, correct && inTime);
        GetNote(correct);
        DataService.TrySaveData();
        InvokeAsync(StateHasChanged);
    }

    protected override void OnParametersSet()
    {
        NoteInputService.NoteUpdate += RecieveUpdate;
    }

    // Unsubscribe from event to avoid intercepting the notes before another component.
    public void Dispose()
    {
        NoteInputService.NoteUpdate -= RecieveUpdate;
    }

    public void RecieveUpdate(object? sender, EventArgs e)
    {
        if (NoteInputService.Notes.Count > 0)
        {
            inputNote = NoteInputService.Notes[^1];
            GiveNote();
            NoteInputService.Clear();
        }

        InvokeAsync(StateHasChanged);
    }

    private StaveDisplay? staveDisplay;
    // Anytime the screen refreshed, update the notes using javascript.
    protected override void OnAfterRender(bool firstRender)
    {
        staveDisplay?.SetNotes(new()
        {new Note(currentNote.Note).Name}, "q", 1, 4, "4/4", "C", currentNote.UseTrebleCleff ? "treble" : "bass");
    }

    // For the expandable data box.
    private bool dataExpanded = false;
    public void ToggleBox()
    {
        dataExpanded = !dataExpanded;
    }
}