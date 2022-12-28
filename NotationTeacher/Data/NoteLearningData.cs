using NotesAndScales;

namespace NotationTeacher
{
    public class NoteLearningData
    {
        public NoteData[] TrebleNotes { get; set; }
        public NoteData[] BassNotes { get; set; }

        public int GoodNoteThreshold { get; set; } = 5;
        public int MaxAttempts { get; set; } = 5;
        public int BadNoteNumber { get; set; } = 5;
        public int GoodNoteChance { get; set; } = 40;

        public NoteLearningData()
        {
            TrebleNotes = Array.Empty<NoteData>();
            BassNotes = Array.Empty<NoteData>();

            List<Note> notes = new();

            notes.AddRange(KeySignature.GetMajorScaleNotes(new("C Natural 4")));
            notes.AddRange(KeySignature.GetMajorScaleNotes(new("C Natural 5")));

            notes.Sort((a, b) => Math.Abs(48 - a.Number) - Math.Abs(48 - b.Number));

            List<NoteData> noteData = new();
            foreach (Note note in notes)
            {
                noteData.Add(new() {Note = note.Number, UseTrebleCleff = true, MaxAttempts = MaxAttempts});
            }

            TrebleNotes = noteData.ToArray();

            notes = new();

            notes.AddRange(KeySignature.GetMajorScaleNotes(new("C Natural 2")));
            notes.AddRange(KeySignature.GetMajorScaleNotes(new("C Natural 3")));

            notes.Sort((a, b) => Math.Abs(48 - a.Number) - Math.Abs(48 - b.Number));

            noteData = new();
            foreach (Note note in notes)
            {
                noteData.Add(new() { Note = note.Number, UseTrebleCleff = false, MaxAttempts = MaxAttempts });
            }

            BassNotes = noteData.ToArray();
        }

        private readonly Random random = new();

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
                goodNotes.Sort((a, b) => a.TurnsElapsed - b.TurnsElapsed);
                return goodNotes.First();
            }
            else if (badNotes.Count > 0)
            {
                return badNotes[random.Next(badNotes.Count)];
            }
            else
            {
                Console.WriteLine("Couldn't find a note to return.");
                return new() { Note = 48, UseTrebleCleff = true, MaxAttempts = MaxAttempts };
            }
        }

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
