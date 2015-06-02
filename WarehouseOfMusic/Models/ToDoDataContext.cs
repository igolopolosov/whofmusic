//-----------------------------------------------------------------------
// <copyright file="ToDoDataContext.cs">
//     Copyright (c) Igor Golopolosov. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace WarehouseOfMusic.Models
{
    using System.Data.Linq;
    using WomAudioComponent;

    /// <summary>
    /// Image of database
    /// </summary>
    public class ToDoDataContext : DataContext
    {
        /// <summary>
        /// Specify a table for the notes
        /// </summary>
        public Table<ToDoNote> Notes;

        /// <summary>
        /// Specify a table for the projects.
        /// </summary>
        public Table<ToDoProject> Projects;

        /// <summary>
        /// Specify a table for the samples.
        /// </summary>
        public Table<ToDoSample> Samples;

        /// <summary>
        /// Specify a table for the tracks
        /// </summary>
        public Table<ToDoTrack> Tracks;

        /// <summary>
        /// Specify a table for the tracks
        /// </summary>
        public Table<ToDoInstrument> Instruments;

        /// <summary>
        /// Initializes a new instance of the <see cref="ToDoDataContext" /> class.
        /// Pass the connection string to the base class.
        /// </summary>
        /// <param name="connectionString"> Path for connection to database</param>
        public ToDoDataContext(string connectionString)
            : base(connectionString)
        {
        }

        /// <summary>
        /// Add simple instruments to DB
        /// </summary>
        public void Populate()
        {
            for (var i = 0; i < 4; i++)
            {
                var instrument = new ToDoInstrument()
                {
                    Name = "" + (WaveformType)i,
                    Waveform = (WaveformType)i
                };
                this.Instruments.InsertOnSubmit(instrument);
            }
            this.SubmitChanges();
        }
    }
}
