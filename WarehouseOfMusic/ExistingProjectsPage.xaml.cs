//-----------------------------------------------------------------------
// <copyright file="ExistingProjectsPage.xaml.cs" company="Igor Golopolosov">
//     Copyright (c) Igor Golopolosov. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace WarehouseOfMusic
{
    using Microsoft.Phone.Controls;

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
    }
}