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
using Com.Apiomat.Frontend.Offline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Com.Apiomat.Frontend.Offline
{
    public class MemoryElseOfflineStorage : AbstractStorage
    {
        private static MemoryElseOfflineStorage instance;

        private MemoryElseOfflineStorage()
            : base()
        {
        }

        public static MemoryElseOfflineStorage Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MemoryElseOfflineStorage();
                }
                return instance;
            }
        }

        // Methods ================================================================


        public override bool StoreObject(string href, string json, HttpMethod httpMethod)
        {
            return InMemoryCache.Instance.StoreObject(href, json, httpMethod);
        }


        public override bool StoreCollection(string requestUrl, string json)
        {
            return InMemoryCache.Instance.StoreCollection(requestUrl, json);
        }


        public override string GetStoredObject(string href)
        {
            string result = InMemoryCache.Instance.GetStoredObject(href);
            return result == null ? SQLiteStorage.Instance.GetStoredObject(href) : result;
        }

        public override string GetStoredCollection(string url)
        {
            string result = InMemoryCache.Instance.GetStoredCollection(url);
            return result == null ? SQLiteStorage.Instance.GetStoredCollection(url) : result;
        }

        internal override long? LoadCollectionLastUpdate(string url)
        {
            long? result = InMemoryCache.Instance.LoadCollectionLastUpdate(url);
            return result == null ? SQLiteStorage.Instance.LoadCollectionLastUpdate(url) : result;
        }

        public override bool RemoveObject(string href)
        {
            bool result = InMemoryCache.Instance.RemoveObject(href);
            result = result || SQLiteStorage.Instance.RemoveObject(href);
            return result;
        }

        public override int RemoveCollection(string href)
        {
            int result = 0;
            result += InMemoryCache.Instance.RemoveCollection(href);
            result += SQLiteStorage.Instance.RemoveCollection(href);
            return result;
        }

		internal int RemoveAllObjects (string modelHref)
		{
			int result = 0;
			result += InMemoryCache.Instance.RemoveAllObjects(modelHref);
			result += SQLiteStorage.Instance.RemoveAllObjects(modelHref);
			return result;
		}

        public override int Clear()
        {
            int result = 0;
            result += InMemoryCache.Instance.Clear();
            result += SQLiteStorage.Instance.Clear();
            return result;
        }

        internal override IList<CollectionStorageContainer> GetAllCollections()
        {
            List<CollectionStorageContainer> result = new List<CollectionStorageContainer> ();
            result.AddRange (InMemoryCache.Instance.GetAllCollections ());
            result.AddRange (SQLiteStorage.Instance.GetAllCollections ());
            return result;
        }
    }
}
