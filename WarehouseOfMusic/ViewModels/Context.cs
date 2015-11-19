namespace WarehouseOfMusic.ViewModels
{
    using System.ComponentModel;
    using Models;

    /// <summary>
    /// Template of class from ViewModel layer.
    /// </summary>
    public abstract class Context : INotifyPropertyChanged
    {
        /// <summary>
        /// LINQ to SQL data context for the local database.
        /// </summary>
        protected readonly ToDoDataContext DataBaseContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="Context" /> class.
        /// </summary>
        /// <param name="toDoDbConnectionString">Path of connection to the database.</param>
        protected Context(string toDoDbConnectionString)
        {
            this.DataBaseContext = new ToDoDataContext(toDoDbConnectionString);
        }

        /// <summary>
        /// Write changes in the data context to the database.
        /// </summary>
        public void SaveChangesToDb()
        {
            this.DataBaseContext.SubmitChanges();
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