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
        private MainViewModel _viewModel;

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
            this._viewModel = new MainViewModel(App.DbConnectionString);
            this._viewModel.LoadProFromDatabase();
            this.DataContext = this._viewModel;
        }
        #endregion

        #region Create, rename and delete dialogs

        /// <summary>
        /// Show dialog to create or rename project
        /// </summary>
        /// <param name="projectId">-1 = for create project dialog, n - rename project dialog </param>
        private void ShowRenameDialog()
        {
            var projectName = _viewModel.OnRenameProject == null ? string.Empty
                : _viewModel.OnRenameProject.Name;
            var dialogTitle = _viewModel.OnRenameProject == null ? AppResources.CreateProject : AppResources.RenameProject;
            var renameProjectDialog = new InputPromptOveride()
            {
                IsSubmitOnEnterKey = false,
                Title = dialogTitle,
                Value = projectName
            };
            renameProjectDialog.LostFocus += RenameProjectDialog_OnLostFocus;
            renameProjectDialog.KeyUp += RenameProjectDialog_OnKeyUp;
            renameProjectDialog.Completed += RenameProjectDialog_OnCompleted;
            renameProjectDialog.Show();
        }

        /// <summary>
        /// Show or hide 'empty name' error message
        /// </summary>
        void RenameProjectDialog_OnLostFocus(object sender, RoutedEventArgs e)
        {
            var input = sender as InputPrompt;
            if (input == null) return;
            input.Message = input.Value == string.Empty ? AppResources.ErrorEmptyName : string.Empty;
        }

        /// <summary>
        /// Detect the end of input text
        /// </summary>
        private void RenameProjectDialog_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != System.Windows.Input.Key.Enter) return;
            var input = sender as InputPrompt;
            if (input == null) return;
            input.Focus();
        }

        private void RenameProjectDialog_OnCompleted(object sender, PopUpEventArgs<string, PopUpResult> e)
        {
            var input = sender as InputPrompt;
            if (input == null) return;
            if (e.PopUpResult == PopUpResult.Ok)
            {
                if (_viewModel.OnRenameProject == null)
                {
                    var newProject = this._viewModel.CreateProject(input.Value);
                    IsoSettingsManager.SaveRecord("CurrentProjectId", newProject.Id);
                    NavigationService.Navigate(new Uri("/Views/ProjectEditorPage.xaml", UriKind.Relative));
                }
                else
                {
                    this._viewModel.RenameProjectTo(input.Value);
                }
            }
            _viewModel.OnRenameProject = null;
        }

        /// <summary>
        /// Show dialog to delete project
        /// </summary>
        private void ShowDeleteDialog()
        {
            var deleteProjectDialog = new MessagePrompt()
            {
                Message = AppResources.MessageDeleteProject
            };
            deleteProjectDialog.Completed += DeleteProjectDialog_OnCompleted;
            deleteProjectDialog.Show();
        }

        private void DeleteProjectDialog_OnCompleted(object sender, PopUpEventArgs<string, PopUpResult> e)
        {
            if (e.PopUpResult == PopUpResult.Ok) this._viewModel.DeleteProject();
            _viewModel.OnDeleteProject = null;
        }

        #endregion

        #region Manipulations with project
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
            IsoSettingsManager.SaveRecord("CurrentProjectId", chosenProject.Id);
            NavigationService.Navigate(new Uri("/Views/ProjectEditorPage.xaml", UriKind.Relative), chosenProject.Id);
        }

        /// <summary>
        /// Shows rename project dialog
        /// </summary>
        private void RenameProject_OnTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var contextMenuItem = sender as MenuItem;
            if (contextMenuItem == null) return;
            _viewModel.OnRenameProject = contextMenuItem.DataContext as ToDoProject;
            ShowRenameDialog();
        }

        /// <summary>
        /// Shows delete project dialog
        /// </summary>
        private void DeleteProject_OnTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var contextMenuItem = sender as MenuItem;
            if (contextMenuItem == null) return;
            _viewModel.OnDeleteProject = contextMenuItem.DataContext as ToDoProject;
            ShowDeleteDialog();
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

            //// Add play button for player
            var createProjectButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.png", UriKind.Relative))
            {
                Text = AppResources.AppBarCreateProject,
            };
            createProjectButton.Click += this.CreateProjectButton_Click;
            this.ApplicationBar.Buttons.Add(createProjectButton);
        }

        private void CreateProjectButton_Click(object sender, EventArgs e)
        {
            ShowRenameDialog();
        }

        #endregion
    }
}