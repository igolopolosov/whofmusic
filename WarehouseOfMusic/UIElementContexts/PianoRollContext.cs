namespace WarehouseOfMusic.UIElementContexts
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using Enums;
    using EventArgs;
    using Models;
    using Resources;

    /// <summary>
    /// Describes template of tact
    /// </summary>
    public class PianoRollContext : INotifyPropertyChanged
    {
        /// <summary>
        /// Number of tact
        /// </summary>
        private readonly int _tactNumber;

        /// <summary>
        /// Collection of all availiable keys
        /// </summary>
        private ObservableCollection<KeyContext> _keys;

        /// <summary>
        /// Name of tact
        /// </summary>
        public string TactName
        {
            get { return _tactNumber + " " + AppResources.TactString; }
        }

        /// <summary>
        /// Number of tact
        /// </summary>
        public int TactNumber
        {
            get { return _tactNumber; }
        }

        /// <summary>
        /// Current value of element located at the top of view
        /// </summary>
        public static Key TopKey = Key.G5;

        /// <summary>
        /// Current value of note duration
        /// </summary>
        public static byte NoteDuration = 4;

        /// <summary>
        /// Collection of all availiable keys
        /// </summary>
        public ObservableCollection<KeyContext> Keys
        {
            get { return _keys; }
        }

        /// <summary>
        /// Create instanse of TactContext
        /// </summary>
        /// <param name="number">Number of tact</param>
        public PianoRollContext(int number)
        {
            _tactNumber = number;
            _keys = new ObservableCollection<KeyContext>();
            var key = Key.B7;
            _keys.Add(new KeyContext(key));
            while (key != Key.C2)
            {
                key = (Key)((int)key - 1);
                _keys.Add(new KeyContext(key));
            }
        }

        /// <summary>
        /// Add new note to the tact
        /// </summary>
        /// <param name="keyValue">Midi number of note</param>
        /// <param name="tactPostition">Position in the tact</param>
        public void AddNote(byte keyValue, byte tactPostition)
        {
            var note = new ToDoNote
            {
                Duration = NoteDuration,
                MidiNumber = keyValue,
                Tact = TactNumber,
                Position = tactPostition
            };
            
            if (AddedNote == null) return;
            var compeletedNote = AddedNote(this, new NoteEventArgs{ Note = note });
            var keyContext = _keys.First(x => x.Value == (Key) keyValue);
            keyContext.Notes.Add(compeletedNote);
        }

        /// <summary>
        /// Delete note from the tact
        /// </summary>
        /// <param name="toDoNote"></param>
        public void DeleteNote(ToDoNote toDoNote)
        {
            var keyContext = _keys.First(x => x.Value == (Key)toDoNote.MidiNumber);
            keyContext.Notes.Remove(toDoNote);
            if (DeletedNote == null) return;
            DeletedNote(this, new NoteEventArgs {Note = toDoNote});
        }
        
        #region INotifyPropertyChanged Members

        /// <summary>
        /// Event of property changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

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

        public delegate ToDoNote NoteChangedHandler(object sender, NoteEventArgs e);

        /// <summary>
        /// Happend, when user add new note on piano roll,
        /// return ID of added note
        /// </summary>
        public event NoteChangedHandler AddedNote;
        /// <summary>
        /// Happend, when user delete note from piano roll
        /// </summary>
        public event NoteChangedHandler DeletedNote;
    }
}