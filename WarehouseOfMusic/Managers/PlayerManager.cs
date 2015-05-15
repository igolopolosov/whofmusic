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
        private List<ToDoTrack> _onPlayTracks;

        /// <summary>
        /// Notes on play
        /// </summary>
        private Queue<ToDoNote> _onPlayNotes;

        /// <summary>
        /// Notes which play in a one time play
        /// </summary>
        private Queue<ToDoNote> _oneTimePlayNotes;

        /// <summary>
        /// Played notes
        /// </summary>
        private List<ToDoNote> _playedNotes;
        #endregion

        #region Constructors
        
        public static void InitializeAudioController()
        {
            _audioController = new AudioController();
            _audioController.CreatePatch();
        }

        #endregion

        #region Properties and events
		/// <summary>
        /// Controls position in tact
        /// </summary>
        public PlayerState State {
            get
            {
                return _state;
            }
            private set
            {
                _state = value;
                if (StateChangeEvent != null)
                {
                    StateChangeEvent(this, new PlayerEventArgs { State = _state});
                }
            }}

        public delegate void PlayerChangedHandler(object sender, PlayerEventArgs e);

        /// <summary>
        /// Happend, when player stops or starts play sound
        /// </summary>
        public event PlayerChangedHandler StateChangeEvent; 
	#endregion

        #region PlayStop functionality
        /// <summary>
        /// Start playing samples
        /// </summary>
        public void Play(ToDoTrack onPlayTrack)
        {
            _onPlayTracks = new List<ToDoTrack> { onPlayTrack };
            _audioController.Start();
            CreateNoteList();
            _tempo = _onPlayTracks.First().ProjectRef.Tempo;
            _position = 0;
            _tact = 1;
            _playerTimer = new DispatcherTimer();
            _playerTimer.Interval = new TimeSpan(TimePeriodsConverter.GetStepPeriod(_tempo) * 10000);
            _playerTimer.Tick += _playerTimer_Tick;
            _playerTimer.Start();
            State = PlayerState.Playing;
        }

        void _playerTimer_Tick(object sender, System.EventArgs e)
        {
            if (_playedNotes.Count == 0 && _onPlayNotes.Count == 0)
            {
                _playerTimer.Stop();
                State = PlayerState.Stopped;
                return;
            }

            foreach (var note in _playedNotes.Where(note => note.EndTact == _tact && note.EndPosition == _position).ToList())
            {
                var keyArgs = new KeyPressedArgs()
                {
                    IsPressed = false,
                    KeyNumber = note.MidiNumber
                };
                _audioController.KeyIsPressedChanged(this, keyArgs);
                _playedNotes.Remove(note);
            }
            
            bool oneMoreTime;
            _oneTimePlayNotes = new Queue<ToDoNote>();
            do
            {
                oneMoreTime = false;
                if (_onPlayNotes.Count <= 0) continue;
                if (_onPlayNotes.Peek().Tact == _tact && _onPlayNotes.Peek().Position == _position)
                {
                    _playedNotes.Add(_onPlayNotes.Peek());
                    _oneTimePlayNotes.Enqueue(_onPlayNotes.Dequeue());
                    oneMoreTime = true;
                }
            }   while (oneMoreTime);

            foreach (var keyArgs in _oneTimePlayNotes.Select(note => new KeyPressedArgs()
            {
                IsPressed = true,
                KeyNumber = note.MidiNumber
            }))
            {
                _audioController.KeyIsPressedChanged(this, keyArgs);
            }

            _position++;
            if (_position != 16) return;
            _position = 0;
            _tact++;
        }

        /// <summary>
        /// Start playing samples
        /// </summary>
        public void Play(ToDoTrack onPlayTrack, int initialTact)
        {
            _onPlayTracks = new List<ToDoTrack> { onPlayTrack };
            _audioController.Start();
            CreateNoteList();
            var tempo = _onPlayTracks.First().ProjectRef.Tempo;
            _playerTimer = new DispatcherTimer();
            _playerTimer.Interval = new TimeSpan(TimePeriodsConverter.GetStepPeriod(_tempo) * 10000);
            _playerTimer.Tick += _playerTimer_Tick;
            _playerTimer.Start();
            _position = 0;
            _tact = initialTact;
            State = PlayerState.Playing;
        }

        /// <summary>
        /// Pause playing samples
        /// </summary>
        public void Pause()
        {
            this._playerTimer.Stop();
            foreach (var note in _playedNotes)
            {
                var keyArgs = new KeyPressedArgs()
                {
                    IsPressed = false,
                    KeyNumber = note.MidiNumber
                };
                _audioController.KeyIsPressedChanged(this, keyArgs);
                _playedNotes.Remove(note);
            }

            State = PlayerState.Paused;
        }

        /// <summary>
        /// Resume playing samples
        /// </summary>
        public void Resume()
        {
            this._playerTimer.Stop();
            State = PlayerState.Playing;
        } 

        /// <summary>
        /// Stop playing samples
        /// </summary>
        public void Stop()
        {
            this._playerTimer.Stop();
            State = PlayerState.Stopped;
        } 
        #endregion

        /// <summary>
        /// Creates list of notes from track
        /// </summary>
        private void CreateNoteList()
        {
            _onPlayNotes = new Queue<ToDoNote>();
            _playedNotes = new List<ToDoNote>();
            foreach (var sample in _onPlayTracks.First().Samples.OrderBy(x => x.InitialTact))
            {
                var notes = sample.Notes.OrderBy(x => x.Tact).ThenBy(x => x.Position);
                foreach (var note in notes)
                {
                    _onPlayNotes.Enqueue(note);
                }
            }
        }

        /// <summary>
        /// One step of timer
        /// </summary>
        private void StepCallback(object state)
        {
            if (_playedNotes.Count == 0 && _onPlayNotes.Count == 0)
            {
                this._playerTimer.Stop();
                State = PlayerState.Stopped;
                return;
            }

            foreach (var note in _playedNotes.Where(note => note.EndTact == _tact && note.EndPosition == _position).ToList())
            {
                var keyArgs = new KeyPressedArgs()
                {
                    IsPressed = false,
                    KeyNumber = note.MidiNumber
                };
                _audioController.KeyIsPressedChanged(this, keyArgs);
                _playedNotes.Remove(note);
            }
            
            bool oneMoreTime;
            _oneTimePlayNotes = new Queue<ToDoNote>();
            do
            {
                oneMoreTime = false;
                if (_onPlayNotes.Count <= 0) continue;
                if (_onPlayNotes.Peek().Tact == _tact && _onPlayNotes.Peek().Position == _position)
                {
                    _playedNotes.Add(_onPlayNotes.Peek());
                    _oneTimePlayNotes.Enqueue(_onPlayNotes.Dequeue());
                    oneMoreTime = true;
                }
            }   while (oneMoreTime);

            foreach (var keyArgs in _oneTimePlayNotes.Select(note => new KeyPressedArgs()
            {
                IsPressed = true,
                KeyNumber = note.MidiNumber
            }))
            {
                _audioController.KeyIsPressedChanged(this, keyArgs);
            }

            _position++;
            if (_position != 16) return;
            _position = 0;
            _tact++;
        }

   }
}
