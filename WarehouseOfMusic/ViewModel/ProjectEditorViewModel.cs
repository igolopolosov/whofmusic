//-----------------------------------------------------------------------
// <copyright file="ProjecteditorViewModel.cs" company="Igor Golopolosov">
//     Copyright (c) Igor Golopolosov. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace WarehouseOfMusic.ViewModel
{
    using System.ComponentModel;
    using System.Linq;
    using Model;
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
                Project = this._currentProject
            };

            this._toDoDb.Tracks.InsertOnSubmit(newTrack);
            this._toDoDb.SubmitChanges();
            this._currentProject.Tracks.Add(newTrack);
            this.AddSample(newTrack);
        }

        /// <summary>
        /// Remove a track from the database and collections.
        /// </summary>
        /// <param name="trackForDelete">Track on removing</param>
        public void DeleteTrack(ToDoTrack trackForDelete)
        {
            foreach (var note in this._currentProject.Tracks.First(x => x.Id == trackForDelete.Id).Notes)
            {
                this._toDoDb.Notes.DeleteOnSubmit(note);
            }

            this._currentProject.Tracks.Remove(trackForDelete);
            this._toDoDb.Tracks.DeleteOnSubmit(trackForDelete);
            this._toDoDb.SubmitChanges();
        }

        /// <summary>
        /// Query database and load the collections and list
        /// </summary>
        /// <param name="currentProject">Current project</param>
        public void LoadCollectionsFromDatabase(ToDoProject currentProject)
        {
            this._currentProject = this._toDoDb.Projects.First(x => Equals(x, currentProject));
        }

        /// <summary>
        /// Write changes in the data context to the database.
        /// </summary>
        public void SaveChangesToDb()
        {
            this._toDoDb.SubmitChanges();
        }

        /// <summary>
        /// Add collection of notes to track
        /// </summary>
        /// <param name="newTrack">Track where will add notes</param>
        private void AddSample(ToDoTrack newTrack)
        {
            if (newTrack.Id % 2 != 0)
            {
                var note = new ToDoNote
                {
                    Duration = 24,
                    MidiNumber = 60,
                    Tact = 1,
                    TactPosition = 1,
                    Title = "C2",
                    TrackRef = newTrack
                };
                this._toDoDb.Notes.InsertOnSubmit(note);
                this._currentProject.Tracks.First(x => x.Id == newTrack.Id).Notes.Add(note);

                note = new ToDoNote
                {
                    Duration = 24,
                    MidiNumber = 62,
                    Tact = 1,
                    TactPosition = 25,
                    Title = "D2",
                    TrackRef = newTrack
                };
                this._toDoDb.Notes.InsertOnSubmit(note);
                this._currentProject.Tracks.First(x => x.Id == newTrack.Id).Notes.Add(note);

                note = new ToDoNote
                {
                    Duration = 24,
                    MidiNumber = 64,
                    Tact = 1,
                    TactPosition = 49,
                    Title = "E2",
                    TrackRef = newTrack
                };
                this._toDoDb.Notes.InsertOnSubmit(note);
                this._currentProject.Tracks.First(x => x.Id == newTrack.Id).Notes.Add(note);

                note = new ToDoNote
                {
                    Duration = 24,
                    MidiNumber = 62,
                    Tact = 1,
                    TactPosition = 73,
                    Title = "D2",
                    TrackRef = newTrack
                };
                this._toDoDb.Notes.InsertOnSubmit(note);
                this._currentProject.Tracks.First(x => x.Id == newTrack.Id).Notes.Add(note);
            }
            else
            {
                var note = new ToDoNote
                {
                    Duration = 48,
                    MidiNumber = 53,
                    Tact = 1,
                    TactPosition = 1,
                    Title = "F1",
                    TrackRef = newTrack
                };
                this._toDoDb.Notes.InsertOnSubmit(note);
                this._currentProject.Tracks.First(x => x.Id == newTrack.Id).Notes.Add(note);

                note = new ToDoNote
                {
                    Duration = 48,
                    MidiNumber = 55,
                    Tact = 1,
                    TactPosition = 49,
                    Title = "G1",
                    TrackRef = newTrack
                };
                this._toDoDb.Notes.InsertOnSubmit(note);
                this._toDoDb.SubmitChanges();
                this._currentProject.Tracks.First(x => x.Id == newTrack.Id).Notes.Add(note);
            }
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
