//-----------------------------------------------------------------------
// <copyright file="ToDoProject.cs" company="github.com/usehotkey">
//     Free code of the application. No copyrights.
// </copyright>
// <author>Igor Golopolosov</author>
//-----------------------------------------------------------------------

namespace WarehouseOfMusic.Model
{
    using System;
    using System.ComponentModel;
    using System.Data.Linq;
    using System.Data.Linq.Mapping;
    
    /// <summary>
    /// Table of music projects
    /// </summary>
    [Table]
    public class ToDoProject : INotifyPropertyChanged, INotifyPropertyChanging
    {
        #region Fields of project
        /// <summary>
        /// ID of the project
        /// </summary>
        private int _id;

        /// <summary>
        /// Name of the project
        /// </summary>
        private string _name;

        /// <summary>
        /// Entity set for the collection side of the relationship.
        /// </summary>
        private EntitySet<ToDoTrack> _tracks;

        /// <summary>
        /// Version column aids update performance.
        /// </summary>
        [Column(IsVersion = true)]
        private Binary _version; 
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ToDoProject" /> class.
        /// Assign handlers for the add and remove operations, respectively.
        /// </summary>
        public ToDoProject()
        {
            this._tracks = new EntitySet<ToDoTrack>(
                new Action<ToDoTrack>(this.AttachToDoTrack),
                new Action<ToDoTrack>(this.DetachToDoTrack));
        }

        /// <summary>
        /// Event of property changed 
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Event of property changing
        /// </summary>
        public event PropertyChangingEventHandler PropertyChanging;

        /// <summary>
        /// Gets or sets ID of the project
        /// </summary>
        [Column(DbType = "INT NOT NULL IDENTITY", IsDbGenerated = true, IsPrimaryKey = true)]
        public int Id
        {
            get
            {
                return this._id; 
            }

            set
            {
                this.NotifyPropertyChanging("Id");
                this._id = value;
                this.NotifyPropertyChanged("Id");
            }
        }

        /// <summary>
        /// Gets or sets Name of the project
        /// </summary>
        [Column]
        public string Name
        {
            get
            {
                return this._name; 
            }

            set
            {
                this.NotifyPropertyChanging("Name");
                this._name = value;
                this.NotifyPropertyChanged("Name");
            }
        }

        /// <summary>
        /// Gets or sets entity set for the collection side of the relationship.
        /// </summary>
        [Association(Storage = "_tracks", OtherKey = "_projectId", ThisKey = "Id")]
        public EntitySet<ToDoTrack> ToDoTrack
        {
            get { return this._tracks; }
            set { this._tracks.Assign(value); }
        }
        
        /// <summary>
        /// Called during an add operation
        /// </summary>
        /// <param name="toDo">Track on adding</param>
        private void AttachToDoTrack(ToDoTrack toDo)
        {
            this.NotifyPropertyChanging("ToDoTrack");
            toDo.Project = this;
        }

        /// <summary>
        ///  Called during a remove operation
        /// </summary>
        /// <param name="toDo">Track on removing</param>
        private void DetachToDoTrack(ToDoTrack toDo)
        {
            this.NotifyPropertyChanging("ToDoTrack");
            toDo.Project = null;
        }

        #region INotifyProperty event methods

        /// <summary>
        /// Used to notify that a property changed
        /// </summary>
        /// <param name="propertyName">Property on changing</param>
        private void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Used to notify that a property is about to change
        /// </summary>
        /// <param name="propertyName">Property on changing</param>
        private void NotifyPropertyChanging(string propertyName)
        {
            if (this.PropertyChanging != null)
            {
                this.PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        #endregion
    }
}
