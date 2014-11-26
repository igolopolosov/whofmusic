//-----------------------------------------------------------------------
// <copyright file="TrackViewModel.cs" company="Igor Golopolosov">
//     Copyright (c) Igor Golopolosov. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace WarehouseOfMusic.ViewModel
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using Model;
    using Resources;

    public class ProjectEditorViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Currently editing project
        /// </summary>
        private ToDoProject _currentProject;

        /// <summary>
        /// Gets or sets current project
        /// </summary>
        public ToDoProject CurrentProject
        {
            get
            {
                return this._currentProject;
            }

            set
            {
                this._currentProject = value;
                this.NotifyPropertyChanged("CurrentProject");
            }
        }

        public event EventHandler<TrackArgs> TrackChanged;

        /// <summary>
        /// Add new track to the database and collections.
        /// </summary>
        public void AddTrack()
        {
            var trackNumber = 1;
            if (this._currentProject.Tracks.Any())
            {
                trackNumber = this._currentProject.Tracks.Count + 1;
            }

            var trackName = AppResources.TrackString + " " + trackNumber;

            var newTrack = new ToDoTrack
            {
                Name = trackName,
                Project = this._currentProject
            };
            
            var trackArgs = new TrackArgs
            {
                Track = newTrack,
                TypeOfEvent = "Insert"
            };

            if (TrackChanged != null) TrackChanged(this, trackArgs);

            this._currentProject.Tracks.Add(newTrack);
        }

        /// <summary>
        /// Remove a track from the database and collections.
        /// </summary>
        /// <param name="trackForDelete">Track on removing</param>
        public void DeleteTrack(ToDoTrack trackForDelete)
        {
            this._currentProject.Tracks.Remove(trackForDelete);

            var trackArgs = new TrackArgs
            {
                Track = trackForDelete,
                TypeOfEvent = "Delete"
            };

            if (TrackChanged != null) TrackChanged(this, trackArgs);
        }

        internal void Set(ToDoProject chosenProject)
        {
            _currentProject = chosenProject;
        }

        /// <summary>
        /// Event of property changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Used to notify the app that a property has changed.
        /// </summary>
        /// <param name="propertyName">Property on changed</param>
        private void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
