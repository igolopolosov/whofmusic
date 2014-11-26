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
        /// By press enter key creates new project or renames chosen by user project
        /// </summary>
        /// <param name="sender">Some object</param>
        /// <param name="e">Click on button</param>
        private void CreateProjectTextBox_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (App.ViewModel.OnRenameProjectId == -1)
                {
                    App.ViewModel.CreateProject(CreateProjectTextBox.Text == AppResources.CreateProject
                        ? new ToDoProject()
                        : new ToDoProject { Name = CreateProjectTextBox.Text });
                    NavigationService.Navigate(new Uri("/ProjectEditorPage.xaml", UriKind.Relative));
                }
                else
                {
                    App.ViewModel.RenameProjectTo(CreateProjectTextBox.Text);
                    App.ViewModel.OnRenameProjectId = -1;
                    this.Focus();
                }
            }
        }

        /// <summary>
        /// CreateProjectTextBox got focus
        /// </summary>
        /// <param name="sender">Some object</param>
        /// <param name="e">Got focus of button</param>
        private void CreateProjectTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var thisTextBox = (TextBox)sender;

            if (thisTextBox.Text == AppResources.CreateProject)
            {
                thisTextBox.Text = string.Empty;
            }
        }

        /// <summary>
        /// CreateProjectTextBox lost focus
        /// </summary>
        /// <param name="sender">Some object</param>
        /// <param name="e">Lost focus of button</param>
        private void CreateProjectTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var thisTextBox = (TextBox)sender;
            thisTextBox.Text = AppResources.CreateProject;
        }
        #endregion

        #region Manipulation with projects from list
        /// <summary>
        /// Chose project for editing.
        /// </summary>
        /// <param name="sender">Project item displayed like a list box item</param>
        /// <param name="e">One tap</param>
        private void ProjectItemGrid_OnTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var grid = sender as Grid;

            if (grid != null)
            {
                var chosenProject = grid.DataContext as ToDoProject;
                App.ViewModel.CurrentProject.Set(chosenProject);
            }

            NavigationService.Navigate(new Uri("/ProjectEditorPage.xaml", UriKind.Relative));
        }

        /// <summary>
        /// Rename project
        /// </summary>
        /// <param name="sender">Project item displayed like a list box item</param>
        /// <param name="e">On tap</param>
        private void RenameProject_OnTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var contextMenuItem = sender as MenuItem;

            if (contextMenuItem != null && ReferenceEquals(contextMenuItem.Header, AppResources.RenameContextMenu))
            {
                var chosenProject = contextMenuItem.DataContext as ToDoProject;
                if (chosenProject != null) App.ViewModel.OnRenameProjectId = chosenProject.Id;
            }
        }

        /// <summary>
        /// Delete project
        /// </summary>
        /// <param name="sender">Project item displayed like a list box item</param>
        /// <param name="e">On tap</param>
        private void DeleteProject_OnTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var contextMenuItem = sender as MenuItem;

            if (contextMenuItem != null)
            {
                var chosenProject = contextMenuItem.DataContext as ToDoProject;
                App.ViewModel.DeleteProject(chosenProject);
            }
        }

        /// <summary>
        /// Rightly set focus after tap on rename
        /// </summary>
        /// <param name="sender">Rename item</param>
        /// <param name="e">On lost focus</param>
        private void RenameProject_OnLostFocus(object sender, RoutedEventArgs e)
        {
            CreateProjectTextBox.Focus();
        }

        /// <summary>
        /// Rightly set focus after tap on delete
        /// </summary>
        /// <param name="sender">Delete item</param>
        /// <param name="e">On lost focus</param>
        private void DeleteProject_OnLostFocus(object sender, RoutedEventArgs e)
        {
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