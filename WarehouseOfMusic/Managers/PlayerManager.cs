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
        private static List<AudioController> AudioControllers;

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
            AudioControllers = new List<AudioController>();
            var controller = new AudioController();
            controller.CreatePatch();
            AudioControllers.Add(controller);
        }

        #endregion
        
        /// <summary>
        /// Start playing samples
        /// </summary>
        public void Play(ToDoTrack onPlayTrack)
        {
            _onPlayTracks = new List<ToDoTrack> { onPlayTrack };
            
            foreach (var controller in AudioControllers)
            {
                controller.Start();
                controller.SetVolumeLevel(100);
            }
            CreateNoteList();
            var tempo = _onPlayTracks.First().ProjectRef.Tempo;
            _playerTimer = new Timer(StepCallback, null, 0, TimePeriodsConverter.GetStepPeriod(tempo));
            _position = 0;
            _tact = 1;
        }

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

        private void StepCallback(object state)
        {
            if (_playedNotes.Count == 0 && _onPlayNotes.Count == 0)
            {
                _playerTimer.Dispose();
                return;
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
                    foreach (var note in _oneTimePlayNotes)
                    {
                        var keyArgs = new KeyPressedArgs()
                        {
                            IsPressed = true,
                            KeyNumber = note.MidiNumber
                        };
                        AudioControllers.First().KeyIsPressedChanged(this, keyArgs);
                    }
                    oneMoreTime = true;
                }
            }   while (oneMoreTime);

            var list = _playedNotes.Where(note => note.EndTact == _tact && note.EndPosition == _position).ToArray();
            foreach (var note in list)
            {
                var keyArgs = new KeyPressedArgs()
                {
                    IsPressed = false,
                    KeyNumber = note.MidiNumber
                };
                AudioControllers.First().KeyIsPressedChanged(this, keyArgs);
                _playedNotes.Remove(note);
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
            foreach (var controller in AudioControllers)
            {
                //controller.Stop();
            }
        }
    }
}
