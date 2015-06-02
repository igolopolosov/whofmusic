//-----------------------------------------------------------------------
// <copyright file="ToDoNote.cs">
//     Copyright (c) Igor Golopolosov. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace WarehouseOfMusic.Models
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
        private byte _duration;

        /// <summary>
        /// Duration of note
        /// </summary>
        private byte _midiNumber;

        /// <summary>
        /// Note contains in the tact
        /// </summary>
        private int _tact;

        /// <summary>
        /// Position of note in a tact
        /// </summary>
        private byte _position;

        /// <summary>
        /// Entity reference, to identify the ToDoTrack "storage" table
        /// </summary>
        private EntityRef<ToDoSample> _sampleRef;

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
        /// Gets or sets duration of note
        /// </summary>
        [Column]
        public byte Duration
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
        /// Caclculate tact in which note stops sound
        /// </summary>
        public int EndTact
        {
            get
            {
                if (_position + _duration > 15) return this._tact + 1;
                return this._tact;
            }
        }

        /// <summary>
        /// Caclculate position in which note stops sound
        /// </summary>
        public int EndPosition
        {
            get
            {
                if (_position + _duration > 15) return _position + _duration - 16;
                else return _position + _duration;
            }
        }

        /// <summary>
        /// Gets or sets duration of note
        /// </summary>
        [Column]
        public byte MidiNumber
        {
            get
            {
                return this._midiNumber;
            }

            set
            {
                if (this._midiNumber != value)
                {
                    this.NotifyPropertyChanging("MidiNumber");
                    this._midiNumber = value;
                    this.NotifyPropertyChanged("MidiNumber");
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
        /// Gets or sets position of in a tact. From 0 to 15
        /// </summary>
        [Column]
        public byte Position
        {
            get
            {
                return this._position;
            }

            set
            {
                if (this._position != value)
                {
                    this.NotifyPropertyChanging("Position");
                    this._position = value;
                    this.NotifyPropertyChanged("Position");
                }
            }
        }

        /// <summary>
        /// Gets or sets association, to describe the relationship between this key and that "storage" table
        /// </summary>
        [Association(Storage = "_sampleRef", ThisKey = "SampleId", OtherKey = "Id", IsForeignKey = true)]
        public ToDoSample SampleRef
        {
            get
            {
                return this._sampleRef.Entity; 
            }

            set
            {
                this.NotifyPropertyChanging("SampleRef");
                this._sampleRef.Entity = value;

                if (value != null)
                {
                    this.SampleId = value.Id;
                }

                this.NotifyPropertyChanging("SampleRef");
            }
        }

        /// <summary>
        /// Gets or sets internal column for the associated ToDoTrack ID value
        /// </summary>
        [Column]
        internal int SampleId { get; set; }

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
