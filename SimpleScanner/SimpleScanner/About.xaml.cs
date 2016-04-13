using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using SimpleScanner.Resources;
using System;
using System.Windows.Media;

namespace SimpleScanner
{
    public partial class About : PhoneApplicationPage
    {
        ApplicationBarIconButton backBtn = new ApplicationBarIconButton();

        public About()
        {
            InitializeComponent();
            BuildAppBar();
        }

        private void RateMe_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Microsoft.Phone.Tasks.MarketplaceReviewTask mk = new Microsoft.Phone.Tasks.MarketplaceReviewTask();
            mk.Show();
        }

        private void EmailMe_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Microsoft.Phone.Tasks.EmailComposeTask emailComposeTask = new Microsoft.Phone.Tasks.EmailComposeTask();
            emailComposeTask.To = "alexandrecz@live.com";
            emailComposeTask.Subject = "SimpleScanner - Feddback";
            emailComposeTask.Body = "Leave your comments here...";
            emailComposeTask.Show();
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
    }
}