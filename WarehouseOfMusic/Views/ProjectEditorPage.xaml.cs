//-----------------------------------------------------------------------
// <copyright file="ProjectEditorPage.xaml.cs">
//     Copyright (c) Igor Golopolosov. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using WomAudioComponent;

namespace WarehouseOfMusic.Views
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Navigation;
    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Shell;
    using Managers;
    using Models;
    using Resources;
    using ViewModels;
    
    /// <summary>
    /// Page of editing projects
    /// </summary>
    public partial class ProjectEditorPage : PhoneApplicationPage
    {
        /// <summary>
        /// ViewModel for this page
        /// </summary>
        private ProjectEditorViewModel _viewModel;

        /// <summary>
        /// Manager of track 
        /// </summary>
        private PlayerManager _playerManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectEditorPage" /> class.
        /// </summary>
        public ProjectEditorPage()
        {
            this.InitializeComponent();
            this.BuildLocalizedAppBar();
        }

        #region Navigation control
        /// <summary>
        /// Called when the page is activated
        /// </summary>
        /// <param name="e">Navigation event</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.InitialiazeDataContext();
        }

        /// <summary>
        /// Loads new state of ViewModel
        /// </summary>
        private void InitialiazeDataContext()
        {
            this._viewModel = new ProjectEditorViewModel(App.DbConnectionString);
            this._viewModel.LoadProjectFromDatabase((int)IsoSettingsManager.GetCurrentProjectId());
            this.DataContext = this._viewModel;
        }

        /// <summary>
        /// Called when the page is deactivated
        /// </summary>
        /// <param name="e">Navigation event</param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _viewModel.SaveChangesToDb();
        }
        #endregion

        #region Track editing(add, delete)
        /// <summary>
        /// Adding new track
        /// </summary>
        /// <param name="sender">Some object</param>
        /// <param name="e">On click</param>
        private void AddTrackButton_Click(object sender, EventArgs e)
        {
            this._viewModel.AddTrack();
        }
        #endregion

        #region For application bar
        /// <summary>
        /// Build Localized application bar
        /// </summary>
        private void BuildLocalizedAppBar()
        {
            this.ApplicationBar = new ApplicationBar();

            //// Add menu item linked with settings page
            var settingsMenuItem = new ApplicationBarMenuItem(AppResources.AppBarSettings);
            settingsMenuItem.Click += this.SettingsMenuItem_OnClick;
            this.ApplicationBar.MenuItems.Add(settingsMenuItem);

            //// Add menu item linked with help page
            var helpMenuItem = new ApplicationBarMenuItem(AppResources.AppBarHelp);
            this.ApplicationBar.MenuItems.Add(helpMenuItem);

            //// Add play button for player
            var addButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.png", UriKind.Relative))
            {
                Text = AppResources.AppBarAddTrack,
            };
            addButton.Click += this.AddTrackButton_Click;
            this.ApplicationBar.Buttons.Add(addButton);

            //// Add play button for player
            var playButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.control.play.png", UriKind.Relative))
            {
                Text = AppResources.AppBarPlay,
            };
            playButton.Click += this.PlayButton_OnClick;
            this.ApplicationBar.Buttons.Add(playButton);

            //// Add stop button for player
            var stopButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.control.stop.png", UriKind.Relative))
            {
                Text = AppResources.AppBarStop,
            };
            stopButton.Click += this.StopButton_OnClick;
            this.ApplicationBar.Buttons.Add(stopButton);
        }

        /// <summary>
        /// Click on SettingsButton
        /// </summary>
        /// <param name="sender">Some object</param>
        /// <param name="e">Click on button</param>
        private void SettingsMenuItem_OnClick(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/ApplicationSettingsPage.xaml", UriKind.Relative));
        }

        /// <summary>
        /// Play track
        /// </summary>
        /// <param name="sender">Page with projects</param>
        /// <param name="e">Click event</param>
        private void PlayButton_OnClick(object sender, EventArgs e)
        {
            var button = sender as ApplicationBarIconButton;
            if (button == null) return;
            if (button.Text == AppResources.AppBarPlay || button.Text == AppResources.AppBarResume)
            {
                button.IconUri = new Uri("/Assets/AppBar/appbar.control.pause.png", UriKind.Relative);
                button.Text = AppResources.AppBarPause;
            }
            else
            {
                button.IconUri = new Uri("/Assets/AppBar/appbar.control.resume.png", UriKind.Relative);
                button.Text = AppResources.AppBarResume;
            }
        }

        /// <summary>
        /// Stop track
        /// </summary>
        /// <param name="sender">Page with projects</param>
        /// <param name="e">Click event</param>
        private void StopButton_OnClick(object sender, EventArgs e)
        {
            var playPauseButton = ApplicationBar.Buttons[0] as ApplicationBarIconButton;
            if (playPauseButton == null) return;
            playPauseButton.IconUri = new Uri("/Assets/AppBar/appbar.control.play.png", UriKind.Relative);
            playPauseButton.Text = AppResources.AppBarPlay;
        }
        #endregion

        /// <summary>
        /// Chose project for editing.
        /// </summary>
        /// <param name="sender">Project item displayed like a list box item</param>
        /// <param name="e">One tap</param>
        private void TrackListBoxItem_OnTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var grid = sender as Grid;
            if (grid == null) return;
            var chosenTrack = grid.DataContext as ToDoTrack;
            if (chosenTrack == null) return;
            IsoSettingsManager.SetCurrentTrack(chosenTrack.Id);
            NavigationService.Navigate(new Uri("/Views/SampleEditorPage.xaml", UriKind.Relative), chosenTrack.Id);
        }

        #region Track Mode
        private void SoloCheckBox_OnChecked(object sender, RoutedEventArgs e)
        {
            var checkBox = e.OriginalSource as CheckBox;
            if (checkBox == null) return;
            var track = checkBox.DataContext as ToDoTrack;
            if (track != null) track.Mute = false;
        }

        private void MuteCheckBox_OnChecked(object sender, RoutedEventArgs e)
        {
            var checkBox = e.OriginalSource as CheckBox;
            if (checkBox == null) return;
            var track = checkBox.DataContext as ToDoTrack;
            if (track != null) track.Solo = false;
        }
        #endregion
    }
}