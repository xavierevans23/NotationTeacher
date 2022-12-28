using NotesAndScales;

namespace NotationTeacher
{
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
