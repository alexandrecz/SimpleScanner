
using System.IO;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;
namespace SimpleScanner.ViewModels
{
    public class SettingsViewModel
    {
        public SettingsViewModel()
        {
            ISBR = false;
            LoadViewModelFromIsolatedStorage();
        }

        public bool ISBR { get; set; }

        public void LoadViewModelFromIsolatedStorage()
        {
            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (var stream =
                   new IsolatedStorageFileStream("SettingsModel.xml", FileMode.OpenOrCreate, FileAccess.Read, store))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        if (!reader.EndOfStream)
                        {
                            var serializer = new XmlSerializer(typeof(SettingsModel));
                            var mySettings = (SettingsModel)serializer.Deserialize(reader);
                            ISBR = mySettings.IsBR;
                        }
                    }
                }
            }
        }
    }
}
