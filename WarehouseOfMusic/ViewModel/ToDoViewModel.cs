//-----------------------------------------------------------------------
// <copyright file="ToDoViewModel.cs" company="Igor Golopolosov">
//     Copyright (c) Igor Golopolosov. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace WarehouseOfMusic.ViewModel
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using Model;
    using Resources;

    /// <summary>
    /// Class to realize access to database and represent information to application pages.
    /// </summary>
    public class ToDoViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// LINQ to SQL data context for the local database.
        /// </summary>
        private readonly ToDoDataContext _toDoDb;

        /// <summary>
        /// ViewModel for currently editing project
        /// </summary>
        private ProjectEditorViewModel _projectEditorViewModel;

        /// <summary>
        /// Project that must be renamed
        /// </summary>
        private int _onRenameProjectId = -1;

        /// <summary>
        /// A list of all projects
        /// </summary>
        private ObservableCollection<ToDoProject> _projectsList;

        /// <summary>
        /// Initializes a new instance of the <see cref="ToDoViewModel" /> class.
        /// Class constructor, create the data context object.
        /// </summary>
        /// <param name="toDoDbConnectionString">Path to connect to database</param>
        public ToDoViewModel(string toDoDbConnectionString)
        {
            this._toDoDb = new ToDoDataContext(toDoDbConnectionString);
            this._projectEditorViewModel = new ProjectEditorViewModel();
            this._projectEditorViewModel.TrackChanged += this.TrackChanged;
        }

        /// <summary>
        /// Event of property changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets ViewModel for editing project page
        /// </summary>
        public ProjectEditorViewModel ProjectEditorViewModel
        {
            get
            {
                return this._projectEditorViewModel;
            }

            set
            {
                this._projectEditorViewModel = value;
                this.NotifyPropertyChanged("CurrentProjectViewModel");
            }
        }

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
        /// Query database and load the collections and list
        /// </summary>
        public void LoadCollectionsFromDatabase()
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
        /// Add a project to the database and collections.
        /// </summary>
        /// <param name="newProject">Project on adding</param>
        internal void CreateProject(ToDoProject newProject)
        {
            if (newProject.Name == null)
            {
                var projectNumber = 1;
                if (this._projectsList.Any())
                {
                    projectNumber = this._projectsList.OrderBy(project => project.Id).Last().Id + 1;
                }

                newProject.Name = AppResources.ProjectString + " " + projectNumber;
            }

            this._toDoDb.Projects.InsertOnSubmit(newProject);
            this._toDoDb.SubmitChanges();
            this._projectsList.Add(newProject);
            this._projectEditorViewModel.CurrentProject = newProject;
        }

        /// <summary>
        /// Remove project and data linked with him from collections and database.
        /// </summary>
        /// <param name="projectForDelete">Project on removing</param>
        internal void DeleteProject(ToDoProject projectForDelete)
        {
            if (Equals(projectForDelete, this._projectEditorViewModel.CurrentProject))
            {
                this._projectEditorViewModel.CurrentProject = new ToDoProject();
            }

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

        /// <summary>
        /// Called on delete or add track
        /// </summary>
        /// <param name="sender">Current project</param>
        /// <param name="trackArgs">Represent information about track on change</param>
        internal void TrackChanged(object sender, TrackArgs trackArgs)
        {
            switch (trackArgs.TypeOfEvent)
            {
                case "Insert":
                {
                    this._toDoDb.Tracks.InsertOnSubmit(trackArgs.Track);
                }

                    break;

                case "Delete":
                {
                    this._toDoDb.Tracks.DeleteOnSubmit(trackArgs.Track);
                }

                    break;
            }

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
