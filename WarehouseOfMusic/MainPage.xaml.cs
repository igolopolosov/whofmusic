using System;
using System.Windows;
using Microsoft.Phone.Controls;

namespace WarehouseOfMusic
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Конструктор
        public MainPage()
        {
            InitializeComponent();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/ApplicationSettingsPage.xaml", UriKind.Relative));
        }

        private void CreateProjectButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/ProjectEditorPage.xaml", UriKind.Relative));
        }
    }
}