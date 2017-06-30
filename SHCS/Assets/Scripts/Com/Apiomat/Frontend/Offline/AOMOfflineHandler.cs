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
using System.Net.NetworkInformation;
using Com.Apiomat.Frontend.Offline;
using System.Net.Http;
using System.Collections.Generic;
using Com.Apiomat.Frontend.Helper;
using Com.Apiomat.Frontend.Extensions;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using System.Net;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Reflection;
using System.Threading;
using Plugin.Connectivity;
using Plugin.Connectivity.Abstractions;
using Com.Apiomat.Frontend.External;
using SQLite;
using Com.Apiomat.Frontend;

namespace Com.Apiomat.Frontend.Offline
{
    /// <summary>
    /// AOMOfflineHandler
    /// </summary>
    public class AOMOfflineHandler : IAOMOfflineHandler
    {
        public static string TASKS_KEY = "tasks";
        public static string HREFMAP_KEY = "hrefmap_keys";
        public ConcurrentQueue<AOMOfflineInfo> tasks = new ConcurrentQueue<AOMOfflineInfo>();
        private bool _isNetworkOnline;
        /* Map which maps localID -> HREF */
        Dictionary<string, string> mapIdToHref = new Dictionary<string, string>();
        /* maps localID -> Reference to real class instance */
        Dictionary<string, AbstractClientDataModel> mapIdToObj = new Dictionary<string, AbstractClientDataModel>();
        readonly Valve valve = new Valve();
        public ConcurrentQueue<Func<Task>> pendingActions = new ConcurrentQueue<Func<Task>>();

        /// <summary>
        /// Attention: The connectivity event in this class gets triggered BEFORE this constructor returns
        /// </summary>
        /// <param name="dbPath"></param>
        public AOMOfflineHandler(string dbPath = null)
        {
            SQLiteStorage.Instance.InitNewConnection(true, dbPath);
        }

        /// <summary>
        /// Attention: The connectivity event in this class gets triggered BEFORE this constructor returns
        /// </summary>
        /// <param name="conn"></param>
        public AOMOfflineHandler(SQLiteConnection conn)
        {
            SQLiteStorage.Instance.InitNewConnection(conn);
        }

        /// <summary>
        /// This method must not be called before the Datastore has an instance of this class assigned to its _offlineHandler variable.
        /// </summary>
        public void Init()
        {
            this.tasks = SQLiteStorage.Instance.GetAllTasksAsQueue();
            this.mapIdToHref = SQLiteStorage.Instance.GetTaskIdHrefMap();
            /* append the event handler*/

            CrossConnectivity.Current.ConnectivityChanged += new ConnectivityChangedEventHandler(CrossConnectivity_ConnectivityChanged);
            _isNetworkOnline = CrossConnectivity.Current.IsConnected;
            SendTasks().Wait();
        }

        public string CreateNewLocalId()
        {
            string rnd = Guid.NewGuid().ToString().Replace("-", string.Empty).Replace("+", string.Empty).Substring(0, 8);
            return rnd;
        }

        /// <summary>
        /// Writes waiting tasks to disc cache
        /// </summary>
        private void WriteInfosToCache()
        {
            //copy current values to a new array
            var taskArray = this.tasks.ToArray();
            foreach (var offInfo in taskArray)
            {
                SQLiteStorage.Instance.StoreAOMOfflineInfo(offInfo);
            }
        }

        /// <summary>
        /// Writes waiting tasks to disc cache
        /// </summary>
        private void WriteInfoToCache(AOMOfflineInfo info)
        {
            SQLiteStorage.Instance.StoreAOMOfflineInfo(info);
        }

        /// <summary>
        /// Writes mapped hrefs to disc cache
        /// </summary>
        private void WriteHrefMapToCache()
        {
            SQLiteStorage.Instance.StoreTaskIdHrefMap(this.mapIdToHref);
        }

        private void WriteHrefMapToCache(string id, string href)
        {
            SQLiteStorage.Instance.StoreTaskIdHref(id, href);
        }

        public virtual bool IsConnected()
        {
            return _isNetworkOnline;
        }

