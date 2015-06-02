//-----------------------------------------------------------------------
// <copyright file="ToDoinstrument.cs">
//     Copyright (c) Igor Golopolosov. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace WarehouseOfMusic.Models
{
    using System.ComponentModel;
    using System.Data.Linq;
    using System.Data.Linq.Mapping;
    using WomAudioComponent;

    /// <summary>
    /// Table of tracks are containing in a project
    /// </summary>
    [Table]
    public class ToDoInstrument : INotifyPropertyChanged, INotifyPropertyChanging
    {
        /// <summary>
        /// ID of sample
        /// </summary>
        private int _id;

        /// <summary>
        /// Number of first tact in the sample
        /// </summary>
        private WaveformType _waveform;

        /// <summary>
        /// Name of the sample
        /// </summary>
        private string _name;

        /// <summary>
        /// Entity reference, to identify the ToDoProject "storage" table
        /// </summary>
        private EntityRef<ToDoTrack> _trackRef;

        /// <summary>
        /// Version column aids update performance.
        /// </summary>
        [Column(IsVersion = true)]
        private Binary _version;

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
        /// Gets or sets name of the sample
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
        /// Gets or sets waveform
        /// </summary>
        [Column]
        public WaveformType Waveform
        {
            get
            {
                return this._waveform;
            }

            set
            {
                if (this._waveform != value)
                {
                    this.NotifyPropertyChanging("Waveform");
                    this._waveform = value;
                    this.NotifyPropertyChanged("Waveform");
                }
            }
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
