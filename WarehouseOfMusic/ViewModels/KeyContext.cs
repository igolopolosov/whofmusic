namespace WarehouseOfMusic.ViewModels
{
    using System.ComponentModel;
    using System.Windows.Media;

    public class KeyContext : INotifyPropertyChanged
    {
        private Key _key;

        public Color Color
        {
            get
            {
                var keyNumber = (int)_key % 12;
                Color color;
                switch (keyNumber)
                {
                    case (int)Key.C0:
                    case (int)Key.D0:
                    case (int)Key.E0:
                    case (int)Key.F0:
                    case (int)Key.G0:
                    case (int)Key.A0:
                    case (int)Key.B0:
                        color = Colors.White;
                        break;
                    default:
                        color = Colors.Black;
                        break;
                }
                return color;
            }
        }

        public Color InversedColor
        {
            get { return Color == Colors.White ? Colors.Black : Colors.White; }
        }
        
        public string Name
        {
            get { return _key.ToString(); }
        }

        public Key Value
        {
            get { return _key; }
        }
        

        public KeyContext(Key key)
        {
            _key = key;
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