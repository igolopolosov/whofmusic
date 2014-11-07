//-----------------------------------------------------------------------
// <copyright file="LocalizedStrings.cs" company="Igor Golopolosov">
//     Copyright (c) Igor Golopolosov. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace WarehouseOfMusic
{
    using Resources;

    /// <summary>
    /// Provides access to localized resources.
    /// </summary>
    public class LocalizedStrings
    {
        /// <summary>
        /// Localized resources.
        /// </summary>
        private static AppResources _localizedResources = new AppResources();

        /// <summary>
        /// Gets localized resources.
        /// </summary>
        public AppResources LocalizedResources
        {
            get { return _localizedResources; }
        }
    }
}