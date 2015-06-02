namespace WarehouseOfMusic.UIElementContexts
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows.Media;
    using Enums;
    using Models;
    
    public class KeyContext : INotifyPropertyChanged
    {
        private Key _key;

        private ObservableCollection<ToDoNote> _notes;

        /// <summary>
        /// Color for background of piano key
        /// </summary>
        public Color Color
        {
            get
            {
                var keyNumber = (int)_key % 12 + 24;
                Color color;
                switch (keyNumber)
                {
                    case (int)Key.C2:
                    case (int)Key.D2:
                    case (int)Key.E2:
                    case (int)Key.F2:
                    case (int)Key.G2:
                    case (int)Key.A2:
                    case (int)Key.B2:
                        color = Colors.White;
                        break;
                    default:
                        color = Colors.Black;
                        break;
                }
                return color;
            }
        }

        /// <summary>
        /// /// <summary>
        /// Color for foreground of piano key name
        /// </summary>
        /// </summary>
        public Color InversedColor
        {
            get { return Color == Colors.White ? Colors.Black : Colors.White; }
        }

        /// <summary>
        /// Name of piano key
        /// </summary>
        public string Name
        {
            get { return _key.ToString(); }
        }

        /// <summary>
        /// Notes with same key.Value
        /// </summary>
        public ObservableCollection<ToDoNote> Notes
        {
            get { return _notes; }
            set
            {
                _notes = value;
                NotifyPropertyChanged("Notes");
            }
        }

        /// <summary>
        /// Midi number
        /// </summary>
        public Key Value
        {
            get { return _key; }
        }

        public KeyContext(Key key)
        {
            _key = key;
            _notes = new ObservableCollection<ToDoNote>();
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
    }
}