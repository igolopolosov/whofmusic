//-----------------------------------------------------------------------
// <copyright file="ProjectEditorContext.cs">
//     Copyright (c) Igor Golopolosov. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace WarehouseOfMusic.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using Models;
    using Resources;

    /// <summary>
    /// ViewModel for project editor page
    /// </summary>
    public class ProjectEditorContext : Context
    {
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
        /// Initializes a new instance of the <see cref="ProjectEditorContext" /> class.
        /// Class constructor, create the data context object.
        /// </summary>
        /// <param name="toDoDbConnectionString">Path to connect to database</param>
        public ProjectEditorContext(string toDoDbConnectionString) : base(toDoDbConnectionString)
        {
        }

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
                ProjectRef = this._currentProject,
                Instrument = this.ToDoDb.Instruments.First()
            };
            this.ToDoDb.Tracks.InsertOnSubmit(newTrack);
            this.ToDoDb.SubmitChanges();
            this._currentProject.Tracks.Add(newTrack);
            newTrack.Instruments = new ObservableCollection<ToDoInstrument>(this.ToDoDb.Instruments);

            var sample = new ToDoSample
            {
                InitialTact = 1,
                Size = 4,
                TrackRef = newTrack,
                Name = newTrack.Name + "_" + newTrack.Samples.Count
            };
            this.ToDoDb.Samples.InsertOnSubmit(sample);
            this.ToDoDb.SubmitChanges();
            newTrack.Samples.Add(sample);
        }

        /// <summary>
        /// Remove a track from the database and collections.
        /// </summary>
        public void DeleteTrack()
        {
            foreach (var sample in _onDeleteTrack.Samples)
            {
                foreach (var note in sample.Notes)
                {
                    this.ToDoDb.Notes.DeleteOnSubmit(note);
                }
                this.ToDoDb.Samples.DeleteOnSubmit(sample);
            }

            this._currentProject.Tracks.Remove(_onDeleteTrack);
            this.ToDoDb.Tracks.DeleteOnSubmit(_onDeleteTrack);
            this.ToDoDb.SubmitChanges();
        }

        /// <summary>
        /// Query database and load the information for project
        /// </summary>
        /// <param name="projectId">ID of loading project</param>
        public override void LoadData(int projectId)
        {
            this._currentProject = this.ToDoDb.Projects.FirstOrDefault(x => x.Id == projectId);
            foreach (var track in _currentProject.Tracks)
            {
                track.Instruments = new ObservableCollection<ToDoInstrument>(this.ToDoDb.Instruments);
            }
            
        }

        /// <summary>
        /// Rename track
        /// </summary>
        /// <param name="newName">New name of track</param>
        public void RenameTrackTo(string newName)
        {
            _onRenameTrack.Name = newName;
            this.ToDoDb.SubmitChanges();
        }

        public void SelectInstrument(ToDoTrack track, ToDoInstrument instrument)
        {
            _currentProject.Tracks.First(x => track.Id == x.Id).Instrument = instrument;
            SaveChangesToDb();
        }
    }
}
