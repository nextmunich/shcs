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
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Com.Apiomat.Frontend.Basics;
using Com.Apiomat.Frontend.Offline;
using Com.Apiomat.Frontend.Extensions;
using Com.Apiomat.Frontend.Helper;
using System.Text;
using System.Runtime.ExceptionServices;
using System.Diagnostics;
using System.Linq;
using System.Collections;
using Com.Apiomat.Frontend.External;
using SQLite;
using Com.Apiomat.Frontend;
using System.IO;

namespace Com.Apiomat.Frontend
{
    public class Datastore
    {
        #region Public data

        public enum AuthType
        {
            GUEST,
            USERNAME_PASSWORD,
            OAUTH2_TOKEN
        }

        public enum AOMCacheStrategy
        {
            NETWORK_ONLY,
            // Don't use caching (on save as well as read)
            NETWORK_ELSE_CACHE,
            // Use the cache only if the server is unreachable or returns 304
            CACHE_ELSE_NETWORK,
            // Use the cache, but if nothing is found there send a request
            CACHE_THEN_NETWORK
            // First read from cache, than send a request to the server
        }
        
        #endregion

        #region Private data

        private static Datastore _instance;

        private static AOMCacheStrategy _cacheStrategy = AOMCacheStrategy.NETWORK_ELSE_CACHE;
        private static bool _useETag = true;
        // default for NETWORK_ELSE_CACHE default strategy
        private static AOMConcurrentDictionary<Type, Boolean> _offlineMapping = new AOMConcurrentDictionary<Type, Boolean>();
        private string _baseUrl;
        private AuthType _usedAuthType;
        private bool wasLoadedFromStorage;
        private IAOMOfflineHandler _offlineHandler = null;
        private bool _useDeltaSync;
        private DeltaSyncStrategy _deltaSyncStrategy;
        private long _maxOfflineFileBytes = 15 * 1000 * 1000; // 15 MB

        #endregion

        #region Private properties

        private string BaseUrl { get { return _baseUrl; } }

        private string AppName { get { return BaseUrl.Substring(BaseUrl.LastIndexOf('/') + 1); } }

        #endregion

        #region Public properties

