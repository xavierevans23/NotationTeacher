using Blazored.LocalStorage;

namespace NotationTeacher
{
    public class DataService
    {
        public DataHolder DataHolder {get; private set;}

        private readonly ILocalStorageService localStorage;
        public bool LoadedFromStorage { get; private set; } = false;        

        public DataService(ILocalStorageService storage)
        {
            localStorage = storage;
            DataHolder = new();
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
                await localStorage.SetItemAsync("XmlData", DataHolder.ToXml());
            }
        }
    }
}
