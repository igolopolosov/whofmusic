using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using WarehouseOfMusic.Model;


namespace WarehouseOfMusic.ViewModel
{
    public class ToDoViewModel : INotifyPropertyChanged
    {
        // LINQ to SQL data context for the local database.
        private ToDoDataContext toDoDB;

        // Class constructor, create the data context object.
        public ToDoViewModel(string toDoDBConnectionString)
        {
            toDoDB = new ToDoDataContext(toDoDBConnectionString);
        }

        // A list of all projects, used by the add task page.
        private string _projectName;
        public string ProjectName
        {
            get { return _projectName; }
            set
            {
                _projectName = value;
                NotifyPropertyChanged("ProjectName");
            }
        }
        
        // To-do tracks.
        private ObservableCollection<ToDoTrack> _toDoTracks;
        public ObservableCollection<ToDoTrack> ToDoTracks
        {
            get { return _toDoTracks; }
            set
            {
                _toDoTracks = value;
                NotifyPropertyChanged("ToDoTracks");
            }
        }

        // A list of all projects, used by the add task page.
        private List<ToDoProject> _projectsList;
        public List<ToDoProject> ProjectsList
        {
            get { return _projectsList; }
            set
            {
                _projectsList = value;
                NotifyPropertyChanged("ProjectsList");
            }
        }

        // Add a project to the database and collections.
        public void AddProject(ToDoProject newProject)
        {
            // Add a to-do item to the data context.
            toDoDB.Projects.InsertOnSubmit(newProject);

            // Save changes to the database.
            toDoDB.SubmitChanges();

            // Add a to-do item to the "all" observable collection.
            ProjectsList.Add(newProject);
        }

        // Remove a project from the database and collections.
        public void DeleteProject(ToDoProject toDoForDelete)
        {
            // Remove the to-do item from the "all" observable collection.
            ProjectsList.Remove(toDoForDelete);

            // Remove the to-do item from the data context.
            toDoDB.Projects.DeleteOnSubmit(toDoForDelete);
            // Save changes to the database.
            toDoDB.SubmitChanges();
        }


        // Query database and load the collections and list used by the pivot pages.
        public void LoadCollectionsFromDatabase()
        {

            // Specify the query for all to-do items in the database.
            var toDoTracksInDB = from ToDoTrack todo in toDoDB.Tracks
                                select todo;

            // Query the database and load all to-do items.
            ToDoTracks = new ObservableCollection<ToDoTrack>(toDoTracksInDB);

            // Load a list of all categories.
            ProjectsList = toDoDB.Projects.ToList();
        }

        // Write changes in the data context to the database.
        public void SaveChangesToDB()
        {
            toDoDB.SubmitChanges();
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify the app that a property has changed.
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
