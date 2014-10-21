using System;
using System.Windows;
using Microsoft.Phone.Controls;
using WarehouseOfMusic.Model;

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

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/ApplicationSettingsPage.xaml", UriKind.Relative));
        }

        private void CreateProjectButton_Click(object sender, RoutedEventArgs e)
        {
            App.ViewModel.AddProject(new ToDoProject { Name = "LowFi" });
            App.ViewModel.ProjectName = "LowFi";
            NavigationService.Navigate(new Uri("/ProjectEditorPage.xaml", UriKind.Relative));
        }

        private void OpenProjectButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/ProjectEditorPage.xaml", UriKind.Relative));
            App.ViewModel.ProjectName = "LowFi";
        }
    }
}