        public void ClearCache()
        {
            AOMOfflineInfo ignored;
            //dequeue all tasks
            while (tasks.TryDequeue(out ignored)) ;

            mapIdToHref.Clear();
            /* remove also persisted elements */
            try
            {
                SQLiteStorage.Instance.ClearTasks();
                SQLiteStorage.Instance.ClearTaskList();

                // load persisted tasks 
                // crawl through each task and remove each persisted object
                // remove persisted tasks then
            }
            catch (Exception e)
            {
                Debug.WriteLine("Can't clear cache: " + e.ToString());
            }

            try
            {
                SQLiteStorage.Instance.ClearTaskIdHrefMapping();
                //fileCache.remove( HREFMAP_KEY );
            }
            catch (Exception e)
            {
                Debug.WriteLine("Can't remove href map: " + e.ToString());
            }
        }

		public async Task<string> AddTask(HttpMethod httpMethod, string url, byte[] content, bool isImage, bool isRef, bool usePersistentStorage)
		{
			return await AddTask (httpMethod, url, new ByteContainer(content), isImage, isRef, usePersistentStorage).ConfigureAwait (false);
		}

        public async Task<string> AddTask(HttpMethod httpMethod, string url, IDataContainer content, bool isImage, bool isRef, bool usePersistentStorage)
        {
			long dataSize = Int64.MaxValue;
			try
			{
				dataSize = content.Size;
			}
			catch (NotSupportedException)
			{
				Debug.WriteLine ("Can't determine size of data.");
			}
			if (content != null && dataSize <=  Datastore.Instance.MaxOfflineFileBytes)
            {
                string returnedUri = url;
                string localId = null;
                /* decide if we need new localId */
                if (httpMethod.Equals(HttpMethod.Post))
                {
                    localId = CreateNewLocalId();
                    returnedUri += localId;
					SaveBinaryToStorage(returnedUri, await content.ToByteArrayAsync().ConfigureAwait(false), isRef, usePersistentStorage);
                }
                else if (httpMethod.Equals(HttpMethod.Put))
                {
					SaveBinaryToStorage(returnedUri, await content.ToByteArrayAsync().ConfigureAwait(false), isRef, usePersistentStorage);
                }

                try
                {
                    string fileKey = httpMethod.ToString().ToLower() + "_" + DateTimeHelper.GetCurrentTimeMillis();
                    if (httpMethod.Equals(HttpMethod.Delete) == false)
                    {
						SQLiteStorage.Instance.StoreTaskBinary(fileKey, await content.ToByteArrayAsync().ConfigureAwait(false));
                    }
                    AOMOfflineInfo info =
                        new AOMOfflineInfo(httpMethod, url, fileKey, null, localId, isImage, isRef, usePersistentStorage);
                    tasks.Enqueue(info);
                    /* also commit task to disc cache */
                    WriteInfoToCache(info);
                    //WriteInfosToCache( );
                }
                catch (Exception ex)
				{
                    Debug.WriteLine("An error occured during adding a task for a binary: " + ex.ToString());
                }

                return returnedUri;
            }
            else
			{
				Debug.WriteLine ("Either the offline file size limit was exceeded or the size of the data couldn't be determined.");
                throw new ApiomatRequestException(Status.MAX_FILE_SIZE_OFFLINE_EXCEEDED);
            }
        }


        private bool SaveBinaryToStorage(string url, byte[] _content, bool isRef, bool usePersistentStorage)
        {
            AbstractStorage storage = GetObjectStorage(usePersistentStorage);
            return storage.StoreBinary(url, _content, HttpMethod.Get);
        }


        public string AddTask(HttpMethod httpMethod, string url, AbstractClientDataModel dataModel, string parentHref, bool isRef, bool usePersistentStorage)
        {
            string returnedUri = url;
            string localId = null;
            /* decide if we need new localId */
            if (httpMethod.Equals(HttpMethod.Post))
            {
                localId = CreateNewLocalId();
                if (parentHref != null)
                {
                    returnedUri += "/" + parentHref;
                }
                returnedUri += "/" + localId;
                mapIdToObj[localId] = dataModel;
                // Addition for also saving to storage and not just outgoing queue
                SaveObjectToStorage(returnedUri, dataModel, true, isRef, usePersistentStorage);
            }
            else if (httpMethod.Equals(HttpMethod.Put))
            {
                /* Needed because otherwise there's an error in e.g. the following case:
                 * save model online, post image to model offline, go online (sync), then load image.
                 * In that case, when going online, the image url (with local id) in the real data model needs to be
                 * exchanged by the one from the temp data model with image url with real id.
                 */
                string id = url.Substring(url.LastIndexOf("/") + 1);
                mapIdToObj[id] = dataModel;
                SaveObjectToStorage(returnedUri, dataModel, false, isRef, usePersistentStorage);
            }
            else if (httpMethod.Equals(HttpMethod.Delete))
            {
                /* immediately delete from offline storage */
                Datastore.Instance.DeleteObjectFromStorage(url, isRef);
            }
            try
            {
                string fileKey =
                   httpMethod.ToString().ToLower() + "_" + DateTimeHelper.GetCurrentTimeMillis() + "_" +
                       (localId != null ? localId : CreateNewLocalId());
                SQLiteStorage.Instance.StoreTaskObject(fileKey, dataModel.ToJson());
                AOMOfflineInfo info =
                   new AOMOfflineInfo(httpMethod, url, fileKey, dataModel.GetType(), localId, parentHref, false,
                       isRef, usePersistentStorage);
                tasks.Enqueue(info);
                /* also commit tasks to disc cache */
                //WriteInfosToCache( );
                WriteInfoToCache(info);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("An error occured during adding a task: " + ex.ToString());
            }
            return returnedUri;
        }

