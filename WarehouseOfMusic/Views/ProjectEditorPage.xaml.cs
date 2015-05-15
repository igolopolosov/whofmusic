//-----------------------------------------------------------------------
// <copyright file="ProjectEditorPage.xaml.cs">
//     Copyright (c) Igor Golopolosov. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------


using WarehouseOfMusic.Enums;
using WarehouseOfMusic.EventArgs;

namespace WarehouseOfMusic.Views
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Navigation;
    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Shell;
    using Coding4Fun.Toolkit.Controls;
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
            this._viewModel.LoadProjectFromDatabase((int)IsoSettingsManager.LoadRecord("CurrentProjectId"));
            this.DataContext = this._viewModel;
            _playerManager = new PlayerManager(_viewModel.CurrentProject.Tempo);
            _playerManager.StateChangedEvent += _playerManager_StateChangeEvent;
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
        
        #region Rename dialog

        /// <summary>
        /// Show dialog to create or rename project
        /// </summary>
        /// <param name="trackId">-1 = for create project dialog, n - rename project dialog </param>
        private void ShowRenameDialog()
        {
            var trackName = _viewModel.OnRenameTrack.Name;
            var inputPrompt = new InputPromptOveride()
            {
                IsCancelVisible = true,
                IsSubmitOnEnterKey = false,
                Title = AppResources.RenameTrack,
                Value = trackName
            };
            inputPrompt.LostFocus += inputPrompt_LostFocus;
            inputPrompt.KeyUp += InputPrompt_KeyUp;
            inputPrompt.Completed += InputPromptOnCompleted;
            inputPrompt.Show();
        }

        /// <summary>
        /// Show or hide 'empty name' error message
        /// </summary>
        void inputPrompt_LostFocus(object sender, RoutedEventArgs e)
        {
            var input = sender as InputPrompt;
            if (input == null) return;
            input.Message = input.Value == string.Empty ? AppResources.ErrorEmptyName : string.Empty;
        }

        /// <summary>
        /// Detect the end of input text
        /// </summary>
        private void InputPrompt_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != System.Windows.Input.Key.Enter) return;
            var input = sender as InputPrompt;
            if (input == null) return;
            input.Focus();
        }

        private void InputPromptOnCompleted(object sender, PopUpEventArgs<string, PopUpResult> e)
        {
            var input = sender as InputPrompt;
            if (input == null) return;
            if (e.PopUpResult == PopUpResult.Ok) this._viewModel.RenameTrackTo(input.Value);
            this._viewModel.OnRenameTrack = null;
        }

        /// <summary>
        /// Show dialog to delete track
        /// </summary>
        private void ShowDeleteDialog()
        {
            var messagePrompt = new MessagePrompt()
            {
                IsCancelVisible = true,
                Message = AppResources.MessageDeleteTrack
            };
            messagePrompt.Completed += MessagePromptOnCompleted;
            messagePrompt.Show();
        }

        private void MessagePromptOnCompleted(object sender, PopUpEventArgs<string, PopUpResult> e)
        {
            if (e.PopUpResult == PopUpResult.Ok) this._viewModel.DeleteTrack();
            _viewModel.OnDeleteTrack = null;
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
            switch (_playerManager.State)
            {
                case PlayerState.Stopped: _playerManager.Play(_viewModel.CurrentProject);
                    break;
                case PlayerState.Playing: _playerManager.Pause();
                    break;
                case PlayerState.Paused: _playerManager.Resume();
                    break;
            }
        }

        /// <summary>
        /// Stop track
        /// </summary>
        /// <param name="sender">Page with projects</param>
        /// <param name="e">Click event</param>
        private void StopButton_OnClick(object sender, EventArgs e)
        {
            _playerManager.Stop();
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

        #region Manipulations with track
        /// <summary>
        /// Adding new track
        /// </summary>
        /// <param name="sender">Some object</param>
        /// <param name="e">On click</param>
        private void AddTrackButton_Click(object sender, EventArgs e)
        {
            this._viewModel.AddTrack();
        }

        /// <summary>
        /// Chose project for editing
        /// </summary>
        private void TrackListBoxItem_OnTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var grid = sender as TextBlock;
            if (grid == null) return;
            var chosenTrack = grid.DataContext as ToDoTrack;
            if (chosenTrack == null) return;
            IsoSettingsManager.SaveRecord("CurrentTrackId", chosenTrack.Id);
            NavigationService.Navigate(new Uri("/Views/TrackEditorPage.xaml", UriKind.Relative), chosenTrack.Id);
        }

        private void RenameProject_OnTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var contextMenuItem = sender as MenuItem;
            if (contextMenuItem == null) return;
            _viewModel.OnRenameTrack = contextMenuItem.DataContext as ToDoTrack;
            ShowRenameDialog();
        }

        private void DeleteProject_OnTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var contextMenuItem = sender as MenuItem;
            if (contextMenuItem == null) return;
            this._viewModel.OnDeleteTrack = contextMenuItem.DataContext as ToDoTrack;
            ShowDeleteDialog();
        }
        #endregion
    }
}