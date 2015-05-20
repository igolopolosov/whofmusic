using System.Windows.Input;
using WarehouseOfMusic.Enums;
using WarehouseOfMusic.EventArgs;

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
            this._viewModel.LoadTrackFromDatabase((int)IsoSettingsManager.LoadRecord("CurrentTrackId"));
            this.DataContext = this._viewModel;
            this._viewModel.CurrentTrack.Samples.CollectionChanged +=Samples_CollectionChanged;
            _playerManager = new PlayerManager(_viewModel.CurrentTrack.ProjectRef.Tempo);
            _playerManager.StateChangedEvent += _playerManager_StateChangeEvent;
            _playerManager.TactChangedEvent += PlayerManagerOnTactChangedEvent;
        }

        private void Samples_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
        }

        private void PlayerManagerOnTactChangedEvent(object sender, PlayerEventArgs e)
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
            this._viewModel.AddSample();
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
        private void _playerManager_StateChangeEvent(object sender, WarehouseOfMusic.EventArgs.PlayerEventArgs e)
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
        private void SampleListSelector_OnLoaded(object sender, RoutedEventArgs e)
        {
            var list = sender as LongListSelector;
            if (list == null) return;
            var cellSize = list.ActualWidth / 4 - 4;
            list.GridCellSize = new Size(cellSize, cellSize);
        }

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
        #endregion

        #region Navigation buttons
        private void ToLeftButton_OnTap(object sender, GestureEventArgs e)
        {
            var previousTrack = _viewModel.PreviousTrack();
            IsoSettingsManager.SaveRecord("CurrentTrackId", previousTrack.Id);
            this.InitialiazeDataContext();
        }

        private void ToRightButton_OnTap(object sender, GestureEventArgs e)
        {
            var nextTrack = _viewModel.NextTrack();
            IsoSettingsManager.SaveRecord("CurrentTrackId", nextTrack.Id);
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