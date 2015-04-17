//-----------------------------------------------------------------------
// <copyright file="PianoRollManager.cs" company="Igor Golopolosov">
//     Copyright (c) Igor Golopolosov. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Linq;
using Microsoft.Phone.Controls;
using WarehouseOfMusic.ViewModels;

namespace WarehouseOfMusic.Managers
{
    using System.Collections.ObjectModel;

    public class PianoRollManager
    {
        private ObservableCollection<KeyContext> _keys;

        public PianoRollManager()
        {
            _keys = new ObservableCollection<KeyContext>();
            var key = Key.G10;
            _keys.Add(new KeyContext(key));
            while (key != Key.C0)
            {
                key = (Key)((int)key - 1);
                _keys.Add(new KeyContext(key));
            } 
        }

        public ObservableCollection<KeyContext> Keys
        {
            get { return _keys; }
        }

        public LongListSelector List { get; set; }
    }
}