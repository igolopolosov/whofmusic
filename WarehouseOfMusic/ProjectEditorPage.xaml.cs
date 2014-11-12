//-----------------------------------------------------------------------
// <copyright file="ProjectEditorPage.xaml.cs" company="Igor Golopolosov">
//     Copyright (c) Igor Golopolosov. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------


namespace WarehouseOfMusic
{
    using Microsoft.Phone.Controls;
    using System.Windows;
    using System.Windows.Controls;
    using Model;

    /// <summary>
    /// Page of editing projects
    /// </summary>
    public partial class ProjectEditorPage : PhoneApplicationPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectEditorPage" /> class.
        /// </summary>
        public ProjectEditorPage()
        {
            this.InitializeComponent();
            this.DataContext = App.ViewModel;
        }

        private void AddTrackButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            App.ViewModel.AddTrackToCurrentProject();
        }

        private void DeleteTrackButton_OnClick(object sender, RoutedEventArgs e)
        {
            // Cast the parameter as a button.
            var button = sender as Button;

            if (button != null)
            {
                // Get a handle for the track bound to the button.
                var toDoForDelete = button.DataContext as ToDoTrack;

                App.ViewModel.DeleteTrack(toDoForDelete);
            }

            // Put the focus back to the main page.
            this.Focus();
        }
    }
}