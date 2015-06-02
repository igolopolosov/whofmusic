//-----------------------------------------------------------------------
// <copyright file="TrackEditorContext.cs">
//     Copyright (c) Igor Golopolosov. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace WarehouseOfMusic.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using EventArgs;
    using Models;
    using UIElementContexts;

    /// <summary>
    /// ViewModel for project editor page
    /// </summary>
    public class SampleEditorContext : Context
    {
        /// <summary>
        /// Represent notes of each separate tact in sample
        /// </summary>
        public ObservableCollection<PianoRollContext> Tacts;

        /// <summary>
        /// Currently editing project
        /// </summary>
        private ToDoSample _currentSample;

        
        /// <summary>
        /// Initializes a new instance of the <see cref="SampleEditorContext" /> class.
        /// Class constructor, create the data context object.
        /// </summary>
        /// <param name="toDoDbConnectionString">Path to connect to database</param>
        public SampleEditorContext(string toDoDbConnectionString) : base(toDoDbConnectionString)
        {
        }
        
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
        /// <param name="sampleId">ID of loading sample</param>
        public override void LoadData(int sampleId)
        {
            this._currentSample = this.ToDoDb.Samples.FirstOrDefault(x => x.Id == sampleId);
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
                            (x => x.MidiNumber == (byte) key.Value && x.Tact == tact.TactNumber));
                }
                Tacts.Add(tact);
            }
        }

        /// <summary>
        /// Add note to current track
        /// </summary>
        private ToDoNote OnAddedNote(object sender, NoteEventArgs e)
        {
            var note = e.Note;
            note.SampleRef = _currentSample;
            ToDoDb.Notes.InsertOnSubmit(note);
            ToDoDb.SubmitChanges();
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
            ToDoDb.Notes.DeleteOnSubmit(note);
            ToDoDb.SubmitChanges();
            //// Restore references
            _currentSample.TrackRef = ToDoDb.Tracks.First(x => x.Id == _currentSample.TrackId);
            return null;
        }
    }
}
