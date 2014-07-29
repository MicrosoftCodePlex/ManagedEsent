// ---------------------------------------------------------------------------
// <copyright file="Database.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------

// ---------------------------------------------------------------------
// <summary>
// </summary>
// ---------------------------------------------------------------------

namespace Microsoft.Isam.Esent.Isam
{
    using System;

    using Microsoft.Isam.Esent.Interop;

    /// <summary>
    /// A Database is a file used by the ISAM to store data.  It is organized
    /// into tables which are in turn comprised of columns and indices and
    /// contain data in the form of records.  The database's schema can be
    /// enumerated and manipulated by this object.  Also, the database's
    /// tables can be opened for access by this object.
    /// </summary>
    public class Database : DatabaseCommon, IDisposable
    {
        /// <summary>
        /// The dbid
        /// </summary>
        private readonly JET_DBID dbid;

        /// <summary>
        /// The table collection
        /// </summary>
        private TableCollection tableCollection = null;

        /// <summary>
        /// The cleanup
        /// </summary>
        private bool cleanup = false;

        /// <summary>
        /// The disposed
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="Database"/> class.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="databaseName">Name of the database.</param>
        internal Database(Session session, string databaseName)
            : base(session)
        {
            lock (session)
            {
                Api.JetOpenDatabase(session.Sesid, databaseName, null, out this.dbid, OpenDatabaseGrbit.None);
                this.cleanup = true;
                this.tableCollection = new TableCollection(this);
            }
        }

        /// <summary>
        /// Finalizes an instance of the Database class
        /// </summary>
        ~Database()
        {
            Dispose(false);
        }

        /// <summary>
        /// Gets a collection of tables in the database.
        /// </summary>
        /// <returns>a collection of tables in the database</returns>
        public override TableCollection Tables
        {
            get
            {
                this.CheckDisposed();
                return this.tableCollection;
            }
        }

