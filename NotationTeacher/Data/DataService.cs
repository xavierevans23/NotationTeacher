namespace NotationTeacher
{
    public class DataService
    {
        public DataHolder DataHolder {get; private set;}

        public DataService()
        {
            DataHolder = new();
        }
    }
}
