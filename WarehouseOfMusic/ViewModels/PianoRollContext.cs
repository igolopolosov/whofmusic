//-----------------------------------------------------------------------
// <copyright file="PianoRollManager.cs" company="Igor Golopolosov">
//     Copyright (c) Igor Golopolosov. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace WarehouseOfMusic.ViewModels
{
    using System.Collections.ObjectModel;

    public class PianoRollContext
    {
        /// <summary>
        /// Collection of all availiable keys
        /// </summary>
        private ObservableCollection<KeyContext> _keys;

        /// <summary>
        /// Create new instance of PianoRollContext
        /// </summary>
        public PianoRollContext()
        {
            _keys = new ObservableCollection<KeyContext>();
            var key = Key.B7;
            _keys.Add(new KeyContext(key));
            while (key != Key.C2)
            {
                key = (Key)((int)key - 1);
                _keys.Add(new KeyContext(key));
            }
            TopKey = Key.G5;
            NoteDuration = 2;
        }

        /// <summary>
        /// Collection of all availiable keys
        /// </summary>
        public ObservableCollection<KeyContext> Keys
        {
            get { return _keys; }
        }

        /// <summary>
        /// Current value of element located at the top of view
        /// </summary>
        public Key TopKey { get; set; }

        /// <summary>
        /// Current value of note duration
        /// </summary>
        public byte NoteDuration { get; set; }
    }
}