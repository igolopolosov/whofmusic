using WarehouseOfMusic.Models;

namespace WarehouseOfMusic
{
    using System;

    public class NoteEventArgs : EventArgs
    {
        /// <summary>
        /// Note partially filled with parameters
        /// </summary>
        public ToDoNote Note;
    }
}