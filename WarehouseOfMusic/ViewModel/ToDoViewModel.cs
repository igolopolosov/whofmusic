//-----------------------------------------------------------------------
// <copyright file="ToDoViewModel.cs" company="github.com/usehotkey">
//     Free code of the application. No copyrights.
// </copyright>
// <author>Igor Golopolosov</author>
//-----------------------------------------------------------------------

namespace WarehouseOfMusic.ViewModel
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using Model;

    /// <summary>
    /// Class to realize access to database and represent information to application pages.\
    /// </summary>
    public class ToDoViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// LINQ to SQL data context for the local database.
        /// </summary>
        private readonly ToDoDataContext _toDoDb;

        /// <summary>
        /// Name of currently editing project
        /// </summary>
        private ToDoProject _currentProject;

        /// <summary>
        /// A list of all projects
        /// </summary>
        private List<ToDoProject> _projectsList;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ToDoViewModel" /> class.
        /// Class constructor, create the data context object.
        /// </summary>
        /// <param name="toDoDbConnectionString">Path to connect to database</param>
        public ToDoViewModel(string toDoDbConnectionString)
        {
            this._toDoDb = new ToDoDataContext(toDoDbConnectionString);
        }
        
        /// <summary>
        /// Gets or sets current project
        /// </summary>
        public ToDoProject CurrentProject
        {
            get
            {
                return this._currentProject;
            }

            set
            {
                this._currentProject = value;
                this.NotifyPropertyChanged("CurrentProject");
            }
        }

        /// <summary>
        /// Gets or sets a list of all projects
        /// </summary>
        public List<ToDoProject> ProjectsList
        {
            get
            {
                return this._projectsList;
            }

            set
            {
                this._projectsList = value;
                this.NotifyPropertyChanged("ProjectsList");
            }
        }

        /// <summary>
        /// Add a project to the database and collections.
        /// </summary>
        /// <param name="newProject">Project on adding</param>
        public void AddProject(ToDoProject newProject)
        {
            this._toDoDb.Projects.InsertOnSubmit(newProject);
            this._toDoDb.SubmitChanges();
            this.ProjectsList.Add(newProject);
            this.CurrentProject = newProject;
        }

        /// <summary>
        /// Remove a project from the database and collections.
        /// </summary>
        /// <param name="toDoForDelete">Project on removing</param>
        public void DeleteProject(ToDoProject toDoForDelete)
        {
            //// Remove the to-do item from the "all" observable collection.
            this.ProjectsList.Remove(toDoForDelete);
            //// Remove the to-do item from the data context.
            this._toDoDb.Projects.DeleteOnSubmit(toDoForDelete);
            //// Save changes to the database.
            this._toDoDb.SubmitChanges();
        }

        /// <summary>
        /// Query database and load the collections and list /
        /// </summary>
        public void LoadCollectionsFromDatabase()
        {
            //// Specify the query for all to-do items in the database.
            var toDoTracksInDb = from ToDoTrack todo in this._toDoDb.Tracks
                                select todo;

            //// Load a list of all categories.
            this.ProjectsList = this._toDoDb.Projects.ToList();
        }

        /// <summary>
        /// Write changes in the data context to the database.
        /// </summary>
        public void SaveChangesToDb()
        {
            this._toDoDb.SubmitChanges();
        }

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Event of property changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Used to notify the app that a property has changed.
        /// </summary>
        /// <param name="propertyName">Property on changed</param>
        private void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
