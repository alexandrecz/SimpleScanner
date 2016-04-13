using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using SimpleScanner.Resources;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Navigation;
using Windows.ApplicationModel.DataTransfer;
using Windows.Phone.Speech.VoiceCommands;

namespace SimpleScanner
{
    public partial class MainPage : PhoneApplicationPage
    {
        #region attributes

        ApplicationBarIconButton scanBtn = new ApplicationBarIconButton();
        ApplicationBarIconButton shareBtn = new ApplicationBarIconButton();
        ApplicationBarIconButton settBtn = new ApplicationBarIconButton();
        ApplicationBarIconButton aboutBtn = new ApplicationBarIconButton();
        private DataTransferManager dataTransferManager;

        #endregion

        #region constructors

        public MainPage()
        {
            InitializeComponent();
            BuildAppBar();
        }

        #endregion

        #region methods

        private void BuildAppBar()
        {
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Mode = ApplicationBarMode.Default;
            ApplicationBar.BackgroundColor = new SolidColorBrush((App.Current.Resources["PhoneAccentBrush"] as SolidColorBrush).Color).Color;
            ApplicationBar.IsVisible = true;
            ApplicationBar.IsMenuEnabled = true;

            scanBtn.IconUri = new Uri("/Assets/scanner.png", UriKind.Relative);
            scanBtn.Text = AppResources.AppBarBtnStart;

            shareBtn.IconUri = new Uri("/Assets/share.png", UriKind.Relative);
            shareBtn.Text = AppResources.AppBarBtnShare;
            shareBtn.IsEnabled = !string.IsNullOrEmpty(ResultText.Text);

            settBtn.IconUri = new Uri("/Assets/settings.png", UriKind.Relative);
            settBtn.Text = "settings";

            aboutBtn.IconUri = new Uri("/Assets/questionmark.png", UriKind.Relative);
            aboutBtn.Text = AppResources.AppBarBtnAbout;

            ApplicationBarMenuItem refBtn = new ApplicationBarMenuItem();
            refBtn.Text = "refresh";
            ApplicationBar.MenuItems.Add(refBtn);

            ApplicationBar.Buttons.Add(scanBtn);
            ApplicationBar.Buttons.Add(shareBtn);
            ApplicationBar.Buttons.Add(settBtn);
            ApplicationBar.Buttons.Add(aboutBtn);

            scanBtn.Click += (s, e) =>
            {
                NavigationService.Navigate(new Uri("/Capture.xaml", UriKind.Relative));
            };

            shareBtn.Click += (s, e) =>
            {
                DataTransferManager.ShowShareUI();
            };

            settBtn.Click += (s, e) =>
            {
                NavigationService.Navigate(new Uri("/Settings.xaml", UriKind.Relative));
            };

            aboutBtn.Click += (s, e) =>
            {
                NavigationService.Navigate(new Uri("/About.xaml", UriKind.Relative));
            };

            refBtn.Click += (s, e) =>
            {
                Clear();
            };
        }

        private void Clear()
        {
            NoticeText.Visibility = Visibility.Visible;
            ResultText.Visibility = Visibility.Collapsed;
            ITFCon.Visibility = Visibility.Collapsed;
            ITFBank.Visibility = Visibility.Collapsed;
            SimpleScanner.Capture.Code = string.Empty;
        }

