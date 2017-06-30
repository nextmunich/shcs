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
    /// Generated class for Client 
    /// </summary>
    public class Client : AbstractClientDataModel
    {
        /// <summary>
        /// Default constructor. Needed for internal processing.
        /// </summary>
        public Client ( ) : base()
        {
        }
    

        /// <summary>
        /// Returns the simple name of this class 
        /// </summary>
        public override string SimpleName { get { return "Client"; } }

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
        public static async Task<IList<Client>> GetClientsAsync(string query = null) 
        {
            Client o = new  Client();
            return await Datastore.Instance.LoadFromServerAsync<Client>(o.ModuleName, o.SimpleName, query, false, false, Datastore.Instance.GetOfflineUsageForType(typeof(Client))).ConfigureAwait(false);
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
        public static async Task<IList<Client>> GetClientsAsync(string query, bool withReferencedHrefs) 
        {
            Client o = new  Client();
            return await Datastore.Instance.LoadFromServerAsync<Client>(o.ModuleName, o.SimpleName, query, withReferencedHrefs, false, Datastore.Instance.GetOfflineUsageForType(typeof(Client))).ConfigureAwait(false);
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
        public static async Task<IList<Client>> GetClientsAsync(string query, bool withReferencedHrefs, bool usePersistentStorage) 
        {
            Client o = new  Client();
            return await Datastore.Instance.LoadFromServerAsync<Client>(o.ModuleName, o.SimpleName, query, withReferencedHrefs, false, usePersistentStorage).ConfigureAwait(false);
        }
        
        /// <summary>
        /// Returns the count of objects of this class filtered by the given query from the server.
        /// </summary>
        /// <param name="query">A query filtering the results in SQL style (see http://www.apiomat.com/docs/concepts/query-language/)</param>
        /// <returns>The (filtered) count of objects of this class</returns>
        /// <exception cref="ApiomatRequestException">When the request fails</exception>
        public static async Task<long> GetClientsCountAsync( string query = null )
        {
            Client o = new Client();
            string countHref = Datastore.Instance.CreateModelHref( o.ModuleName, o.SimpleName ) + "/count";
            return await Datastore.Instance.CountFromServerAsync(countHref, query, false, false).ConfigureAwait(false);
        }


            public string Address1
        {
            get
            {
                return Data.GetOrDefault<string>("address1");
            }

            set
            {
                Data["address1"] = JToken.Parse(JsonConvert.SerializeObject(value));
            }
        }

            public string Address2
        {
            get
            {
                return Data.GetOrDefault<string>("address2");
            }

            set
            {
                Data["address2"] = JToken.Parse(JsonConvert.SerializeObject(value));
            }
        }

            public string City
        {
            get
            {
                return Data.GetOrDefault<string>("city");
            }

            set
            {
                Data["city"] = JToken.Parse(JsonConvert.SerializeObject(value));
            }
        }

            public string Country
        {
            get
            {
                return Data.GetOrDefault<string>("country");
            }

            set
            {
                Data["country"] = JToken.Parse(JsonConvert.SerializeObject(value));
            }
        }

            public string FirstName
        {
            get
            {
                return Data.GetOrDefault<string>("firstName");
            }

            set
            {
                Data["firstName"] = JToken.Parse(JsonConvert.SerializeObject(value));
            }
        }

            public string LastName
        {
            get
            {
                return Data.GetOrDefault<string>("lastName");
            }

            set
            {
                Data["lastName"] = JToken.Parse(JsonConvert.SerializeObject(value));
            }
        }

            public string PhoneNumber
        {
            get
            {
                return Data.GetOrDefault<string>("phoneNumber");
            }

            set
            {
                Data["phoneNumber"] = JToken.Parse(JsonConvert.SerializeObject(value));
            }
        }

            public string PostalCode
        {
            get
            {
                return Data.GetOrDefault<string>("postalCode");
            }

            set
            {
                Data["postalCode"] = JToken.Parse(JsonConvert.SerializeObject(value));
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
        /// Note 1: This ONLY removes objects fetched with getClientsAsync(), which doesn't include loaded referenced objects!
        /// Note 2: The SDK doesn't contain a query parser, so you need to pass the same query as in the fetch request.
        ///  Also, if an object was included in the result of multiple queries, the object doesn't get removed
        /// </summary>
        /// <returns>The number of removed objects</returns>
        /// <param name="query">The query you used when fetching the object collection</param>
        /// <param name="withReferencedHrefs">Use true or false depending on what you used when fetching the object collection</param>
        public static int RemoveFromStorage( string query = null, bool withReferencedHrefs = false )
        {
            Client o = new Client();
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
            Client o = new Client();
            return Datastore.Instance.RemoveAllFromStorage( o.ModuleName, o.SimpleName );
        }
        
    }
}
