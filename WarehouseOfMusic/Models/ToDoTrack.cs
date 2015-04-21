//-----------------------------------------------------------------------
// <copyright file="ToDoTrack.cs">
//     Copyright (c) Igor Golopolosov. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace WarehouseOfMusic.Models
{
    using System;
    using System.ComponentModel;
    using System.Data.Linq;
    using System.Data.Linq.Mapping;

    /// <summary>
    /// Table of tracks are containing in a project
    /// </summary>
    [Table]
    public class ToDoTrack : INotifyPropertyChanged, INotifyPropertyChanging
    {
        /// <summary>
        /// ID of track
        /// </summary>
        private int _id;

        /// <summary>
        /// Name of track
        /// </summary>
        private string _name;

        /// <summary>
        /// Entity set for the collection side of the relationship.
        /// </summary>
        private EntitySet<ToDoNote> _notes;

        /// <summary>
        /// Entity reference, to identify the ToDoProject "storage" table
        /// </summary>
        private EntityRef<ToDoProject> _projectRef;

        /// <summary>
        /// Version column aids update performance.
        /// </summary>
        [Column(IsVersion = true)] private Binary _version;

        /// <summary>
        /// Initializes a new instance of the <see cref="ToDoTrack" /> class.
        /// Assign handlers for the add and remove operations, respectively.
        /// </summary>
        public ToDoTrack()
        {
            this._notes = new EntitySet<ToDoNote>(
                new Action<ToDoNote>(this.AttachToDoNote),
                new Action<ToDoNote>(this.DetachToDoNote));
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
        /// Gets or sets ID of track
        /// </summary>
        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public int Id
        {
            get
            {
                return this._id; 
            }

            set
            {
                if (this._id != value)
                {
                    this.NotifyPropertyChanging("Id");
                    this._id = value;
                    this.NotifyPropertyChanged("Id");
                }
            }
        }

        /// <summary>
        /// Gets or sets Name of track
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
                if (this._name != value)
                {
                    this.NotifyPropertyChanging("Name");
                    this._name = value;
                    this.NotifyPropertyChanged("Name");
                }
            }
        }

        /// <summary>
        /// Gets or sets association, to describe the relationship between this key and that "storage" table
        /// </summary>
        [Association(Storage = "_projectRef", ThisKey = "ProjectId", OtherKey = "Id", IsForeignKey = true)]
        public ToDoProject ProjectRef
        {
            get
            {
                return this._projectRef.Entity; 
            }

            set
            {
                this.NotifyPropertyChanging("ProjectRef");
                this._projectRef.Entity = value;

                if (value != null)
                {
                    this.ProjectId = value.Id;
                }

                this.NotifyPropertyChanging("ProjectRef");
            }
        }

        /// <summary>
        /// Gets or sets entity set for the collection side of the relationship.
        /// </summary>
        [Association(Storage = "_notes", OtherKey = "TrackId", ThisKey = "Id")]
        public EntitySet<ToDoNote> Notes
        {
            get { return this._notes; }
            set { this._notes.Assign(value); }
        }

        /// <summary>
        /// Gets or sets internal column for the associated ToDoProject ID value
        /// </summary>
        [Column]
        internal int ProjectId { get; set; }

        /// <summary>
        /// Called during an add operation
        /// </summary>
        /// <param name="toDo">Note on adding</param>
        private void AttachToDoNote(ToDoNote toDo)
        {
            this.NotifyPropertyChanging("ToDoNote");
            toDo.TrackRef = this;
        }

        /// <summary>
        ///  Called during a remove operation
        /// </summary>
        /// <param name="toDo">Note on removing</param>
        private void DetachToDoNote(ToDoNote toDo)
        {
            this.NotifyPropertyChanging("ToDoNote");
            toDo.TrackRef = null;
        }

        #region INotifyProperty Members
        
        /// <summary>
        /// Used to notify that a property changed
        /// </summary>
        /// <param name="propertyName">Property on changed</param>
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
