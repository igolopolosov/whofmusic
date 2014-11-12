//-----------------------------------------------------------------------
// <copyright file="ExistingProjectsPage.xaml.cs" company="Igor Golopolosov">
//     Copyright (c) Igor Golopolosov. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace WarehouseOfMusic
{
    using System;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Microsoft.Phone.Controls;
    using Model;

    /// <summary>
    /// Page with existing projects
    /// </summary>
    public partial class ExistingProjectsPage : PhoneApplicationPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExistingProjectsPage" /> class.
        /// </summary>
        public ExistingProjectsPage()
        {
            this.InitializeComponent();
            this.DataContext = App.ViewModel;
        }

        /// <summary>
        /// Chose project for editing.
        /// </summary>
        /// <param name="sender">Project item displayed like a list box item</param>
        /// <param name="e">One tap</param>
        private void ProjectItemGrid_OnTap(object sender, GestureEventArgs e)
        {
            var grid = sender as Grid;

            if (grid != null)
            {
                var chosenProject = grid.DataContext as ToDoProject;
                App.ViewModel.CurrentProject = chosenProject;
            }

            NavigationService.Navigate(new Uri("/ProjectEditorPage.xaml", UriKind.Relative));
        }
    }
}