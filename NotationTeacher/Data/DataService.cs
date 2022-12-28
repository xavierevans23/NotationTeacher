namespace NotationTeacher
{
    public class DataService
    {
        public DataHolder DataHolder {get; private set;}

        public bool LoadedFromStorage { get; private set; } = false;        

        public DataService()
        {
            DataHolder = new();
        }
    }
}
