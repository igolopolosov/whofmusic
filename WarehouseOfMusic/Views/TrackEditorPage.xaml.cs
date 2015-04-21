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
    using Coding4Fun.Toolkit.Controls;
    using Managers;
    using Models;
    using Resources;
    using ViewModels;
    using UIElementContexts;

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
            this.PianoRoll.ItemsSource = this._trackEditorContext.Tacts;
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
            var playPauseButton =
                new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.control.play.png", UriKind.Relative))
                {
                    Text = AppResources.AppBarPlay,
                };
            playPauseButton.Click += this.PlayPauseButton_OnClick;
            this.ApplicationBar.Buttons.Add(playPauseButton);

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
        private void SettingsMenuItem_OnClick(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/ApplicationSettingsPage.xaml", UriKind.Relative));
        }

        /// <summary>
        /// Play track
        /// </summary>
        private void PlayPauseButton_OnClick(object sender, EventArgs e)
        {
            var button = sender as ApplicationBarIconButton;

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
            playPauseButton.IconUri = new Uri("/Assets/AppBar/appbar.control.play.png", UriKind.Relative);
            playPauseButton.Text = AppResources.AppBarPlay;
        }

        #endregion

        private void PianoRoll_OnLoadedPivotItem(object sender, PivotItemEventArgs e)
        {
            var list = e.Item.GetFirstLogicalChildByType<PianoRollPage>(true);
            if (list != null)
            {
                list.Scroll();
            }
        }

        private void PianoRoll_OnUnloadedPivotItem(object sender, PivotItemEventArgs e)
        {
            var list = e.Item.GetFirstLogicalChildByType<PianoRollPage>(true);
            if (list != null)
            {
                list.SaveOffset();
            }
        }

        private int PianoRollPage_OnAddedNote(object sender, NoteEventArgs e)
        {
            var tactContext = PianoRoll.SelectedItem as TactContext;
            if (tactContext == null) return 0;
            var note = new ToDoNote
            {
                Duration = TactContext.PianoRollContext.NoteDuration,
                MidiNumber = (byte)e.Key,
                Tact = tactContext.Number,
                TactPosition = e.TactPosition
            };
            _trackEditorContext.AddNote(note);
            return note.Id;
        }

        private int PianoRollPage_OnDeletedNote(object sender, NoteEventArgs e)
        {
            _trackEditorContext.DeleteNote(e.Id);
            return 0;
        }
    }
}