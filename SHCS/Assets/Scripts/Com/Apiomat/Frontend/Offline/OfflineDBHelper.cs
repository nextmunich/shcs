/*
 * Copyright (c) 2011 - 2016, Apinauten GmbH
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without modification, 
 * are permitted provided that the following conditions are met:
 *
 *  * Redistributions of source code must retain the above copyright notice, this 
 *    list of conditions and the following disclaimer.
 *  * Redistributions in binary form must reproduce the above copyright notice, 
 *    this list of conditions and the following disclaimer in the documentation 
 *    and/or other materials provided with the distribution.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND 
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. 
 * IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, 
 * INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING,
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, 
 * DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF 
 * LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE 
 * OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED 
 * OF THE POSSIBILITY OF SUCH DAMAGE.
 * 
 * THIS FILE IS GENERATED AUTOMATICALLY. DON'T MODIFY IT.
 */
using System;
using System.Diagnostics;
using System.Collections.Generic;
using SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Com.Apiomat.Frontend.Offline;

namespace Com.Apiomat.Frontend.Offline
{
    public class OfflineDBHelper
    {
        /* When you change the database schema, you must increment the database version.
         * 
         * History:
         * 1: Initial
         * 2: AOM-1751
         */
        private static string DATABASE_PERSISTENT_NAME = "Offline.db";
        private static string DATABASE_INMEMORY_NAME = ":memory:";
        private SQLiteConnection m_dbConnection;
		/* Not static, because theoretically multiple OfflineDBHelper objects can exist,
		 * each with its own SQLiteConnection, using different SQLite DBs */
		private readonly object DB_LOCK = new object ();

        #region Constructors
        public OfflineDBHelper( bool isPersistent = true, string dbPath = null, string dbName = null)
        {
            InitNewConnection(isPersistent, dbPath, dbName);
        }

        #endregion
        public int Delete<T>(string key)
        {
			lock (DB_LOCK)
			{
				return m_dbConnection.Delete<T> (key);
			}
        }
        public int DeleteAll<T>()
		{
			lock (DB_LOCK)
			{
				return m_dbConnection.DeleteAll<T> ();
			}
        }


