//-----------------------------------------------------------------------
// <copyright file="ApplicationSettingsPage.xaml.cs">
//     Copyright (c) Igor Golopolosov. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace WarehouseOfMusic.Views
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using Microsoft.Phone.Controls;
    
    /// <summary>
    /// Page with settings of application
    /// </summary>
    public partial class ApplicationSettingsPage : PhoneApplicationPage
    {
        #region Initialize values on page
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationSettingsPage" /> class.
        /// </summary>
        public ApplicationSettingsPage()
        {
            this.InitializeComponent();
            this.SetCurrentSettings();
        }

        /// <summary>
        /// Set phone settings at page
        /// </summary>
        private void SetCurrentSettings()
        {
            ////Set language
            switch (CultureInfo.CurrentCulture.Name)
            {
                case "en-GB":
                    LanguageListBox.SelectedIndex = 0;
                    break;
                case "ru-RU":
                    LanguageListBox.SelectedIndex = 1;
                    break;
            }

            ////Set theme
            if ((Visibility)Application.Current.Resources["PhoneDarkThemeVisibility"] == Visibility.Visible)
            {
                ThemeListBox.SelectedIndex = 0;
            }
            else
            {
                ThemeListBox.SelectedIndex = (Visibility)Application.Current.Resources["PhoneLightThemeVisibility"] == Visibility.Visible ? 1 : 2;
            }

            ApplySettingsButton.IsEnabled = false;
        } 
        #endregion

        #region Languages control
        /// <summary>
        /// Detect changes of list box with languages
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Change event</param>
        private void LanguageListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var culture = string.Empty;
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
        /// <summary>
        /// Detect changes of list box with themes
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Change event</param>
        private void ThemeListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplySettingsButton.IsEnabled = true;
        }

        /// <summary>
        /// Set selection of an item at the ThemeListBox
        /// </summary>
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

        /// <summary>
        /// Apply settings and navigate to MainPage
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Click event</param>
        private void ApplySettingsButton_Click(object sender, RoutedEventArgs e)
        {
            this.CheckValueThemeListBox();
            if (NavigationService.CanGoBack) NavigationService.GoBack();
        }
    }
}