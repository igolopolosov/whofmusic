//-----------------------------------------------------------------------
// <copyright file="ToDoViewModel.cs" company="Igor Golopolosov">
//     Copyright (c) Igor Golopolosov. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Linq;

namespace WarehouseOfMusic
{
    using System.Collections.Generic;
    using Model;

    /// <summary>
    /// Provides information from DB to Keyboard
    /// </summary>
    public class PlayerManager
    {
        /// <summary>
        /// Keep data fro tracks
        /// </summary>
        private List<List<ToDoNote>> _tracksOnPlay;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerManager" /> class.
        /// </summary>
        /// <param name="TracksOnPlay">List of tracks</param>
        public PlayerManager(IEnumerable<ToDoTrack> TracksOnPlay)
        {
            _tracksOnPlay = new List<List<ToDoNote>>();
            foreach (var track in TracksOnPlay)
            {
                _tracksOnPlay.Add(track.Notes.ToList());
            }
        }

        /// <summary>
        /// Start playing samples
        /// </summary>
        public void Play()
        {
        }
    }
}
