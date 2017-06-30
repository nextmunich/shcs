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
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http;
using System.Diagnostics;
using Com.Apiomat.Frontend.Helper;

namespace Com.Apiomat.Frontend.Offline
{
    public class InMemoryCache : AbstractStorage
    {
        private static InMemoryCache _instance;

        private static IDictionary<string, AbstractStorageContainer> storage;

        private InMemoryCache()
            : base()
        {
            if (storage == null)
            {
                storage = new Dictionary<string, AbstractStorageContainer>();
            }
        }


        public static InMemoryCache Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new InMemoryCache();
                }
                return _instance;
            }
        }


        /// <summary>
        /// Stores an object to the DB or updates it if it already exists
        /// </summary>
        /// <returns><c>true</c>, if object was stored, <c>false</c> otherwise.</returns>
        /// <param name="href">The HREF of the object. If a url with query parameters gets passed, it will be cut off.</param>
        /// <param name="json">The object as json.</param>
        /// <param name="httpMethod">Http method,e.g. "GET", "PUT".</param>

        public override bool StoreObject(string href, string json, HttpMethod httpMethod)
        {
            string cutHref = CutOffParameters(href);

            ObjectStorageContainer container = new ObjectStorageContainer(cutHref, json, httpMethod.ToString());
            container.LastUpdate = DateTimeHelper.GetCurrentTimeMillis();
            storage[cutHref] = container;
            return true;
        }


        /// <summary>
        /// Stores a collection of objects to the DB or updates it if it already exist.
        /// </summary>
        /// <returns><c>true</c>, if collection was stored, <c>false</c> otherwise.</returns>
        /// <param name="requestUrl">Request URL.</param>
        /// <param name="json">Json.</param>
        public override bool StoreCollection(string requestUrl, string json)
        {
            /* Store the objects that are in the collection on their own,
            * create a json array with just the object urls inside,
            * store both and the according mapping */
            JArray hrefCollection = new JArray();
            try
            {
                JArray collection = JArray.Parse(json);
                for (int i = 0; i < collection.Count; i++) // no foreach for JArray
                {
                    JObject obj = (JObject)collection[i];
                    /* Store the objects and at the same time, if one storage procedure fails, return false.
                 * Storing the objects takes care of insert vs. update */
                    JToken objHrefToken;
                    if (obj.TryGetValue("href", out objHrefToken))
                    {
                        string objectHref = objHrefToken.ToString();
                        if (StoreObject(objectHref, obj.ToString(), HttpMethod.Get) == false)
                        {
                            return false;
                        }
                        hrefCollection.Add(objectHref);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Failed to store collection" + e.ToString());
                return false;
            }

            CollectionStorageContainer container = new CollectionStorageContainer(requestUrl, hrefCollection.ToString());
            container.LastUpdate = DateTimeHelper.GetCurrentTimeMillis();
            storage[requestUrl] = container;
            return true;
        }

        /// <summary>
        /// Returns the JSON of a cached object. NULL if not found.
        /// </summary>
        /// <returns>The stored object.</returns>
        /// <param name="href">Href.</param>
        public override string GetStoredObject(string href)
        {
            string cutHref = CutOffParameters(href);
            return storage.ContainsKey(cutHref) ? storage[cutHref].Body : null;
        }


        public override string GetStoredCollection(string url)
        {
            string body = storage.ContainsKey(url) ? storage[url].Body : null;
            if (body == null)
            {
                return null;
            }

            StringBuilder sb = new StringBuilder("[");

            /* Now go through the entries and fetch all objects.
             * If the body is "[]" it's okay, no Exception will be thrown and "[]" will be returned. */
            try
            {
                JArray collectionJson = JArray.Parse(body);
                string fetchedObjectJson;
                for (int i = 0; i < collectionJson.Count; i++)
                {
                    fetchedObjectJson = GetStoredObject(collectionJson[i].ToString());
                    if (fetchedObjectJson != null)
                    {
                        char previousChar = sb.ToString().ToCharArray()[sb.Length - 1];
                        if (i != 0 && previousChar != '[' && previousChar != ',')
                        {
                            sb.Append(",");
                        }
                        sb.Append(fetchedObjectJson);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Failed to get stored collection: " + e.ToString());
                return null;
            }
            sb.Append("]");
            return sb.ToString();
        }

        internal override long? LoadCollectionLastUpdate(string url)
        {
            long? result = null;
            if (storage.ContainsKey(url))
            {
                result = storage[url].LastUpdate;
            }
            return result;
        }

        public override bool RemoveObject(string href)
        {
            base.RemoveObject(href);

            if (href.Contains("?"))
            {
                href = href.Substring(0, href.IndexOf("?"));
            }

            return storage.Remove(href);
        }


        public override int RemoveCollection(string url)
        {
            base.RemoveCollection(url);

            int result = 0;

            /* get all object hrefs that belong to a collection
             *
             * note: this code is similar to getStoredCollection. factorize!
             */

            string body = storage.ContainsKey(url) ? storage[url].Body : null;
            if (body == null)
            {/* else: body is null. ignore */
                return 0;
            }

            /* Now go through the entries and delete all objects and also their LastModified values.
            * If the body is "[]" it's okay, no Exception will be thrown and "[]" will be returned. */
            try
            {
                JArray collectionJson = JArray.Parse(body);
                string hrefOfObjectToRemove;
                for (int i = 0; i < collectionJson.Count; i++)
                {
                    try
                    {
                        hrefOfObjectToRemove = collectionJson[i].ToString(); // JsonException might be thrown here? 
                        RemoveObjectLastModified(hrefOfObjectToRemove);
                        RemoveObject(hrefOfObjectToRemove); // actual deletion
                        result++;
                    }
                    catch (JsonException ex)
                    {
                        Debug.WriteLine("A JSONException occured while trying to get an item from a JArray" +
                            " to delete it from the storage", ex);
                    }
                }
            }
            catch (JsonException ex)
            {
                Debug.WriteLine("A JSONException occured while trying to create a JArray from a collection",
                      ex);
            }

            /* delete the collection entry */
            storage.Remove(url);
            return result;
        }

		internal override int RemoveAllObjects (string modelHref)
		{
			base.RemoveAllObjects (modelHref);

			int count = 0;
			List<string> matchingKeys = storage.AsParallel ()
				.Select (kvPair => kvPair.Key.StartsWith (modelHref) ? kvPair.Key : null)
				.Where (key => key != null)
				.ToList ();
			foreach (string key in matchingKeys)
			{
				if (RemoveObject (key))
				{
					count++;
				}
			}
			return count;
		}

        public override int Clear()
        {
            base.Clear();
            int result = storage.Count;
            storage.Clear();
            return result;
        }

        internal override IList<CollectionStorageContainer> GetAllCollections ()
        {
            IList<CollectionStorageContainer> result = new List<CollectionStorageContainer> ();
            foreach (AbstractStorageContainer asc in storage.Values)
            {
                CollectionStorageContainer csc = asc as CollectionStorageContainer;
                if (csc != null)
                {
                    result.Add (csc);
                }
            }
            return result;
        }
    }
}