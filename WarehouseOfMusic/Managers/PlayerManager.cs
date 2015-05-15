//-----------------------------------------------------------------------
// <copyright file="PlayerManager.cs">
//     Copyright (c) Igor Golopolosov. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Windows.Threading;
using WarehouseOfMusic.Converters;
using WarehouseOfMusic.Enums;
using WarehouseOfMusic.EventArgs;

namespace WarehouseOfMusic.Managers
{
    using System.Collections.Generic;
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
        private static AudioController _audioController;

        /// <summary>
        /// Timer for playing trakcs
        /// </summary>
        private DispatcherTimer _playerTimer;
        
        /// <summary>
        /// Controls position in tact
        /// </summary>
        private int _position;

        /// <summary>
        /// Controls position in tact
        /// </summary>
        private PlayerState _state = PlayerState.Stopped;

        /// <summary>
        /// Controls tact number
        /// </summary>
        private int _tact;

        /// <summary>
        /// Tempo of playing project
        /// </summary>
        private int _tempo;

        /// <summary>
        /// Tracks on play
        /// </summary>
        private List<TrackManager> _onPlayTracks;
        
        #endregion

        #region Properties and events
        /// <summary>
        /// Controls position in tact
        /// </summary>
        public PlayerState State
        {
            get
            {
                return _state;
            }
            private set
            {
                _state = value;
                if (StateChangeEvent != null)
                {
                    StateChangeEvent(this, new PlayerEventArgs { State = _state });
                }
            }
        }

        public delegate void PlayerChangedHandler(object sender, PlayerEventArgs e);

        /// <summary>
        /// Happend, when player stops or starts play sound
        /// </summary>
        public event PlayerChangedHandler StateChangeEvent;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates static audio controller for full application
        /// </summary>
        public static void InitializeAudioController()
        {
            _audioController = new AudioController();
            _audioController.CreatePatch();
        }
        
        public PlayerManager(int tempo)
        {
            _tempo = tempo;
            _playerTimer = new DispatcherTimer
            {
                Interval = new TimeSpan(TimePeriodsConverter.GetStepPeriod(_tempo) * 10000)
            };
            _playerTimer.Tick += _playerTimer_Tick;
        }
        #endregion

        #region PlayStop functionality
        /// <summary>
        /// Start playing samples
        /// </summary>
        public void Play(ToDoTrack onPlayTrack)
        {
            _audioController.Start();
            _onPlayTracks = new List<TrackManager> {new TrackManager(onPlayTrack)};
            _position = 0;
            _tact = 1;
            _playerTimer.Start();
            State = PlayerState.Playing;
        }

        /// <summary>
        /// Start playing samples
        /// </summary>
        public void Play(ToDoTrack onPlayTrack, int initialTact)
        {
            _audioController.Start();
            _onPlayTracks = new List<TrackManager> {new TrackManager(onPlayTrack)};
            _position = 0;
            _tact = initialTact;
            _playerTimer.Start();
            State = PlayerState.Playing;
        }

        /// <summary>
        /// Pause playing samples
        /// </summary>
        public void Pause()
        {
            this._playerTimer.Stop();
            ////
            State = PlayerState.Paused;
        }

        /// <summary>
        /// Resume playing samples
        /// </summary>
        public void Resume()
        {
            this._playerTimer.Start();
            State = PlayerState.Playing;
        } 

        /// <summary>
        /// Stop playing samples
        /// </summary>
        public void Stop()
        {
            this._playerTimer.Stop();
            ////
            State = PlayerState.Stopped;
        } 
        #endregion
        
        void _playerTimer_Tick(object sender, System.EventArgs e)
        {
            if (_onPlayTracks.All(x=>x.IsTrackEnd))
            {
                _playerTimer.Stop();
                State = PlayerState.Stopped;
                return;
            }

            foreach (var track in _onPlayTracks)
            {
                foreach (var note in track.PlayedNotes.Where(note => note.EndTact == _tact && note.EndPosition == _position).ToList())
                {
                    var keyArgs = new KeyPressedArgs()
                    {
                        IsPressed = false,
                        KeyNumber = note.MidiNumber
                    };
                    _audioController.KeyIsPressedChanged(this, keyArgs);
                    track.PlayedNotes.Remove(note);
                }
            }

            foreach (var track in _onPlayTracks)
            {
                bool oneMoreTime;
                track.OneTimePlayNotes = new Queue<ToDoNote>();
                do
                {
                    oneMoreTime = false;
                    if (track.OnPlayNotes.Count <= 0) continue;
                    if (track.OnPlayNotes.Peek().Tact == _tact && track.OnPlayNotes.Peek().Position == _position)
                    {
                        track.PlayedNotes.Add(track.OnPlayNotes.Peek());
                        track.OneTimePlayNotes.Enqueue(track.OnPlayNotes.Dequeue());
                        oneMoreTime = true;
                    }
                } while (oneMoreTime);

                foreach (var keyArgs in track.OneTimePlayNotes.Select(note => new KeyPressedArgs()
                {
                    IsPressed = true,
                    KeyNumber = note.MidiNumber
                }))
                {
                    _audioController.KeyIsPressedChanged(this, keyArgs);
                }
            }

            _position++;
            if (_position != 16) return;
            _position = 0;
            _tact++;
        }
   }
}
