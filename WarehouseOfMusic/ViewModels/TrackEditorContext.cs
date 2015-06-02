//-----------------------------------------------------------------------
// <copyright file="ProjecteditorViewModel.cs">
//     Copyright (c) Igor Golopolosov. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace WarehouseOfMusic.ViewModels
{
    using System.Data.Linq;
    using System.Linq;
    using Models;

    /// <summary>
    /// ViewModel for project editor page
    /// </summary>
    public class TrackEditorContext : Context
    {
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
        /// Next track in list
        /// </summary>
        public ToDoTrack NextTrack
        {
            get
            {
                var currentProject = _currentTrack.ProjectRef;
                var currentTrackIndex = currentProject.Tracks.IndexOf(_currentTrack);
                currentTrackIndex++;
                return currentTrackIndex == currentProject.Tracks.Count
                    ? currentProject.Tracks[0]
                    : currentProject.Tracks[currentTrackIndex];
            }
        }

        /// <summary>
        /// Previous track in list
        /// </summary>
        public ToDoTrack PreviousTrack
        {
            get
            {
                var currentProject = _currentTrack.ProjectRef;
                var currentTrackIndex = currentProject.Tracks.IndexOf(_currentTrack);
                currentTrackIndex--;
                return currentTrackIndex == -1
                    ? currentProject.Tracks[currentProject.Tracks.Count - 1]
                    : currentProject.Tracks[currentTrackIndex];
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectEditorContext" /> class.
        /// Class constructor, create the data context object.
        /// </summary>
        /// <param name="toDoDbConnectionString">Path to connect to database</param>
        public TrackEditorContext(string toDoDbConnectionString)
            : base(toDoDbConnectionString)
        {
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
            var nameAddition = lastSample == null ? 0 : lastSample.Id;
            var sample = new ToDoSample
            {
                InitialTact = initialTact,
                Size = size,
                TrackRef = _currentTrack,
                Name = _currentTrack.Name + "_" + nameAddition
            };
            this.ToDoDb.Samples.InsertOnSubmit(sample);
            this.ToDoDb.SubmitChanges();
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
                this.ToDoDb.Notes.DeleteOnSubmit(note);
            }

            this._currentTrack.Samples.Remove(_onDeleteSample);
            this.ToDoDb.Samples.DeleteOnSubmit(_onDeleteSample);
            this.ToDoDb.SubmitChanges();
            this._currentTrack.ProjectRef = project;
        }

        /// <summary>
        /// Add to list of track sample track as chosen
        /// </summary>
        /// <param name="sample"></param>
        public void Duplicate(ToDoSample sample)
        {
            var index = _currentTrack.Samples.IndexOf(sample) + 1;
            var duplicateSample = new ToDoSample()
            {
                InitialTact = sample.InitialTact,
                Size = sample.Size,
                TrackRef = _currentTrack,
                Name = sample.Name + "(" + _currentTrack.Samples.Count + ")"
            };
            this.ToDoDb.Samples.InsertOnSubmit(duplicateSample);
            this.ToDoDb.SubmitChanges();
            _currentTrack.Samples.Insert(index, duplicateSample);

            foreach (var note in sample.Notes)
            {
                var duplicateNote = new ToDoNote()
                {
                    Duration = note.Duration,
                    MidiNumber = note.MidiNumber,
                    Position = note.Position,
                    Tact = (duplicateSample.InitialTact - sample.InitialTact) + note.Tact,
                    SampleRef = duplicateSample
                };
                ToDoDb.Notes.InsertOnSubmit(duplicateNote);
                ToDoDb.SubmitChanges();
                duplicateSample.Notes.Add(duplicateNote);
            }
        }
        
        /// <summary>
        /// Rename Sample
        /// </summary>
        /// <param name="newName">New name of Sample</param>
        public void RenameSampleTo(string newName)
        {
            _onRenameSample.Name = newName;
            this.ToDoDb.SubmitChanges();
        }

        /// <summary>
        /// Fix bugs with null value of reference
        /// </summary>
        public void RestoreReferences(ToDoSample movedSample)
        {
            movedSample.TrackRef = _currentTrack;
        }

        /// <summary>
        /// Query database and load the information for project
        /// </summary>
        /// <param name="trackId">ID of loading project</param>
        public override void LoadData(int trackId)
        {
            var options = new DataLoadOptions();
            options.AssociateWith<ToDoTrack>(x => x.Samples.OrderBy(y => y.InitialTact));
            this.ToDoDb.LoadOptions = options;
            this._currentTrack = this.ToDoDb.Tracks.FirstOrDefault(x => x.Id == trackId);
        }
    }
}
