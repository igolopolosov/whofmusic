//-----------------------------------------------------------------------
// <copyright file="ProjecteditorViewModel.cs">
//     Copyright (c) Igor Golopolosov. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace WarehouseOfMusic.ViewModels
{
    using System.ComponentModel;
    using System.Linq;
    using Models;
    using Resources;

    /// <summary>
    /// ViewModel for project editor page
    /// </summary>
    public class ProjectEditorViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// LINQ to SQL data context for the local database.
        /// </summary>
        private readonly ToDoDataContext _toDoDb;
        
        /// <summary>
        /// Currently editing project
        /// </summary>
        private ToDoProject _currentProject;

        /// <summary>
        /// Track that must be deleted
        /// </summary>
        private ToDoTrack _onDeleteTrack;
        
        /// <summary>
        /// Track that must be renamed
        /// </summary>
        private ToDoTrack _onRenameTrack;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectEditorViewModel" /> class.
        /// Class constructor, create the data context object.
        /// </summary>
        /// <param name="toDoDbConnectionString">Path to connect to database</param>
        public ProjectEditorViewModel(string toDoDbConnectionString)
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

        /// <summary>
        /// Gets or sets track on delete
        /// </summary>
        public ToDoTrack OnDeleteTrack
        {
            get
            {
                return this._onDeleteTrack;
            }

            set
            {
                this._onDeleteTrack = value;
                this.NotifyPropertyChanged("OnDeleteTrack");
            }
        }
        
        /// <summary>
        /// Gets or sets track on rename
        /// </summary>
        public ToDoTrack OnRenameTrack
        {
            get
            {
                return this._onRenameTrack;
            }

            set
            {
                this._onRenameTrack = value;
                this.NotifyPropertyChanged("OnRenameTrack");
            }
        }

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
                ProjectRef = this._currentProject
            };
            this._toDoDb.Tracks.InsertOnSubmit(newTrack);
            this._toDoDb.SubmitChanges();
            this._currentProject.Tracks.Add(newTrack);

            var sample = new ToDoSample
            {
                InitialTact = 1,
                Size = 4,
                TrackRef = newTrack
            };
            this._toDoDb.Samples.InsertOnSubmit(sample);
            this._toDoDb.SubmitChanges();
            newTrack.Samples.Add(sample);
        }

        /// <summary>
        /// Remove a track from the database and collections.
        /// </summary>
        /// <param name="trackForDelete">Track on removing</param>
        public void DeleteTrack()
        {
            foreach (var sample in _onDeleteTrack.Samples)
            {
                foreach (var note in sample.Notes)
                {
                    this._toDoDb.Notes.DeleteOnSubmit(note);
                }
                this._toDoDb.Samples.DeleteOnSubmit(sample);
            }

            this._currentProject.Tracks.Remove(_onDeleteTrack);
            this._toDoDb.Tracks.DeleteOnSubmit(_onDeleteTrack);
            this._toDoDb.SubmitChanges();
        }

        /// <summary>
        /// Query database and load the information for project
        /// </summary>
        /// <param name="projectId">ID of loading project</param>
        public void LoadProjectFromDatabase(int projectId)
        {
            this._currentProject = this._toDoDb.Projects.FirstOrDefault(x => x.Id == projectId);
        }

        /// <summary>
        /// Rename track
        /// </summary>
        /// <param name="newName">New name of track</param>
        public void RenameTrackTo(string newName)
        {
            _onRenameTrack.Name = newName;
            this._toDoDb.SubmitChanges();
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

        
    }
}
