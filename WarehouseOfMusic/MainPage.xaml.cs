//-----------------------------------------------------------------------
// <copyright file="MainPage.xaml.cs" company="Igor Golopolosov">
//     Copyright (c) Igor Golopolosov. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace WarehouseOfMusic
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Shell;
    using Model;
    using Resources;

    /// <summary>
    /// Main Page
    /// </summary>
    public partial class MainPage : PhoneApplicationPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage" /> class.
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();
            this.DataContext = App.ViewModel;
            this.BuildLocalizedAppBar();
        }

        #region CreateProjectButton events
        /// <summary>
        /// Click on CreateProjectButton
        /// </summary>
        /// <param name="sender">Some object</param>
        /// <param name="e">Click on button</param>
        private void OkButton_OnTap(object sender, GestureEventArgs e)
        {
            App.ViewModel.CreateProject(CreateProjectTextBox.Text == AppResources.CreateProject
                    ? new ToDoProject()
                    : new ToDoProject { Name = CreateProjectTextBox.Text });
            NavigationService.Navigate(new Uri("/ProjectEditorPage.xaml", UriKind.Relative));
        }

        /// <summary>
        /// CreateProjectTextBox lost focus
        /// </summary>
        /// <param name="sender">Some object</param>
        /// <param name="e">Lost focus of button</param>
        private void CreateProjectTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var _this = (TextBox)sender;
            if (_this.Text == string.Empty)
            {
                _this.Text = AppResources.CreateProject;
            }
        }

        /// <summary>
        /// CreateProjectTextBox got focus
        /// </summary>
        /// <param name="sender">Some object</param>
        /// <param name="e">Got focus of button</param>
        private void CreateProjectTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var _this = (TextBox)sender;
            if (_this.Text == AppResources.CreateProject)
            {
                _this.Text = string.Empty;
            }
        }
        #endregion

        #region Manipulation with projects from list
        /// <summary>
        /// Chose project for editing.
        /// </summary>
        /// <param name="sender">Project item displayed like a list box item</param>
        /// <param name="e">One tap</param>
        private void EditProjectButton_OnTap(object sender, GestureEventArgs e)
        {
            var button = sender as Button;

            if (button != null)
            {
                var chosenProject = button.DataContext as ToDoProject;
                App.ViewModel.CurrentProject = chosenProject;
            }

            NavigationService.Navigate(new Uri("/ProjectEditorPage.xaml", UriKind.Relative));
        }

        /// <summary>
        /// Chose project for deleting.
        /// </summary>
        /// <param name="sender">Project item displayed like a list box item</param>
        /// <param name="e">One tap</param>
        private void DeleteProjectButton_OnTap(object sender, GestureEventArgs e)
        {
            var button = sender as Button;

            if (button != null)
            {
                var chosenProject = button.DataContext as ToDoProject;
                App.ViewModel.DeleteProject(chosenProject);
            }
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