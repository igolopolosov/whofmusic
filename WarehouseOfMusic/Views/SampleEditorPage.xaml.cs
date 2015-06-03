//-----------------------------------------------------------------------
// <copyright file="TrackEditorPage.xaml.cs">
//     Copyright (c) Igor Golopolosov. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using WarehouseOfMusic.Enums;
using WarehouseOfMusic.UIElementContexts;

namespace WarehouseOfMusic.Views
{
    using System;
    using System.Windows.Navigation;
    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Shell;
    using Coding4Fun.Toolkit.Controls;
    using Managers;
    using Resources;
    using ViewModels;
    using EventArgs;

    public partial class SampleEditorPage : PhoneApplicationPage
    {
        /// <summary>
        /// ViewModel for this page
        /// </summary>
        private SampleEditorContext _viewModel;

        /// <summary>
        /// Manager of track 
        /// </summary>
        private PlayerManager _playerManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectEditorPage" /> class.
        /// </summary>
        public SampleEditorPage()
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
            this._viewModel = new SampleEditorContext(App.DbConnectionString);
            this._viewModel.LoadData((int) IsoSettingsManager.LoadRecord("CurrentSampleId"));
            this.DataContext = this._viewModel;
            this.PianoRoll.ItemsSource = this._viewModel.Tacts;
            _playerManager = new PlayerManager(_viewModel.CurrentSample.TrackRef.ProjectRef.Tempo);
            _playerManager.StateChangedEvent +=_playerManager_StateChangeEvent;
        }
        #endregion

        #region For application bar

        /// <summary>
        /// Build Localized application bar
        /// </summary>
        private void BuildLocalizedAppBar()
        {
            this.ApplicationBar = new ApplicationBar();

            //// Add menu item linked with help page
            var helpMenuItem = new ApplicationBarMenuItem(AppResources.AppBarHelp);
            helpMenuItem.Click +=
                (sender, args) => NavigationService.Navigate(new Uri("/Views/HelpPage.xaml", UriKind.Relative));
            this.ApplicationBar.MenuItems.Add(helpMenuItem);

            //// Add play button for player
            var noteDurationButton =
                new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.music.select.png", UriKind.Relative))
                {
                    Text = AppResources.AppBarNoteDuration
                };
            noteDurationButton.Click += NoteDurationButton_OnClick;
            this.ApplicationBar.Buttons.Add(noteDurationButton);

            //// Add play button for player
            var playPauseButton =
                new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.control.play.png", UriKind.Relative))
                {
                    Text = AppResources.AppBarPlay
                };
            playPauseButton.Click += (sender, args) =>
            {
                var tact = this.PianoRoll.SelectedItem as PianoRollContext;
                if (tact == null) return;
                switch (_playerManager.State)
                {
                    case PlayerState.Stopped:
                        _playerManager.Play(_viewModel.CurrentSample.TrackRef, tact.TactNumber);
                        break;
                    case PlayerState.Playing:
                        _playerManager.Pause();
                        break;
                    case PlayerState.Paused:
                        _playerManager.Resume();
                        break;
                }
            };
            this.ApplicationBar.Buttons.Add(playPauseButton);

            //// Add stop button for player
            var stopButton =
                new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.control.stop.png", UriKind.Relative))
                {
                    Text = AppResources.AppBarStop
                };
            stopButton.Click += (sender, args) => _playerManager.Stop();
            ;
            this.ApplicationBar.Buttons.Add(stopButton);
        }

        /// <summary>
        /// Show dialog for chose duration of note
        /// </summary>
        private void NoteDurationButton_OnClick(object sender, EventArgs eventArgs)
        {
            var noteDurationPickDialog = new MessagePrompt()
            {
                Title = AppResources.MessageChoseNoteSize,
                Body = new NoteSizePicker()
            };
            noteDurationPickDialog.Completed += NoteDurationPickDialog_Completed;
            noteDurationPickDialog.Show();
        }

        private void NoteDurationPickDialog_Completed(object sender, PopUpEventArgs<string, PopUpResult> e)
        {
            if (e.PopUpResult != PopUpResult.Ok) return;
            var dialog = sender as MessagePrompt;
            if (dialog == null) return;
            var durationPicker = dialog.Body as NoteSizePicker;
            if (durationPicker != null) PianoRollContext.NoteDuration = durationPicker.Duration;
        }

        /// <summary>
        /// Dispatcher used to avoid threading exeptions
        /// </summary>
        private void _playerManager_StateChangeEvent(object sender, PlayerEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                var button = ApplicationBar.Buttons[1] as ApplicationBarIconButton;
                if (button == null) return;
                switch (e.State)
                {
                    case PlayerState.Stopped:
                    {
                        button.IconUri = new Uri("/Assets/AppBar/appbar.control.play.png", UriKind.Relative);
                        button.Text = AppResources.AppBarPlay;
                    }
                        break;
                    case PlayerState.Playing:
                    {
                        button.IconUri = new Uri("/Assets/AppBar/appbar.control.pause.png", UriKind.Relative);
                        button.Text = AppResources.AppBarPause;
                    }
                        break;
                    case PlayerState.Paused:
                    {
                        button.IconUri = new Uri("/Assets/AppBar/appbar.control.resume.png", UriKind.Relative);
                        button.Text = AppResources.AppBarResume;
                    }
                        break;
                }
            });
        }

        #endregion

        #region Pianoroll populating

        private void PianoRoll_OnLoadedPivotItem(object sender, PivotItemEventArgs e)
        {
            var pianoRollPage = e.Item.GetFirstLogicalChildByType<PianoRollPage>(true);
            if (pianoRollPage == null) return;
            pianoRollPage.Scroll();
        }

        private void PianoRoll_OnUnloadedPivotItem(object sender, PivotItemEventArgs e)
        {
            var pianoRollPage = e.Item.GetFirstLogicalChildByType<PianoRollPage>(true);
            if (pianoRollPage == null) return;
            pianoRollPage.SaveOffset();
        }

        #endregion


    }
}