        /// <summary>
        /// Returns a singleton instance of the Datastore
        /// </summary>
        /// <value>The singleton instance.</value>
        public static Datastore Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new InvalidOperationException("The datastore is not configured yet.");
                }
                else
                {
                    return _instance;
                }
            }
        }

        /// <summary>
        /// Gets the type of auth method that's being used
        /// </summary>
        /// <value>The type of auth method that's being used</value>
        public AuthType UsedAuthType { get { return _usedAuthType; } }

        [Obsolete("Use DeltaSyncStrategy instead")]
        public bool UseDeltaSync
        {
            get { return _useDeltaSync; }
            set
            {
                _useDeltaSync = value;
                DeltaSyncStrategy = value ? DeltaSyncStrategy.OBJECT_BASED : DeltaSyncStrategy.NONE;
            }
        }

        public DeltaSyncStrategy DeltaSyncStrategy { get { return _deltaSyncStrategy; } set { _deltaSyncStrategy = value; } }

        public long MaxOfflineFileBytes { get { return _maxOfflineFileBytes; } set { _maxOfflineFileBytes = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether an object's state gets checked when working with the object.
        /// For example, when an object gets saved asynchronously, it's in the "persisting" state until a response is received from the server.
        /// When saving again while the object is in the "persisting" state, an exception gets thrown.
        /// The throwing of an exception can be turned of by setting this property to false.
        /// </summary>
        /// <value><c>true</c> to check object state; otherwise, <c>false</c>.</value>
        public bool CheckObjectState { get; set; }

        public int ClientTimeout
        {
            get { return AOMHttpClientFactory.GetAomHttpClient().ClientTimeout; }
            set { AOMHttpClientFactory.GetAomHttpClient().ClientTimeout = value; }
        }

		public Dictionary<string, string> CustomHeaders
		{
			get { return AOMHttpClientFactory.GetAomHttpClient().CustomHeaders; }
			set { AOMHttpClientFactory.GetAomHttpClient().CustomHeaders = value; }
		}

        public IAOMOfflineHandler OfflineHandler { get { return _offlineHandler; } }

        #endregion

        #region Constructors

        private Datastore(string baseUrl, string apiKey, string system, string userName, string password)
        {
            _baseUrl = baseUrl;
            CommonInit();
            _usedAuthType = (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password)) ? AuthType.GUEST : AuthType.USERNAME_PASSWORD;
            AOMHttpClientFactory.CreateHttpClient(baseUrl, apiKey, userName, password, system, this._usedAuthType, null);
        }

        private Datastore(string baseUrl, string apiKey, string system, string sessionToken)
        {
            _baseUrl = baseUrl;
            CommonInit();
            _usedAuthType = string.IsNullOrEmpty(sessionToken) ? AuthType.GUEST : AuthType.OAUTH2_TOKEN;
            AOMHttpClientFactory.CreateHttpClient(baseUrl, apiKey, null, null, system, this._usedAuthType, sessionToken);
        }

        /* Can't set the other common fields because they're readonly and only assignable in a constructor */
        private void CommonInit()
        {
            CheckObjectState = true;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Configures and returns a Datastore instance. If no parameter is given, the static values from User will be used.
        /// </summary>
        /// <param name="baseUrl">(Optional) The base URL of the ApiOmat service; usually https://apiomat.org/yambas/rest/apps/YOURAPPNAME (see the User class)</param>
        /// <param name="apiKey">(Optional) The API key of your application (see the User class)</param>
        /// <param name="system">(Optional) The system which will be used (see the User class) (should be LIVE, TEST or STAGING)</param>
        /// <returns>The configured Datastore instance</returns>
        public static Datastore ConfigureAsGuest(string baseUrl = null, string apiKey = null, string system = null)
        {
            return ConfigureWithCredentials(baseUrl, apiKey, system);
        }

        /// <summary>
        /// Configures and returns a Datastore instance
        /// </summary>
        /// <param name="user">The user where userName and password are the login credentials</param>
        /// <returns>The configured Datastore instance</returns>
        public static Datastore ConfigureWithCredentials(User user)
        {
            return ConfigureWithCredentials(user.UserName, user.Password, User.BaseUrl, User.ApiKey, User.System);
        }

        /// <summary>
        /// Configures and returns a Datastore instance
        /// </summary>
        /// <param name="userName">Username</param>
        /// <param name="password">Password</param>
        /// <param name="baseUrl">(Optional) The base URL of the ApiOmat service; usually https://apiomat.org/yambas/rest/apps/YOURAPPNAME (see the User class). The static attribute in the User class will be used if the parameter isn't provided.</param>
        /// <param name="apiKey">(Optional) The API key of your application (see the User class). The static attribute in the User class will be used if the parameter isn't provided.</param>
        /// <param name="system">(Optional) The system which will be used (see the User class) (should be LIVE, TEST or STAGING). The static attribute in the User class will be used if the parameter isn't provided.</param>
        /// <returns>The configured Datastore instance</returns>
        public static Datastore ConfigureWithCredentials(string userName, string password, string baseUrl = null, string apiKey = null, string system = null)
        {
            baseUrl = baseUrl == null ? User.BaseUrl : baseUrl;
            apiKey = apiKey == null ? User.ApiKey : apiKey;
            system = system == null ? User.System : system;
            _instance = new Datastore(baseUrl, apiKey, system, userName, password);
            return Instance;
        }

        /// <summary>
        /// Configures and returns a Datastore instance with a session token
        /// </summary>
        /// <param name="sessionToken">The session token</param>
        /// <param name="baseUrl">(Optional) The base URL of the ApiOmat service; usually https://apiomat.org/yambas/rest/apps/YOURAPPNAME (see the User class). The static attribute in the User class will be used if the parameter isn't provided.</param>
        /// <param name="apiKey">(Optional) The API key of your application (see the User class). The static attribute in the User class will be used if the parameter isn't provided.</param>
        /// <param name="system">(Optional) The system which will be used (see the User class) (should be LIVE, TEST or STAGING). The static attribute in the User class will be used if the parameter isn't provided.</param>
        /// <returns>The configured Datastore instance</returns>
        public static Datastore ConfigureWithSessionToken(string sessionToken, string baseUrl = null, string apiKey = null, string system = null)
        {
            baseUrl = baseUrl == null ? User.BaseUrl : baseUrl;
            apiKey = apiKey == null ? User.ApiKey : apiKey;
            system = system == null ? User.System : system;
            _instance = new Datastore(baseUrl, apiKey, system, sessionToken);
            return Instance;
        }

        /// <summary>
        /// Configures and returns a Datastore instance with a session token
        /// </summary>
        /// <param name="user">The user that has a session token attached</param>
        /// <returns>The configured Datastore instance</returns>
        public static Datastore ConfigureWithSessionToken(User user)
        {
            _instance = new Datastore(User.BaseUrl, User.ApiKey, User.System, user.SessionToken);
            return Instance;
        }
        /// <summary>
        /// Creates a href for the class REST resource with parameters.
        ///  Don't use this method for appending a model object ID!
        /// </summary>
        /// <returns>The created href.</returns>
        /// <param name="moduleName">moduleName e.g. "Basics"</param>
        /// <param name="simpleModelName">the simple model name, e.g."User"</param>
        /// <param name="withReferencedHrefs">Set to <c>true</c> to get also all HREFs of referenced class instances</param>
        /// <param name="query">the query param </param>
        /// 
        public string CreateModelHrefWithParams(string moduleName, string simpleModelName, bool withReferencedHrefs = false, string query = null)
        {
            string createdHref = CreateModelHref(moduleName, simpleModelName);
            Uri url = new Uri (createdHref);
            url = url.AddParameterToUri("withReferencedHrefs", withReferencedHrefs.ToString().ToLower());
            url = url.AddParameterToUri("q", query == null ? "" : query);
            return url.ToString ();
        }

        public string CreateModelHref(string moduleName, string simpleName)
        {
            return BaseUrl + "/models/" + moduleName + "/" + simpleName;
        }

        public string CreateStaticDataHref(bool image)
        {
            string resource = image == true ? "images" : "files";
            return BaseUrl + "/data/" + resource + "/";
        }

        /// <summary>
        /// Return true if we have to use offline handler
        /// The decision depends on selected AOMCacheStrategy and given _method
        /// </summary>
        /// <param name="_method">The HttpMethod</param>
        /// <returns>true if we will use offline store else false</returns>
        public bool SendOffline(HttpMethod _method)
        {
            return _cacheStrategy.Equals(AOMCacheStrategy.NETWORK_ONLY) == false &&
                _offlineHandler != null && _offlineHandler.IsConnected() == false;
        }

        /// <summary>
        /// Method for posting the class instance to the server.
        /// Use without href if the class instance hasn't been posted before. Use with href to use post in an update manner.
        /// </summary>
        /// <param name="dataModel">The class instance which will be saved on server</param>
        /// <param name="href">HREF of the class instance to post (or the address to post the class instance to)</param>
        /// <returns>The HREF of the posted class instance</returns>
        /// <exception cref="ApiomatRequestException">When the request fails</exception>
        public async Task<string> PostOnServerAsync(AbstractClientDataModel dataModel, string href, bool isRef, bool usePersistentStorage)
        {
            byte[] rawData = System.Text.Encoding.UTF8.GetBytes(dataModel.ToJson());
            IDataContainer datacontainer = new ByteContainer(rawData);
            href = href == null ? CreateModelHref(dataModel.ModuleName, dataModel.SimpleName) : href;
            bool isCollection = false;
            bool cacheThenNetworkFirstCall = false;
            AOMClientResponse<string> response = await PostOnServerAsync(datacontainer, href, isCollection, isRef, cacheThenNetworkFirstCall, usePersistentStorage).ConfigureAwait(false);
            return response.ResponseObject;
        }

        public async Task<string> PostOnServerAsync(AbstractClientDataModel dataModel, bool isRef, bool usePersistentStorage)
        {
            return await PostOnServerAsync(dataModel, CreateModelHref(dataModel.ModuleName, dataModel.SimpleName), isRef, usePersistentStorage).ConfigureAwait(false);
        }

		/// <summary>
		/// Method to post static data to the server. Do not forget to store the returned HREF to the owner model, since this method only stores the byte array on the server.
		/// </summary>
		/// <param name="rawData">The raw data as byte array</param>
		/// <param name="isImage"><c>true</c> to store the raw data as image, <c>false</c> to store as file</param>
		/// <returns>The HREF of the posted data</returns>
		/// <exception cref="ApiomatRequestException">When the request fails</exception>
		public async Task<string> PostStaticDataOnServerAsync(byte[] rawData, bool isImage, bool isRef, bool usePersistentStorage)
		{
			return await PostStaticDataOnServerAsync (new ByteContainer (rawData), isImage, isRef, usePersistentStorage).ConfigureAwait (false);
		}

		/// <summary>
		/// Method to post static data to the server. Do not forget to store the returned HREF to the owner model, since this method only stores the byte array on the server.
		/// </summary>
		/// <param name="rawData">The raw data as stream</param>
		/// <param name="isImage"><c>true</c> to store the raw data as image, <c>false</c> to store as file</param>
		/// <returns>The HREF of the posted data</returns>
		/// <exception cref="ApiomatRequestException">When the request fails</exception>
		public async Task<string> PostStaticDataOnServerAsync(Stream rawData, bool isImage, bool isRef, bool usePersistentStorage)
		{
			return await PostStaticDataOnServerAsync (new StreamContainer (rawData), isImage, isRef, usePersistentStorage).ConfigureAwait (false);
		}

        internal async Task<string> PostStaticDataOnServerAsync(IDataContainer rawData, bool isImage, bool isRef, bool usePersistentStorage)
        {
            string dataModelHref = CreateStaticDataHref(isImage);
            if (string.IsNullOrEmpty(dataModelHref))
            {
                throw new ApiomatRequestException(Status.HREF_NOT_FOUND);
            }
            if (Datastore.Instance.SendOffline(HttpMethod.Post))
            {
				return await Datastore.Instance.OfflineHandler.AddTask (HttpMethod.Post, dataModelHref, rawData, isImage, isRef, usePersistentStorage).ConfigureAwait (false);
            }

            bool withReferencedHrefs = true;
            bool asOctetStream = true;
            bool isCollection = false;
            bool cacheThenNetworkFirstCall = false;

            AOMClientResponse<string> response = await SendRequestAsync(dataModelHref, "", withReferencedHrefs, HttpMethod.Post, rawData, new List<HttpStatusCode>() { HttpStatusCode.Created }, asOctetStream, isCollection, isRef, cacheThenNetworkFirstCall, usePersistentStorage).ConfigureAwait(false);
            return response.ResponseObject;

        }
        /// <summary>
        /// Loads an existing class instance from the server
        /// </summary>
        /// <param name="dataModelHref">HREF address of the class instance</param>
        /// <param name="withReferencedHrefs">Set to <c>true</c> to get also all HREFs of referenced class instances</param>
        /// <typeparam name="T">The type of the class instance</typeparam>
        /// <returns>The loaded class instance</returns>
        /// <exception cref="ApiomatRequestException">When the request fails</exception>
        public async Task<T> LoadFromServerAsync<T>(string dataModelHref, bool withReferencedHrefs, bool usePersistentStorage) where T : AbstractClientDataModel
        {
            return await LoadFromServerAsync<T>(dataModelHref, withReferencedHrefs, false, usePersistentStorage).ConfigureAwait(false);
        }

        /// <summary>
        /// Loads an existing class instance from the server
        /// </summary>
        /// <param name="dataModelHref">HREF address of the class instance</param>
        /// <param name="withReferencedHrefs">Set to <c>true</c> to get also all HREFs of referenced class instances</param>
        /// <param name="isRef">indicates whether the object is a reference or not</param>
        /// <param name="usePersistentStorage">If set to <c>true</c> use persistent storage.</param>
        /// <typeparam name="T">The type of the class instance</typeparam>
        /// <returns>The loaded class instance</returns>
        /// <exception cref="ApiomatRequestException">When the request fails</exception>
        public async Task<T> LoadFromServerAsync<T>(string dataModelHref, bool withReferencedHrefs, bool isRef, bool usePersistentStorage) where T : AbstractClientDataModel
        {
            T element;
            try
            {
                element = (T)Activator.CreateInstance(typeof(T));
            }
            catch (Exception e)
            {
                throw new ApiomatRequestException(Status.INSTANTIATE_EXCEPTION, e);
            }
            bool cacheThenNetworkFirstCall = false;
            return await LoadFromServerAsync<T>(element, dataModelHref, withReferencedHrefs, isRef, cacheThenNetworkFirstCall, usePersistentStorage).ConfigureAwait(false);
        }

        /// <summary>
        /// Loads an existing class instance from the server
        /// </summary>
        /// <typeparam name="T">The type of the class instance</typeparam>
        /// <param name="dataModel"></param>
        /// <param name="dataModelHref">HREF address of the class instance</param>
        /// <param name="withReferencedHrefs">(Optional) Set to <c>true</c> to get also all HREFs of referenced class instances</param>
        /// <returns>The loaded class instance</returns>
        public async Task<T> LoadFromServerAsync<T>(T dataModel, string dataModelHref, bool withReferencedHrefs = false) where T : AbstractClientDataModel
        {
            bool isRef = false;
            bool cacheThenNetworkFirstCall = false;
            return await LoadFromServerAsync<T>(dataModel, dataModelHref, withReferencedHrefs, isRef, cacheThenNetworkFirstCall, Datastore.Instance.GetOfflineUsageForType(dataModel.GetType())).ConfigureAwait(false);
        }

        /// <summary>
        /// Loads an existing class instance from the server. The new values from server are written directly to the dataModel parameter.
        /// </summary>
        /// <param name="dataModel">The class instance</param>
        /// <param name="dataModelHref">HREF address of the class instance</param>
        /// <param name="withReferencedHrefs">set to <c>true</c> to also get all HREFs of referenced class instances</param>
        /// <param name="cacheThenNetworkFirstCall">When the caching strategy is set to CACHE_THEN_NETWORK, this flag indicates whether it's the first call or not. For other caching strategies this should always be false.</param>
        /// <param name="isRef">indicates whether the object is a reference or not</param>
        /// <param name="usePersistentStorage">If set to <c>true</c> use persistent storage.</param>
        /// <typeparam name="T">The type of the class instance</typeparam>
        /// <returns>The loaded class instance</returns>
        /// <exception cref="ApiomatRequestException">When the request fails</exception>
        public async Task<T> LoadFromServerAsync<T>(T dataModel, string dataModelHref, bool withReferencedHrefs, bool isRef, bool cacheThenNetworkFirstCall, bool usePersistentStorage) where T : AbstractClientDataModel
        {
            IList<HttpStatusCode> expectedCodes = new List<HttpStatusCode>() { HttpStatusCode.OK, HttpStatusCode.NotModified };
            if (isRef)
            {
                expectedCodes.Add(HttpStatusCode.NoContent);
            }
            bool asOctetStream = false;
            bool isCollection = false;
            AOMClientResponse<string> returnedResponse = await SendRequestAsync(CreateHref(dataModelHref), null, withReferencedHrefs, HttpMethod.Get, null, expectedCodes, asOctetStream, isCollection, isRef, cacheThenNetworkFirstCall, usePersistentStorage).ConfigureAwait(false);
            int responseStatusCode = returnedResponse.ReturnedStatusCode;
            if (string.IsNullOrWhiteSpace(returnedResponse.ResponseObject))
            {
                return null;
            }

            dataModel.FromJson(returnedResponse.ResponseObject);
            return dataModel;
        }

        /// <summary>
        /// Loads existing class instances from the server
        /// </summary>
        /// <param name="moduleName">Name of the module where the class instances are used in</param>
        /// <param name="simpleName">The simple class name of the class instances</param>
        /// <param name="withReferencedHrefs">Set to <c>true</c> to also get all HREFs of referenced class instances</param>
        /// <param name="query">Optional query string to filter the results</param>
        /// <param name="isRef">indicates whether the object is a reference or not</param>
        /// <param name="usePersistentStorage">If set to <c>true</c> use persistent storage.</param>
        /// <typeparam name="T">The type of the class instances</typeparam>
        /// <returns>A list of the loaded class instances that were selected by the query</returns>
        /// <exception cref="ApiomatRequestException">When the request fails</exception>
        public async Task<IList<T>> LoadFromServerAsync<T>(string moduleName, string simpleName, string query = null, bool withReferencedHrefs = false, bool isRef = false, bool usePersistentStorage = false) where T : AbstractClientDataModel
        {
            bool cacheThenNetworkFirstCall = false;
            return await LoadFromServerAsync<T>(CreateModelHref(moduleName, simpleName), query, withReferencedHrefs, isRef, cacheThenNetworkFirstCall, usePersistentStorage).ConfigureAwait(false);
        }

        /// <summary>
        /// Loads existing class instances from the server
        /// </summary>
        /// <param name="dataModelHref">The HREF of the class instances</param>
        /// <param name="withReferencedHrefs">Optional: Set to <c>true</c> to also get all HREFs of referenced class instances</param>
        /// <param name="query">Optional query string to filter the results</param>
        /// <param name="cacheThenNetworkFirstCall">When the caching strategy is set to CACHE_THEN_NETWORK, this flag indicates whether it's the first call or not. For other caching strategies this should always be false.</param>
        /// <param name="isRef">indicates whether the object is a reference or not</param>
        /// <param name="usePersistentStorage">If set to <c>true</c> use persistent storage.</param>
        /// <typeparam name="T">The type of the class instances</typeparam>
        /// <returns>A list of the loaded class instances that were selected by the query</returns>
        /// <exception cref="ApiomatRequestException">When the request fails</exception>
        public async Task<IList<T>> LoadFromServerAsync<T>(string dataModelHref, string query, bool withReferencedHrefs,
            bool isRef, bool cacheThenNetworkFirstCall, bool usePersistentStorage) where T : AbstractClientDataModel
        {
            IList<HttpStatusCode> expectedCodes = new List<HttpStatusCode>() { HttpStatusCode.OK, HttpStatusCode.NotModified };
            bool asOctetStream = false;
            bool isCollection = true;
            AOMClientResponse<string> response = await SendRequestAsync(dataModelHref, query, withReferencedHrefs, HttpMethod.Get, null, expectedCodes, asOctetStream, isCollection, isRef, cacheThenNetworkFirstCall, usePersistentStorage).ConfigureAwait(false);

            if (response.ResponseObject == null)
            { //no response, return empty list
                return new List<T>();
            }
            try
            {
                return AbstractClientDataModel.FromJArray<T>(JArray.Parse(response.ResponseObject));
            }
            catch (Exception e)
            {
                throw new ApiomatRequestException(Status.INSTANTIATE_EXCEPTION, HttpStatusCode.OK, e);
            }
        }

        /// <summary>
        /// Loads the resource from server
        /// </summary>
        /// <param name="href">the href of the resource</param>
        /// <param name="cacheThenNetworkFirstCall">When the caching strategy is set to CACHE_THEN_NETWORK, this flag indicates whether it's the first call or not. For other caching strategies this should always be false.</param>
        /// <param name="isRef">indicates whether the object is a reference or not</param>
        /// <param name="usePersistentStorage">If set to <c>true</c> use persistent storage.</param>
        /// <returns>the loaded resource as byte array</returns>
        /// <exception cref="ApiomatRequestException">When the request fails</exception>

        public async Task<byte[]> LoadResourceAsync(string href, bool isRef, bool cacheThenNetworkFirstCall, bool usePersistentStorage)
        {
            if (string.IsNullOrWhiteSpace(href))
            {
                throw new ApiomatRequestException(Status.HREF_NOT_FOUND);
            }
            byte[] result = null;

            if (this.OfflineHandler == null) // no offline, just cache. Unknown if connected or not.
            {
                /* The following only applies to network_else_cache and it's only necessary for that one.
                 * If another strategy was set, there's a context (and offline handler). */
                // Send request, but take care of a timeout
                try
                {
                    result = await SendResourceRequestWithEtagAndSave(href, usePersistentStorage).ConfigureAwait(false);
                }
                catch (ApiomatRequestException e)
                {
                    throw e;
                }
                catch (Exception ex)
                {
                    result = ChooseStorageImpl(usePersistentStorage).GetStoredBinary(href);
                    wasLoadedFromStorage = true;
                    /* Throw an exception if no data was found in offline storage */
                    if (result == null)
                    {
                        throw new ApiomatRequestException(Status.ID_NOT_FOUND_OFFLINE, HttpStatusCode.OK);
                    }
                }
            }
            else
            // we have an offline handler
            {
                if (this.OfflineHandler.IsConnected() == false)
                {
                    /* If caching is deactivated and device is offline return immediately */
                    if (_cacheStrategy == AOMCacheStrategy.NETWORK_ONLY)
                    {
                        throw new ApiomatRequestException(Status.NO_NETWORK, HttpStatusCode.OK);
                    }
                    /* Else: Any strategy that includes caching. Because the device is offline, get from cache now. */
                    Debug.WriteLine("trying to get stored binary for href: " + href);
                    result = ChooseStorageImpl(usePersistentStorage).GetStoredBinary(href);
                    wasLoadedFromStorage = true;
                    /* Throw an exception if no data was found in offline storage */
                    if (result == null)
                    {
                        throw new ApiomatRequestException(Status.ID_NOT_FOUND_OFFLINE, HttpStatusCode.OK);
                    }
                    // Else don't do anything here, the result gets returned at the end
                }
                // connected
                else
                {
                    if ((_cacheStrategy.Equals(AOMCacheStrategy.CACHE_THEN_NETWORK) && cacheThenNetworkFirstCall) ||
                        _cacheStrategy.Equals(AOMCacheStrategy.CACHE_ELSE_NETWORK))
                    { /* Read from cache */
                        result = ChooseStorageImpl(usePersistentStorage).GetStoredBinary(href);
                        wasLoadedFromStorage = true;
                    }
                    // Send a request in every case except 
                    // 1) cache_else_network and a result was just retrieved; 
                    // 2) cache_then_network and it's the first call; 
                    // 3) cache_then_network second call but there's a result
                    /* Explanation of case 3:
                     * When a resource was found in cache, it doesn't make sense to load it from the network, because
                     * resources at URLs don't change.
                     * When a resource gets changed, the model receives a new URL. (in which case nothing gets found in the
                     * cache and a network request gets executed) 
                     */
                    // CEN and (
                    // 1. a result exists OR
                    // 2. its the first call OR
                    // 3. not first fall but result exists)

                    // CEN AND ( RESULT OR FIRSTCALL)
                    if ((_cacheStrategy.Equals(AOMCacheStrategy.CACHE_ELSE_NETWORK) && result != null) == false
                        &&
                        (_cacheStrategy.Equals(AOMCacheStrategy.CACHE_THEN_NETWORK) && cacheThenNetworkFirstCall) == false
                        &&
                        (_cacheStrategy.Equals(AOMCacheStrategy.CACHE_THEN_NETWORK) && cacheThenNetworkFirstCall == false && result != null) == false)
                    {
                        try
                        {
                            result = await SendResourceRequestWithEtagAndSave(href, usePersistentStorage).ConfigureAwait(false);
                        }
                        catch (ApiomatRequestException are)
                        {
                            throw are;
                        }
                        catch (Exception ex)
                        {
                            throw new ApiomatRequestException(Status.REQUEST_TIMEOUT, HttpStatusCode.OK, ex);
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Counts existing class instances from server
        /// </summary>
        /// <param name="countHref">HREF of count</param>
        /// <param name="query">A query filtering the results in SQL style (see http://www.apiomat.com/docs/concepts/query-language/ ) </param>
        /// <param name="isRef">indicates whether the object is a reference or not</param>
        /// <param name="cacheThenNetworkFirstCall">When the caching strategy is set to CACHE_THEN_NETWORK, this flag indicates whether it's the first call or not. For other caching strategies this should always be false.</param>
        /// <returns>The (filtered) count of objects of this class</returns>
        /// <exception cref="ApiomatRequestException">When the request fails</exception>
        public async Task<long> CountFromServerAsync(string countHref, string query, bool isRef, bool cacheThenNetworkFirstCall)
        {
            long result = 0;

            IList<HttpStatusCode> expectedCodes = new List<HttpStatusCode>();
            expectedCodes.Add(HttpStatusCode.OK);
            expectedCodes.Add(HttpStatusCode.NotModified);

            bool withReferencedHrefs = false;
            bool asOctetStream = false;
            bool isCollection = true;
            bool usePersistentStorage = false;
            AOMClientResponse<string> response = await SendRequestAsync(countHref, query, withReferencedHrefs, HttpMethod.Get, null, expectedCodes, asOctetStream, isCollection, isRef, cacheThenNetworkFirstCall, usePersistentStorage).ConfigureAwait(false);
            if (response.ResponseObject != null)
            {
                result = long.Parse(response.ResponseObject);
            }
            return result;
        }

        /// <summary>
        /// Updates the class instance on the server
        /// </summary>
        /// <param name="dataModel">The class instance</param>
        /// <param name="isRef">indicates whether the object is a reference or not</param>
        /// <param name="usePersistentStorage">If set to <c>true</c> use persistent storage.</param>
        /// <exception cref="ApiomatRequestException">When the request fails</exception>
        public async Task UpdateOnServerAsync(AbstractClientDataModel dataModel, bool isRef, bool usePersistentStorage)
        {
            string href = dataModel.Href;
            string data = dataModel.ToJson();
            await UpdateOnServerAsync(href, data, isRef, usePersistentStorage).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a PUT request with given json data to the server
        /// </summary>
        /// <param name="href">The HREF of the object that shall be updated</param>
        /// <param name="json">The JSON of the object that shall be updated</param>
        /// <param name="isRef">indicates whether the object is a reference or not</param>
        /// <param name="usePersistentStorage">If set to <c>true</c> use persistent storage.</param>
        /// <exception cref="ApiomatRequestException">When the request fails</exception>
        public async Task UpdateOnServerAsync(string href, string json, bool isRef, bool usePersistentStorage)
        {
            bool asOctetStream = false;
            bool isCollection = false;
            bool cacheThenNetworkFirstCall = false;
            IDataContainer datacontainer = new ByteContainer(Encoding.UTF8.GetBytes(json));
            /* AOM-3334: Also accept HttpStatusCode.NoContent, so that this method can be used with custom REST endpoints */
            await SendRequestAsync(href, null, false, HttpMethod.Put, datacontainer, new List<HttpStatusCode>() { HttpStatusCode.OK, HttpStatusCode.NoContent }, asOctetStream, isCollection, isRef, cacheThenNetworkFirstCall, usePersistentStorage).ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes the class instance from the server
        /// </summary>
        /// <returns>An awaitable Task</returns>
        /// <param name="dataModel">the class instance</param>
        /// <param name="isRef">indicates whether the object is a reference or not</param>
        /// <exception cref="ApiomatRequestException">When the request fails</exception>
        public async Task DeleteOnServerAsync(AbstractClientDataModel dataModel, bool isRef)
        {
            await DeleteOnServerAsync(dataModel.Href, isRef, GetOfflineUsageForType(dataModel.GetType())).ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes the class instance from the server based on its href
        /// </summary>
        /// <returns>An awaitable Task</returns>
        /// <param name="dataModel">the class instance</param>
        /// <param name="isRef">indicates whether the object is a reference or not</param>
        /// <param name="usePersistentStorage">If set to <c>true</c> use persistent storage.</param>
        /// <exception cref="ApiomatRequestException">When the request fails</exception>
        public async Task DeleteOnServerAsync(string href, bool isRef, bool usePersistentStorage)
        {
            bool withReferencedHrefs = false;
            bool asOctetStream = false;
            bool isCollection = false;
            bool cacheThenNetworkFirstCall = false;

            await SendRequestAsync(href, null, withReferencedHrefs, HttpMethod.Delete, null, new List<HttpStatusCode>() { HttpStatusCode.NoContent }, asOctetStream, isCollection, isRef, cacheThenNetworkFirstCall, usePersistentStorage).ConfigureAwait(false);
        }


        public IDictionary<string, string> ConvertApiTokenJsonToSdkDictionary(JObject tokenJson)
        {
            IDictionary<string, string> result = new Dictionary<string, string>();

            GetObjectFromTokenMapAndAdd(tokenJson, ref result, "access_token", "SessionToken");
            GetObjectFromTokenMapAndAdd(tokenJson, ref result, "refresh_token", "RefreshToken");
            GetObjectFromTokenMapAndAdd(tokenJson, ref result, "aom_module", "Module");
            GetObjectFromTokenMapAndAdd(tokenJson, ref result, "aom_model", "Model");
            GetObjectFromTokenMapAndAdd(tokenJson, ref result, "aom_extra", "Extra");
            JToken outToken;
            if (tokenJson.TryGetValue("expires_in", out outToken) && outToken.Type != JTokenType.Null)
            {
                long expiresInMs = (long)Convert.ToDouble(outToken.ToObject<string>()) * 1000;
                long nowMs = DateTimeHelper.GetCurrentTimeMillis();
                result.Add("ExpirationDate", Convert.ToString(nowMs + expiresInMs));
            }

            return result;
        }

        /// <summary>
        /// Sends a request to the ApiOmat facebook REST resource to either create or retrieve a user for a specified facebook token
        /// </summary>
        /// <param name="facebookToken">Facebook token of the user to create or retrieve</param>
        /// <returns>A JSON string containing a user object in the value for "user" and a token map in "tokenMap"</returns>
        public async Task<string> GetOrCreateUserAsync(string facebookToken)
        {
            string appName = BaseUrl.Substring(User.BaseUrl.LastIndexOf('/') + 1);
            string url = BaseUrl.Substring(0, User.BaseUrl.IndexOf("yambas/rest") + 11) + "/modules/facebook/spec/" + appName + "/aomuser" + "?facebookToken=" + facebookToken;
            AOMCacheStrategy origCacheStrategy = _cacheStrategy;
            AOMClientResponse<string> response = null;

            try
            {
                if (_offlineHandler != null)
                {
                    SetCachingStrategy(AOMCacheStrategy.NETWORK_ONLY);
                }
                /* Else: only if the user never set another strategy (and it's network_else_cache)
                 * In that case, check "wasLoadedFromStorage" and return null */
                response = await SendRequestAsync(HttpMethod.Get, url, null, false, null, false, new List<HttpStatusCode>() { HttpStatusCode.OK }, false, false, false, false).ConfigureAwait(false);
                if (wasLoadedFromStorage)
                {
                    response.ResponseObject = null;
                }
            }
            /* Always re-set the original strategy */
            finally
            {
                if (_offlineHandler != null)
                {
                    SetCachingStrategy(origCacheStrategy);
                }
                /* Else: In that case the strategy was never changed */
            }

            return response.ResponseObject;

        }

        /// <summary>
        /// Gets the caching strategy of this datastore
        /// </summary>
        /// <returns>the currently set AOMCacheStrategy</returns>
        public static AOMCacheStrategy getCachingStrategy()
        {
            return Datastore._cacheStrategy;
        }

        /// <summary>
        /// Sets the caching strategy for this datastore
        /// </summary>
        /// <param name="cacheStrategy">the caching strategxy to use, see AOMCacheStrategy</param>
        /// <returns> the datastore </returns>
        public static Datastore SetCachingStrategy(AOMCacheStrategy cacheStrategy, IAOMOfflineHandler offlineHandler = null, string dbPath = null)
        {
            if (_instance == null)
            {
                throw new InvalidOperationException(
                    "The datastore hasn't been configured yet - call Datastore.configure(...) before sending requests.");
            }
            Datastore._cacheStrategy = cacheStrategy;

            _useETag = (_cacheStrategy == AOMCacheStrategy.NETWORK_ELSE_CACHE);

            return InitOfflineHandler(offlineHandler, dbPath);
        }

        /// <summary>
        /// initializes the offline handler
        /// </summary>
        /// <param name="dbPath">the path where the database is stored 
        /// (if null, it should be stored to current directory of this project, 
        /// this seems to work for windows-apps, but iOS and android apps built 
        /// with xamarin will need a path)</param>
        /// <returns>The datastore instance</returns>
        public static Datastore InitOfflineHandler(string dbPath = null)
        {
            return InitOfflineHandler(null, dbPath);
        }

        /// <summary>
        /// Initializes the offline handler
        /// </summary>
        /// <param name="conn">the SQLiteConnection for the database.
        /// Use this method when creating iOS and Android apps with Xamarin.
        /// See our Xamarin documentation for more details.</param>
        /// <returns>The datastore instance</returns>
        public static Datastore InitOfflineHandler(SQLiteConnection conn = null)
        {
            return InitOfflineHandler(null, null, conn);
        }

        /// <summary>
        /// initializes the offline handler
        /// </summary>
        /// <param name="offlineHandler">the offline handler implementation to set</param>
        /// <returns>The datastore instance</returns>
        public static Datastore InitOfflineHandler(IAOMOfflineHandler offlineHandler = null)
        {
            return InitOfflineHandler(offlineHandler, null, null);
        }

        /// <summary>
        /// initializes the offline handler
        /// </summary>
        /// <param name="offlineHandler">the offline handler implementation to set</param>
        /// <param name="dbPath">the path where the database is stored 
        /// (if null, it should be stored to current directory of this project, 
        /// this seems to work for windows-apps, but iOS and android apps built 
        /// with xamarin will need a path)</param>
        /// <returns>The datastore instance</returns>
        private static Datastore InitOfflineHandler(IAOMOfflineHandler offlineHandler = null, string dbPath = null, SQLiteConnection conn = null)
        {
            if (_instance == null)
            {
                throw new InvalidOperationException(
                    "The datastore hasn't been configured yet - call Datastore.configure(...) before sending requests.");
            }
            if (offlineHandler != null)
            {
                _instance._offlineHandler = offlineHandler;
            }
            else if (_instance._offlineHandler == null)
            {  /* initialize handler */
                if (conn == null)
                {
                    /* Attention: The connectivity event in the AOMOfflineHandler gets triggered BEFORE the constructor returns*/
                    _instance._offlineHandler = new AOMOfflineHandler(dbPath);
                }
                else
                {
                    /* Attention: The connectivity event in the AOMOfflineHandler gets triggered BEFORE the constructor returns*/
                    _instance._offlineHandler = new AOMOfflineHandler(conn);
                }
                _instance._offlineHandler.Init();
            }
            return _instance;
        }

        public bool GetOfflineUsageForType(Type type)
        {
            return _offlineMapping.GetOrDefault(type, false);
        }

        public void SetOfflineUsageForType(Type type, bool usePersistentStorage)
        {
            _offlineMapping[type] = usePersistentStorage;
        }

        /// <summary>
        /// Deletes an object from offline storage (from both - cache and persistent storage)
        /// </summary>
        /// <param name="href">the href of the object</param>
        /// <param name="isRef">indicates whether the object is a reference or not</param>
        /// <returns>if an object was deleted</returns>
        public bool DeleteObjectFromStorage(string href, bool isRef)
        {
            if (string.IsNullOrWhiteSpace(href) || IsHref(href) == false)
            {
                throw new ArgumentException("Argument href is null, empty or not an actual href.");
            }

            bool result = false;
            IList<AbstractStorage> storages = GetStorages();
            for (int i = 0; i < storages.Count; i++) // can't use foreach here
            {
                AbstractStorage storage = storages[i];
                /* href can be for object, image/file or ref.
             * In case of ref, a mapping needs to be considered.
             * But not if it's an image or a file (isRef is true for another reason)
             */
                if (isRef == false)
                {// no ref -> no mapping
                    result = result || storage.RemoveObject(href);
                    continue;
                }

                // mapping
                int levels = 0;

                string refHref = String.Concat(href);
                string refHref2 = null;
                href = storage.GetStoredObject(href);
                if (string.IsNullOrWhiteSpace(href) == false && IsHref(href))
                {
                    levels++;
                    /* another mapping level ? */
                    refHref2 = String.Concat(href);
                    href = storage.GetStoredObject(refHref2);
                    if (string.IsNullOrWhiteSpace(href) == false && IsHref(href))
                    {
                        levels++;
                    }
                    else
                    {
                        /* too deep -> reset href */
                        href = refHref2;
                    }
                }
                else
                {
                    /* too deep -> reset href */
                    href = refHref;
                }
                /* Always delete the final object */
                result = result || storage.RemoveObject(href);
                /* Also delete mappings, depending on level.
             * No need to check refHref or refHref2 for emptyness or content
             * because they're derived from href which gets checked for every level (see above) */
                if (levels > 0)
                {
                    storage.RemoveObject(refHref);
                }
                if (levels > 1)
                {
                    storage.RemoveObject(refHref2);
                }
            }

            return result;
        }

        /// <summary>
        /// Deletes a collection from offline storage (from both - cache and persistent storage)
        /// </summary>
        /// <returns>the number of removed entries</returns>
        /// <param name="href">The href of the collection</param>
        public int DeleteCollectionFromStorage(string href)
        {
            int result = 0;
            IList<AbstractStorage> storages = GetStorages();
            /* In case one storage is MemoryElseOfflineStorage and the other is SQLiteStorage,
         * the same delete query gets executed twice.
         * But that's ok (second delete doesn't affect any rows) and shouldn't be changed
         * in case one of their implementations gets changed in the future */
            foreach (AbstractStorage storage in storages)
            {
                result += storage.RemoveCollection(href);
            }
            return result;
        }

		internal int RemoveAllFromStorage(string moduleName, string modelName)
		{
			int result = 0;
			IList<AbstractStorage> storages = GetStorages();
			/* In case one storage is MemoryElseOfflineStorage and the other is SQLiteStorage,
	         * the same delete query gets executed twice.
	         * But that's ok (second delete doesn't affect any rows) and shouldn't be changed
         	 * in case one of their implementations gets changed in the future */
			foreach (AbstractStorage storage in storages)
			{
				result += storage.RemoveAllObjects(CreateModelHref(moduleName, modelName));
			}
			return result;
		}

        /// <summary>
        /// Check if ApiOmat service is reachable 
        /// The request will timeout after x ms or if connection cannot be established in y ms (values can be changed)
        /// </summary>
        /// <returns>true if service is available otherwise false</returns>

        public async Task<bool> IsServiceAvailable()
        {
            if (this.OfflineHandler == null)
            {
                /* We have to check with trying to send a request */
                return await AOMHttpClientFactory.GetAomHttpClient().IsServiceAvailable().ConfigureAwait(false);
            }

            return this.OfflineHandler.IsConnected();
        }

        #endregion

        #region Internal methods

        /// <summary>
        /// Requests the session token, optionally with a refresh token
        /// </summary>
        /// <returns>A Dictionary that maps "SessionToken", "RefreshToken" and "ExpiresIn" to their values.</returns>
        /// <param name="refreshToken">Optional refresh token. If set, it will be used regardless of the current set AuthType (which gets set when initially configuring the Datastore).
        /// If not set it will only work then the AuthType is set to USERNAME_PASSWORD - otherwise an exception gets thrown</param>
        /// <exception cref="ApiomatRequestException">When the request fails or the Datastore isn't properly configured</exception>
        internal async Task<IDictionary<string, string>> RequestSessionTokenAsync(string refreshToken = null)
        {
            JObject originalTokenJson = null;
            if (string.IsNullOrEmpty(refreshToken))
            {
                if (UsedAuthType != AuthType.USERNAME_PASSWORD)
                {
                    throw new ApiomatRequestException(Status.BAD_DATASTORE_CONFIG);
                }
                originalTokenJson = await AOMHttpClientFactory.GetAomHttpClient().SendActualTokenRequestAsync().ConfigureAwait(false);
                return ConvertApiTokenJsonToSdkDictionary(originalTokenJson);
            }
            originalTokenJson = await AOMHttpClientFactory.GetAomHttpClient().SendActualTokenRequestAsync(refreshToken).ConfigureAwait(false);
            return ConvertApiTokenJsonToSdkDictionary(originalTokenJson);
        }

        #endregion

        #region Private methods

        private string CreateHref(string href)
        {
            if (href.StartsWith("http"))
            {
                return href;
            }
            else if (href.StartsWith("/apps"))
            {
                return BaseUrl.Substring(0, BaseUrl.IndexOf("/apps"))
                    + href;
            }
            else
            {
                return BaseUrl + "/" + href;
            }
        }

        /// <exception cref="ApiomatRequestException">When the request fails</exception>
        private async Task<AOMClientResponse<string>> PostOnServerAsync(IDataContainer requestBody, string dataModelHref, bool isCollection, bool isRef, bool cacheThenNetworkFirstCall, bool usePersistentStorage)
        {
            if (string.IsNullOrEmpty(dataModelHref))
            {
                throw new ApiomatRequestException(Status.HREF_NOT_FOUND);
            }
            bool withReferencedHrefs = false;
            bool asOctetStream = false;
            /* In addition to 201 CREATED also allow 200 OK, which is used in the response for requesting a password reset */
            return await SendRequestAsync(dataModelHref, null, withReferencedHrefs, HttpMethod.Post, requestBody, new List<HttpStatusCode>() { HttpStatusCode.Created, HttpStatusCode.OK }, asOctetStream, isCollection, isRef, cacheThenNetworkFirstCall, usePersistentStorage).ConfigureAwait(false);
        }

        /// <exception cref="ApiomatRequestException">When the request fails</exception>
        private async Task<AOMClientResponse<string>> SendRequestAsync(string href, string query, bool withReferencedHrefs, HttpMethod requestType, IDataContainer requestBody, IList<HttpStatusCode> expectedCodes, bool asOctetStream, bool isCollection, bool isRef, bool cacheThenNetworkFirstCall, bool usePersistentStorage)
        {
            return await SendRequestAsync(requestType, href, query, withReferencedHrefs, requestBody, asOctetStream, expectedCodes, isCollection, isRef, cacheThenNetworkFirstCall, usePersistentStorage).ConfigureAwait(false);
        }


        /// <summary>
        /// Sends the specified request
        /// </summary>
        /// <param name="method">The HTTP-Method (GET, POST, ...)</param>
        /// <param name="href">The url to request</param>
        /// <param name="query">Query params</param>
        /// <param name="withReferencedHrefs">Set to <c>true</c> to get also all HREFs of referenced class instances</param>
        /// <param name="postEntity">the entity of the request as bytes for POST or PUT</param>
        /// <param name="asOctetStream">indicates whether it will be sent as application/octet-stream or not</param>
        /// <param name="expectedCodes">the expected HttpStatusCodes</param>
        /// <param name="isCollection">indicates whether the object is a collection or not</param>
        /// <param name="isRef">indicates whether the object is a reference or not</param>
        /// <param name="cacheThenNetworkFirstCall">When the caching strategy is set to CACHE_THEN_NETWORK, this flag indicates whether it's the first call or not. For other caching strategies this should always be false.</param>
        /// <param name="usePersistentStorage">If set to <c>true</c> use persistent storage.</param>
        /// <returns></returns>
        /// <exception cref="ApiomatRequestException">When the request fails</exception>
        private async Task<AOMClientResponse<string>> SendRequestAsync(HttpMethod method, string href, string query, bool withReferencedHrefs, IDataContainer postEntity, bool asOctetStream,
            IList<HttpStatusCode> expectedCodes, bool isCollection, bool isRef, bool cacheThenNetworkFirstCall, bool usePersistentStorage)
        {
            if (string.IsNullOrEmpty(href))
            {
                throw new ApiomatRequestException(Status.HREF_NOT_FOUND);
            }

            AOMClientResponse<string> clientResponse = new AOMClientResponse<string>();
            try
            {
                href = CreateHref(href);
                Uri url = new Uri(href);
                if (HttpMethod.Get.Equals(method))
                {
                    // GET
                    url = url.AddParameterToUri("withReferencedHrefs", withReferencedHrefs.ToString().ToLower());
                    url = url.AddParameterToUri("q", query == null ? "" : query);
                    
                    clientResponse = await SendGetRequestAsync(url, null, asOctetStream, expectedCodes, isCollection, isRef, cacheThenNetworkFirstCall, usePersistentStorage).ConfigureAwait(false);
                }
                else if (HttpMethod.Delete.Equals(method))
                {
                    //DELETE
                    if (isCollection)
                    { // Should actually never be the case / not possible
                        throw new Exception("An SDK internal error occured: Collections can't be deleted");
                    }
                    clientResponse = await AOMHttpClientFactory.GetAomHttpClient().SendActualRequestAsync(method, url, postEntity,
                        asOctetStream, expectedCodes, null, null).ConfigureAwait(false);
                    /* delete from storage */
                    DeleteObjectFromStorage(url.ToString(), isRef);
                }
                else if (HttpMethod.Put.Equals(method))
                {
                    //PUT
                    /* When updating an object, the offline object needs to be updated as well for this and maybe other
                     * cases:
                     * When using cache_else_network, loading an obj (gets stored in cache), updating it and then loading
                     * again -> loads from cache, so it's important that the update has been applied to the cached object */
                    if (_cacheStrategy != AOMCacheStrategy.NETWORK_ONLY)
                    {
                        string dataString;
						/* Cast to int and access of Size is OK, because in the non-octetStream case, it's not static data but just some JSON, so the length won't exceed Int32.MaxValue */
						dataString = System.Text.Encoding.UTF8.GetString(await postEntity.ToByteArrayAsync().ConfigureAwait(false), 0, (int)postEntity.Size);
                        /* modify lastmodified */
                        try
                        {
                            JObject dataJson = JObject.Parse(dataString);
                            dataJson["lastModifiedAt"] = DateTimeHelper.GetCurrentTimeMillis();
                            dataString = dataJson.ToString(Formatting.None);
                        }
                        catch (JsonException ex)
                        {
                            Debug.WriteLine("During updating the offline stored object the lastmodified attribute" +
                                "couldn't be updated due to a JSONException." +
                                "The update will continue nevertheless.", ex);
                        }
                        /* no need to differentiate between isRef or not at this point
                            * - so actualHrefs can be null and actualHref = url */
                        string putUrl = url.ToString();
                        bool isCollection2 = false;
                        StoreObjectOrCollection(method, putUrl, dataString, null, putUrl, isCollection2, isRef,
                            usePersistentStorage);
                    }
                    clientResponse = await AOMHttpClientFactory.GetAomHttpClient().SendActualRequestAsync(method, url, postEntity,
                        asOctetStream, expectedCodes, null, null).ConfigureAwait(false);
                    wasLoadedFromStorage = false;
                }
                else
                {
                    // POST
                    clientResponse = await AOMHttpClientFactory.GetAomHttpClient().SendActualRequestAsync(method, url, postEntity,
                        asOctetStream, expectedCodes, null, null).ConfigureAwait(false);
                    wasLoadedFromStorage = false;
                }
            }
            catch (ApiomatRequestException e)
            {
                throw e;
            }
            catch (ArgumentException e)
            {
                //MalformedURI
                throw new ApiomatRequestException(Status.WRONG_URI_SYNTAX, expectedCodes[0], e);
            }
            catch (TaskCanceledException e)
            {
                //Timeout
                Debug.WriteLine("Connection timeout" + e.ToString());
                throw new ApiomatRequestException(Status.REQUEST_TIMEOUT, expectedCodes[0], e);
            }
            catch (HttpRequestException e)
            {
                //other request failures (sth. like socketException)
                Debug.WriteLine("HttpRequest failed" + e.ToString());
                throw new ApiomatRequestException(Status.REQUEST_TIMEOUT, expectedCodes[0], e);
            }
            catch (AggregateException e)
            {
                if (e.InnerException != null && e.InnerException.GetType().GetTypeInfo().IsAssignableFrom(typeof(TaskCanceledException).GetTypeInfo()))
                {
                    Debug.WriteLine("Connection timeout" + e.ToString());
                    throw new ApiomatRequestException(Status.REQUEST_TIMEOUT, expectedCodes[0], e);
                }
            }
            catch (Exception ex)
            {
                //any other exception
                throw new ApiomatRequestException(Status.IO_EXCEPTION, HttpStatusCode.OK, ex);
            }

            return clientResponse;
        }

        private async Task<AOMClientResponse<string>> SendGetRequestAsync(Uri url, byte[] postEntity, bool asOctetStream,
            IList<HttpStatusCode> expectedCodes, bool isCollection, bool isRef, bool cacheThenNetworkFirstCall, bool usePersistentStorage)
        {
            AOMClientResponse<string> clientResponse = new AOMClientResponse<string>();
            string urlString = url.ToString();
            /* Offline handling */
            if (this.OfflineHandler != null)
            {
                // we have an offline handler
                return await HandleGetWithOfflineHandler(url, postEntity, asOctetStream, expectedCodes, isCollection, isRef, cacheThenNetworkFirstCall, usePersistentStorage).ConfigureAwait(false);
            }

            // no offline, just cache. Unknown if connected or not.
            /* The following only applies to network_else_cache and it's only necessary for that one.
              * If another strategy was set, there's a context (and offline handler). */
            // Send request, but take care of a timeout
            bool usePersistentStorage2 = false;
            try
            {
                clientResponse = await SendWithETagAndSave(HttpMethod.Get, url, postEntity, asOctetStream, expectedCodes, isCollection, isRef, usePersistentStorage2).ConfigureAwait(false);
            }
            catch (ApiomatRequestException ex)
            {
                if (ex.ReturnCode.Equals(Status.NO_NETWORK.StatusCode) == false)
                {
                    throw ex;
                }
                // No-Network-Exception occurred

                clientResponse.ResponseObject = ReadFromStorage(urlString, isCollection, isRef, usePersistentStorage2);
                /* Throw an exception if no data was found in offline storage */
                if (clientResponse.ResponseObject == null)
                {
                    throw new ApiomatRequestException(Status.ID_NOT_FOUND_OFFLINE, HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                if (ex.GetType().GetTypeInfo().IsAssignableFrom(typeof(ArgumentException).GetTypeInfo())
                    || ex.GetType().GetTypeInfo().IsAssignableFrom(typeof(TaskCanceledException).GetTypeInfo())
                    || ex.GetType().GetTypeInfo().IsAssignableFrom(typeof(HttpRequestException).GetTypeInfo()))
                {
                    clientResponse.ResponseObject = ReadFromStorage(urlString, isCollection, isRef, usePersistentStorage2);

                    /* Throw an exception if no data was found in offline storage */
                    if (clientResponse.ResponseObject == null)
                    {
                        throw new ApiomatRequestException(Status.ID_NOT_FOUND_OFFLINE, HttpStatusCode.OK, ex);
                    }
                }
                else if (ex.GetType().GetTypeInfo().IsAssignableFrom(typeof(AggregateException).GetTypeInfo()))
                {
                    if (ex.InnerException == null || ex.InnerException.GetType().GetTypeInfo().IsAssignableFrom(typeof(TaskCanceledException).GetTypeInfo()) == false)
                    {
                        throw ex;
                    }
                    clientResponse.ResponseObject = ReadFromStorage(urlString, isCollection, isRef, usePersistentStorage2);

                    /* Throw an exception if no data was found in offline storage */
                    if (clientResponse.ResponseObject == null)
                    {
                        throw new ApiomatRequestException(Status.ID_NOT_FOUND_OFFLINE, HttpStatusCode.OK, ex);
                    }
                }
                else
                {
                    throw ex;
                }
            }
            return clientResponse;
        }

        private async Task<AOMClientResponse<string>> HandleGetWithOfflineHandler(Uri url, byte[] postEntity, bool asOctetStream,
            IList<HttpStatusCode> expectedCodes, bool isCollection, bool isRef, bool cacheThenNetworkFirstCall, bool usePersistentStorage)
        {
            AOMClientResponse<string> clientResponse = new AOMClientResponse<string>();
            if (this.OfflineHandler.IsConnected() == false)
            { // not connected
                /* If caching is deactivated and device is offline return immediately */
                if (_cacheStrategy == AOMCacheStrategy.NETWORK_ONLY)
                {
                    throw new ApiomatRequestException(Status.NO_NETWORK, expectedCodes[0]);
                }
                /* Else: Any strategy that includes caching. Because the device is offline, get from cache now. */
                clientResponse.ResponseObject = ReadFromStorage(url.ToString(), isCollection, isRef, usePersistentStorage);
                /* Throw an exception if no data was found in offline storage */
                if (clientResponse.ResponseObject == null)
                {
                    throw new ApiomatRequestException(Status.ID_NOT_FOUND_OFFLINE, expectedCodes[0]);
                }
            }
            else
            {// connected
                if ((_cacheStrategy.Equals(AOMCacheStrategy.CACHE_THEN_NETWORK) && cacheThenNetworkFirstCall) || _cacheStrategy.Equals(AOMCacheStrategy.CACHE_ELSE_NETWORK))
                { /* Read from cache */
                    clientResponse.ResponseObject = ReadFromStorage(url.ToString(), isCollection, isRef, usePersistentStorage);
                }
                // send a request in every case except:
                // 1) cache_else_network and a result was just retrieved;
                // 2) cache_then_network and it's the first call
                if ((_cacheStrategy.Equals(AOMCacheStrategy.CACHE_ELSE_NETWORK) && clientResponse.ResponseObject != null) == false
                    && (_cacheStrategy.Equals(AOMCacheStrategy.CACHE_THEN_NETWORK) && cacheThenNetworkFirstCall) == false)
                {
                    bool exceptionOccured = false;
                    Exception ex = null;
                    try
                    {
                        clientResponse = await SendWithETagAndSave(HttpMethod.Get, url, postEntity, asOctetStream, expectedCodes,
                            isCollection, isRef, usePersistentStorage).ConfigureAwait(false);
                    }
                    catch (TaskCanceledException e)
                    {
                        ex = e;
                        //Timeout
                        exceptionOccured = true;
                    }
                    catch (HttpRequestException e)
                    {
                        ex = e;
                        //other request failures (sth. like socketException)
                        exceptionOccured = true;
                    }catch(ApiomatRequestException are)
                    {
                        if (are.ReturnCode == Status.IO_EXCEPTION.StatusCode || are.ReturnCode == Status.REQUEST_TIMEOUT.StatusCode)
                        {
                            exceptionOccured = true;
                            ex = are;
                        } else
                        {
                            throw are;
                        }
                    }
                    catch (AggregateException e)
                    {
                        if (e.InnerException != null)
                        {
                            TypeInfo typeInf = e.InnerException.GetType ().GetTypeInfo ();
                            if (typeInf.IsAssignableFrom (typeof(TaskCanceledException).GetTypeInfo ()))
                            {
                                exceptionOccured = true;
                                ex = e;
                            } else if (typeInf.IsAssignableFrom (typeof(ApiomatRequestException).GetTypeInfo ()))
                            {
                                ApiomatRequestException are = e.InnerException as ApiomatRequestException;
                                if (are.ReturnCode == Status.IO_EXCEPTION.StatusCode || are.ReturnCode == Status.REQUEST_TIMEOUT.StatusCode)
                                {
                                    exceptionOccured = true;
                                    ex = e;
                                } else
                                {
                                    throw e;
                                }
                            }else
                            {
                                throw e;
                            }
                        }else
                        {
                            throw e;
                        }
                    }catch(Exception e)
                    {
                        throw e;
                    }
                    if (exceptionOccured && (_cacheStrategy.Equals(AOMCacheStrategy.NETWORK_ONLY) == false))
                    {
                        clientResponse.ResponseObject = ReadFromStorage (url.ToString(), isCollection, isRef, usePersistentStorage);
                        if (String.IsNullOrEmpty (clientResponse.ResponseObject))
                        {
                            throw new ApiomatRequestException (Status.ID_NOT_FOUND_OFFLINE);
                        }
                    }
                    else if(exceptionOccured)
                    {
                        throw ex;
                    }

                }
            }
            return clientResponse;
        }

        /* Currently used for GET only */
        private async Task<AOMClientResponse<string>> SendWithETagAndSave(HttpMethod method, Uri url, byte[] postEntity, bool asOctetStream, IList<HttpStatusCode> expectedCodes, bool isCollection, bool isRef, bool usePersistentStorage)
        {
            AOMClientResponse<string> clientResponse = null;
            string urlString = url.ToString();
            AbstractStorage storage = ChooseStorageImpl(usePersistentStorage);

            // Load ETag
            string eTag = null;
            if (_useETag)
            {
                if (isCollection)
                {
                    eTag = storage.LoadCollectionETag(urlString);
                }
                else
                {
                    eTag = storage.LoadObjectLastModified(urlString);
                }
            }

            /* Delta-Sync */
            string deltaSync = null;
            string storageResultStr = null;
            if (isCollection && _cacheStrategy.Equals(AOMCacheStrategy.NETWORK_ONLY) == false)
            {
                if (DeltaSyncStrategy == DeltaSyncStrategy.OBJECT_BASED)
                {
                    storageResultStr = ReadFromStorage(urlString, isCollection, isRef, usePersistentStorage);
                    if (string.IsNullOrWhiteSpace(storageResultStr) == false)
                    {
                        try
                        {
                            JArray ja = JArray.Parse(storageResultStr);
                            /* Only send deltaSync header if we have cached objects. */
                            if (ja.Count > 0)
                            {
                                IDictionary<string, string> hrefLMMap = JsonHelper.GetHrefLMDictionaryFromJsonArrayWithAomModels(ja);
                                JObject hrefLMJsonObject = new JObject();
                                foreach (var kvp in hrefLMMap)
                                {
                                    hrefLMJsonObject[kvp.Key] = kvp.Value;
                                }
                                deltaSync = hrefLMJsonObject.ToString(Formatting.None);
                            }
                        }
                        catch (JsonException)
                        {// deltaSyncValue stays null, which is ok
                        }
                    }
                }
                else if (DeltaSyncStrategy == DeltaSyncStrategy.COLLECTION_BASED)
                {
                    long? collectionLastUpdate = LoadLastUpdateFromStorage(urlString, true, usePersistentStorage);
                    if (collectionLastUpdate != null)
                    {
                        deltaSync = collectionLastUpdate.ToString();
                    }
                }
            }

            // Actual request
            IDataContainer datacontainer = new ByteContainer(postEntity);
            clientResponse = await AOMHttpClientFactory.GetAomHttpClient().SendActualRequestAsync(method, url, datacontainer, asOctetStream,
                expectedCodes, eTag, deltaSync).ConfigureAwait(false);
            wasLoadedFromStorage = false;

            /* In the case of 304 (currently only possible with strategy network_else_cache),
         * when the result is null or empty: get the data from cache */
            if (clientResponse != null && string.IsNullOrWhiteSpace(clientResponse.ResponseObject) && clientResponse.ReturnedStatusCode == (int)HttpStatusCode.NotModified)
            {
                clientResponse.ResponseObject = storageResultStr = (storageResultStr == null) ? ReadFromStorage(urlString, isCollection, isRef, usePersistentStorage) : storageResultStr;
                wasLoadedFromStorage = true;
                /* If no offline data could be found, the network request needs to be repeated, but this time without
                 * Etag/Not-Modified header to get all data */
                if (clientResponse.ResponseObject == null)
                {
                    /* don't send the etag header in the request, but we need the etag from the response */
                    eTag = "";
                    clientResponse = await AOMHttpClientFactory.GetAomHttpClient().SendActualRequestAsync(method, url, datacontainer,
                        asOctetStream, expectedCodes, eTag, null).ConfigureAwait(false);
                    wasLoadedFromStorage = false;
                }
            }

            /* Note: The result string can be empty for 204 responses,
         * e.g. when a reference attribute gets fetched, but there's no reference attached. */

            /* For references the actual href needs to be extracted to save a mapping later */
            string actualHref = null;
            IList<string> actualHrefs = null;
            if (isRef && string.IsNullOrWhiteSpace(clientResponse.ResponseObject) == false)
            {
                try
                {
                    if (isCollection)
                    {
                        actualHrefs = JsonHelper.GetHrefListFromJsonArrayWithAomModels(clientResponse.ResponseObject);
                    }
                    else
                    {
                        JObject resultJO = JObject.Parse(clientResponse.ResponseObject);
                        actualHref = null;
                        JToken aHrefToken;
                        if (resultJO.TryGetValue("href", out aHrefToken))
                        {
                            actualHref = aHrefToken.ToString();
                        }
                    }
                }
                catch (JsonException e)
                {
                    throw new Exception("JSON could not be parsed", e);
                }
            }

            // Save ETag
            if (_useETag && clientResponse != null && clientResponse.ReturnedStatusCode == (int)HttpStatusCode.OK &&
                string.IsNullOrWhiteSpace(clientResponse.ETag) == false && wasLoadedFromStorage == false)
            {
                if (isCollection)
                {
                    storage.StoreCollectionETag(urlString, clientResponse.ETag);
                    /* Also store the lastModifieds of the objects within the collection,
                 * so that a load of an object that was in a requested collection before doesn't have to be reloaded.
                 * In the actual process of storing the collection the single objects get saved too.
                 * So this works well together. */
                    try
                    {
                        JArray collection = JArray.Parse(clientResponse.ResponseObject);
                        JToken hrefToken = null;
                        JToken lastModToken = null;
                        JObject obj;
                        for (int i = 0; i < collection.Count; i++) // no foreach for JArray
                        {
                            obj = (JObject)collection[i];
                            hrefToken = null;
                            lastModToken = null;
                            if (obj.TryGetValue("href", out hrefToken) && obj.TryGetValue("lastModifiedAt", out lastModToken))
                            {
                                storage.StoreObjectLastModified(hrefToken.ToString(), lastModToken.ToString());
                            }
                        }
                    }
                    catch (JsonException ex)
                    {
                        Debug.WriteLine("An error occurred during parsing the collection from the response to save the contained objects eTag: " + ex.ToString());
                    }
                }
                else
                {
                    storage.StoreObjectLastModified(urlString, clientResponse.ETag);
                }
            }

            /* In the case of DeltaSync, reconstruct the actual response. Only necessary if not 304.
             * Also only necessary if the deltaSync AtomicReference is not null, because that
             * means we never sent a DeltaSync header and thus the response can't contain DeltaSync info as well. */
            if (DeltaSyncStrategy != DeltaSyncStrategy.NONE && isCollection && _cacheStrategy.Equals(AOMCacheStrategy.NETWORK_ONLY) == false
                && clientResponse != null && clientResponse.ReturnedStatusCode != (int)HttpStatusCode.NotModified && clientResponse.DeltaSync != null)
            {
                ExceptionDispatchInfo capturedException = null;

                try
                {
                    /* Load from cache */
                    storageResultStr =
                        storageResultStr == null ? ReadFromStorage(urlString, isCollection, isRef, usePersistentStorage)
                        : storageResultStr;
                    IList<JObject> cachedJsonObjectList = null;

                    if (string.IsNullOrWhiteSpace(storageResultStr) == false)
                    {

                        JArray ja = JArray.Parse(storageResultStr);

                        if (ja.Count > 0)
                        {
                            cachedJsonObjectList = JsonHelper.GetJsonObjectListFromJsonArrayWithAomModels(ja);
                            /* Delete what's in the response delete header. */
                            JArray deltaDeleteIds = JArray.Parse(clientResponse.DeltaSync);
                            if (deltaDeleteIds.Count > 0)
                            {
                                JToken currentId = null;
                                IList<JObject> cachedJsonObjectListCopy = new List<JObject>(cachedJsonObjectList);
                                foreach (JObject jo in cachedJsonObjectListCopy)
                                {
                                    if (jo.TryGetValue("id", out currentId))
                                    {
                                        if (JsonHelper.StringJsonArrayContains(deltaDeleteIds, currentId.ToString()))
                                        {
                                            cachedJsonObjectList.Remove(jo);
                                        }
                                    }
                                }

                            }
                        }
                    }
                    /* Delete from cached objects what's in the response (updated) and
                     * add what's in the response additionally (created).
                     * Only necessary if we have cached objects! */
                    if (cachedJsonObjectList != null && cachedJsonObjectList.Count > 0
                        && string.IsNullOrWhiteSpace(clientResponse.ResponseObject) == false)
                    {
                        wasLoadedFromStorage = true;
                        IList<JObject> returnedJsonObjectList =
                            JsonHelper.GetJsonObjectListFromJsonArrayWithAomModels(JArray.Parse(clientResponse.ResponseObject));
                        foreach (JObject returnedObject in returnedJsonObjectList)
                        {
                            /* Delete from cached objects what's in the response (updated) */
                            JToken returnedObjectId = null;
                            JToken cachedObjectId = null;
                            if (returnedObject.TryGetValue("id", out returnedObjectId))
                            {
                                IList<JObject> cachedJsonObjectListCopy = new List<JObject>(cachedJsonObjectList);
                                foreach (JObject cachedObject in cachedJsonObjectListCopy)
                                {
                                    if (cachedObject.TryGetValue("id", out cachedObjectId) &&
                                        returnedObjectId.Equals(cachedObjectId))
                                    {
                                        cachedJsonObjectList.Remove(cachedObject);
                                    }
                                }
                            }
                            /* Add what's in the response additionally (created) */
                            cachedJsonObjectList.Add(returnedObject);
                        }
                        if (returnedJsonObjectList.Count > 0)
                        {
                            wasLoadedFromStorage = false; // It's actually a mix of both, but false must be set so the
                            // result gets stored in next step
                        }
                        clientResponse.ResponseObject = JsonHelper.ToStringFromJObjectList(cachedJsonObjectList);
                    }
                }
                catch (JsonException e)
                {
                    capturedException = ExceptionDispatchInfo.Capture(e);
                }

                if (capturedException != null)
                {
                    clientResponse = await AOMHttpClientFactory.GetAomHttpClient().SendActualRequestAsync(method, url, datacontainer, asOctetStream, expectedCodes, null, null).ConfigureAwait(false);
                    wasLoadedFromStorage = false;
                }
            }

            /* Store in cache or offline (chosen by chooseStorageImpl) and differentiate between object and collection
             * But only when strategy is not "NETWORK_ONLY" */
                            if (_cacheStrategy != AOMCacheStrategy.NETWORK_ONLY && wasLoadedFromStorage == false)
            {
                bool isCollection2 = clientResponse.ResponseObject.StartsWith("["); // can differ from isCollection
                StoreObjectOrCollection(method, urlString, clientResponse.ResponseObject, actualHrefs, actualHref, isCollection2, isRef,
                    usePersistentStorage);
            }

            return clientResponse;
        }

        private string ReadFromStorage(string hrefOrUrl, bool isCollection, bool isRef, bool usePersistentStorage)
        {/* Differentiate between cache vs. offline (via chooseStorageImpl) and object vs. collection */
            AbstractStorage storage = ChooseStorageImpl(usePersistentStorage);
            return ReadFromStorage(hrefOrUrl, isCollection, isRef, storage);
        }

        private string ReadFromStorage(string hrefOrUrl, bool isCollection, bool isRef, AbstractStorage storageImpl)
        {/* Differentiate between object or collection */
            wasLoadedFromStorage = true;
            if (isCollection)
            {
                return storageImpl.GetStoredCollection(hrefOrUrl);
            }

            string result = storageImpl.GetStoredObject(hrefOrUrl);
            if (isRef == false || string.IsNullOrEmpty(result))
            {
                return result;
            }

            // First load was only for mapping
            result = storageImpl.GetStoredObject(result);
            // second depends - when storage was during offline-post, then not
            if (string.IsNullOrWhiteSpace(result) == false && result.StartsWith("{") == false)
            {
                result = storageImpl.GetStoredObject(result);
            }
            return result;
        }

        /// <summary>
        /// Loads the last update time of the collection or object
        /// </summary>
        /// <param name="hrefOrUrl"></param>
        /// <param name="isCollection"></param>
        /// <param name="usePersistentStorage"></param>
        /// <returns>The last update time of the collection or object, or null if nothing was found in offline storage</returns>
        internal long? LoadLastUpdateFromStorage(string hrefOrUrl, bool isCollection, bool usePersistentStorage)
        {
            AbstractStorage storage = ChooseStorageImpl(usePersistentStorage);
            if (isCollection == false)
            {
                /* Don't use storage.LoadObjectLastModified(), because it loads the timestamp from the ETAG store,
                 * which currently is an in-memory cache, so it's not persistant,
                 * while the actual object might be persisted in the SQLite DB. */
                throw new NotImplementedException("Not implemented yet");
            }
            return storage.LoadCollectionLastUpdate(hrefOrUrl);
        }

        /// <exception cref="ApiomatRequestException">When the request fails</exception>
        private async Task<byte[]> SendResourceRequestAsync(string href, bool usePersistentStorage = false)
        {
            return await SendResourceRequestWithEtagAndSave(href, usePersistentStorage).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends the resource request with etag and save.
        /// </summary>
        /// <returns>The resource request with etag and save.</returns>
        /// <param name="href">Href.</param>
        /// <param name="usePersistentStorage">If set to <c>true</c> use persistent storage.</param>
        private async Task<byte[]> SendResourceRequestWithEtagAndSave(string href, bool usePersistentStorage)
        {
            if (string.IsNullOrEmpty(href))
            {
                throw new ApiomatRequestException(Status.HREF_NOT_FOUND);
            }
            AOMClientResponse<byte[]> result;

            /* load ETag */
            string eTag = null;
            if (_useETag)
            {
                eTag = ChooseStorageImpl(usePersistentStorage).LoadObjectLastModified(href);
            }

            /* send actual request */
            result = await AOMHttpClientFactory.GetAomHttpClient().SendActualResourceRequest(href, eTag).ConfigureAwait(false);
            wasLoadedFromStorage = false;

            /* If server responded with 304 not modified, get from storage. Boolean is used here - may be null */
            if (result != null && result.NotModified)
            {
                result.ResponseObject = ChooseStorageImpl(usePersistentStorage).GetStoredBinary(href);
                wasLoadedFromStorage = true;
                /* if nothing was found in storage, make actual request, without any etag. but we need the etag from the reponse */
                if (result.ResponseObject == null)
                {
                    result = await AOMHttpClientFactory.GetAomHttpClient().SendActualResourceRequest(href, "").ConfigureAwait(false);
                    wasLoadedFromStorage = false;
                }
            }
            else if (_cacheStrategy.Equals(AOMCacheStrategy.NETWORK_ONLY) == false &&
				result != null &&
				result.ResponseObject != null &&
				wasLoadedFromStorage == false &&
				result.ResponseObject.LongLength <= MaxOfflineFileBytes)
            {
                AbstractStorage storage = ChooseStorageImpl(usePersistentStorage);
                /* store ETag */
                if (_useETag && string.IsNullOrWhiteSpace(result.ETag) == false)
                {
                    storage.StoreObjectLastModified(href, result.ETag);
                }
                /* store data */
                storage.StoreBinary(href, result.ResponseObject, HttpMethod.Get);
            }
            return result.ResponseObject;
        }

        /// <summary>
        /// Chooses the storage impl.
        /// In case usePersistentStorage is false and the cache gets chosen, it will still look up the SQLite db if the
        /// object / collection isn't found in the cache.
        /// UNLESS no context is available. In that case, the pure in-memory cache one gets chosen.
        /// </summary>
        /// <returns>The storage impl.</returns>
        /// <param name="">.</param>
        private AbstractStorage ChooseStorageImpl(bool usePersistentStorage)
        {
            if (usePersistentStorage)
            {
                if (this.OfflineHandler == null)
                {
                    // Context must be set
                    throw new Exception(
                        "No offlinehandler is set. Please initialize the AOMOfflineHandler with InitOfflineHandler(dbPath) or InitOfflineHandler(conn) or set a cache strategy");
                }
                return SQLiteStorage.Instance;
            }

            if (this.OfflineHandler == null)
            {
                return InMemoryCache.Instance;
            }
            else
            {
                return MemoryElseOfflineStorage.Instance;
            }
        }

        private void StoreObjectOrCollection(HttpMethod method, string urlString, string resultStr, IList<string> actualHrefs,
            string actualHref, bool isCollection, bool isRef, bool usePersistentStorage)
        {
            AbstractStorage storage = ChooseStorageImpl(usePersistentStorage);
            if (isCollection)
            {
                storage.StoreCollection(urlString, resultStr);
                if (isRef == false || actualHrefs == null)
                {/*no refs to store*/
                    return;
                }

                // also save refhref mapping (necessary for delete)
                foreach (string currentActualHref in actualHrefs)
                {
                    string id =
                        currentActualHref.Substring(currentActualHref.LastIndexOf("/") + 1);
                    string mappingHref = AddIdToRefHref(urlString, id);
                    storage.StoreObject(mappingHref, currentActualHref, HttpMethod.Get);
                }
                return;
            }

            if (isRef == false || string.IsNullOrWhiteSpace(actualHref))
            {
                storage.StoreObject(urlString, resultStr, method);
                return;
            }
            // two mappings are needed, one for later loading, one for later deleting
            // Mapping 1, e.g. /session/123/conference -> /session/123/conference/abc
            string objectId = actualHref.Substring(actualHref.LastIndexOf("/") + 1);
            string mapping1Href =
                urlString.Contains("?") ? urlString.Substring(0, urlString.IndexOf("?")) : urlString;
            string intermediateHref = AddIdToRefHref(urlString, objectId);
            storage.StoreObject(mapping1Href, intermediateHref, method);
            // Mapping 2, e.g. /session/123/conference/abc -> /conference/abc
            storage.StoreObject(intermediateHref, actualHref, method);
            // Real href, e.g. /conference/abc -> body
            storage.StoreObject(actualHref, resultStr, method);
            return;
        }

        private string AddIdToRefHref(string refHref, string id)
        {
            // remove params and add id
            // Remove url parameters here, because otherwise, when deleting an object later, there are no params and the url
            // can't be found
            refHref = refHref.Contains("?") ? refHref.Substring(0, refHref.IndexOf("?")) : refHref;
            refHref = refHref + "/" + id;
            return refHref;
        }

        private IList<AbstractStorage> GetStorages()
        {
            IList<AbstractStorage> storages = new List<AbstractStorage>();
            storages.Add(ChooseStorageImpl(false)); // cache
            if (this.OfflineHandler != null)
            {
                storages.Add(ChooseStorageImpl(true)); // persistent storage
            }
            return storages;
        }

        /* The check for "h" is to make sure it's "http..." instead of "{" or "[". */
        private bool IsHref(string href)
        {
            return "h".Equals(href.Substring(0, 1));
        }

        /// <summary>
        /// Performs the actual request to the server to get a session token either with username + password or with a refresh token, depending on the provided parameter
        /// </summary>
        /// <returns>A Dictionary that maps the following keys to their values: "SessionToken", "RefreshToken", "ExpirationDate" (Unix UTC timestamp in ms), "Module" and "Model"</returns>
        /// <param name="content">The key value pairs to use as form data in the request</param>
        /// <exception cref="ApiomatRequestException">When the request fails</exception>
        private async Task<IDictionary<string, string>> RequestSessionTokenAsync(IEnumerable<KeyValuePair<string, string>> requestData)
        {
            IDictionary<string, string> result = null;
            int expectedCode = (int)HttpStatusCode.OK;

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                string oauth2Uri = BaseUrl.Substring(0, BaseUrl.IndexOf("yambas") + 6) + "/oauth/token";
                FormUrlEncodedContent content = new FormUrlEncodedContent(requestData);
                HttpResponseMessage response = await client.PostAsync(oauth2Uri, content).ConfigureAwait(false);
                int statusCode = (int)response.StatusCode;
                HttpContent responseBody = response.Content;
                if (response.IsSuccessStatusCode == false || responseBody == null)
                {
                    string errorStr = responseBody != null ? await responseBody.ReadAsStringAsync().ConfigureAwait(false) : "";
                    throw new ApiomatRequestException(statusCode, expectedCode, "An error occurred during sending the token request", errorStr);
                }

                string jsonString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                JObject tokenJson = JObject.Parse(jsonString);
                result = ConvertApiTokenJsonToSdkDictionary(tokenJson);
            }
            return result;
        }



        private static IDictionary<string, string> GetObjectFromTokenMapAndAdd(JObject tokenJson, ref IDictionary<string, string> result, string otmKey, string resKey)
        {
            JToken outToken;
            if (tokenJson.TryGetValue(otmKey, out outToken) && outToken.Type != JTokenType.Null)
            {
                result.Add(resKey, outToken.ToObject<string>());
            }
            return result;
        }

        #endregion
    }
}
