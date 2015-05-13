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
        public void DeleteTrack(ToDoTrack trackForDelete)
        {
            foreach (var sample in this._currentProject.Tracks.First(x => x.Id == trackForDelete.Id).Samples)
            {
                foreach (var note in sample.Notes)
                {
                    this._toDoDb.Notes.DeleteOnSubmit(note);
                }
                this._toDoDb.Samples.DeleteOnSubmit(sample);
            }

            this._currentProject.Tracks.Remove(trackForDelete);
            this._toDoDb.Tracks.DeleteOnSubmit(trackForDelete);
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