        private Object GetDeclaredFieldViaReflection(Object instance, string propertyName)
        {

            var field = instance.GetType().GetTypeInfo().GetDeclaredField(propertyName);
            if (field == null)
            {
                return null;
            }
            return field.GetValue(instance);
        }

        private bool SaveObjectToStorage(string url, AbstractClientDataModel dataModel, bool injectHref, bool isRef, bool usePersistentStorage)
        {
            AbstractStorage storage = GetObjectStorage(usePersistentStorage);
            string dataToStore;
            try
            {
                if (injectHref && isRef == false)
                {
                    dataModel.Data["href"] = url;
                }
            }
            // NoSuchFieldException, IllegalAccessException, JSONException
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                return false;
            }
            dataToStore = dataModel.Data.ToString();
            if (isRef)
            {
                // store mapping - in opposite to ref mapping at GET while online, the id needs to be removed instead of added
                string mappingHref = url.Substring(0, url.LastIndexOf("/"));
                storage.StoreObject(mappingHref, url, HttpMethod.Get);
            }
            return storage.StoreObject(url, dataToStore, HttpMethod.Get);
        }

        public void AddTask(HttpMethod httpMethod, string url, bool isImage, bool isRef, bool usePersistentStorage)
        {
			IDataContainer nullContainer = null;
			AddTask(httpMethod, url, nullContainer, isImage, isRef, usePersistentStorage);
        }

        public async Task SendTasks1()
        {
            //This runs on the UI thread.
            Func<Task> current;
            while (pendingActions.TryDequeue(out current))
                await current().ConfigureAwait(false);
            valve.Exit();
        }

        public async Task SendTasks()
        {
            while (IsConnected() && tasks.Count > 0)
            {
                AOMOfflineInfo task = null;
                tasks.TryPeek(out task);

                Debug.WriteLine("Size of tasks " + tasks.Count);

                if (task == null)
                {
                    continue;
                }
                try
                {
                    /* also write to persisted list of tasks */
                    //WriteInfosToCache( );

                    bool isStaticData = task.Type == null;
                    AbstractClientDataModel tmpModel = null;
                    byte[] staticData = null;
                    //First: try to load binary, else try to load object
                    if (isStaticData)
                    {
                        staticData = SQLiteStorage.Instance.GetStoredTaskBinary(task.FileKey);
                        if (staticData == null)
                        {
                            Debug.WriteLine(
                            "Can't find persisted class instance. Maybe cache size was exceeded and class instance was deleted");
                            throw new ApiomatRequestException(Status.CANT_WRITE_IN_CACHE, 0);
                        }
                        /* remove file from cache */
                        SQLiteStorage.Instance.RemoveTaskObject(task.FileKey);
                        await SendStaticDataToServer(task, staticData).ConfigureAwait(false);
                    }
                    else
                    {
                        string json = SQLiteStorage.Instance.GetStoredTaskObject(task.FileKey);
                        if (json == null)
                        {
                            Debug.WriteLine(
                            "Can't find persisted class instance. Maybe cache size was exceeded and class instance was deleted");
                            throw new ApiomatRequestException(Status.CANT_WRITE_IN_CACHE, 0);
                        }
                        /* generate new ACDM */
                        tmpModel = (AbstractClientDataModel)Activator.CreateInstance(task.Type);
                        /* Get JSON from disc cache */
                        tmpModel.FromJson(json);
                        /* remove file from cache */
                        SQLiteStorage.Instance.RemoveTaskObject(task.FileKey);
                        await SendModelToServer(task, tmpModel).ConfigureAwait(false);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Can't update class instance: " + ex.ToString());
                }
                finally
                {
                    AOMOfflineInfo task2;
                    tasks.TryDequeue(out task2);
                    SQLiteStorage.Instance.RemoveTaskFromTaskList(task.FileKey);
                    if (task != null && task2 != null && task.FileKey != null && task2.FileKey != null && task2.FileKey.Equals(task.FileKey) == false)
                    {
                        Debug.WriteLine("dequeued task was not equal to peeked task");
                    }
                }
            }
            mapIdToObj.Clear();
            return;
        }


        internal protected void CrossConnectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            ConnectivityChanged(e.IsConnected);
        }
        internal protected void ConnectivityChanged(bool isAvailable)
        {
            _isNetworkOnline = isAvailable;

            if (IsConnected())
            {
                pendingActions.Enqueue(SendTasks);
                if (valve.TryEnter())
                    Task.Run(() => SendTasks1().Wait());
            }
            Debug.WriteLine("network status changed: " + (isAvailable ? "now online!" : "now offline!"));
        }


