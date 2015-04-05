//-----------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="Igor Golopolosov">
//     Copyright (c) Igor Golopolosov. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace WarehouseOfMusic.ViewModels
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using Models;

    /// <summary>
    /// Class to realize access to database and represent information to application pages.
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// LINQ to SQL data context for the local database.
        /// </summary>
        private readonly ToDoDataContext _toDoDb;

        /// <summary>
        /// Project that must be renamed. If the value is -1, there is no project to rename.
        /// </summary>
        private int _onRenameProjectId = -1;

        /// <summary>
        /// A list of all projects
        /// </summary>
        private ObservableCollection<ToDoProject> _projectsList;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel" /> class.
        /// Class constructor, create the data context object.
        /// </summary>
        /// <param name="toDoDbConnectionString">Path to connect to database</param>
        public MainViewModel(string toDoDbConnectionString)
        {
            this._toDoDb = new ToDoDataContext(toDoDbConnectionString);
        }

        /// <summary>
        /// Event of property changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets id of project on rename
        /// </summary>
        public int OnRenameProjectId
        {
            get
            {
                return this._onRenameProjectId;
            }

            set
            {
                this._onRenameProjectId = value;
                this.NotifyPropertyChanged("OnRenameProjectId");
            }
        }

        /// <summary>
        /// Gets or sets a list of all projects
        /// </summary>
        public ObservableCollection<ToDoProject> ProjectsList
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
        /// Query database and load the list of projects
        /// </summary>
        public void LoadProFromDatabase()
        {
            //// Load a list of all projects. 
            this._projectsList = this._toDoDb.Projects.Any()
                ? new ObservableCollection<ToDoProject>(this._toDoDb.Projects)
                : new ObservableCollection<ToDoProject>();
        }

        /// <summary>
        /// Write changes in the data context to the database.
        /// </summary>
        public void SaveChangesToDb()
        {
            this._toDoDb.SubmitChanges();
        }

        /// <summary>
        /// Add a project with specific name to the database and collections.
        /// </summary>
        /// <param name="projectName">Name of project</param>
        /// <returns>New project</returns>
        internal ToDoProject CreateProject(string projectName)
        {
            var newProject = new ToDoProject()
            {
                Name = projectName
            };

            this._toDoDb.Projects.InsertOnSubmit(newProject);
            this._toDoDb.SubmitChanges();
            this._projectsList.Add(newProject);
            return newProject;
        }

        /// <summary>
        /// Remove project and data linked with him from collections and database.
        /// </summary>
        /// <param name="projectForDelete">Project on removing</param>
        internal void DeleteProject(ToDoProject projectForDelete)
        {
            foreach (var track in projectForDelete.Tracks)
            {
                this._toDoDb.Tracks.DeleteOnSubmit(track);
            }

            this._projectsList.Remove(projectForDelete);
            this._toDoDb.Projects.DeleteOnSubmit(projectForDelete);
            this._toDoDb.SubmitChanges();
        }

        /// <summary>
        /// Give the project new name
        /// </summary>
        /// <param name="newName">New name of the project</param>
        internal void RenameProjectTo(string newName)
        {
            var onRenameProject = (from prj in this._projectsList
                                   where prj.Id == this._onRenameProjectId
                                   select prj).First();

            onRenameProject.Name = newName;
            this._toDoDb.SubmitChanges();
        }

        #region INotifyPropertyChanged Members

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
