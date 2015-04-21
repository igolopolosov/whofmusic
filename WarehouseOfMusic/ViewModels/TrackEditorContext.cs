//-----------------------------------------------------------------------
// <copyright file="TrackEditorContext.cs">
//     Copyright (c) Igor Golopolosov. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Data.Linq;

namespace WarehouseOfMusic.ViewModels
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using Models;
    using UIElementContexts;

    /// <summary>
    /// ViewModel for project editor page
    /// </summary>
    public class TrackEditorContext : INotifyPropertyChanged
    {
        public ObservableCollection<TactContext> Tacts = new ObservableCollection<TactContext>
        {
            new TactContext(1),
            new TactContext(2),
            new TactContext(3),
            new TactContext(4)
        }; 
        
        #region DataBaseLayer

        /// <summary>
        /// LINQ to SQL data context for the local database.
        /// </summary>
        private readonly ToDoDataContext _toDoDb;
        
        /// <summary>
        /// Currently editing project
        /// </summary>
        private ToDoTrack _currentTrack;

        
        /// <summary>
        /// Initializes a new instance of the <see cref="TrackEditorContext" /> class.
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
                this.NotifyPropertyChanged("CurrentProject");
            }
        }

        /// <summary>
        /// Query database and load the information for project
        /// </summary>
        /// <param name="trackId">ID of loading project</param>
        public void LoadTrackFromDatabase(int trackId)
        {
            this._currentTrack = this._toDoDb.Tracks.FirstOrDefault(x => x.Id == trackId);
        }
        #endregion

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
        /// Add note to current track
        /// </summary>
        /// <param name="note">New note</param>
        public void AddNote(ToDoNote note)
        {
            note.TrackRef = _currentTrack;
            _toDoDb.Notes.InsertOnSubmit(note);
            _toDoDb.SubmitChanges();
            CurrentTrack.Notes.Add(note);
        }

        /// <summary>
        /// Delete note from current track
        /// </summary>
        /// <param name="id">Id of deleting note</param>
        public void DeleteNote(int id)
        {
            var note = _currentTrack.Notes.First(x => x.Id == id);
            CurrentTrack.Notes.Remove(note);
            _toDoDb.Notes.DeleteOnSubmit(note);
            _toDoDb.SubmitChanges();
            //// [Hack] Restore references
            _currentTrack.ProjectRef = _toDoDb.Projects.First(x => x.Id == _currentTrack.ProjectId);
        }
    }
}
