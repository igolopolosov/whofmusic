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
        public void AddSample()
        {
            var lastSample = _currentTrack.Samples.LastOrDefault();
            if (lastSample == null) return;
            var sample = new ToDoSample
            {
                InitialTact = lastSample.InitialTact + lastSample.Size,
                Size = 4,
                TrackRef = _currentTrack
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
        }

        /// <summary>
        /// Query database and load the information for project
        /// </summary>
        /// <param name="projectId">ID of loading project</param>
        public void LoadTrackFromDatabase(int trackId)
        {
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

        
    }
}
