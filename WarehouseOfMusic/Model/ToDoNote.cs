﻿//-----------------------------------------------------------------------
// <copyright file="ToDoNote.cs" company="Igor Golopolosov">
//     Copyright (c) Igor Golopolosov. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace WarehouseOfMusic.Model
{
    using System.ComponentModel;
    using System.Data.Linq;
    using System.Data.Linq.Mapping;

    /// <summary>
    /// Table of notes are containing in a track
    /// </summary>
    [Table]
    public class ToDoNote : INotifyPropertyChanged, INotifyPropertyChanging
    {
        /// <summary>
        /// ID of note
        /// </summary>
        private int _id;

        /// <summary>
        /// Duration of note
        /// </summary>
        private string _duration;

        /// <summary>
        /// Note contains in the tact
        /// </summary>
        private int _tact;

        /// <summary>
        /// Position of note in a tact
        /// </summary>
        private string _tactposition;

        /// <summary>
        /// Name of note
        /// </summary>
        private string _title;

        /// <summary>
        /// Entity reference, to identify the ToDoTrack "storage" table
        /// </summary>
        private EntityRef<ToDoTrack> _trackRef;

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
        /// Gets or sets ID of note
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
        /// Gets or sets tact, which contains note
        /// </summary>
        [Column]
        public int Tact
        {
            get
            {
                return this._tact;
            }

            set
            {
                if (this._tact != value)
                {
                    this.NotifyPropertyChanging("Tact");
                    this._tact = value;
                    this.NotifyPropertyChanged("Tact");
                }
            }
        }

        /// <summary>
        /// Gets or sets position of in a tact
        /// </summary>
        [Column]
        public string TactPosition
        {
            get
            {
                return this._tactposition;
            }

            set
            {
                if (this._tactposition != value)
                {
                    this.NotifyPropertyChanging("TactPosition");
                    this._tactposition = value;
                    this.NotifyPropertyChanged("TactPosition");
                }
            }
        }

        /// <summary>
        /// Gets or sets duration of note
        /// </summary>
        [Column]
        public string Duration
        {
            get
            {
                return this._duration;
            }

            set
            {
                if (this._duration != value)
                {
                    this.NotifyPropertyChanging("Duration");
                    this._duration = value;
                    this.NotifyPropertyChanged("Duration");
                }
            }
        }

        /// <summary>
        /// Gets or sets Name of note
        /// </summary>
        [Column]
        public string Title
        {
            get
            {
                return this._title; 
            }

            set
            {
                if (this._title != value)
                {
                    this.NotifyPropertyChanging("Name");
                    this._title = value;
                    this.NotifyPropertyChanged("Name");
                }
            }
        }

        /// <summary>
        /// Gets or sets association, to describe the relationship between this key and that "storage" table
        /// </summary>
        [Association(Storage = "_trackRef", ThisKey = "TrackId", OtherKey = "Id", IsForeignKey = true)]
        public ToDoTrack TrackRef
        {
            get
            {
                return this._trackRef.Entity; 
            }

            set
            {
                this.NotifyPropertyChanging("TrackRef");
                this._trackRef.Entity = value;

                if (value != null)
                {
                    this.TrackId = value.Id;
                }

                this.NotifyPropertyChanging("TrackRef");
            }
        }

        /// <summary>
        /// Gets or sets internal column for the associated ToDoTrack ID value
        /// </summary>
        [Column]
        internal int TrackId { get; set; }

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