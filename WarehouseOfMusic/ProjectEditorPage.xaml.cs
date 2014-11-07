//-----------------------------------------------------------------------
// <copyright file="ProjectEditorPage.xaml.cs" company="Igor Golopolosov">
//     Copyright (c) Igor Golopolosov. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace WarehouseOfMusic
{
    using Microsoft.Phone.Controls;

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
    }
}