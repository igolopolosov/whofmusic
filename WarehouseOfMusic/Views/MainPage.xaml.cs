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

        #region CreateProjectButton events

        /// <summary>
        /// Informs the user about error
        /// </summary>
        private void ShowInputPromt(int projectId)
        {
            this._viewModel.OnRenameProjectId = projectId;
            var projectName = projectId == -1 ? string.Empty
                : this._viewModel.ProjectsList.FirstOrDefault(x => x.Id == projectId).Name;
            var inputPromptTitle = projectId == -1 ? AppResources.CreateProject : AppResources.RenameProject;
            var inputPrompt = new InputPromptOveride()
            {
                IsCancelVisible = true,
                IsSubmitOnEnterKey = false,
                Title = inputPromptTitle,
                Value = projectName
            };
            inputPrompt.LostFocus += inputPrompt_LostFocus;
            inputPrompt.KeyUp += InputPrompt_KeyUp;
            inputPrompt.Completed += InputPromptOnCompleted;
            inputPrompt.Show();
        }

        void inputPrompt_LostFocus(object sender, RoutedEventArgs e)
        {
            var input = sender as InputPrompt;
            if (input == null) return;
            input.Message = input.Value == string.Empty ? AppResources.ErrorEmptyName : string.Empty;
        }

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
            
            if (this._viewModel.OnRenameProjectId == -1)
            {
                var newProject = this._viewModel.CreateProject(input.Value);
                IsoSettingsManager.SetCurrentProject(newProject.Id);
                NavigationService.Navigate(new Uri("/Views/ProjectEditorPage.xaml", UriKind.Relative));
            }
            else
            {
                this._viewModel.RenameProjectTo(input.Value);
                this._viewModel.OnRenameProjectId = -1;
                this.Focus();
            }
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
            if (chosenProject != null) ShowInputPromt(chosenProject.Id);
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
            this._viewModel.DeleteProject(chosenProject);
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
            ShowInputPromt(-1);
        }

        #endregion
    }
}