using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace WarehouseOfMusic.Model
{
    public class ToDoDataContext : DataContext
    {
        // Pass the connection string to the base class.
        public ToDoDataContext(string connectionString)
            : base(connectionString)
        { }

        // Specify a table for the to-do tracks.
        public Table<ToDoTrack> Tracks;

        // Specify a table for the projects.
        public Table<ToDoProject> Projects;
    }

    
    [Table]
    public class ToDoProject : INotifyPropertyChanged, INotifyPropertyChanging
    {
        // Define ID: private field, public property, and database column.
        private int _id;

        [Column(DbType = "INT NOT NULL IDENTITY", IsDbGenerated = true, IsPrimaryKey = true)]
        public int Id
        {
            get { return _id; }
            set
            {
                NotifyPropertyChanging("Id");
                _id = value;
                NotifyPropertyChanged("Id");
            }
        }

        // Define p name: private field, public property, and database column.
        private string _name;

        [Column]
        public string Name
        {
            get { return _name; }
            set
            {
                NotifyPropertyChanging("Name");
                _name = value;
                NotifyPropertyChanged("Name");
            }
        }

        // Define the entity set for the collection side of the relationship.
        private EntitySet<ToDoTrack> _todos;

        [Association(Storage = "_todos", OtherKey = "_projectId", ThisKey = "Id")]
        public EntitySet<ToDoTrack> ToDos
        {
            get { return this._todos; }
            set { this._todos.Assign(value); }
        }


        // Assign handlers for the add and remove operations, respectively.
        public ToDoProject()
        {
            _todos = new EntitySet<ToDoTrack>(
                new Action<ToDoTrack>(this.attach_ToDo),
                new Action<ToDoTrack>(this.detach_ToDo)
                );
        }

        // Called during an add operation
        private void attach_ToDo(ToDoTrack toDo)
        {
            NotifyPropertyChanging("ToDoTrack");
            toDo.Project = this;
        }

        // Called during a remove operation
        private void detach_ToDo(ToDoTrack toDo)
        {
            NotifyPropertyChanging("ToDoTrack");
            toDo.Project = null;
        }

        // Version column aids update performance.
        [Column(IsVersion = true)]
        private Binary _version;
        
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify that a property changed
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region INotifyPropertyChanging Members

        public event PropertyChangingEventHandler PropertyChanging;

        // Used to notify that a property is about to change
        private void NotifyPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        #endregion
    }

    [Table]
    public class ToDoTrack : INotifyPropertyChanged, INotifyPropertyChanging
    {
        // Define ID: private field, public property, and database column.
        private int _toDoTrackId;

        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public int ToDoTrackId
        {
            get { return _toDoTrackId; }
            set
            {
                if (_toDoTrackId != value)
                {
                    NotifyPropertyChanging("ToDoTrackId");
                    _toDoTrackId = value;
                    NotifyPropertyChanged("ToDoTrackId");
                }
            }
        }

        // Define item name: private field, public property, and database column.
        private string _trackName;

        [Column]
        public string TrackName
        {
            get { return _trackName; }
            set
            {
                if (_trackName != value)
                {
                    NotifyPropertyChanging("TrackName");
                    _trackName = value;
                    NotifyPropertyChanged("TrackName");
                }
            }
        }

        // Internal column for the associated ToDoProject ID value
        [Column]
        internal int _projectId;

        // Entity reference, to identify the ToDoProject "storage" table
        private EntityRef<ToDoProject> _project;

        // Association, to describe the relationship between this key and that "storage" table
        [Association(Storage = "_project", ThisKey = "_projectId", OtherKey = "Id", IsForeignKey = true)]
        public ToDoProject Project
        {
            get { return _project.Entity; }
            set
            {
                NotifyPropertyChanging("Project");
                _project.Entity = value;

                if (value != null)
                {
                    _projectId = value.Id;
                }

                NotifyPropertyChanging("Project");
            }
        }

        // Version column aids update performance.
        [Column(IsVersion = true)]
        private Binary _version;

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify that a property changed
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region INotifyPropertyChanging Members

        public event PropertyChangingEventHandler PropertyChanging;

        // Used to notify that a property is about to change
        private void NotifyPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        #endregion
    }
}
