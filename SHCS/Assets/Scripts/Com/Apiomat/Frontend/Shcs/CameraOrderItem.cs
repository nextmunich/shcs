/*
 * Copyright (c) 2011 - 2017, Apinauten GmbH
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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Com.Apiomat.Frontend.Basics;
using Com.Apiomat.Frontend.Extensions;
using Com.Apiomat.Frontend.Helper;

					

namespace Com.Apiomat.Frontend.SHCS
{
    /// <summary>
    /// Generated class for CameraOrderItem 
    /// </summary>
    public class CameraOrderItem : AbstractClientDataModel
    {
        private Com.Apiomat.Frontend.SHCS.Camera camera = null;
        /// <summary>
        /// Default constructor. Needed for internal processing.
        /// </summary>
        public CameraOrderItem ( ) : base()
        {
        }
    

        /// <summary>
        /// Returns the simple name of this class 
        /// </summary>
        public override string SimpleName { get { return "CameraOrderItem"; } }

        /// <summary>
        /// Returns the name of the module where this class belongs to
        /// </summary>
        public override string ModuleName { get { return "SHCS"; } }

        /// <summary>
        /// Returns a list of objects of this class filtered by the given query from server.
        /// The size of the resultset is limited to an installation specific value ('maxResults') and defaults to 1000. 
        /// Use limit and offset to return all results if the expected size is larger.
        /// </summary>
        /// <param name="query">A query filtering the results in SQL style (see http://www.apiomat.com/docs/concepts/query-language/)</param>
        /// <returns>A list of objects of this class filtered by the given query from server</returns>
        /// <exception cref="Exception">When something fails</exception>
        public static async Task<IList<CameraOrderItem>> GetCameraOrderItemsAsync(string query = null) 
        {
            CameraOrderItem o = new  CameraOrderItem();
            return await Datastore.Instance.LoadFromServerAsync<CameraOrderItem>(o.ModuleName, o.SimpleName, query, false, false, Datastore.Instance.GetOfflineUsageForType(typeof(CameraOrderItem))).ConfigureAwait(false);
        }
        
        /// <summary>
        /// Returns a list of objects of this class filtered by the given query from server.
        /// The size of the resultset is limited to an installation specific value ('maxResults') and defaults to 1000. 
        /// Use limit and offset to return all results if the expected size is larger.
        /// </summary>
        /// <param name="query">A query filtering the results in SQL style (see http://www.apiomat.com/docs/concepts/query-language/)</param>
        /// <param name="withReferencedHrefs">Set <c>true</c> to also get all HREFs of referenced class instances</param>
        /// <returns>A list of objects of this class filtered by the given query from server</returns>
        /// <exception cref="Exception">When something fails</exception>
        public static async Task<IList<CameraOrderItem>> GetCameraOrderItemsAsync(string query, bool withReferencedHrefs) 
        {
            CameraOrderItem o = new  CameraOrderItem();
            return await Datastore.Instance.LoadFromServerAsync<CameraOrderItem>(o.ModuleName, o.SimpleName, query, withReferencedHrefs, false, Datastore.Instance.GetOfflineUsageForType(typeof(CameraOrderItem))).ConfigureAwait(false);
        }
    
        /// <summary>
        /// Returns a list of objects of this class filtered by the given query from server.
        /// The size of the resultset is limited to an installation specific value ('maxResults') and defaults to 1000. 
        /// Use limit and offset to return all results if the expected size is larger.
        /// </summary>
        /// <param name="query">A query filtering the results in SQL style (see http://www.apiomat.com/docs/concepts/query-language/)</param>
        /// <param name="withReferencedHrefs">Set <c>true</c> to also get all HREFs of referenced class instances</param>
        /// <param name="usePersistentStorage">Set <c>true</c> to store the object in persistent storage </param>
        /// <returns>A list of objects of this class filtered by the given query from server</returns>
        /// <exception cref="Exception">When something fails</exception>
        public static async Task<IList<CameraOrderItem>> GetCameraOrderItemsAsync(string query, bool withReferencedHrefs, bool usePersistentStorage) 
        {
            CameraOrderItem o = new  CameraOrderItem();
            return await Datastore.Instance.LoadFromServerAsync<CameraOrderItem>(o.ModuleName, o.SimpleName, query, withReferencedHrefs, false, usePersistentStorage).ConfigureAwait(false);
        }
        
        /// <summary>
        /// Returns the count of objects of this class filtered by the given query from the server.
        /// </summary>
        /// <param name="query">A query filtering the results in SQL style (see http://www.apiomat.com/docs/concepts/query-language/)</param>
        /// <returns>The (filtered) count of objects of this class</returns>
        /// <exception cref="ApiomatRequestException">When the request fails</exception>
        public static async Task<long> GetCameraOrderItemsCountAsync( string query = null )
        {
            CameraOrderItem o = new CameraOrderItem();
            string countHref = Datastore.Instance.CreateModelHref( o.ModuleName, o.SimpleName ) + "/count";
            return await Datastore.Instance.CountFromServerAsync(countHref, query, false, false).ConfigureAwait(false);
        }


    
        /// <summary>
        /// Getter for local variable containing the referenced object
        /// </summary>
        public Com.Apiomat.Frontend.SHCS.Camera Camera { get { return camera; } }

        /// <summary>
        /// Loads the referenced object, adds the result from the server to the local member variable of this object and also returns a Task containing the result
        /// </summary>
        /// <exception cref="ApiomatRequestException">When the request fails</exception>
        public async Task<Com.Apiomat.Frontend.SHCS.Camera> LoadCameraAsync( )
        {
            return await LoadCameraAsync<Com.Apiomat.Frontend.SHCS.Camera>(Datastore.Instance.GetOfflineUsageForType(typeof(Com.Apiomat.Frontend.SHCS.Camera))).ConfigureAwait(false);
        }
    
        public async Task< T > LoadCameraAsync<T>(  ) where T : Com.Apiomat.Frontend.SHCS.Camera
        {
            return await LoadCameraAsync<T>(Datastore.Instance.GetOfflineUsageForType(typeof(T))).ConfigureAwait(false);
        }
        /// <summary>
        /// Loads the referenced object, adds the result from the server to the local member variable of this object and also returns a Task containing the result.
        /// A precondition is that a referenced object was attached to this object before, so that when loading this object,
        /// there's a known URL of camera, or alternatively you attach a referenced object after loading this object.
        /// </summary>
        /// <exception cref="ApiomatRequestException">With status <see cref="Status.ATTACHED_HREF_MISSING"/>
        /// when no reference object was attached to the current object before. Other statuses when the request fails</exception>
        public async Task< T > LoadCameraAsync<T>(bool usePersistentStorage  ) where T : Com.Apiomat.Frontend.SHCS.Camera
        {
            string refUrl = Data["cameraHref"].ToObject<string>();
            if ( string.IsNullOrEmpty(refUrl) )
            {
                throw new ApiomatRequestException(Status.ATTACHED_HREF_MISSING);
            }
            T result = await Datastore.Instance.LoadFromServerAsync<T>(refUrl, false, true, usePersistentStorage).ConfigureAwait(false);
            camera = result;
            return result;
        }


        public async Task<string> PostCameraAsync(Com.Apiomat.Frontend.SHCS.Camera refData) 
        {
            return await PostCameraAsync(refData, Datastore.Instance.GetOfflineUsageForType(typeof(Com.Apiomat.Frontend.SHCS.Camera))).ConfigureAwait(false);
        }
        
        /// <exception cref="ApiomatRequestException">When the request fails</exception>
        public async Task<string> PostCameraAsync(Com.Apiomat.Frontend.SHCS.Camera refData, bool usePersistentStorage) {
            string href = refData.Href;
            if( string.IsNullOrEmpty(href) )
            {
                throw new ApiomatRequestException(Status.SAVE_REFERENECE_BEFORE_REFERENCING);
            }
            string refHref = null;
            /* Let's check if we use offline storage or send req to server */
            if(Datastore.Instance.SendOffline(HttpMethod.Post))
            {
                refHref = Datastore.Instance.OfflineHandler.AddTask(HttpMethod.Post, Href, refData, "camera", true, usePersistentStorage );
            } 
            else
            {
                refHref = await Datastore.Instance.PostOnServerAsync(refData, Data.GetOrDefault<string>("cameraHref", string.Empty, true), true, usePersistentStorage).ConfigureAwait(false);
            }
            camera = refData;
            return refHref;
        }
        
        /// <exception cref="ApiomatRequestException">When the request fails</exception>
        public async Task RemoveCameraAsync( Com.Apiomat.Frontend.SHCS.Camera refData ) 
        {
            await RemoveCameraAsync( refData, Datastore.Instance.GetOfflineUsageForType(typeof(Com.Apiomat.Frontend.SHCS.Camera))).ConfigureAwait(false);
        }
        
        /// <exception cref="ApiomatRequestException">When the request fails</exception>
        public async Task RemoveCameraAsync( Com.Apiomat.Frontend.SHCS.Camera refData, bool usePersistentStorage )
        {
            string id = refData.Href.Substring( refData.Href.LastIndexOf( "/" ) + 1 );
            if(Datastore.Instance.SendOffline(HttpMethod.Delete))
            {
                Datastore.Instance.OfflineHandler.AddTask(HttpMethod.Delete, Href, refData, "camera", true, usePersistentStorage );
            }
            else
            {
                await Datastore.Instance.DeleteOnServerAsync( Data.GetOrDefault<string>("cameraHref", string.Empty, true) + "/" + id, true, usePersistentStorage ).ConfigureAwait(false);
            }
            camera = null;
        }

 

            public long CameraColor
        {
            get
            {
                return Data.GetOrDefault<long>("cameraColor");
            }

            set
            {
                Data["cameraColor"] = JToken.Parse(JsonConvert.SerializeObject(value));
            }
        }



        /// <summary>
        /// Obsolete. Use RemoveFromStorage instead.
        /// </summary>
        [Obsolete]
        public static int DeleteAllFromStorage(string query = null)
        {
        	return RemoveFromStorage( query, false );
        }
        
        /// <summary>
        /// Obsolete. Use RemoveFromStorage instead.
        /// </summary>
        [Obsolete]
        public static int DeleteAllFromStorageWithReferencedHrefs( string query = null )
        {
        	return RemoveFromStorage( query, true ); 
        }
        
        /// <summary>
        /// Removes objects of this class that were loaded into storage before.
        /// 
        /// Note 1: This ONLY removes objects fetched with getCameraOrderItemsAsync(), which doesn't include loaded referenced objects!
        /// Note 2: The SDK doesn't contain a query parser, so you need to pass the same query as in the fetch request.
        ///  Also, if an object was included in the result of multiple queries, the object doesn't get removed
        /// </summary>
        /// <returns>The number of removed objects</returns>
        /// <param name="query">The query you used when fetching the object collection</param>
        /// <param name="withReferencedHrefs">Use true or false depending on what you used when fetching the object collection</param>
        public static int RemoveFromStorage( string query = null, bool withReferencedHrefs = false )
        {
            CameraOrderItem o = new CameraOrderItem();
            string collectionHref = Datastore.Instance.CreateModelHrefWithParams( o.ModuleName,
                o.SimpleName, withReferencedHrefs, query );
            return Datastore.Instance.DeleteCollectionFromStorage( collectionHref );
        }
        
        /// <summary>
        /// Removes ALL objects of this class that were loaded into storage before, independent of any queries.
        /// </summary>
        /// <returns>The number of removed objects</returns>
        public static int RemoveAllFromStorage( )
        {
            CameraOrderItem o = new CameraOrderItem();
            return Datastore.Instance.RemoveAllFromStorage( o.ModuleName, o.SimpleName );
        }
        
    }
}