        private async Task SendStaticDataToServer(AOMOfflineInfo task, byte[] data)
        {
            try
            {
                if (task.Method.Equals(HttpMethod.Post))
                {
                    IDataContainer datacontainer = new ByteContainer(data);
                    string href = await Datastore.Instance.PostStaticDataOnServerAsync(datacontainer, task.IsImage,
                        task.IsRef, task.UsePersistentStorage).ConfigureAwait(false);
                    if (string.IsNullOrWhiteSpace(href))
                    {
                        throw new ApiomatRequestException(Status.HREF_NOT_FOUND, HttpStatusCode.Created);
                    }

                    mapIdToHref[task.LocalId] = href;
                    WriteHrefMapToCache(task.LocalId, href);

                }
                else if (task.Method.Equals(HttpMethod.Delete))
                {
                    await Datastore.Instance.DeleteOnServerAsync(task.Url, task.IsRef, task.UsePersistentStorage).ConfigureAwait(false);
                }
            }
            catch (ApiomatRequestException e)
            {
                Debug.WriteLine("Can't delete or save static data: " + e.ToString());

            }
        }

        private string GetHref(AbstractClientDataModel tmpModel)
        {
            string href = tmpModel.Href;
            if (tmpModel.Offline == false)
            {
                return href;
            }
            InjectDataUrl(tmpModel);

            string id = tmpModel.Id;
            if (id == null || id.Length == 0)
            {
                Debug.WriteLine("No local ID found");
                return href;
            }
            string tempHref = mapIdToHref.GetOrDefault<string, string>(id, null);
            href = tempHref != null && href.Length > 0 ? tempHref : href;
            href = InjectHref(ref tmpModel, href);
            return href;
        }



        /// <summary>
        /// Returns "real" href for given local href, if exists otherwise false
        /// </summary>
        /// <param name="_localHref"></param>
        /// <returns>"real" href or null if not found in list</returns>
        private string GetHrefForLocalHref(string _localHref)
        {
            string id = _localHref.Substring(_localHref.LastIndexOf("/") + 1);
            return mapIdToHref.GetOrDefault<string, string>(id, null);
        }

        private string InjectHref(ref AbstractClientDataModel tmpModel, string href)
        {
            /* inject server href */
            try
            {
                var field = typeof(AbstractClientDataModel).GetTypeInfo().GetDeclaredField("_href");
                //var field = typeof(AbstractClientDataModel).GetField("_href", BindingFlags.NonPublic | BindingFlags.Instance);
                field.SetValue(tmpModel, href);

                // Also inject into data, because at some points fromJson gets called
                JObject data;
                var dataFieldInfo = typeof(AbstractClientDataModel).GetTypeInfo().GetDeclaredField("_data");
                //var dataFieldInfo = typeof(AbstractClientDataModel).GetField("_data", BindingFlags.NonPublic | BindingFlags.Instance);
                var dataField = dataFieldInfo.GetValue(tmpModel);
                data = (JObject)dataField;
                data["href"] = href;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Can't inject href" + e.ToString());
                href = null;
            }
            return href;
        }

