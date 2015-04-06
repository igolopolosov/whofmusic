//-----------------------------------------------------------------------
// <copyright file="TrackEditorPage.xaml.cs">
//     Copyright (c) Igor Golopolosov. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace WarehouseOfMusic.Views
{
    using System;
    using System.Windows.Navigation;
    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Shell;
    using Managers;
    using Resources;
    using ViewModels;


    public partial class TrackEditorPage : PhoneApplicationPage
    {
        /// <summary>
        /// ViewModel for this page
        /// </summary>
        private TrackEditorContext _trackEditorContext;

        /// <summary>
        /// Manager of track 
        /// </summary>
        private PlayerManager _playerManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectEditorPage" /> class.
        /// </summary>
        public TrackEditorPage()
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
            this._trackEditorContext = new TrackEditorContext(App.DbConnectionString);
            this._trackEditorContext.LoadTrackFromDatabase((int) IsoSettingsManager.GetCurrentTrackId());
            this.DataContext = this._trackEditorContext;
        }

        #endregion

        #region For application bar

        /// <summary>
        /// Build Localized application bar
        /// </summary>
        private void BuildLocalizedAppBar()
        {
            this.ApplicationBar = new ApplicationBar();

            //// Add button linked with help page
            var helpButton =
                new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.question.png", UriKind.Relative))
                {
                    Text = AppResources.AppBarHelp
                };
            this.ApplicationBar.Buttons.Add(helpButton);

            //// Add button linked with settings page
            var settingsButton =
                new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.settings.png", UriKind.Relative))
                {
                    Text = AppResources.AppBarSettings,
                };
            settingsButton.Click += this.SettingsButton_OnClick;
            this.ApplicationBar.Buttons.Add(settingsButton);

            //// Add play button for player
            var playButton =
                new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.control.play.png", UriKind.Relative))
                {
                    Text = AppResources.AppBarPlay,
                };
            playButton.Click += this.PlayButton_OnClick;
            this.ApplicationBar.Buttons.Add(playButton);

            //// Add stop button for player
            var stopButton =
                new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.control.stop.png", UriKind.Relative))
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
        private void SettingsButton_OnClick(object sender, EventArgs e)
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
            this._playerManager = new PlayerManager(this._trackEditorContext.CurrentTrack);
            this._playerManager.Play();
        }

        /// <summary>
        /// Stop track
        /// </summary>
        /// <param name="sender">Page with projects</param>
        /// <param name="e">Click event</param>
        private void StopButton_OnClick(object sender, EventArgs e)
        {
            if (this._playerManager != null)
            {
                this._playerManager.Stop();
            }

            this._playerManager = null;
        }

        #endregion
    }
}