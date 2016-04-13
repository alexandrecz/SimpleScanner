using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using SimpleScanner.Resources;
using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Xml.Serialization;

namespace SimpleScanner
{
    public partial class Settings : PhoneApplicationPage
    {
        ApplicationBarIconButton backBtn = new ApplicationBarIconButton();

        internal SettingsModel MySettingsModel { get; set; }

        public Settings()
        {
            InitializeComponent();
            BuildAppBar();
            MySettingsModel = new SettingsModel();
            ReadSettings();
        }

        private void BuildAppBar()
        {
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Mode = ApplicationBarMode.Default;
            ApplicationBar.BackgroundColor = new SolidColorBrush((App.Current.Resources["PhoneAccentBrush"] as SolidColorBrush).Color).Color;
            ApplicationBar.IsVisible = true;
            ApplicationBar.IsMenuEnabled = false;

            backBtn.IconUri = new Uri("/Assets/back.png", UriKind.Relative);
            backBtn.Text = AppResources.AppBarBtnBack;

            ApplicationBar.Buttons.Add(backBtn);

            backBtn.Click += (s, e) =>
            {
                NavigationService.GoBack();
            };
        }


        private void UseBRPattern_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.MySettingsModel.IsBR = UseBRPattern.IsChecked.Value;
            ChangeBRPattern();
            SaveViewModelToIsolatedStorage();
        }

        private void ChangeBRPattern()
        {
            UseBRPattern.Content = (this.MySettingsModel.IsBR ? "brazilian bank payment pattern ON" : "brazilian bank payment pattern OFF");
            TextBR.Visibility = (this.MySettingsModel.IsBR ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed);
        }


        private void ReadSettings()
        {
            LoadViewModelFromIsolatedStorage();
            UseBRPattern.IsChecked = this.MySettingsModel.IsBR;

            ChangeBRPattern();
        }


        #region Isolated

        private void SaveViewModelToIsolatedStorage()
        {
            DeleteStorage();
            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (var stream =
                   new IsolatedStorageFileStream("SettingsModel.xml", FileMode.Create, FileAccess.Write, store))
                {
                    var serializer = new XmlSerializer(typeof(SettingsModel));
                    serializer.Serialize(stream, MySettingsModel);
                }
            }
        }

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
                            MySettingsModel = (SettingsModel)serializer.Deserialize(reader);
                        }
                    }
                }
            }
        }

        private void DeleteStorage()
        {
            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (store.FileExists("SettingsModel.xml"))
                {
                    store.DeleteFile("SettingsModel.xml");
                }

                using (var f = new StreamWriter(new IsolatedStorageFileStream("SettingsModel.xml", FileMode.CreateNew, store)))
                {
                    f.Flush();
                    f.Close();
                    if (store.FileExists("SettingsModel.xml"))
                    {
                        store.DeleteFile("SettingsModel.xml");
                    }
                }
            }
        }

        #endregion

    }


    public class SettingsModel
    {
        public SettingsModel()
        {
            IsBR = false;
        }


        public bool IsBR { get; set; }
    }
}