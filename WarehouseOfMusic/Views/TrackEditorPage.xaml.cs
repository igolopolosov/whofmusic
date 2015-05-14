﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using WarehouseOfMusic.Managers;
using WarehouseOfMusic.Models;
using WarehouseOfMusic.Resources;
using WarehouseOfMusic.ViewModels;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;

namespace WarehouseOfMusic.Views
{
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
            var playPauseButton = ApplicationBar.Buttons[1] as ApplicationBarIconButton;
            if (playPauseButton == null) return;
            playPauseButton.IconUri = new Uri("/Assets/AppBar/appbar.control.play.png", UriKind.Relative);
            playPauseButton.Text = AppResources.AppBarPlay;
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
        private void SampleTitle_OnTap(object sender, GestureEventArgs e)
        {
            var title = sender as TextBlock;
            if (title == null) return;
            var chosenSample = title.DataContext as ToDoSample;
            if (chosenSample == null) return;
            IsoSettingsManager.SaveRecord("CurrentSampleId", chosenSample.Id);
            NavigationService.Navigate(new Uri("/Views/SampleEditorPage.xaml", UriKind.Relative), chosenSample.Id);
        } 
        #endregion
    }
}