        private void InjectDataUrl(AbstractClientDataModel model)
        {
            try
            {
                bool wasUpdateFound = false;
                /* check if there are also localHREFs for files/images in class instances */
                string jsonStr = model.ToJson();
                JObject jsonRep = JObject.Parse(jsonStr);
                foreach (var curToken in jsonRep)
                {
                    string jsonKey = curToken.Key;
                    /* we only check properties with URL at the end */
                    if (jsonKey.EndsWith("URL") == false)
                    {
                        continue;
                    }
                    string jsonObj = jsonRep.GetOrDefault<string>(jsonKey, null);
                    if (jsonObj == null)
                    {
                        continue;
                    }
                    string realHref = GetHrefForLocalHref(jsonObj);
                    if (realHref == null)
                    {
                        continue;
                    }
                    jsonRep[jsonKey] = realHref;
                    wasUpdateFound = true;
                }
                /* if we updated a field than update the class instance */
                if (wasUpdateFound)
                {
                    model.FromJson(jsonRep.ToString());
                }
            }
            catch (JsonException e)
            {
                Debug.WriteLine("injecting data url failed: " + e.ToString());
            }
        }

        private async Task SendModelToServer(AOMOfflineInfo task, AbstractClientDataModel tmpModel)
        {
            /* check httpMethod and decide if we have to call post/put/delete on datastore */
            if (task.Method.Equals(HttpMethod.Post))
            {
                try
                {
                    string url = task.Url;
                    /* seems to be a reference */
                    if (task.RefName != null && task.RefName.Length > 0)
                    {
                        string parentID = url.Substring(url.LastIndexOf("/") + 1);
                        /* add correct href to referenced class instance */
                        GetHref(tmpModel);

                        string parentHref = mapIdToHref.GetOrDefault<string, string>(parentID, null);
                        if (string.IsNullOrWhiteSpace(parentHref))
                        {
                            parentHref = task.Url;
                        }
                        url = parentHref + "/" + task.RefName;
                    }
                    if (url == null)
                    {
                        ApiomatRequestException e =
                            new ApiomatRequestException(Status.HREF_NOT_FOUND, HttpStatusCode.Created);
                        Debug.WriteLine("Can't save class instance: " + e.ToString());
                    }
                    if (task.IsRef == false)
                    {
                        var field = typeof(AbstractClientDataModel).GetTypeInfo().GetDeclaredField("_href");
                        //var field = typeof(AbstractClientDataModel).GetField("_href", BindingFlags.NonPublic | BindingFlags.Instance);
                        field.SetValue(tmpModel, null);

                        tmpModel.Data.Remove("href");
                    }

                    string href = await Datastore.Instance.PostOnServerAsync(tmpModel, url, task.IsRef, task.UsePersistentStorage).ConfigureAwait(false);
                    if (href == null || href.Length == 0)
                    {
                        ApiomatRequestException e = new ApiomatRequestException(Status.HREF_NOT_FOUND, HttpStatusCode.Created);
                        Debug.WriteLine("Can't save class instance: " + e.ToString());
                    }
                    /* inform listeners */
                    mapIdToHref[task.LocalId] = href;
                    WriteHrefMapToCache(task.LocalId, href);
                    //WriteHrefMapToCache();
                    /* update reference object if there */
                    AbstractClientDataModel realModel = UpdateRealModelAfterPost(task.LocalId, href);
                    if (realModel != null)
                    {
                        UpdateOfflineStorageAfterPost(task.LocalId, href, realModel.ToJson(),
                            task.UsePersistentStorage);
                    }

                    /* Load, because otherwise when loading a reference later, there might be no reference href in some cases */
                    await realModel.LoadAsync(task.UsePersistentStorage).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Can't save class instance: " + e.ToString());
                }
            }
            else if (task.Method.Equals(HttpMethod.Put))
            {
                tmpModel.Offline = true;
                string href = GetHref(tmpModel);
                if (href == null || href.Length == 0)
                {
                    ApiomatRequestException e =
                        new ApiomatRequestException(Status.HREF_NOT_FOUND, HttpStatusCode.OK);
                    Debug.WriteLine("Can't save class instance: " + e.ToString());
                }

                try
                {
                    await Datastore.Instance.UpdateOnServerAsync(tmpModel, task.IsRef,
                        task.UsePersistentStorage).ConfigureAwait(false);
                    AbstractClientDataModel realModel = UpdateRealModelAfterPut(tmpModel.Id);

                    if (realModel != null)
                    {
                        /* Necessary so that the object contains for example an imageUrl
                            * similar to load above (in POST - necessary for references) */
                        await realModel.LoadAsync(task.UsePersistentStorage).ConfigureAwait(false);
                    }
                }
                catch (ApiomatRequestException e)
                {
                    Debug.WriteLine("Can't update class instance: " + e.ToString());

                }
            }
            else if (task.Method.Equals(HttpMethod.Delete))
            {
                string href = GetHref(tmpModel);
                bool isRef = task.RefName != null && task.RefName.Length > 0;
                /* seems to be a reference */
                if (isRef)
                {
                    string parentID = task.Url.Substring(task.Url.LastIndexOf("/") + 1);
                    /* add correct href to referenced class instance */
                    string parentHref = mapIdToHref.GetOrDefault<string, string>(parentID, null);
                    if (string.IsNullOrWhiteSpace(parentHref))
                    {
                        parentHref = task.Url;
                    }
                    href =
                        parentHref + "/" + task.RefName + "/" +
                            tmpModel.Href.Substring(tmpModel.Href.LastIndexOf("/") + 1);
                }
                if (href != null && href.Length > 0)
                {
                    try
                    {
                        await Datastore.Instance.DeleteOnServerAsync(href, task.IsRef,
                            task.UsePersistentStorage).ConfigureAwait(false);

                    }
                    catch (ApiomatRequestException e)
                    {
                        Debug.WriteLine("Can't delete class instance: " + e.ToString());
                    }
                }
            }
        }



