﻿//-----------------------------------------------------------------------
// <copyright file="ToDoDataContext.cs" company="Igor Golopolosov">
//     Copyright (c) Igor Golopolosov. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace WarehouseOfMusic.Model
{
    using System.Data.Linq;

    /// <summary>
    /// Image of database
    /// </summary>
    public class ToDoDataContext : DataContext
    {
        /// <summary>
        /// Specify a table for the to-do tracks.
        /// </summary>
        public Table<ToDoTrack> Tracks;

        /// <summary>
        /// Specify a table for the projects.
        /// </summary>
        public Table<ToDoProject> Projects;

        /// <summary>
        /// Initializes a new instance of the <see cref="ToDoDataContext" /> class.
        /// Pass the connection string to the base class.
        /// </summary>
        /// <param name="connectionString"> Path for connection to database</param>
        public ToDoDataContext(string connectionString)
            : base(connectionString)
        {
        }
    }
}
