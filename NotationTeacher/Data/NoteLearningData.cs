using NotesAndScales;

namespace NotationTeacher
{
    // Represents a set of notes for testing and parameters on how they are tested.
    public class NoteLearningData
    {
        public NoteData[] TrebleNotes { get; set; }
        public NoteData[] BassNotes { get; set; }

        // How many successful tests does a note need before it is considered good.
        public int GoodNoteThreshold { get; set; } = 6;

        // How many past attempts should each note store.
        public int MaxAttempts { get; set; } = 8;

        // How many different not good notes should be tested at one time (before waiting for the user to be better).
        public int BadNoteNumber { get; set; } = 5;

        // Out of 100, how often should a good note be tested.
        public int GoodNoteChance { get; set; } = 33;

        public NoteLearningData()
        {
            TrebleNotes = Array.Empty<NoteData>();
            BassNotes = Array.Empty<NoteData>();

            List<Note> notes = new()
            {
                // All notes in the treble cleff, in order of importance.
               new("C Natural 4"),
               new("D Natural 4"),
               new("E Natural 4"),
               new("F Natural 4"),
               new("G Natural 4"),
               new("B Natural 3"),
               new("A Natural 3"),
               new("A Natural 4"),
               new("B Natural 4"),
               new("C Natural 5"),
               new("D Natural 5"),
               new("E Natural 5"),
               new("F Natural 5"),
               new("G Natural 5"),
               new("A Natural 5"),
               new("B Natural 5"),
               new("C Natural 6"),
               new("G Natural 3"),
               new("F Natural 3"),
               new("E Natural 3"),
               new("D Natural 3"),
               new("C Natural 3"),
               new("D Natural 6"),
               new("E Natural 6"),
               new("F Natural 6"),
               new("G Natural 6"),
               new("A Natural 6"),
               new("B Natural 6"),
               new("C Natural 7"),
               new("D Natural 7"),
               new("E Natural 7"),
               new("F Natural 7"),
               new("G Natural 7"),
               new("A Natural 7"),
               new("B Natural 7"),
               new("C Natural 8"),
            };


            List<NoteData> noteData = new();
            foreach (Note note in notes)
            {
                noteData.Add(new() { Note = note.Number, UseTrebleCleff = true, MaxAttempts = MaxAttempts });
            }

            TrebleNotes = noteData.ToArray();

            notes = new()
            {
                // All notes in the bass cleff, in order of importance.
                new("C Natural 4"),
                new("B Natural 3"),
                new("A Natural 3"),
                new("G Natural 3"),
                new("F Natural 3"),
                new("E Natural 3"),
                new("D Natural 3"),
                new("C Natural 3"),
                new("D Natural 4"),
                new("E Natural 4"),
                new("B Natural 2"),
                new("A Natural 2"),
                new("G Natural 2"),
                new("F Natural 2"),
                new("E Natural 2"),
                new("D Natural 2"),
                new("C Natural 2"),
                new("F Natural 4"),
                new("G Natural 4"),
                new("A Natural 4"),
                new("B Natural 4"),
                new("C Natural 5"),
                new("B Natural 1"),
                new("A Natural 1"),
                new("G Natural 1"),
                new("F Natural 1"),
                new("E Natural 1"),
                new("D Natural 1"),
                new("C Natural 1"),
                new("B Natural 0"),
                new("A Natural 0"),
            };

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
            List<NoteData> notes;

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

            bool useGoodNotes = random.Next(100) <= GoodNoteChance;

            if (useGoodNotes && goodNotes.Count > 0 || badNotes.Count == 0)
            {
                NoteData selected = goodNotes.First();

                foreach (NoteData note in goodNotes)
                {
                    if (note.TurnsElapsed > selected.TurnsElapsed)
                    {
                        selected = note;
                    }
                }

                return selected;
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

        public readonly record struct ChordInfo(List<NoteLabel> NotesNames, List<NoteLabel> NotePositions, Key Key);

        // Gets a chord
        public ChordInfo GetChord(bool useTrebleCleff)
        {
            var cleff = useTrebleCleff ? TrebleNotes : BassNotes;

            // Gets a list of the 'good' notes.
            var noteData = cleff.Where(n => n.CountAttempts() >= GoodNoteThreshold).Select(n => new Note(n.Note)).ToArray();

            // Gets the number of notes to use.
            var noteCount = random.Next(1, 4);

            // Uses a default if no 'good' notes were found.
            if (noteData.Length == 0)
            {
                return new(new() { new(Letter.C, Accidental.Natural, 4) }, new() { new(Letter.C, Accidental.Natural, 4) }, new(new(Letter.C, Accidental.Natural)));
            }

            // List of chosen notes (still in C Major).
            var notes = new List<Note>() { noteData[random.Next(noteData.Length)] };

            // Foreach of the notes that are gonna be added.
            for (int i = 0; i < noteCount; i++)
            {
                var possibleNotes = from n in noteData where !notes.Contains(n) && Math.Abs(n.Number - notes.First().Number) <= 7 select n;

                if (possibleNotes.Any())
                {
                    notes.Add(possibleNotes.ElementAt(random.Next(possibleNotes.Count())));
                }
            }

            // Get a random key by chosing a note between c 4 and c 5.
            var keyNote = new Note(random.Next(48, 60));
            var key = new Key(keyNote.Name.PitchClass);

            var signature = key.KeySignature;
            if (random.Next(0, 2) == 0 && key.AlternativeKeySignature is not null)
            {
                signature = key.AlternativeKeySignature;
            }

            // Get the notes that are sharp flat.
            var accidentals = key.KeySignature.Accidentals().Select(a => a.Letter);

            // Get if the key uses sharps or flats.
            var accidentalModifier = key.KeySignature.Accidental;

            // Two lists: one contains the notes as letters for the stave and the other contains actual note.
            var noteNames = new List<NoteLabel>();
            var notePositions = new List<NoteLabel>();

            foreach (var note in notes)
            {
                notePositions.Add(new NoteLabel(note.Name.PitchClass.Letter, Accidental.Natural, note.Octave));
                if (accidentals.Contains(note.Name.PitchClass.Letter))
                {
                    noteNames.Add(new NoteLabel(note.Name.PitchClass.Letter, accidentalModifier, note.Octave));
                }
                else
                {
                    noteNames.Add(note.Name);
                }
            }

            return new(noteNames, notePositions, key);
        }
    }
}
