//-----------------------------------------------------------------------
// <copyright file="TrackEditorContext.cs">
//     Copyright (c) Igor Golopolosov. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
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
        public ObservableCollection<PianoRollContext> Tacts;

        /// <summary>
        /// Add note to current track
        /// </summary>
        private ToDoNote OnAddedNote(object sender, NoteEventArgs e)
        {
            var note = e.Note;
            note.TrackRef = _currentTrack;
            _toDoDb.Notes.InsertOnSubmit(note);
            _toDoDb.SubmitChanges();
            CurrentTrack.Notes.Add(note);
            return note;
        }

        /// <summary>
        /// Add note to current track
        /// </summary>
        private ToDoNote OnDeletedNote(object sender, NoteEventArgs e)
        {
            var note = e.Note;
            CurrentTrack.Notes.Remove(note);
            _toDoDb.Notes.DeleteOnSubmit(note);
            _toDoDb.SubmitChanges();
            //// [Hack] Restore references
            _currentTrack.ProjectRef = _toDoDb.Projects.First(x => x.Id == _currentTrack.ProjectId);
            return null;
        }

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
            Tacts = new ObservableCollection<PianoRollContext>();
            for (var i = 1; i < 5; i++)
            {
                var tact = new PianoRollContext(i);
                tact.AddedNote += OnAddedNote;
                tact.DeletedNote += OnDeletedNote;
                Tacts.Add(tact);
            }
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
    }
}
