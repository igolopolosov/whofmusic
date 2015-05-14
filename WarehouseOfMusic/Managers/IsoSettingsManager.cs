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
        public static void SetCurrentProject(int projectId)
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;
            if (settings.Contains("currentProjectId")) settings["currentProjectId"] = projectId;
            else settings.Add("currentProjectId", projectId);
            settings.Save();
        }

        public static void SetCurrentSample(int sampleId)
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;
            if (settings.Contains("currentSampleId")) settings["currentTrackId"] = sampleId;
            else settings.Add("currentSampleId", sampleId);
            settings.Save();
        }

        public static void SetCurrentTrack(int trackId)
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;
            if (settings.Contains("currentTrackId")) settings["currentTrackId"] = trackId;
            else settings.Add("currentTrackId", trackId);
            settings.Save();
        }

        public static object GetCurrentProjectId()
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;
            return settings.Contains("currentProjectId") ? settings["currentProjectId"] : null;
        }

        public static object GetCurrentSampleId()
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;
            return settings.Contains("currentSampleId") ? settings["currentSampleId"] : null;
        }

        public static object GetCurrentTrackId()
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;
            return settings.Contains("currentTrackId") ? settings["currentTrackId"] : null;
        }
    }
}
