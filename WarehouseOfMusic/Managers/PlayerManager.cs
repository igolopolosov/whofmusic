//-----------------------------------------------------------------------
// <copyright file="PlayerManager.cs">
//     Copyright (c) Igor Golopolosov. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using WarehouseOfMusic.Converters;

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
        private static AudioController _audioController;

        /// <summary>
        /// Controls position in tact
        /// </summary>
        private int _position;

        /// <summary>
        /// Controls tact number
        /// </summary>
        private int _tact;

        /// <summary>
        /// Timer for playing trakcs
        /// </summary>
        private Timer _playerTimer;

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
        
        /// <summary>
        /// Start playing samples
        /// </summary>
        public void Play(ToDoTrack onPlayTrack)
        {
            _onPlayTracks = new List<ToDoTrack> { onPlayTrack };
            _audioController.Start();
            CreateNoteList();
            var tempo = _onPlayTracks.First().ProjectRef.Tempo;
            _playerTimer = new Timer(StepCallback, null, 0, TimePeriodsConverter.GetStepPeriod(tempo));
            _position = 0;
            _tact = 1;
        }

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
                _playerTimer.Dispose();
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
        /// Stop playing samples
        /// </summary>
        public void Stop()
        {
            this._playerTimer.Dispose();
            _audioController.Stop();
        }
    }
}
