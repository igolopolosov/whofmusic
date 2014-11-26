//-----------------------------------------------------------------------
// <copyright file="TrackArgs.cs" company="Igor Golopolosov">
//     Copyright (c) Igor Golopolosov. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace WarehouseOfMusic
{
    using Model;

    /// <summary>
    /// Track and type of changes for this track
    /// </summary>
    public sealed class TrackArgs
    {
        /// <summary>
        /// Gets or sets changed track
        /// </summary>
        public ToDoTrack Track { get; set; }

        /// <summary>
        /// Gets or sets type of change
        /// </summary>
        public string TypeOfEvent { get; set; }
    }
}
