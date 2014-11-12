//-----------------------------------------------------------------------
// <copyright file="MainPage.xaml.cs" company="Igor Golopolosov">
//     Copyright (c) Igor Golopolosov. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace WarehouseOfMusic
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using Microsoft.Phone.Controls;
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
        }

        #region CreateProjectButton events
        /// <summary>
        /// Click on CreateProjectButton
        /// </summary>
        /// <param name="sender">Some object</param>
        /// <param name="e">Click on button</param>
        private void CreateProjectButton_Click(object sender, RoutedEventArgs e)
        {
            if (object.Equals(CreateProjectTextBox.Visibility, Visibility.Collapsed))
            {
                CreateProjectTextBox.Visibility = Visibility.Visible;
                var firstRow = CreateProjectGrid.RowDefinitions.First();
                firstRow.Height = new GridLength(1, GridUnitType.Star);
            }
            else
            {
                App.ViewModel.CreateProject(CreateProjectTextBox.Text == AppResources.CreateProjectTextBoxPlaceholder
                    ? new ToDoProject()
                    : new ToDoProject { Name = CreateProjectTextBox.Text });
                NavigationService.Navigate(new Uri("/ProjectEditorPage.xaml", UriKind.Relative));
            }
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
                _this.Text = AppResources.CreateProjectTextBoxPlaceholder;
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
            if (_this.Text == AppResources.CreateProjectTextBoxPlaceholder)
            {
                _this.Text = string.Empty;
            }
        }
        #endregion

        /// <summary>
        /// Click on ExistingProjectsButton
        /// </summary>
        /// <param name="sender">Some object</param>
        /// <param name="e">Click on button</param>
        private void ExistingProjectsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/ExistingProjectsPage.xaml", UriKind.Relative));
        }

        /// <summary>
        /// Click on SettingsButton
        /// </summary>
        /// <param name="sender">Some object</param>
        /// <param name="e">Click on button</param>
        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/ApplicationSettingsPage.xaml", UriKind.Relative));
        }
    }
}