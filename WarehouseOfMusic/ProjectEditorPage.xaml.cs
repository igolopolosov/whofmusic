//-----------------------------------------------------------------------
// <copyright file="ProjectEditorPage.xaml.cs" company="Igor Golopolosov">
//     Copyright (c) Igor Golopolosov. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace WarehouseOfMusic
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Shell;
    using Model;
    using Resources;

    /// <summary>
    /// Page of editing projects
    /// </summary>
    public partial class ProjectEditorPage : PhoneApplicationPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectEditorPage" /> class.
        /// </summary>
        public ProjectEditorPage()
        {
            this.InitializeComponent();
            this.DataContext = App.ViewModel;
            this.BuildLocalizedAppBar();
        }

        #region Track editing(add, delete)
        /// <summary>
        /// Adding new track
        /// </summary>
        /// <param name="sender">Some object</param>
        /// <param name="e">On click</param>
        private void AddTrackButton_Click(object sender, RoutedEventArgs e)
        {
            App.ViewModel.CurrentProject.AddTrack();
        }

        /// <summary>
        /// Deleting of chosen track
        /// </summary>
        /// <param name="sender">Some object</param>
        /// <param name="e">On click</param>
        private void DeleteTrackButton_OnClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            if (button != null)
            {
                var trackForDelete = button.DataContext as ToDoTrack;
                App.ViewModel.CurrentProject.DeleteTrack(trackForDelete);
            }

            this.Focus();
        } 
        #endregion

        #region For application bar
        /// <summary>
        /// Click on SettingsButton
        /// </summary>
        /// <param name="sender">Some object</param>
        /// <param name="e">Click on button</param>
        private void SettingsButton_OnClick(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/ApplicationSettingsPage.xaml", UriKind.Relative));
        }

        /// <summary>
        /// Bulid Localized application bar
        /// </summary>
        private void BuildLocalizedAppBar()
        {
            this.ApplicationBar = new ApplicationBar();

            //// Add button linked with help page
            var helpButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.question.png", UriKind.Relative))
            {
                Text = AppResources.AppBarHelp
            };
            this.ApplicationBar.Buttons.Add(helpButton);

            //// Add button linked with settings page
            var settingsButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.settings.png", UriKind.Relative))
            {
                Text = AppResources.AppBarSettings,
            };
            settingsButton.Click += this.SettingsButton_OnClick;
            this.ApplicationBar.Buttons.Add(settingsButton);
        }
        #endregion
    }
}