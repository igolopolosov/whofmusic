using WarehouseOfMusic.Models;

namespace WarehouseOfMusic.EventArgs
{
    public class NoteEventArgs : System.EventArgs
    {
        /// <summary>
        /// Note partially filled with parameters
        /// </summary>
        public ToDoNote Note;
    }
}