        /// <summary>
        /// This method updates the reference class instance with a new href from server
        /// </summary>
        /// <param name="_localId"></param>
        /// <param name="_href"></param>
        /// <returns></returns>
        private AbstractClientDataModel UpdateRealModelAfterPost(string _localId, string _href)
        {
            Debug.WriteLine("Size: " + mapIdToObj.Count);

            AbstractClientDataModel model = mapIdToObj[_localId];
            if (mapIdToObj.Remove(_localId) == false)
            {
                return null;
            }
            if (model == null || model.Offline == false)
            {
                return model;
            }
            Debug.WriteLine("LocalID is " + _localId);
            /* inject new HREF to class instance */
            InjectHref(ref model, _href);
            model.Offline = false;
            mapIdToObj[model.Id] = model;
            return model;
        }

        private void UpdateOfflineStorageAfterPost(string _localId, string _newHref, string _newJson,
            bool usePersistentStorage)
        {
            string oldHref =
                _newHref.Replace(_newHref.Substring(_newHref.LastIndexOf("/") + 1), _localId);
            AbstractStorage storage = GetObjectStorage(usePersistentStorage);

            string loadedObj = storage.GetStoredObject(oldHref);
            if (string.IsNullOrWhiteSpace(loadedObj))
            {
                return;
            }
            storage.RemoveObject(oldHref);
            storage.StoreObject(_newHref, _newJson, HttpMethod.Get);
        }

        private AbstractStorage GetObjectStorage(bool usePersistentStorage)
        {
            if (usePersistentStorage)
            {
                return SQLiteStorage.Instance;
            }
            return InMemoryCache.Instance;
        }

        /// <summary>
        /// Updates real model binary *URLs with changed tempModel binary *URLs
        /// </summary>
        /// <param name="_realId"></param>
        /// <returns></returns>
        private AbstractClientDataModel UpdateRealModelAfterPut(string _realId)
        {
            Debug.WriteLine("Size: " + mapIdToObj.Count);
            AbstractClientDataModel model = mapIdToObj.GetOrDefault<string, AbstractClientDataModel>(_realId, null);
            if (model == null)
            {
                return null;
            }

            Debug.WriteLine("LocalID is " + _realId);
            /* inject new HREF to class instance */
            InjectDataUrl(model);
            return model;
        }

    }

    ///<summary>Ensures that a block of code is only executed once at a time.</summary>
    class Valve
    {
        int isEntered;  //0 means false; 1 true

        ///<summary>Tries to enter the valve.</summary>
        ///<returns>True if no other thread is in the valve; false if the valve has already been entered.</returns>
        public bool TryEnter()
        {
            if (Interlocked.CompareExchange(ref isEntered, 1, 0) == 0)
                return true;
            return false;
        }

        ///<summary>Allows the valve to be entered again.</summary>
        public void Exit()
        {
            Debug.Assert(isEntered == 1);
            isEntered = 0;
        }
    }
}
