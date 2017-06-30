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
using Com.Apiomat.Frontend.Helper;
using Com.Apiomat.Frontend.Offline;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using Com.Apiomat.Frontend.External;

namespace Com.Apiomat.Frontend.Offline
{
	public class SQLiteStorage : AbstractStorage
	{
		#region Private Data

        private static OfflineDBHelper offlineDbHelper;
        private static SQLiteStorage instance;

        #endregion

        #region Constructors

        private SQLiteStorage()
        {
            offlineDbHelper = new OfflineDBHelper();
        }

        #endregion

        #region Public data

        public static SQLiteStorage Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SQLiteStorage();
                }
                return instance;
            }
        }

        #endregion

        /* Methods */

        #region Public Methods

        public void InitNewConnection(SQLiteConnection conn)
        {
            offlineDbHelper.InitNewConnection(conn);
		}

		public void InitNewConnection(bool isPersistent = true, string dbPath = null , string dbName = null)
		{
			offlineDbHelper.InitNewConnection(isPersistent, dbPath, dbName);
		}

		/// <summary>
		/// Stores an object to the DB or updates it if it already exists
		/// </summary>
		/// <param name="href">The HREF of the object. If a url with query parameters gets passed, it will be cut off.</param>
		/// <param name="json">The object as json</param>
		/// <param name="httpMethod">httpMethod e.g. "GET", "PUT"</param>
		/// <returns>true if storing the object was successful. False otherwise.</returns>
		public override bool StoreObject(string href, string json, HttpMethod httpMethod)
		{
            string cutHref = CutOffParameters(href);

			long lastUpdated = DateTimeHelper.GetCurrentTimeMillis();
            ObjectStorageContainer objectContainer = new ObjectStorageContainer(cutHref, json, httpMethod.ToString());
			objectContainer.LastUpdate = lastUpdated;
			return offlineDbHelper.Insert(objectContainer) == 1;
		}

		/// <summary>
		/// Stores an object to the DB or updates it if it already exists
		/// </summary>
		/// <param name="href">The HREF of the object. If a url with query parameters gets passed, it will be cut off.</param>
		/// <param name="json">The object as json</param>
		/// <param name="httpMethod">httpMethod e.g. "GET", "PUT"</param>
		/// <returns>true if storing the object was successful. False otherwise.</returns>
		public bool StoreAOMOfflineInfo(AOMOfflineInfo offInfo)
		{
			return offlineDbHelper.Insert(offInfo) == 1;
			//FIXME set/update timestamp?
		}

		public AOMOfflineInfo GetAOMOfflineInfo(string fileKey)
		{
			return offlineDbHelper.FindTaskListById(fileKey);
		}

		public ConcurrentQueue<AOMOfflineInfo> GetAllTasksAsQueue()
		{
			ConcurrentQueue<AOMOfflineInfo> tasks = new ConcurrentQueue<AOMOfflineInfo>();
			var enumTasks = offlineDbHelper.GetTaskQueue();
			foreach (var obj in enumTasks)
			{
				tasks.Enqueue(obj);
			}
			return tasks;
		}

		/// <summary>
		/// Stores an object to the DB or updates it if it already exists
		/// </summary>
		/// <param name="href">The HREF of the object. If a url with query parameters gets passed, it will be cut off.</param>
		/// <param name="json">The object as json</param>
		/// <param name="httpMethod">httpMethod e.g. "GET", "PUT"</param>
		/// <returns>true if storing the object was successfull. False otherwise.</returns>
		public bool StoreTaskObject(string href, string json)
		{
            string cutHref = CutOffParameters(href);
            TaskObject taskObj = new TaskObject(cutHref, json);
            return offlineDbHelper.Insert(taskObj) == 1;
		}

		public bool StoreTaskIdHrefMap(IDictionary<string, string> idHrefMap)
		{
			bool res = true;
			foreach (var entry in idHrefMap)
			{
				res = res && StoreTaskIdHref(entry.Key, entry.Value);
			}
			return res;
		}

		public bool StoreTaskIdHref(string id, string href)
		{
			TaskIdHrefMapping idHrefMap = new TaskIdHrefMapping(id, href);
			return offlineDbHelper.Insert(idHrefMap) == 1;

		}
		public Dictionary<string, string> GetTaskIdHrefMap()
		{
			IEnumerable<TaskIdHrefMapping> enumTaskIdMap = offlineDbHelper.GetTaskIdHrefMap();
			Dictionary<string, string> idHrefMap = new Dictionary<string, string>();
			foreach (var obj in enumTaskIdMap)
			{
				idHrefMap[obj.Id] = obj.Href;
			}
			return idHrefMap;
		}

		/// <summary>
		///Stores a collection of objects to the DB or updates it if it already exists 
		/// </summary>
		/// <param name="requestUrl"></param>
		/// <param name="json"></param>
		/// <returns></returns>
		public override bool StoreCollection(string requestUrl, string json)
		{
			/* Store the objects that are in the collection on their own,
			 * and create a mapping entry for each one */
            JArray hrefCollection = new JArray();
			try
			{
				JArray collection = JArray.Parse(json);
				for (int i = 0; i < collection.Count; i++)
				{
					JObject obj = (JObject)collection[i];
					/* Store the objects and at the same time, if one storage procedure fails, return false.
					 * Storing the objects takes care of insert vs. update.
					 * Speedup might be possible with beginTransaction/endTransaction or bulk insert */
					string objectHref = obj["href"].ToString();
                    string cutObjectHref = CutOffParameters(objectHref);
                    if (StoreObject(cutObjectHref, obj.ToString(), HttpMethod.Get) == false)
					{
						return false;
					}
					/* mapping */
                    if (StoreMapping(requestUrl, cutObjectHref) == false)
					{
						return false;
					}
                    hrefCollection.Add(objectHref);
				}
			}
			catch (Exception e)
			{
                Debug.WriteLine("Failed to store collection " + e.ToString());
				return false;
			}

            /* Collection meta info.
             * Instead of storing the requestUrl, which doesn't seem to get used anywhere, store
             * the HREFs of each object that's in the collection, which is useful for finding out
             * which collections contain a specific object more quickly.
             * InMemoryCache already does this, so by streamlining it, some logic can be written in AbstractStorage.
             */
            CollectionStorageContainer collObj = new CollectionStorageContainer(requestUrl, hrefCollection.ToString());
            collObj.LastUpdate = DateTimeHelper.GetCurrentTimeMillis();
            return offlineDbHelper.Insert(collObj) == 1;
		}

		/// <summary>
		/// Returns the JSON of a cached object. NULL if not found. 
		/// </summary>
		/// <param name="href"></param>
		/// <returns>the JSON of a cached object. NULL if not found. </returns>
		public override string GetStoredObject(string href)
		{
            string cutHref = CutOffParameters(href);

            var obj = offlineDbHelper.FindObjectContainerById(cutHref);
			if (obj == null)
			{
				return null;
			}
			return obj.Body;
		}

		/// <summary>
		/// Returns the JSON of a cached object. NULL if not found.
		/// </summary>
		/// <param name="href"></param>
		/// <returns></returns>
		public string GetStoredTaskObject(string href)
		{
            string cutHref = CutOffParameters(href);
            var obj = offlineDbHelper.FindTaskObjectById(cutHref);
			if (obj == null)
			{
				return null;
			}
			return obj.Body;
		}

		public byte[] GetStoredTaskBinary(string href)
		{
			Debug.WriteLine("trying to get stored binary for href: " + href);
			string binaryDataString = GetStoredTaskObject(href);
			if (binaryDataString == null)
			{
				return null;
			}
			return Convert.FromBase64String(binaryDataString);
		}

		public bool StoreTaskBinary(string href, byte[] binaryData)
		{
			/* SQLite supports blob, but to reuse the existing object table, we convert the bytearray to hex text */
			string binaryDataString = Convert.ToBase64String(binaryData);
			return StoreTaskObject(href, binaryDataString);
		}

		public bool RemoveTaskObject(string href)
		{
			return offlineDbHelper.Delete<TaskObject>(href) == 1;
		}

		public bool RemoveTaskFromTaskList(string fileKey)
		{
			return offlineDbHelper.Delete<AOMOfflineInfo>(fileKey) == 1;
		}

		public override string GetStoredCollection(string url)
		{
			string result = "[";
			try
			{
				StringBuilder sb = new StringBuilder(result);

				/* fetch all objects with a join of the collection with the mapping table */
				string mappingJoin = GetMappingJoin (url);
				IEnumerable<ObjectStorageContainer> cont = offlineDbHelper.GetStoredCollection(mappingJoin);
				/* Note: The previously called method GetStoredCollection() returns an empty list in the following two cases:
					* - A collection was fetched and later all objects were deleted
					* - The collection was never fetched in the first place
					* This needs to be handled.
					* Due to the implementation of InMemoryCache and the need for consistent behavior across all storage types,
					* return an empty list in the first, and null in the latter case.
					*/
				if (cont == null)
				{
					return null;
				}
				if (cont.Count() == 0)
				{
					/* Check which of the previously mentioned two cases this is */
					if (GetCollection(url) == null)
					{
						/* Above condition means: a collection was never fetched in the first place */
						return null;
					}
					result += "]";
					return result;
				}
				// else: Count > 0
				foreach (var obj in cont)
				{
					sb.Append(obj.Body);
					sb.Append(",");
				}
				result = sb.ToString();
				result = result.Substring(0, result.Length - 1); // cut off last ","
			}
			catch (Exception ex)
			{
				Debug.WriteLine("Failed to get stored collection: " + ex.ToString());
				return null;
			}
			result += "]";
			return result;
		}

        internal override long? LoadCollectionLastUpdate(string url)
        {
            long? result = null;
            CollectionStorageContainer collectionContainer = GetCollection(url);
            if (collectionContainer != null)
            {
                result = collectionContainer.LastUpdate;
            }
            return result;
        }

		public override bool RemoveObject(string href)
		{
			base.RemoveObject(href);
			return offlineDbHelper.Delete<ObjectStorageContainer>(CutOffParameters(href)) == 1;
		}

        public override int RemoveCollection(string url)
        {
            base.RemoveCollection(url);
            /* First delete ETags of all objects that are in the collection */

//            string mappingJoin = GetMappingJoin (hashedUrl);
//            IEnumerable<ObjectStorageContainer> objects = offlineDbHelper.GetStoredCollection (mappingJoin);
//            if (objects != null)
//            {
//                foreach (ObjectStorageContainer osc in objects)
//                {
//                    RemoveObjectLastModified (osc.URL);
//                }
//            }
            CollectionStorageContainer csc = GetCollection(url);
            if (csc != null)
            {
                JArray collection = JArray.Parse (csc.Body);
                foreach (JToken jToken in collection)
                {
                    string dataModelHref = (string)(JValue)jToken;
                    RemoveObjectLastModified (dataModelHref);
                }
            }

            return offlineDbHelper.RemoveCollection(url);
        }

		internal override int RemoveAllObjects (string modelHref)
		{
			base.RemoveAllObjects (modelHref);

			int count = 0;
			/* Append "/" in case it's not there yet, so that http://abc/foobar doesn't match http://abc/foo. */
			modelHref = modelHref.EndsWith ("/") ? modelHref : modelHref + "/";
//			modelHref = modelHref.Replace("/", "\\/"); // only for regex
			/* REGEXP isn't implemented by SQLite by default and GLOB doesn't match "/" properly, so use LIKE */
			string query = "select * from ObjectStorageContainer where URL LIKE '" + modelHref + "%'";
			List<ObjectStorageContainer> matchingObjects = offlineDbHelper.FindObjectContainers (query);
			foreach (ObjectStorageContainer osc in matchingObjects)
			{
				count += offlineDbHelper.Delete<ObjectStorageContainer> (osc.URL);
			}
			return count;
		}

		/// <summary>
		/// Will clear ALL data stored in the database (persisted objects, collections, taskobjects and tasklists)
		/// </summary>
		/// <returns></returns>
		public override int Clear()
		{
            base.Clear();
			int result = offlineDbHelper.DeleteAll<ObjectStorageContainer>();
			result += offlineDbHelper.DeleteAll<CollectionStorageContainer>();
			result += offlineDbHelper.DeleteAll<CollectionObjectMapping>();
			result += offlineDbHelper.DeleteAll<TaskObject>();
			result += offlineDbHelper.DeleteAll<AOMOfflineInfo>();
			result += offlineDbHelper.DeleteAll<TaskIdHrefMapping>();
			return result;
		}

		public int ClearTasks()
		{
			return offlineDbHelper.DeleteAll<TaskObject>();
		}
		public int ClearTaskList()
		{
			return offlineDbHelper.DeleteAll<AOMOfflineInfo>();
		}
		public int ClearTaskIdHrefMapping()
		{
			return offlineDbHelper.DeleteAll<TaskIdHrefMapping>();
		}

		public void Close()
		{
			offlineDbHelper.CloseConnection();
		}

        internal override IList<CollectionStorageContainer> GetAllCollections ()
        {
            return offlineDbHelper.GetStoredCollections ();
        }

        internal CollectionStorageContainer GetCollection(string url)
        {
            CollectionStorageContainer result = offlineDbHelper.GetStoredCollections ().Where (collectionStorageContainer => collectionStorageContainer.URL == url).FirstOrDefault();
            return result;
        }

		#endregion
		#region Private Methods
        private bool StoreMapping(string collectionUrl, string objectUrl)
		{
            var colObjMap = new CollectionObjectMapping(collectionUrl, objectUrl);
			return offlineDbHelper.Insert(colObjMap) == 1;
		}

        private string GetMappingJoin(string url)
        {
            return "SELECT HttpMethod, MimeType, URL, Body, LastUpdate " +
            "from ObjectStorageContainer " +
            "inner join " +
            "  (select * from CollectionObjectMapping where CollectionUrl = '" + url + "') mapping " +
            "on ObjectStorageContainer.URL = mapping.ObjectUrl;";
        }

		#endregion
	}
}
