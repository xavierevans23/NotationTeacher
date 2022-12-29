using NotesAndScales;

namespace NotationTeacher
{
    // Represents a set of notes for testing and parameters on how they are tested.
    public class NoteLearningData
    {
        public NoteData[] TrebleNotes { get; set; }
        public NoteData[] BassNotes { get; set; }

        // How many successful tests does a note need before it is considered good.
        public int GoodNoteThreshold { get; set; } = 5;

        // How many past attempts should each note store.
        public int MaxAttempts { get; set; } = 5;

        // How many different not good notes should be tested at one time (before waiting for the user to be better).
        public int BadNoteNumber { get; set; } = 5;

        // Out of 100, how often should a good note be tested.
        public int GoodNoteChance { get; set; } = 33;

        public NoteLearningData()
        {
            TrebleNotes = Array.Empty<NoteData>();
            BassNotes = Array.Empty<NoteData>();

            List<Note> notes = new();

            // All notes in the treble cleff, in order of importance.
            notes.Add(new("C Natural 4"));
            notes.Add(new("D Natural 4"));
            notes.Add(new("E Natural 4"));
            notes.Add(new("F Natural 4"));
            notes.Add(new("G Natural 4"));
            notes.Add(new("A Natural 4"));
            notes.Add(new("B Natural 4"));
            notes.Add(new("C Natural 5"));
            
            notes.Add(new("B Natural 3"));
            notes.Add(new("A Natural 3"));
            notes.Add(new("G Natural 3"));

            notes.Add(new("D Natural 5"));
            notes.Add(new("E Natural 5"));
            notes.Add(new("F Natural 5"));
            notes.Add(new("G Natural 5"));
            notes.Add(new("A Natural 5"));
            notes.Add(new("B Natural 5"));
            notes.Add(new("C Natural 6"));

            List<NoteData> noteData = new();
            foreach (Note note in notes)
            {
                noteData.Add(new() {Note = note.Number, UseTrebleCleff = true, MaxAttempts = MaxAttempts});
            }

            TrebleNotes = noteData.ToArray();

            notes = new();

            // All notes in the bass cleff, in order of importance.
            notes.Add(new("C Natural 4"));
            notes.Add(new("B Natural 3"));
            notes.Add(new("A Natural 3"));
            notes.Add(new("G Natural 3"));
            notes.Add(new("F Natural 3"));
            notes.Add(new("E Natural 3"));
            notes.Add(new("D Natural 3"));
            notes.Add(new("C Natural 3"));
            notes.Add(new("B Natural 2"));
            notes.Add(new("A Natural 2"));
            
            notes.Add(new("G Natural 2"));
            notes.Add(new("F Natural 2"));
            notes.Add(new("E Natural 2"));
            notes.Add(new("D Natural 2"));
            notes.Add(new("C Natural 2"));

            noteData = new();
            foreach (Note note in notes)
            {
                noteData.Add(new() { Note = note.Number, UseTrebleCleff = false, MaxAttempts = MaxAttempts });
            }

            BassNotes = noteData.ToArray();
        }

        private readonly Random random = new();

        // Gets the next note the user should be tested on.
        public NoteData GetNextNote(bool useTrebleCleff)
        {
            List<NoteData> notes = new();

            if (useTrebleCleff)
            {
                notes = TrebleNotes.ToList();
            }
            else
            {
                notes = BassNotes.ToList();
            }

            // Starts by making a list of the first x bad notes, and any good notes found along the way.
            List<NoteData> goodNotes = new();
            List<NoteData> badNotes = new();

            int badNoteCount = 0;
            int index = 0;

            while (badNoteCount < BadNoteNumber && index < notes.Count)
            {
                if (notes[index].CountAttempts() < GoodNoteThreshold)
                {
                    badNotes.Add(notes[index]);
                    badNoteCount++;
                }
                else
                {
                    goodNotes.Add(notes[index]);
                }
                index++;
            }

            bool useGoodNotes = random.Next(GoodNoteChance) <= GoodNoteThreshold;

            if (useGoodNotes && goodNotes.Count > 0)
            {
                // Good notes are in order of whichever was the least recently tested.
                goodNotes.Sort((a, b) => a.TurnsElapsed - b.TurnsElapsed);
                return goodNotes.First();
            }
            else if (badNotes.Count > 0)
            {
                // Bad notes are in any order.
                return badNotes[random.Next(badNotes.Count)];
            }
            else
            {
                Console.WriteLine("Couldn't find a note to return.");
                return new() { Note = 48, UseTrebleCleff = true, MaxAttempts = MaxAttempts };
            }
        }

        // Accepts result of if a user got a note right or not.
        public void GiveResult(NoteData noteData, bool correct)
        {
            foreach (NoteData data in TrebleNotes)
            {
                data.IncrementTurnsElapsed();
                if (data == noteData) 
                {
                    data.AddAttempt(correct);
                }
            }

            foreach (NoteData data in BassNotes)
            {
                data.IncrementTurnsElapsed();
                if (data == noteData)
                {
                    data.AddAttempt(correct);
                }
            }
        }
    }
}