        /// <summary>
        /// Gets the dbid.
        /// </summary>
        /// <value>
        /// The dbid.
        /// </value>
        internal JET_DBID Dbid
        {
            get
            {
                return this.dbid;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [disposed].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [disposed]; otherwise, <c>false</c>.
        /// </value>
        internal override bool Disposed
        {
            get
            {
                return this.disposed || this.Session.Disposed;
            }

            set
            {
                this.disposed = value;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public new void Dispose()
        {
            lock (this)
            {
                this.Dispose(true);
            }

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Creates a single table with the specified definition in the database
        /// </summary>
        /// <param name="tableDefinition">The table definition.</param>
        public override void CreateTable(TableDefinition tableDefinition)
        {
            lock (Session)
            {
                CheckDisposed();

                using (Transaction trx = new Transaction(Session))
                {
                    // FUTURE-2013/11/15-martinc: Consider using JetCreateTableColumnIndex(). It would be
                    // a bit faster because it's only a single managed/native transition.

                    // Hard-code the initial space and density.
                    JET_TABLEID tableid;
                    Api.JetCreateTable(this.Session.Sesid, dbid, tableDefinition.Name, 16, 90, out tableid);

                    foreach (ColumnDefinition columnDefinition in tableDefinition.Columns)
                    {
                        JET_COLUMNDEF columndef = new JET_COLUMNDEF();
                        columndef.coltyp = Database.ColtypFromColumnDefinition(columnDefinition);
                        columndef.cp = JET_CP.Unicode;
                        columndef.cbMax = columnDefinition.MaxLength;

                        columndef.grbit = Converter.ColumndefGrbitFromColumnFlags(columnDefinition.Flags);
                        byte[] defaultValueBytes = Converter.BytesFromObject(
                            columndef.coltyp,
                            false /*ASCII */,
                            columnDefinition.DefaultValue);

                        JET_COLUMNID columnid;
                        int defaultValueLength = (defaultValueBytes == null) ? 0 : defaultValueBytes.Length;
                        Api.JetAddColumn(
                            this.Session.Sesid,
                            tableid,
                            columnDefinition.Name,
                            columndef,
                            defaultValueBytes,
                            defaultValueLength,
                            out columnid);
                    }

                    foreach (IndexDefinition indexDefinition in tableDefinition.Indices)
                    {
                        JET_INDEXCREATE[] indexcreates = new JET_INDEXCREATE[1];
                        indexcreates[0] = new JET_INDEXCREATE();

                        indexcreates[0].szIndexName = indexDefinition.Name;
                        indexcreates[0].szKey = Database.IndexKeyFromIndexDefinition(indexDefinition);
                        indexcreates[0].cbKey = indexcreates[0].szKey.Length;
                        indexcreates[0].grbit = Database.GrbitFromIndexDefinition(indexDefinition);
                        indexcreates[0].ulDensity = indexDefinition.Density;
                        indexcreates[0].pidxUnicode = new JET_UNICODEINDEX();
                        indexcreates[0].pidxUnicode.lcid = indexDefinition.CultureInfo.LCID;
                        indexcreates[0].pidxUnicode.dwMapFlags = (uint)Converter.UnicodeFlagsFromCompareOptions(indexDefinition.CompareOptions);
                        indexcreates[0].rgconditionalcolumn = Database.ConditionalColumnsFromIndexDefinition(indexDefinition);
                        indexcreates[0].cConditionalColumn = indexcreates[0].rgconditionalcolumn.Length;
                        indexcreates[0].cbKeyMost = indexDefinition.MaxKeyLength;
                        Api.JetCreateIndex2(this.Session.Sesid, tableid, indexcreates, indexcreates.Length);
                    }

                    // The initially-created tableid is opened exclusively.
                    Api.JetCloseTable(this.Session.Sesid, tableid);
                    trx.Commit();
                    DatabaseCommon.SchemaUpdateID++;
                }
            }
        }

        /// <summary>
        /// Deletes a single table in the database.
        /// </summary>
        /// <param name="tableName">The name of the table to be deleted.</param>
        /// <remarks>
        /// It is currently not possible to delete a table that is being used
        /// by a Cursor.  All such Cursors must be disposed before the
        /// table can be successfully deleted.
        /// </remarks>
        public override void DropTable(string tableName)
        {
            lock (Session)
            {
                CheckDisposed();

                Api.JetDeleteTable(Session.Sesid, dbid, tableName);
                DatabaseCommon.SchemaUpdateID++;
            }
        }

        /// <summary>
        /// Determines if a given table exists in the database
        /// </summary>
        /// <param name="tableName">The name of the table to evaluate for existence.</param>
        /// <returns>
        /// true if the table was found, false otherwise
        /// </returns>
        public override bool Exists(string tableName)
        {
            CheckDisposed();

            return Tables.Contains(tableName);
        }

        /// <summary>
        /// Opens a cursor over the specified table.
        /// </summary>
        /// <param name="tableName">the name of the table to be opened</param>
        /// <param name="exclusive">when true, the table will be opened for exclusive access</param>
        /// <returns>a cursor over the specified table in this database</returns>
        public Cursor OpenCursor(string tableName, bool exclusive)
        {
            lock (Session)
            {
                CheckDisposed();

                OpenTableGrbit grbit = exclusive ? OpenTableGrbit.DenyRead : OpenTableGrbit.None;
                return new Cursor(Session, this, tableName, grbit);
            }
        }

        /// <summary>
        /// Opens a cursor over the specified table.
        /// </summary>
        /// <param name="tableName">the name of the table to be opened</param>
        /// <returns>a cursor over the specified table in this database</returns>
        public override Cursor OpenCursor(string tableName)
        {
            lock (Session)
            {
                CheckDisposed();

                return OpenCursor(tableName, false);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        void IDisposable.Dispose()
        {
            this.Dispose();
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            lock (this.Session)
            {
                if (!this.Disposed)
                {
                    if (this.cleanup)
                    {
                        Api.JetCloseDatabase(this.Session.Sesid, this.dbid, CloseDatabaseGrbit.None);
                        base.Dispose(disposing);
                        this.cleanup = false;
                    }

                    this.Disposed = true;
                }
            }
        }

        /// <summary>
        /// Checks the disposed.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the object is already disposed.
        /// </exception>
        private void CheckDisposed()
        {
            lock (this.Session)
            {
                if (this.Disposed)
                {
                    throw new ObjectDisposedException(this.GetType().Name);
                }
            }
        }
    }
}
