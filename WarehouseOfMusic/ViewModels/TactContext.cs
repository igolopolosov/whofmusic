using System.Collections.ObjectModel;
using System.ComponentModel;
using WarehouseOfMusic.Resources;

namespace WarehouseOfMusic.ViewModels
{
    /// <summary>
    /// Describes template of tact
    /// </summary>
    public class TactContext : INotifyPropertyChanged
    {
        private int _number;

        public string Name
        {
            get { return _number + " " + AppResources.TactString; }
        }

        public int Number
        {
            get { return _number; }
        }

        public ObservableCollection<KeyContext> PianoKeys { get; set; }

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