using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using WarehouseOfMusic.Model;
using WarehouseOfMusic.Resources;

namespace WarehouseOfMusic
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Конструктор
        public MainPage()
        {
            InitializeComponent();
            this.DataContext = App.ViewModel;
        }

        #region CreateProjectButton events
        private void CreateProjectButton_Click(object sender, RoutedEventArgs e)
        {
            if (Equals(CreateProjectTextBox.Visibility, Visibility.Collapsed))
            {
                CreateProjectTextBox.Visibility = Visibility.Visible;
                var firstRow = CreateProjectGrid.RowDefinitions.First();
                firstRow.Height = new GridLength(1, GridUnitType.Star);
            }
            else
            {
                if (CreateProjectTextBox.Text == AppResources.CreateProjectTextBoxPlaceholder)
                {
                    App.ViewModel.AddProject(new ToDoProject {Name = "Default"});
                }
                else App.ViewModel.AddProject(new ToDoProject {Name = CreateProjectTextBox.Text});
                NavigationService.Navigate(new Uri("/ProjectEditorPage.xaml", UriKind.Relative));
            }
        }

        private void CreateProjectTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var _this = (TextBox)sender;
            if (_this.Text == "")
                _this.Text = AppResources.CreateProjectTextBoxPlaceholder;
        }

        private void CreateProjectTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var _this = (TextBox)sender;
            if (_this.Text == AppResources.CreateProjectTextBoxPlaceholder)
                _this.Text = "";
        }
        #endregion

        private void OpenProjectButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/AllExistingProjectsPage.xaml", UriKind.Relative));
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/ApplicationSettingsPage.xaml", UriKind.Relative));
        }
    }
}