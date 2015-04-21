//-----------------------------------------------------------------------
// <copyright file="MainPage.xaml.cs">
//     Copyright (c) Igor Golopolosov. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace WarehouseOfMusic.Views
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Navigation;
    using Coding4Fun.Toolkit.Controls;
    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Shell;
    using Managers;
    using Models;
    using Resources;
    using ViewModels;
    
    /// <summary>
    /// Main Page
    /// </summary>
    public partial class MainPage : PhoneApplicationPage
    {
        /// <summary>
        /// ViewModel for main page
        /// </summary>
        private MainViewModel _mainViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage" /> class.
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();
            this.InitialiazeDataContext();
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
            this._mainViewModel = new MainViewModel(App.DbConnectionString);
            this._mainViewModel.LoadProFromDatabase();
            this.DataContext = this._mainViewModel;
        }
        #endregion

        #region CreateProjectButton events

        /// <summary>
        /// By press enter key creates new project or renames chosen by user project
        /// </summary>
        private void CreateProjectTextBox_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != System.Windows.Input.Key.Enter) return;
            if (CreateProjectTextBox.Text == string.Empty)
            {
                ShowErrorMessage();
                return;
            }
            
            if (this._mainViewModel.OnRenameProjectId == -1)
            {
                var newProject = this._mainViewModel.CreateProject(CreateProjectTextBox.Text);
                IsoSettingsManager.SetCurrentProject(newProject.Id);
                NavigationService.Navigate(new Uri("/Views/ProjectEditorPage.xaml", UriKind.Relative));
            }
            else
            {
                this._mainViewModel.RenameProjectTo(CreateProjectTextBox.Text);
                this._mainViewModel.OnRenameProjectId = -1;
                this.Focus();
            }
        }

        /// <summary>
        /// Informs the user about error
        /// </summary>
        private void ShowErrorMessage()
        {
            var messagePrompt = new MessagePrompt()
            {
                Body = AppResources.ErrorEmptyName,
                VerticalAlignment = VerticalAlignment.Center,
            };
            messagePrompt.Completed += messagePrompt_Completed;
            messagePrompt.Show();
        }

        /// <summary>
        /// Returns focus on CreateProjectTextBox
        /// </summary>
        private void messagePrompt_Completed(object sender, PopUpEventArgs<string, PopUpResult> e)
        {
            CreateProjectTextBox.Focus();
        }

        /// <summary>
        /// CreateProjectTextBox got focus
        /// </summary>s
        private void CreateProjectTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var thisTextBox = (TextBox) sender;
            thisTextBox.Text = this._mainViewModel.OnRenameProjectId == -1
                ? string.Empty
                : this._mainViewModel.ProjectsList
                    .FirstOrDefault(x => x.Id == this._mainViewModel.OnRenameProjectId).Name;
            thisTextBox.Select(thisTextBox.Text.Length, 0);
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
            if (grid == null) return;

            var chosenProject = grid.DataContext as ToDoProject;
            if (chosenProject == null) return;
            IsoSettingsManager.SetCurrentProject(chosenProject.Id);
            NavigationService.Navigate(new Uri("/Views/ProjectEditorPage.xaml", UriKind.Relative), chosenProject.Id);
        }

        /// <summary>
        /// Rename project
        /// </summary>
        /// <param name="sender">Project item displayed like a list box item</param>
        /// <param name="e">On tap</param>
        private void RenameProject_OnTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var contextMenuItem = sender as MenuItem;

            if (contextMenuItem == null) return;
            var chosenProject = contextMenuItem.DataContext as ToDoProject;
            this._mainViewModel.OnRenameProjectId = chosenProject.Id;
        }

        /// <summary>
        /// Delete project
        /// </summary>
        /// <param name="sender">Project item displayed like a list box item</param>
        /// <param name="e">On tap</param>
        private void DeleteProject_OnTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var contextMenuItem = sender as MenuItem;

            if (contextMenuItem == null) return;
            var chosenProject = contextMenuItem.DataContext as ToDoProject;
            this._mainViewModel.DeleteProject(chosenProject);
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
        private void SettingsMenuItem_OnClick(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/ApplicationSettingsPage.xaml", UriKind.Relative));
        }

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
        }

        #endregion
    }
}