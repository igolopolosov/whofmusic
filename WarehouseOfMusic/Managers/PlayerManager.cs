//-----------------------------------------------------------------------
// <copyright file="PlayerManager.cs">
//     Copyright (c) Igor Golopolosov. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace WarehouseOfMusic.Managers
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

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerManager" /> class.
        /// </summary>
        /// <param name="onPlayTrack">Track on play</param>
        public PlayerManager(ToDoTrack onPlayTrack)
        {
            _onPlayTracks = new List<ToDoTrack> {onPlayTrack};
            _audioController = new AudioController();
            _audioController.CreatePatch();
        }
        #endregion
        
        /// <summary>
        /// Start playing samples
        /// </summary>
        public void Play()
        {
            _audioController.Start();
        }

        /// <summary>
        /// Stop playing samples
        /// </summary>
        public void Stop()
        {
            this._playerTimer.Dispose();
            _audioController.Stop();
        }
    }
}