        public AOMOfflineInfo FindTaskListById(Object id)
        {
            try
			{
				lock (DB_LOCK)
				{
                	return m_dbConnection.Find<AOMOfflineInfo>(id);
				}
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        public CollectionStorageContainer FindCollectionContainerById(Object id)
        {
            try
			{
				lock (DB_LOCK)
				{
                	return m_dbConnection.Find<CollectionStorageContainer>(id);
				}
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        public ObjectStorageContainer FindObjectContainerById(Object id)
        {
            try
			{
				lock (DB_LOCK)
				{
                	return m_dbConnection.Find<ObjectStorageContainer>(id);
				}
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

		internal ObjectStorageContainer FindObjectContainer(System.Linq.Expressions.Expression<Func<ObjectStorageContainer, bool>> predicate)
		{
			try
			{
				lock (DB_LOCK)
				{
					return m_dbConnection.Find<ObjectStorageContainer>(predicate);
				}
			}
			catch (InvalidOperationException)
			{
				return null;
			}
		}

		internal List<ObjectStorageContainer> FindObjectContainers(string query)
		{
			try
			{
				lock (DB_LOCK)
				{
					return m_dbConnection.Query<ObjectStorageContainer>(query);
				}
			}
			catch (InvalidOperationException)
			{
				return null;
			}
		}

        public TaskObject FindTaskObjectById(Object id)
        {
            try
			{
				lock (DB_LOCK)
				{
                	return m_dbConnection.Find<TaskObject>(id);
				}
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        public IEnumerable<TaskIdHrefMapping> GetTaskIdHrefMap()
		{
			lock (DB_LOCK)
			{
				return m_dbConnection.Table<TaskIdHrefMapping> ().ToList ();
			}
        }

        public IEnumerable<AOMOfflineInfo> GetTaskQueue()
		{
			lock (DB_LOCK)
			{
				return m_dbConnection.Table<AOMOfflineInfo> ()
	                .OrderBy (aomOfflineInfo => aomOfflineInfo.Timestamp)
	                .ToList ();
			}
        }

        public int RemoveCollection(string url)
        {
            /* delete all object hrefs that are in the result after a join of the collection with the mapping table
             * but ONLY delete the objects that are not part of another collection (count = 1).
             */
            string statement = "DELETE FROM ObjectStorageContainer WHERE url IN " +
            "  (SELECT filteredmappingfidobject " +
            "   FROM ( " +
                "     (select CollectionObjectMapping.ObjectUrl as filteredmappingfidobject from CollectionObjectMapping where CollectionObjectMapping.CollectionUrl = '" + url + "' ) as filteredmapping " +
            "     inner join " +
            "      (select CollectionObjectMapping.ObjectUrl as countedmappingfidobject, count( CollectionObjectMapping.ObjectUrl ) as objectcount " +
            "      from CollectionObjectMapping " +
            "      group by CollectionObjectMapping.ObjectUrl ) as countedmapping " +
            "     on filteredmappingfidobject = countedmappingfidobject " +
				"   ) WHERE objectcount = 1 )";
			lock (DB_LOCK)
			{
				int result = m_dbConnection.Execute (statement);
				m_dbConnection.Table<CollectionObjectMapping> ().Delete (x => x.CollectionUrl.Equals (url));
				m_dbConnection.Table<CollectionStorageContainer> ().Delete (x => x.URL.Equals (url));
				return result;
			}
        }

        public IEnumerable<ObjectStorageContainer> GetStoredCollection(string query)
	{
		lock (DB_LOCK)
		{
			return m_dbConnection.Query<ObjectStorageContainer>(query);
		}
        }

        internal IList<CollectionStorageContainer> GetStoredCollections()
		{
			lock (DB_LOCK)
			{
				return m_dbConnection.Table<CollectionStorageContainer> ().ToList ();
			}
        }

        public TaskIdHrefMapping FindTaskIdHrefMappingById(Object id)
		{
			lock (DB_LOCK)
			{
				return m_dbConnection.Find<TaskIdHrefMapping> (id);
			}
        }

		public int Insert(object objToStore)
		{
			lock (DB_LOCK)
			{
				return m_dbConnection.InsertOrReplace (objToStore);
			}
		}
        
		public void InitNewConnection(SQLiteConnection conn)
		{
			m_dbConnection = conn;
			createTables ();
        }

		public void InitNewConnection(bool isPersistent = true, string dbPath = null, string dbName = null)
		{
			if (m_dbConnection != null)
			{
				CloseConnection ();
			}
			string dbTarget;
			if (isPersistent)
			{
				dbTarget = String.IsNullOrEmpty(dbPath) ? "" : dbPath;
				dbTarget += String.IsNullOrEmpty(dbName) ? DATABASE_PERSISTENT_NAME : dbName;
			}
			else
			{
				dbTarget = DATABASE_INMEMORY_NAME;
			}
            try
            {
                m_dbConnection = new SQLiteConnection(dbTarget);
                createTables();
            }
            catch (Exception ex)
			{
				/* When using Xamarin, this exception is OK, as long as the DB connection gets properly initiated
				 * with a SQLiteConnection object at a later point in time */
				Debug.WriteLine("Can't init new connection to DB: " + ex.ToString());
			}
		}

        private void createTables()
		{
			lock (DB_LOCK)
			{
				m_dbConnection.CreateTable<ObjectStorageContainer> ();
				m_dbConnection.CreateTable<CollectionStorageContainer> ();
				m_dbConnection.CreateTable<CollectionObjectMapping> ();
				m_dbConnection.CreateTable<TaskObject> ();
				m_dbConnection.CreateTable<AOMOfflineInfo> ();
				m_dbConnection.CreateTable<TaskIdHrefMapping> ();
			}
            /* TODO: DB schema update
             * 
             * Tables might need to be altered if the DB version changes (schema gets changed).
             * PRAGMA user_version didn't work.
             * Manual check via ColumnInfo doesn't work as well because it only contains Name and notnull info,
             * not type info of the column for example.
             * 
             * DB schema change history:
             * First change: Column type of "Id" in CollectionObjectMapping from int to string
             * 
             * Maybe the CreateTable command used above takes care of schema updates automatically?
             * That's not documented very well.
             */
        }

		public void CloseConnection()
		{
			lock (DB_LOCK)
			{
				m_dbConnection.Close ();
			}
		}

    }
}