        private async void InstallVoiceCommands()
        {
            const string wp81vcdPath = "ms-appx:///VoiceCommandDefinition.xml";

            try
            {
                bool using81orAbove = ((Environment.OSVersion.Version.Major >= 8)
                    && (Environment.OSVersion.Version.Minor >= 10));

                Uri vcdUri = new Uri(wp81vcdPath);

                await VoiceCommandService.InstallCommandSetsFromFileAsync(vcdUri);
            }
            catch (Exception vcdEx)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show(String.Format(
                        AppResources.VoiceCommandInstallErrorTemplate, vcdEx.HResult, vcdEx.Message));
                });
            }
        }

        private void Read()
        {
            string value = ResultText.Text;

            string primeiroNumero = value.Substring(0, 1);
            string dvGeral = string.Empty;

            string campo1 = string.Empty;
            string campo2 = string.Empty;
            string campo3 = string.Empty;
            string campo4 = string.Empty;
            string campo5 = string.Empty;
            string dv1 = string.Empty;
            string dv2 = string.Empty;
            string dv3 = string.Empty;
            string dv4 = string.Empty;
            string codigoBarrasDigitavel = string.Empty;
            string dv = string.Empty;


            if (primeiroNumero == "8")
            {
                //tipoCodigoBarras = TipoProduto.Concessionaria;
                dvGeral = value.Substring(3, 1);

                campo1 = value.Substring(0, 11);
                campo2 = value.Substring(11, 11);
                campo3 = value.Substring(22, 11);
                campo4 = value.Substring(33, 11);
                dv1 = Mod_dig10(campo1);
                dv2 = Mod_dig10(campo2);
                dv3 = Mod_dig10(campo3);
                dv4 = Mod_dig10(campo4);
                codigoBarrasDigitavel = string.Concat(campo1, dv1, campo2, dv2, campo3, dv3, campo4, dv4);
                dv = Mod_dig10(value.Remove(3, 1));

                ResultText.Text = codigoBarrasDigitavel;
                ResultText.Text += dv;

                //{0-11 } {12-23} {24 35} {36 47}
                ResultText.Visibility = Visibility.Collapsed;
                NoticeText.Visibility = Visibility.Collapsed;
                ITFCon.Visibility = Visibility.Visible;
                First.Text = ResultText.Text.Substring(0, 12);
                Second.Text = ResultText.Text.Substring(12, 12);
                Third.Text = ResultText.Text.Substring(24, 12);
                Fourth.Text = ResultText.Text.Substring(36, 12);

            }
            else
            {
                dvGeral = value.Substring(4, 1);

                campo1 = string.Concat(value.Substring(0, 4), value.Substring(19, 5));
                campo2 = value.Substring(24, 10);
                campo3 = value.Substring(34, 10);
                campo4 = value.Substring(4, 1);
                campo5 = value.Substring(5, 14);
                dv1 = Mod_dig10(campo1);
                dv2 = Mod_dig10(campo2);
                dv3 = Mod_dig10(campo3);
                dv4 = Mod_dig10(campo4);
                codigoBarrasDigitavel = string.Concat(campo1, dv1, campo2, dv2, campo3, dv3, campo5);
                dv = Mod_dig11(value.Remove(4, 1));



                ResultText.Text = codigoBarrasDigitavel;

                //To do 8 textboxes passarcampos
                //{0-5 } {6-5} {11 5} {16 6} {22 5} {27 6} {32 1}{34 14}
                ResultText.Visibility = Visibility.Collapsed;
                NoticeText.Visibility = Visibility.Collapsed;
                ITFBank.Visibility = Visibility.Visible;
                BFirst.Text = ResultText.Text.Substring(0, 5);
                BSecond.Text = ResultText.Text.Substring(5, 5);
                BThird.Text = ResultText.Text.Substring(10, 5);
                BFourth.Text = ResultText.Text.Substring(15, 6);
                BFifth.Text = ResultText.Text.Substring(21, 5);
                BSixth.Text = ResultText.Text.Substring(26, 6);
                BSeventh.Text = dv;
                BEight.Text = ResultText.Text.Substring(32, 14);

            }
        }

        public string Mod_dig10(string codigo)
        {

            int soma = 0, mult = 2, indice, prod;
            for (indice = codigo.Length - 1; indice >= 0; indice--)
            {
                prod = (Convert.ToByte(codigo[indice]) - 48) * mult;

                if (prod > 9) soma += prod - 9;
                else soma += prod;
                if (mult == 2) mult = 1;
                else mult = 2;
            }


            soma = soma % 10;

            if (soma > 0) soma = 10 - soma;

            return soma.ToString();

        }

        public string Mod_dig11(string codigo)
        {
            string dv = "0";
            int soma = 0, mult = 2, indice;

            for (indice = codigo.Length - 1; indice >= 0; indice--)
            {
                soma += (Convert.ToByte(codigo[indice]) - 48) * mult;

                if (mult == 9) mult = 2;
                else mult++;

            }
            soma = soma * 10;
            soma = soma % 11;

            if ((soma > 9) || (soma < 2)) dv = "1";

            else dv = soma.ToString();
            return dv;
        }

        #endregion

        #region events

        private void DataRequested(DataTransferManager sender, DataRequestedEventArgs e)
        {
            DataRequest request = e.Request;
            request.Data.Properties.Title = "Simple Scanner";
            request.Data.Properties.Description = "This is the code I've just spotted";
            request.Data.SetText(ResultText.Text);
        }


        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            #region camera

            if (!string.IsNullOrEmpty(SimpleScanner.Capture.Code))
            {
                ResultText.Text = SimpleScanner.Capture.Code;
                ResultText.Visibility = !string.IsNullOrEmpty(ResultText.Text) ? Visibility.Visible : Visibility.Collapsed;
                NoticeText.Visibility = string.IsNullOrEmpty(ResultText.Text) ? Visibility.Visible : Visibility.Collapsed;
                ITFCon.Visibility = Visibility.Collapsed;
                ITFBank.Visibility = Visibility.Collapsed;
                shareBtn.IsEnabled = !string.IsNullOrEmpty(ResultText.Text);

                if (App.SettViewModel.ISBR)
                {
                    Read();
                }
            }

            #endregion

            #region dataShare

            dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += DataRequested;

            #endregion

            #region Voice

            if (e.NavigationMode == NavigationMode.New)
            {
                string voiceCommandName;

                if (NavigationContext.QueryString.TryGetValue("voiceCommandName", out voiceCommandName))
                {
                    if (NavigationContext.QueryString["voiceCommandName"].ToString() == "Start" | NavigationContext.QueryString["voiceCommandName"].ToString() == "Dec2")
                    {
                        NavigationService.Navigate(new Uri("/Capture.xaml", UriKind.Relative));
                    }
                }
                else
                {
                    Task.Run(() => InstallVoiceCommands());
                }
            }

            #endregion
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            dataTransferManager.DataRequested -= DataRequested;
        }

        #endregion

    }
}