namespace NotationTeacher
{
    // Represents a note that the user can be tested on.
    public class NoteData
    {
        // Uses note number system in NotesAndScales library.
        public int Note { get; set; } = 48;

        public bool UseTrebleCleff { get; set; } = true;

        // How many times has this note been tested ever.
        public int TotalAttempts { get; set; } = 0;

        // How long has it been since this note was last tested.
        public int TurnsElapsed { get; set; } = 1000;

        // How many attempts are saved. (e.g. last 5, last 10).
        public int MaxAttempts { get; set; } = 8;

        // The last x attempts (true = correct). Stored as a list not a queue for serialization purposes.
        public List<bool> AttemptHistory { get; set; } = new();

        public int CountAttempts()
        {
            int total = 0;
            foreach (bool attempt in AttemptHistory)
            {
                if (attempt)
                {
                    total++;
                }
            }
            return total;
        }

        public void AddAttempt(bool correct)
        {
            while (AttemptHistory.Count >= MaxAttempts)
            {
                AttemptHistory.RemoveAt(0);
            }

            AttemptHistory.Add(correct);
            TotalAttempts++;
            TurnsElapsed = 0;
        }

        public void IncrementTurnsElapsed()
        {
            TurnsElapsed++;
        }
    }
}
