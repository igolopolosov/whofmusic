using System;
using System.Globalization;
using System.Windows.Controls;
using Microsoft.Phone.Controls;

namespace WarehouseOfMusic
{
    public partial class ApplicationSettingsPage : PhoneApplicationPage
    {
        public ApplicationSettingsPage()
        {
            InitializeComponent();
            SetLanguageListboxSelectedIndex();
        }

        private void SetLanguageListboxSelectedIndex()
        {
            switch (CultureInfo.CurrentCulture.Name)
            {
                case "en-GB":
                    LanguageListBox.SelectedIndex = 0;
                    break;
                case "ru-RU":
                    LanguageListBox.SelectedIndex = 1;
                    break;
            }
        }

        public void LanguageListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string culture = "";
            switch (LanguageListBox.SelectedIndex)
            {
                case 0:
                    culture = "en-GB";
                    break;
                case 1:
                    culture = "ru-RU";
                    break;
            }

            // set app current culture to the culture associated with the selected locale
            if (culture == CultureInfo.CurrentCulture.Name) return;

            var newCulture = new CultureInfo(culture);
            CultureInfo.DefaultThreadCurrentCulture = newCulture;
            CultureInfo.DefaultThreadCurrentUICulture = newCulture;
            ApplySettingsButton.IsEnabled = true;
        }

        private void ApplySettingsButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }
    }
}