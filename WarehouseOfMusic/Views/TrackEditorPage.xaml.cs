namespace WarehouseOfMusic.Views
{
    using System;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Navigation;
    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Shell;
    using Coding4Fun.Toolkit.Controls;
    using Enums;
    using EventArgs;
    using Managers;
    using Models;
    using Resources;
    using ViewModels;
    using GestureEventArgs = System.Windows.Input.GestureEventArgs;

    public partial class TrackEditorPage : PhoneApplicationPage
    {
        /// <summary>
        /// ViewModel for this page
        /// </summary>
        private TrackEditorContext _viewModel;

        /// <summary>
        /// Manager of track 
        /// </summary>
        private PlayerManager _playerManager;
        
        public TrackEditorPage()
        {
            InitializeComponent();
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
            this._viewModel = new TrackEditorContext(App.DbConnectionString);
            this._viewModel.LoadData((int)IsoSettingsManager.LoadRecord("CurrentTrackId"));
            this.DataContext = this._viewModel;
            this._viewModel.CurrentTrack.Samples.CollectionChanged +=Samples_CollectionChanged;
            _playerManager = new PlayerManager(_viewModel.CurrentTrack.ProjectRef.Tempo);
            _playerManager.StateChangedEvent += PlayerManager_OnStateChangeEvent;
            _playerManager.TactChangedEvent += PlayerManager_OnTactChangedEvent;
        }

        private void Samples_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                var movedSample = e.OldItems[0] as ToDoSample;
                if (movedSample == null) return;
                foreach (var sample in _viewModel.CurrentTrack.Samples.Where(x => x.InitialTact > movedSample.InitialTact))
                {
                    sample.InitialTact = sample.InitialTact - movedSample.Size;
                }
            }
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var movedSample = e.NewItems[0] as ToDoSample;
                var newInitialtact = e.NewStartingIndex == 0
                    ? 1
                    : _viewModel.CurrentTrack.Samples[e.NewStartingIndex - 1].InitialTact + _viewModel.CurrentTrack.Samples[e.NewStartingIndex - 1].Size;
                if (movedSample == null) return;
                foreach (var sample in _viewModel.CurrentTrack.Samples.Where(x => x.InitialTact >= newInitialtact))
                {
                    sample.InitialTact = sample.InitialTact + movedSample.Size;
                }
                movedSample.InitialTact = newInitialtact;
                _viewModel.RestoreReferences(movedSample);
                _viewModel.SaveChangesToDb();
            }
        }

        private void PlayerManager_OnTactChangedEvent(object sender, PlayerEventArgs e)
        {
            this._viewModel.ChangeSamplesState(e.PlaybleTact);
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

        #region Rename and delete sample dialogs

        /// <summary>
        /// Show dialog to create or rename project
        /// </summary>
        private void ShowRenameSampleDialog()
        {
            var sampleName = _viewModel.OnRenameSample.Name;
            var renameSampleDialog = new InputPromptOveride()
            {
                IsSubmitOnEnterKey = false,
                Title = AppResources.RenameSample,
                Value = sampleName
            };
            renameSampleDialog.LostFocus += RenameSampleDialog_OnLostFocus;
            renameSampleDialog.KeyUp += RenameSampleDialog_OnKeyUp;
            renameSampleDialog.Completed += RenameSampleDialog_OnCompleted;
            renameSampleDialog.Show();
        }

        /// <summary>
        /// Show or hide 'empty name' error message
        /// </summary>
        void RenameSampleDialog_OnLostFocus(object sender, RoutedEventArgs e)
        {
            var input = sender as InputPrompt;
            if (input == null) return;
            input.Message = input.Value == string.Empty ? AppResources.ErrorEmptyName : string.Empty;
        }

        /// <summary>
        /// Detect the end of input text
        /// </summary>
        private void RenameSampleDialog_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != System.Windows.Input.Key.Enter) return;
            var input = sender as InputPrompt;
            if (input == null) return;
            input.Focus();
        }

        private void RenameSampleDialog_OnCompleted(object sender, PopUpEventArgs<string, PopUpResult> e)
        {
            var input = sender as InputPrompt;
            if (input == null) return;
            if (e.PopUpResult == PopUpResult.Ok) this._viewModel.RenameSampleTo(input.Value);
            this._viewModel.OnRenameSample = null;
        }

        /// <summary>
        /// Show dialog to delete sample
        /// </summary>
        private void ShowDeleteSampleDialog()
        {
            var deleteSampleDialog = new MessagePrompt()
            {
                Message = AppResources.MessageDeleteSample
            };
            deleteSampleDialog.Completed += DeleteSampleDialog_OnCompleted;
            deleteSampleDialog.Show();
        }

        private void DeleteSampleDialog_OnCompleted(object sender, PopUpEventArgs<string, PopUpResult> e)
        {
            if (e.PopUpResult == PopUpResult.Ok) this._viewModel.DeleteSample();
            _viewModel.OnDeleteSample = null;
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
            this.ApplicationBar.MenuItems.Add(helpMenuItem);

            //// Add play button for player
            var addButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.png", UriKind.Relative))
            {
                Text = AppResources.AppBarAddSample,
            };
            addButton.Click += this.AddSampleButton_Click;
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
        /// Add new sample
        /// </summary>
        private void AddSampleButton_Click(object sender, EventArgs e)
        {
            var tactSizePickDialog = new MessagePrompt()
            {
                Title = AppResources.MessageChoseSampleSize,
                Body = new TactSizePicker()
            };
            tactSizePickDialog.Completed += tactSizePickDialog_Completed;
            tactSizePickDialog.Show();
        }

        private void tactSizePickDialog_Completed(object sender, PopUpEventArgs<string, PopUpResult> e)
        {
            if (e.PopUpResult != PopUpResult.Ok) return;
            var dialog = sender as MessagePrompt;
            if (dialog == null) return;
            var tactPicker = dialog.Body as TactSizePicker;
            if (tactPicker != null) this._viewModel.AddSample(tactPicker.TactSize);
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
                case PlayerState.Stopped: _playerManager.Play(_viewModel.CurrentTrack);
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
        private void PlayerManager_OnStateChangeEvent(object sender, WarehouseOfMusic.EventArgs.PlayerEventArgs e)
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

        #region Manipulation around sample list

        /// <summary>
        /// Chose sample for editing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Sample_OnTap(object sender, GestureEventArgs e)
        {
            var grid = sender as Grid;
            if (grid == null) return;
            var chosenSample = grid.DataContext as ToDoSample;
            if (chosenSample == null) return;
            IsoSettingsManager.SaveRecord("CurrentSampleId", chosenSample.Id);
            NavigationService.Navigate(new Uri("/Views/SampleEditorPage.xaml", UriKind.Relative), chosenSample.Id);
        }

        private void SamplePlayButton_OnTap(object sender, GestureEventArgs e)
        {
            var image = sender as Image;
            if (image == null) return;
            e.Handled = true;
            var chosenSample = image.DataContext as ToDoSample;
            if (chosenSample == null) return;
            _playerManager.Play(_viewModel.CurrentTrack, chosenSample.InitialTact);
        }

        private void RenameSample_OnTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var contextMenuItem = sender as MenuItem;
            if (contextMenuItem == null) return;
            _viewModel.OnRenameSample = contextMenuItem.DataContext as ToDoSample;
            ShowRenameSampleDialog();
        }

        private void DeleteSample_OnTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var contextMenuItem = sender as MenuItem;
            if (contextMenuItem == null) return;
            this._viewModel.OnDeleteSample = contextMenuItem.DataContext as ToDoSample;
            ShowDeleteSampleDialog();
        }

        private void DuplicateSample_OnTap(object sender, GestureEventArgs e)
        {
            var contextMenuItem = sender as MenuItem;
            if (contextMenuItem == null) return;
            var sample = contextMenuItem.DataContext as ToDoSample;
            this._viewModel.Duplicate(sample);
        }
        #endregion

        #region Navigation buttons
        private void ToLeftButton_OnTap(object sender, GestureEventArgs e)
        {
            IsoSettingsManager.SaveRecord("CurrentTrackId", _viewModel.PreviousTrack.Id);
            this.InitialiazeDataContext();
        }

        private void ToRightButton_OnTap(object sender, GestureEventArgs e)
        {
            IsoSettingsManager.SaveRecord("CurrentTrackId", _viewModel.NextTrack.Id);
            this.InitialiazeDataContext();
        }

        private void TitleGrid_OnLoaded(object sender, RoutedEventArgs e)
        {
            ToLeftButton.Width = TitleGrid.ActualHeight;
            ToRightButton.Width = TitleGrid.ActualHeight;
        } 
        #endregion
    }
}