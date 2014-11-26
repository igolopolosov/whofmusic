//-----------------------------------------------------------------------
// <copyright file="ToDoTrack.cs" company="Igor Golopolosov">
//     Copyright (c) Igor Golopolosov. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace WarehouseOfMusic.Model
{
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
        /// Entity reference, to identify the ToDoProject "storage" table
        /// </summary>
        private EntityRef<ToDoProject> _project;

        /// <summary>
        /// Version column aids update performance.
        /// </summary>
        [Column(IsVersion = true)] private Binary _version;

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
        [Association(Storage = "_project", ThisKey = "ProjectId", OtherKey = "Id", IsForeignKey = true)]
        public ToDoProject Project
        {
            get
            {
                return this._project.Entity; 
            }

            set
            {
                this.NotifyPropertyChanging("Project");
                this._project.Entity = value;

                if (value != null)
                {
                    this.ProjectId = value.Id;
                }

                this.NotifyPropertyChanging("Project");
            }
        }

        /// <summary>
        /// Gets or sets internal column for the associated ToDoProject ID value
        /// </summary>
        [Column]
        internal int ProjectId { get; set; }

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
