using Blazored.LocalStorage;

namespace NotationTeacher
{
    // Service which provides app with a copy of the data service.
    public class DataService
    {
        public DataHolder DataHolder {get; private set;}

        private readonly ILocalStorageService localStorage;
        public bool LoadedFromStorage { get; private set; } = false;

        // Uses Blazored ILocalStorageService service.
        public DataService(ILocalStorageService storage)
        {
            localStorage = storage;
            DataHolder = new();
        }

        public void ResetData()
        {
            LoadedFromStorage = false;
            DataHolder = new();
        }

        public void LoadXml(string xml)
        {
            LoadedFromStorage = false;
            try
            {
                DataHolder = DataHolder.FromXml(xml);                
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to load data from the xml given.");
            }
        }

        public async Task LoadData()
        {
            string data = await localStorage.GetItemAsync<string>("XmlData");
            try
            {
                DataHolder = DataHolder.FromXml(data);
                LoadedFromStorage = true;
            }
            catch (Exception)
            {
                LoadedFromStorage = false;
            }
        }

        public async Task SaveData()
        {
            if (DataHolder is not null)
            {
                DataHolder.TimeSaved = DateTime.Now;
                await localStorage.SetItemAsync("XmlData", DataHolder.ToXml());
                Console.WriteLine("Saved data successfully");
            }
        }

        private static DateTime lastSave = DateTime.Now;
        
        // Data will only save once every 10 seconds.
        public void TrySaveData()
        {
            if ((DateTime.Now - lastSave).TotalSeconds > 10)
            {
                lastSave = DateTime.Now;
                Task.Run(SaveData);
            }
        }
    }
}
