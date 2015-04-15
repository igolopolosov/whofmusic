//-----------------------------------------------------------------------
// <copyright file="PianoRollManager.cs" company="Igor Golopolosov">
//     Copyright (c) Igor Golopolosov. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Linq;
using WarehouseOfMusic.ViewModels;

namespace WarehouseOfMusic.Managers
{
    using System.Collections.ObjectModel;

    public class PianoRollManager
    {
        private ObservableCollection<KeyContext> _keys;

        public PianoRollManager()
        {
            _keys = new ObservableCollection<KeyContext>
            {
                new KeyContext(Key.C4),
                new KeyContext(Key.Db4),
                new KeyContext(Key.D4),
                new KeyContext(Key.Eb4),
                new KeyContext(Key.E4),
                new KeyContext(Key.F4),
                new KeyContext(Key.Gb4),
                new KeyContext(Key.G4),
                new KeyContext(Key.Ab4),
                new KeyContext(Key.A4),
                new KeyContext(Key.Bb4),
                new KeyContext(Key.B4),
                new KeyContext(Key.C5),
                new KeyContext(Key.Db5),
                new KeyContext(Key.D5),
                new KeyContext(Key.Eb5),
                new KeyContext(Key.E5),
                new KeyContext(Key.F5),
                new KeyContext(Key.Gb5),
                new KeyContext(Key.G5),
                new KeyContext(Key.Ab5),
                new KeyContext(Key.A5),
                new KeyContext(Key.Bb5),
                new KeyContext(Key.B5)
            };
        }

        public ObservableCollection<KeyContext> Keys
        {
            get { return _keys; }
        }

        /// <summary>
        /// Delete last element of keys collection and add on top of collection element from enum Key
        /// </summary>
        public void ScrollUp()
        {
            if (_keys.First().Value == Key.C0) return;
            _keys.RemoveAt(_keys.Count - 1);
            var newKey = new KeyContext((Key)((int) _keys.First().Value - 1));
            _keys.Insert(0, newKey);
        }

        /// <summary>
        /// Delete first element of keys collection and add at the bottom of collection element from enum Key
        /// </summary>
        public void ScrollDown()
        {
            if (_keys.Last().Value == Key.G10) return;
            _keys.RemoveAt(0);
            var newKey = new KeyContext((Key)((int)_keys.Last().Value+ 1));
            _keys.Add(newKey);
        }
    }
}