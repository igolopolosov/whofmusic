//-----------------------------------------------------------------------
// <copyright file="NoteArgs.cs" company="Igor Golopolosov">
//     Copyright (c) Igor Golopolosov. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace WarehouseOfMusic
{
    /// <summary>
    /// Arguments of message that condition of some note was changed
    /// </summary>
    public sealed class NoteArgs
    {
        /// <summary>
        /// Gets or sets midi number of note
        /// </summary>
        public int NoteMidiNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether current condition
        /// </summary>
        public bool IsPlayed { get; set; }
    }
}
