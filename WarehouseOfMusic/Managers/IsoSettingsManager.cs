//-----------------------------------------------------------------------
// <copyright file="IsoSettingsManager.cs" company="Igor Golopolosov">
//     Copyright (c) Igor Golopolosov. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace WarehouseOfMusic.Managers
{
    using System.IO.IsolatedStorage;

    public static class IsoSettingsManager
    {
        /// <summary>
        /// Saves record to application settings
        /// </summary>
        public static void SaveRecord(string key, int value)
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;
            if (settings.Contains(key)) settings[key] = value;
            else settings.Add(key, value);
            settings.Save();
        }

        /// <summary>
        /// Load record from application settings
        /// </summary>
        public static object LoadRecord(string key)
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;
            return settings.Contains(key) ? settings[key] : null;
        }
    }
}
