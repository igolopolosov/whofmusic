namespace WarehouseOfMusic
{
    using System;
    using ViewModels;

    public class NoteEventArgs : EventArgs
    {
        /// <summary>
        /// Duration of note
        /// </summary>
        public Byte Duration { get; set; }
        /// <summary>
        /// Key
        /// </summary>
        public Key Key { get; set; }
        /// <summary>
        /// Number of tact
        /// </summary>
        public int TactNumber { get; set; }
        /// <summary>
        /// Position note at the tact
        /// </summary>
        public byte TactPosition { get; set; }
    }
}