//-----------------------------------------------------------------------
// <copyright file="TrackEditorContext.cs">
//     Copyright (c) Igor Golopolosov. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

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
    public class SampleEditorContext : INotifyPropertyChanged
    {
        /// <summary>
        /// Represent notes of each separate tact in sample
        /// </summary>
        public ObservableCollection<PianoRollContext> Tacts;

        /// <summary>
        /// Add note to current track
        /// </summary>
        private ToDoNote OnAddedNote(object sender, NoteEventArgs e)
        {
            var note = e.Note;
            note.SampleRef = _currentSample;
            _toDoDb.Notes.InsertOnSubmit(note);
            _toDoDb.SubmitChanges();
            CurrentSample.Notes.Add(note);
            return note;
        }

        /// <summary>
        /// Add note to current track
        /// </summary>
        private ToDoNote OnDeletedNote(object sender, NoteEventArgs e)
        {
            var note = e.Note;
            CurrentSample.Notes.Remove(note);
            _toDoDb.Notes.DeleteOnSubmit(note);
            _toDoDb.SubmitChanges();
            //// [Hack] Restore references
            _currentSample.TrackRef = _toDoDb.Tracks.First(x => x.Id == _currentSample.TrackId);
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
        private ToDoSample _currentSample;

        
        /// <summary>
        /// Initializes a new instance of the <see cref="SampleEditorContext" /> class.
        /// Class constructor, create the data context object.
        /// </summary>
        /// <param name="toDoDbConnectionString">Path to connect to database</param>
        public SampleEditorContext(string toDoDbConnectionString)
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
        public ToDoSample CurrentSample
        {
            get
            {
                return this._currentSample;
            }

            set
            {
                this._currentSample = value;
                this.NotifyPropertyChanged("CurrentSample");
            }
        }

        /// <summary>
        /// Query database and load the information for sample
        /// </summary>
        /// <param name="trackId">ID of loading track</param>
        public void LoadSampleFromDatabase(int trackId)
        {
            var currentTrack = this._toDoDb.Tracks.FirstOrDefault(x => x.Id == trackId);
            if (currentTrack != null) this._currentSample = currentTrack.Samples.FirstOrDefault();

            Tacts = new ObservableCollection<PianoRollContext>();
            if (_currentSample == null) return;
            for (var i = _currentSample.InitialTact; i < _currentSample.InitialTact + _currentSample.Size; i++)
            {
                var tact = new PianoRollContext(i);
                tact.AddedNote += OnAddedNote;
                tact.DeletedNote += OnDeletedNote;
                foreach (var key in tact.Keys)
                {
                    key.Notes =
                        new ObservableCollection<ToDoNote>(_currentSample.Notes.Where
                            (x => x.MidiNumber == (byte) key.Value && x.Tact == tact.Number));
                }
                Tacts.Add(tact);
            }
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
