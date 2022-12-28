namespace NotationTeacher
{
    public class NoteData
    {
        public int Note { get; set; } = 48;

        public bool UseTrebleCleff { get; set; } = true;

        public int TotalAttempts { get; set; } = 0;
        public int TurnsElapsed { get; set; } = 1000;

        public int MaxAttempts { get; set; } = 5;
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
