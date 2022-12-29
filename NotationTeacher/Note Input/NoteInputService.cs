using NotesAndScales;

namespace NotationTeacher
{
    // Service used to provide app with data on which nots have been inputed.
    public class NoteInputService
    {
        private readonly List<Note> notes;

        public IReadOnlyList<Note> Notes
        {
            get
            {
                return notes;
            }
        }

        public NoteInputService()
        {
            notes = new();
        }

        public event EventHandler? NoteUpdate;

        // Called when a note is pressed. (Also called when a the midi configuration changes).
        public void OnNoteUpdate()
        {
            NoteUpdate?.Invoke(this, new());
        }

        public void NoteDown(Note note)
        {
            notes.Add(note);            
            OnNoteUpdate();
        }

        public void Clear()
        {
            notes.Clear();
        }
    }
}
