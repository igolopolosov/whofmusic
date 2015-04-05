//-----------------------------------------------------------------------
// <copyright file="PlayerManager.cs" company="Igor Golopolosov">
//     Copyright (c) Igor Golopolosov. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace WarehouseOfMusic
{
    using System.Collections.Generic;
    using System.Threading;
    using Models;
    using WomAudioComponent;
    
    /// <summary>
    /// Supports to play tracks
    /// </summary>
    public class PlayerManager
    {
        #region Private fields
        /// <summary>
        /// Audio API 
        /// </summary>
        private readonly AudioController _audioController;

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
            _audioController =  new AudioController();
            _audioController.CreatePatch();
        }
        #endregion
        
        /// <summary>
        /// Start playing samples
        /// </summary>
        public void Play()
        {
            _audioController.Start();
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
                        var args = new KeyPressedArgs
                        {
                            IsPressed = true,
                            KeyNumber = note.MidiNumber
                        };
                        this._audioController.KeyIsPressedChanged(this, args);
                    }

                    if (note.TactPosition + note.Duration == _stepControl)
                    {
                        var args = new KeyPressedArgs
                        {
                            IsPressed = false,
                            KeyNumber = note.MidiNumber
                        };
                        this._audioController.KeyIsPressedChanged(this, args);
                    }
                }
            }
            _stepControl++;
        }
    }
}
