namespace WarehouseOfMusic.ViewModels
{
    using System.ComponentModel;
    using Models;

    public abstract class Context : INotifyPropertyChanged
    {
        /// <summary>
        /// LINQ to SQL data context for the local database.
        /// </summary>
        protected readonly ToDoDataContext ToDoDb;

        /// <summary>
        /// Initializes a new instance of the <see cref="Context" /> class.
        /// Class constructor, create the data context object.
        /// </summary>
        /// <param name="toDoDbConnectionString">Path to connect to database</param>
        protected Context(string toDoDbConnectionString)
        {
            this.ToDoDb = new ToDoDataContext(toDoDbConnectionString);
        }

        /// <summary>
        /// Write changes in the data context to the database.
        /// </summary>
        public void SaveChangesToDb()
        {
            this.ToDoDb.SubmitChanges();
        }
        
        /// <summary>
        /// Query database and load entities
        /// </summary>
        public abstract void LoadData(int id);
        
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}