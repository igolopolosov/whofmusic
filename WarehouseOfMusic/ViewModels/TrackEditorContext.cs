//-----------------------------------------------------------------------
// <copyright file="ProjecteditorViewModel.cs">
//     Copyright (c) Igor Golopolosov. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Data.Linq;

namespace WarehouseOfMusic.ViewModels
{
    using System.ComponentModel;
    using System.Linq;
    using Models;

    /// <summary>
    /// ViewModel for project editor page
    /// </summary>
    public class TrackEditorContext : INotifyPropertyChanged
    {
        /// <summary>
        /// LINQ to SQL data context for the local database.
        /// </summary>
        private readonly ToDoDataContext _toDoDb;
        
        /// <summary>
        /// Currently editing project
        /// </summary>
        private ToDoTrack _currentTrack;

        /// <summary>
        /// Sample that must be deleted
        /// </summary>
        private ToDoSample _onDeleteSample;

        /// <summary>
        /// Sample that must be renamed
        /// </summary>
        private ToDoSample _onRenameSample;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectEditorViewModel" /> class.
        /// Class constructor, create the data context object.
        /// </summary>
        /// <param name="toDoDbConnectionString">Path to connect to database</param>
        public TrackEditorContext(string toDoDbConnectionString)
        {
            this._toDoDb = new ToDoDataContext(toDoDbConnectionString);
        }

        /// <summary>
        /// Event of property changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the current project
        /// </summary>
        public ToDoTrack CurrentTrack
        {
            get
            {
                return this._currentTrack;
            }

            set
            {
                this._currentTrack = value;
                this.NotifyPropertyChanged("CurrentTrack");
            }
        }

        /// <summary>
        /// Gets or sets track on delete
        /// </summary>
        public ToDoSample OnDeleteSample
        {
            get
            {
                return this._onDeleteSample;
            }

            set
            {
                this._onDeleteSample = value;
                this.NotifyPropertyChanged("OnDeleteSample");
            }
        }

        /// <summary>
        /// Gets or sets track on rename
        /// </summary>
        public ToDoSample OnRenameSample
        {
            get
            {
                return this._onRenameSample;
            }

            set
            {
                this._onRenameSample = value;
                this.NotifyPropertyChanged("OnRenameSample");
            }
        }

        /// <summary>
        /// Switch IsPlaying parameter for samples
        /// </summary>
        public void ChangeSamplesState(int playbleTactNumber)
        {
            foreach (var sample in _currentTrack.Samples)
            {
                sample.IsPlaying = sample.InitialTact <= playbleTactNumber && playbleTactNumber < (sample.InitialTact + sample.Size);
            }
        }

        /// <summary>
        /// Add new Sample to the database and collections.
        /// </summary>
        public void AddSample(byte size)
        {
            var lastSample = _currentTrack.Samples.LastOrDefault();
            var initialTact = lastSample == null ? 1 : lastSample.InitialTact + lastSample.Size;
            var sample = new ToDoSample
            {
                InitialTact = initialTact,
                Size = size,
                TrackRef = _currentTrack,
                Name = _currentTrack.Name + _currentTrack.Samples.Count
            };
            this._toDoDb.Samples.InsertOnSubmit(sample);
            this._toDoDb.SubmitChanges();
            _currentTrack.Samples.Add(sample);
        }

        /// <summary>
        /// Remove a Sample from the database and collections.
        /// </summary>
        public void DeleteSample()
        {
            var project = _currentTrack.ProjectRef;
            foreach (var note in _onDeleteSample.Notes)
            {
                this._toDoDb.Notes.DeleteOnSubmit(note);
            }

            this._currentTrack.Samples.Remove(_onDeleteSample);
            this._toDoDb.Samples.DeleteOnSubmit(_onDeleteSample);
            this._toDoDb.SubmitChanges();
            this._currentTrack.ProjectRef = project;
        }

        /// <summary>
        /// Find and return next track in list
        /// </summary>
        public ToDoTrack NextTrack()
        {
            var currentProject = _currentTrack.ProjectRef;
            var currentTrackIndex = currentProject.Tracks.IndexOf(_currentTrack);
            currentTrackIndex++;
            return currentTrackIndex == currentProject.Tracks.Count ? currentProject.Tracks[0] : currentProject.Tracks[currentTrackIndex];
        }

        /// <summary>
        /// Find and return previous track in list
        /// </summary>
        public ToDoTrack PreviousTrack()
        {
            var currentProject = _currentTrack.ProjectRef;
            var currentTrackIndex = currentProject.Tracks.IndexOf(_currentTrack);
            currentTrackIndex--;
            return currentTrackIndex == -1 ? currentProject.Tracks[currentProject.Tracks.Count - 1] : currentProject.Tracks[currentTrackIndex];
        }

        /// <summary>
        /// Rename Sample
        /// </summary>
        /// <param name="newName">New name of Sample</param>
        public void RenameSampleTo(string newName)
        {
            _onRenameSample.Name = newName;
            this._toDoDb.SubmitChanges();
        }

        /// <summary>
        /// Query database and load the information for project
        /// </summary>
        /// <param name="trackId">ID of loading project</param>
        public void LoadTrackFromDatabase(int trackId)
        {
            var options = new DataLoadOptions();
            options.AssociateWith<ToDoTrack>(x => x.Samples.OrderBy(y => y.InitialTact));
            this._toDoDb.LoadOptions = options;
            this._currentTrack = this._toDoDb.Tracks.FirstOrDefault(x => x.Id == trackId);
        }

        /// <summary>
        /// Write changes in the data context to the database.
        /// </summary>
        public void SaveChangesToDb()
        {
            this._toDoDb.SubmitChanges();
        }

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

        /// <summary>
        /// Fix bugs with null value of reference
        /// </summary>
        public void RestoreReferences(ToDoSample movedSample)
        {
            movedSample.TrackRef = _currentTrack;
        }
    }
}
