namespace WarehouseOfMusic.UIElementContexts
{
    using System.ComponentModel;
    using Resources;

    /// <summary>
    /// Describes template of tact
    /// </summary>
    public class TactContext : INotifyPropertyChanged
    {
        /// <summary>
        /// Number of tact
        /// </summary>
        private int _number;

        private static PianoRollContext _pianoRollManager = new PianoRollContext();

        /// <summary>
        /// Name of tact
        /// </summary>
        public string Name
        {
            get { return _number + " " + AppResources.TactString; }
        }

        /// <summary>
        /// Number of tact
        /// </summary>
        public int Number
        {
            get { return _number; }
        }

        /// <summary>
        /// Grid of piano keys
        /// </summary>
        public static PianoRollContext PianoRollContext
        {
            get { return _pianoRollManager; }
        }

        /// <summary>
        /// Create instanse of TactContext
        /// </summary>
        /// <param name="number">Number of tact</param>
        public TactContext(int number)
        {
            _number = number;
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