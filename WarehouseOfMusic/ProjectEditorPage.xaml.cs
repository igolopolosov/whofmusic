//-----------------------------------------------------------------------
// <copyright file="ProjectEditorPage.xaml.cs" company="Igor Golopolosov">
//     Copyright (c) Igor Golopolosov. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace WarehouseOfMusic
{
    using System.Windows;
    using System.Windows.Controls;
    using Microsoft.Phone.Controls;
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

        /// <summary>
        /// Adding new track
        /// </summary>
        /// <param name="sender">Some object</param>
        /// <param name="e">On click</param>
        private void AddTrackButton_Click(object sender, RoutedEventArgs e)
        {
            App.ViewModel.AddTrackToCurrentProject();
        }

        /// <summary>
        /// Deleting of chosen track
        /// </summary>
        /// <param name="sender">Some object</param>
        /// <param name="e">On click</param>
        private void DeleteTrackButton_OnClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            if (button != null)
            {
                var trackForDelete = button.DataContext as ToDoTrack;
                App.ViewModel.DeleteTrack(trackForDelete);
            }

            this.Focus();
        }
    }
}