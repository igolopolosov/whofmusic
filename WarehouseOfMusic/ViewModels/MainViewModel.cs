//-----------------------------------------------------------------------
// <copyright file="MainViewModel.cs">
//     Copyright (c) Igor Golopolosov. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using WarehouseOfMusic.Resources;

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
        /// Project that must be deleted
        /// </summary>
        private ToDoProject _onDeleteProject;

        /// <summary>
        /// Project that must be renamed
        /// </summary>
        private ToDoProject _onRenameProject;

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
        /// Gets or sets project on rename
        /// </summary>
        public ToDoProject OnRenameProject
        {
            get
            {
                return this._onRenameProject;
            }

            set
            {
                this._onRenameProject = value;
                this.NotifyPropertyChanged("OnRenameProject");
            }
        }

        /// <summary>
        /// Gets or sets project on delete
        /// </summary>
        public ToDoProject OnDeleteProject
        {
            get
            {
                return this._onDeleteProject;
            }

            set
            {
                this._onDeleteProject = value;
                this.NotifyPropertyChanged("OnDeleteProject");
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
                Name = projectName,
                CreationTime = DateTime.Now,
                LastModificationTime = DateTime.Now,
                Tempo = 120
            };
            this._toDoDb.Projects.InsertOnSubmit(newProject);
            this._toDoDb.SubmitChanges();
            this._projectsList.Add(newProject);

            var trackName = AppResources.TrackString + " 1";
            var newTrack = new ToDoTrack
            {
                Name = trackName,
                ProjectRef = newProject
            };
            this._toDoDb.Tracks.InsertOnSubmit(newTrack);
            this._toDoDb.SubmitChanges();
            newProject.Tracks.Add(newTrack);

            var sample = new ToDoSample
            {
                InitialTact = 1,
                Size = 4,
                TrackRef = newTrack,
                Name = newTrack.Name + newTrack.Samples.Count
            };
            this._toDoDb.Samples.InsertOnSubmit(sample);
            this._toDoDb.SubmitChanges();
            newTrack.Samples.Add(sample);

            return newProject;
        }

        /// <summary>
        /// Remove project and data linked with him from collections and database.
        /// </summary>
        internal void DeleteProject()
        {
            foreach (var track in _onDeleteProject.Tracks)
            {
                foreach (var sample in track.Samples)
                {
                    foreach (var note in sample.Notes)
                    {
                        this._toDoDb.Notes.DeleteOnSubmit(note);
                    }
                    this._toDoDb.Samples.DeleteOnSubmit(sample);
                }
                this._toDoDb.Tracks.DeleteOnSubmit(track);
            }

            this._projectsList.Remove(_onDeleteProject);
            this._toDoDb.Projects.DeleteOnSubmit(_onDeleteProject);
            this._toDoDb.SubmitChanges();
        }

        /// <summary>
        /// Give the project new name
        /// </summary>
        /// <param name="newName">New name of the project</param>
        internal void RenameProjectTo(string newName)
        {
            _onRenameProject.Name = newName;
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
