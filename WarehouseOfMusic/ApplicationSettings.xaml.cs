using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;

namespace WarehouseOfMusic
{
    public partial class ApplicationSettingsPage : PhoneApplicationPage
    {
        #region Initialize values on page
        public ApplicationSettingsPage()
        {
            InitializeComponent();
            SetCurrentSettings();
        }

        private void SetCurrentSettings()
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

            if ((Visibility)Application.Current.Resources["PhoneDarkThemeVisibility"] == Visibility.Visible)
                ThemeListBox.SelectedIndex = 0;
            else if ((Visibility)Application.Current.Resources["PhoneLightThemeVisibility"] == Visibility.Visible)
                ThemeListBox.SelectedIndex = 1;
            else ThemeListBox.SelectedIndex = 2;
            ApplySettingsButton.IsEnabled = false;
        } 
        #endregion

        #region Languages control

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

            var newCulture = new CultureInfo(culture);
            CultureInfo.DefaultThreadCurrentCulture = newCulture;
            CultureInfo.DefaultThreadCurrentUICulture = newCulture;
            ApplySettingsButton.IsEnabled = true;
        } 
        #endregion

        #region Themes control

        private void ThemeListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplySettingsButton.IsEnabled = true;
        }

        private void CheckValueThemeListBox()
        {
            switch (ThemeListBox.SelectedIndex)
            {
                case 0:
                    ThemeManager.ToDarkTheme();
                    break;
                case 1:
                    ThemeManager.ToLightTheme();
                    break;
                case 2:
                    ThemeManager.ToLightTheme();
                    break;
            }
        } 
        #endregion

        private void ApplySettingsButton_Click(object sender, RoutedEventArgs e)
        {
            CheckValueThemeListBox();
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }
    }
}