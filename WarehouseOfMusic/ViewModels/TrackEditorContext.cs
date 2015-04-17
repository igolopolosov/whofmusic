//-----------------------------------------------------------------------
// <copyright file="TrackEditorContext.cs">
//     Copyright (c) Igor Golopolosov. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.ObjectModel;

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

        /// <summary>
        /// Write changes in the data context to the database.
        /// </summary>
        public void SaveChangesToDb()
        {
            this._toDoDb.SubmitChanges();
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
