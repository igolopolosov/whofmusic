//-----------------------------------------------------------------------
// <copyright file="Extensions.cs" company="Igor Golopolosov">
//     Copyright (c) Igor Golopolosov. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace WarehouseOfMusic
{
    using System;
    using System.Windows.Navigation;

    /// <summary>
    /// Extensions for navigation namespace
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Data attached to navigation
        /// </summary>
        private static object _data;

        /// <summary>
        /// Navigates to the content specified by uniform resource identifier (URI).
        /// </summary>
        /// <param name="navigationService">The navigation service.</param>
        /// <param name="source">The URI of the content to navigate to.</param>
        /// <param name="data">The data that you need to pass to the other page 
        /// specified in URI.</param>
        public static void Navigate(this NavigationService navigationService, Uri source, object data)
        {
            _data = data;
            navigationService.Navigate(source);
        }

        /// <summary>
        /// Gets the navigation data passed from the previous page.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <returns>System object.</returns>
        public static object GetNavigationData(this NavigationService service)
        {
            return _data;
        }
    }
}
