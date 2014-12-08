//-----------------------------------------------------------------------
// <copyright file="PlayerManager.cs" company="Igor Golopolosov">
//     Copyright (c) Igor Golopolosov. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace WarehouseOfMusic
{
    using System;
    using System.Threading;
    using Model;
    using WOMAudioComponent;

    /// <summary>
    /// Supports to play tracks
    /// </summary>
    public class PlayerManager
    {
        #region Private fields
        /// <summary>
        /// Audio API 
        /// </summary>
        private AudioController _audioController;

        /// <summary>
        /// Controls step of timer
        /// </summary>
        private int _stepControl = 1;

        /// <summary>
        /// Timer for playing trakcs
        /// </summary>
        private Timer _playerTimer;

        /// <summary>
        /// Project on play
        /// </summary>
        private List<ToDoTrack> _onPlayTracks;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerManager" /> class.
        /// </summary>
        /// <param name="onPlayProject">Project on play</param>
        public PlayerManager(ToDoProject onPlayProject)
        {
            _onPlayTracks = new List<ToDoTrack>(onPlayProject.Tracks);
            _audioController = new AudioController(7);
            this._audioController.Start();
        }
        #endregion
        
        /// <summary>
        /// Start playing samples
        /// </summary>
        public void Play()
        {
            this._playerTimer = new Timer(Step, null, 100, 25);
        }

        /// <summary>
        /// Stop playing samples
        /// </summary>
        public void Stop()
        {
            this._playerTimer.Dispose();
            _audioController.Stop();
        }

        /// <summary>
        /// Call on every step of timer
        /// </summary>
        /// <returns>No things</returns>
        private void Step(object state)
        {
            foreach (var track in _onPlayTracks)
            {
                foreach (var note in track.Notes)
                {
                    if (note.TactPosition == _stepControl)
                    {
                        this._audioController.NoteOn(note.MidiNumber, note.MidiNumber%1000);
                    }

                    if (note.TactPosition + note.Duration == _stepControl)
                    {
                        this._audioController.NoteOff(note.MidiNumber);
                    }
                }
            }
            _stepControl++;
        }
    }
}
