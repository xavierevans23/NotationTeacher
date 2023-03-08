using NotationTeacher.Shared;
using NotesAndScales;
using System.Text;

namespace NotationTeacher.Pages;

public partial class LearnStackNotes : IDisposable
{
    // Where the notes appear on the stave (accidentals are considered automatically)
    private List<NoteLabel> chordNotePositions = new() { new(Letter.C, Accidental.Natural, 4) };

    // The notes and their names within the scale (also can be converted to the actual notes)
    private List<NoteLabel> chordNoteNames = new() { new(Letter.C, Accidental.Natural, 4) };

    // The key of the chord
    private Key chordKey = new(new(Letter.C, Accidental.Natural));    
    
    private bool usingTrebleCleff = true;
    
    // The notes the user has entered.
    private readonly List<Note> inputNotes = new();
    
    private DateTime timeRecieved = DateTime.Now;
    
    // The text telling the user if they got a note right or wrong.
    // Prepopulated feedback queue with invisible unicode.
    private readonly Queue<string> feedback = new(new[] { "⠀", "⠀", "⠀", "⠀", "⠀", "⠀", "⠀" });
    private readonly Random random = new();
    
    // Variables bound to inputs options.
    private bool useTimer = false;
    private string timerInputText = "2";
    private float TimerLength => float.TryParse(timerInputText, out float time) ? time : 2;
    private string cleffSelectionString = "both";

    // Gets a new chord
    public void GetChord()
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

        usingTrebleCleff = useTrebleCleff;

        var chord = DataService.DataHolder.NoteLearningData.GetChord(useTrebleCleff);
        chordNotePositions = chord.NotePositions;
        chordNoteNames = chord.NotesNames;
        chordKey = chord.Key;
        
        inputNotes.Clear();
        timeRecieved = DateTime.Now;
    }

    public void GiveNote()
    {
        var correct = true;

        // Check if enough notes have been pressed.
        if (inputNotes.Count < chordNoteNames.Count)
        {
            // Check if any of the notes pressed down are wrong.
            foreach (var note in inputNotes)
            {
                if (!chordNoteNames.Contains(note.Name) && !(note.AlternativeName is { } a && chordNoteNames.Contains(a)))
                {
                    correct = false;
                }
            }

            // If no notes are wrong, the user can carry on.
            if (correct)
            {
                return;
            }
        }

        if (inputNotes.Count != chordNoteNames.Count)
        {
            correct = false;
        }

        // Create string of the correct notes.
        StringBuilder sb = new();
        for (int i = 0; i < chordNoteNames.Count; i++)
        {
            NoteLabel note = chordNoteNames[i];
            sb.Append(note);
            if (i != chordNoteNames.Count - 1)
            {
                sb.Append(", ");
            }
            if (!inputNotes.Contains(new(note)))
            {
                correct = false;
            }
        }
        var noteNames = sb.ToString();

        // Create string of user entered notes.
        sb = new();
        for (int i = 0; i < inputNotes.Count; i++)
        {
            Note? note = inputNotes[i];

            var name = note.AlternativeName is { } n ? (chordNoteNames.Contains(n) ? n : note.Name) : (note.Name);

            sb.Append(name);
            if (i != inputNotes.Count - 1)
            {
                sb.Append(", ");
            }
        }
        var inputNames = sb.ToString();

        bool inTime = (DateTime.Now - timeRecieved).TotalSeconds < TimerLength;
        if (correct)
        {
            if (inTime || !useTimer)
            {
                feedback.Enqueue($"{noteNames} ✔️");
            }
            else
            {
                feedback.Enqueue($"{noteNames} ⏱");
            }
        }
        else
        {
            feedback.Enqueue($"{noteNames} ❌ (You pressed {inputNames})");
        }

        while (feedback.Count > 7)
        {
            feedback.Dequeue();
        }

        if (correct) GetChord();

        DataService.TrySaveData();

        inputNotes.Clear();

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
            inputNotes.AddRange(NoteInputService.Notes.Where(n => !inputNotes.Contains(n)));
            GiveNote();
            NoteInputService.Clear();
        }

        InvokeAsync(StateHasChanged);
    }

    private StaveDisplay? staveDisplay;
    // Anytime the screen refreshed, update the notes using javascript.
    protected override void OnAfterRender(bool firstRender)
    {
        var signatureString = chordKey.KeySignature.First.Letter.ToString();
        var accidental = chordKey.KeySignature.First.Accidental;
        if (accidental == Accidental.Sharp)
        {
            signatureString += "#";
        }
        else if (accidental == Accidental.Flat)
        {
            signatureString += "b";
        }

        staveDisplay?.SetNotes(chordNotePositions, "q", 1, 4, "4/4", signatureString, usingTrebleCleff ? "treble" : "bass");
    }

    // For the expandable data box.
    private bool dataExpanded = false;
    public void ToggleBox()
    {
        dataExpanded = !dataExpanded;
    }
}