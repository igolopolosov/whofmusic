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
        /// Changed track
        /// </summary>
        public ToDoTrack Track { set; get; }

        /// <summary>
        /// Type of change
        /// </summary>
        public string TypeOfEvent { set; get; }
    }
}
