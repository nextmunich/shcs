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

									
using CountlySDK;
using PCLStorage;

namespace Com.Apiomat.Frontend.Basics
{
    /// <summary>
    /// Generated class representing a user in your app 
    /// </summary>
    public class User : AbstractClientDataModel
    {
        private static readonly string _baseUrl = "https://poc.apiomat.enterprises/yambas/rest/apps/SHCS";
        private static readonly string _apiKey = "2261031135213171686";
        private static readonly string _system = "LIVE";
        private static readonly string _sdkVersion = "2.5.2-146";
        private static readonly string _analyticsHost = "https://poc.apiomat.enterprises/analytics";
        private static readonly string _analyticsAppKey = "1629edf9565a6df2523248b45a11859a06bbbdad";

        public static string BaseUrl { get { return _baseUrl; } }
        public static string ApiKey { get { return _apiKey; } }
        public new static string System { get { return _system; } }
        public static string SdkVersion { get { return _sdkVersion; } }
        public static string AnalyticsHost { get { return _analyticsHost; } }
        public static string AnalyticsAppKey { get { return _analyticsAppKey; } }    
        /// <summary>
        /// Default constructor. Needed for internal processing.
        /// </summary>
        public User ( ) : base()
        {
        }
    
        /// <summary>
        /// Contructor for initializing the user with a username and password
        /// </summary>
        public User ( string userName, string password )
        {
            UserName = userName;
            Password = password;
        }

        /// <summary>
        /// Returns the simple name of this class 
        /// </summary>
        public override string SimpleName { get { return "User"; } }

        /// <summary>
        /// Returns the name of the module where this class belongs to
        /// </summary>
        public override string ModuleName { get { return "Basics"; } }

        /// <summary>
        /// Initialize the Datastore if it hasn't been configured yet. First tries to use username + password. If not present: Session token. If not present and parameter allowGuest set to true: Init as GUEST.
        /// </summary>
        /// <param name="allowGuest">Indicates whether the datastore may be configured as guest when neither credentials nor a session token is present in the user object</param>
        /// <exception cref="InvalidOperationException">When the parameter allowGuest is set to false and neither user credentials nor a session token is present in the user object.</exception>
        protected void InitDatastoreIfNeeded(bool allowGuest) 
        {
            try
            {
                var ignore = Datastore.Instance;
            }
            catch (InvalidOperationException)
            {
                if (!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password))
                {
                    Datastore.ConfigureWithCredentials(this);
                }
                else if (!string.IsNullOrEmpty(SessionToken))
                {
                    Datastore.ConfigureWithSessionToken(SessionToken);
                }
                else if (allowGuest)
                {
                    Datastore.ConfigureAsGuest();
                }
                else
                {
                    throw new InvalidOperationException("The Datastore needs to be configured with user credentials or a session token for this method to work." );
                }
            }
        }
    
        /// <summary>
        /// Updates this class from server in the background and not on the UI thread
        /// </summary>
        /// <exception cref="ApiomatRequestException">When the request fails</exception>
        public virtual async Task LoadMeAsync()
        {
            InitDatastoreIfNeeded(false);
            await LoadAsync("models/me").ConfigureAwait(false);
        }
    
        /// <summary>
        /// Saves this object. If it has no HREF this leads to a post and the class instance
        /// is created on the server, else it is updated. After the save a load will
        /// be called to load the actual object from the server. 
        /// </summary>
        /// <param name="loadAfterwards">Indicates whether after saving the object, the local object should be loaded with the values from the server (on the first save, new values like createdAt and href get added on the server)</param>
        /// <returns>The Href of the persisted (saved or updated) object</returns>
        /// <exception cref="ApiomatRequestException">When the request fails</exception>
        public override async Task<string> SaveAsync( bool loadAfterwards = true )
        {
            InitDatastoreIfNeeded(false);
            return await base.SaveAsync(loadAfterwards).ConfigureAwait(false);
        }

        /// <summary>
        /// Lets the user reset his password in case he forgot his old one.
        /// The user will receive an email to confirm.
        /// If you just want to change the user's password, use the ChangePasswordAsync(string) method instead.
        /// </summary>
        /// <exception cref="ApiomatRequestException">When the request fails</exception>
        [Obsolete("Use ResetPasswordAsync() instead")]
        public async Task RequestNewPasswordAsync( )
        {
            await ResetPasswordAsync ().ConfigureAwait(false);
        }

        /// <summary>
        /// Lets the user reset his password in case he forgot his old one.
        /// The user will receive an email to confirm.
        /// If you just want to change the user's password, use the ChangePasswordAsync(string) method instead.
        /// </summary>
        /// <exception cref="ApiomatRequestException">When the request fails</exception>
        public async Task ResetPasswordAsync( )
        {
            await Datastore.Instance.PostOnServerAsync(this, BaseUrl + "/models/requestResetPassword/", false, false).ConfigureAwait(false);
        }

        /// <summary>
        /// Change the user's password.
        /// If the user forgot his old password, you can reset the password with ResetPasswordAsync().
        /// </summary>
        /// <param name="newPassword">The new password</param>
        /// <exception cref="ApiomatRequestException">When the request fails</exception>
        /// <exception cref="InvalidOperationException">When the object is currently in persisting process</exception>
        [Obsolete("Use ChangePasswordAsync(string) instead")]
        public async Task ResetPasswordAsync( string newPassword)
        {
            await ChangePasswordAsync (newPassword).ConfigureAwait(false);
        }

        /// <summary>
        /// Change the user's password.
        /// If the user forgot his old password, you can reset the password with ResetPasswordAsync().
        /// </summary>
        /// <param name="newPassword">The new password</param>
        /// <exception cref="ApiomatRequestException">When the request fails</exception>
        /// <exception cref="InvalidOperationException">When the object is currently in persisting process</exception>
        public async Task ChangePasswordAsync( string newPassword)
        {
            if (CurrentState == ObjectState.PERSISTING)
            {
                throw new InvalidOperationException("Object is in persisting process. Please try again later.");
            }
            this.Password = newPassword;
            await Datastore.Instance.UpdateOnServerAsync( this, false, false ).ConfigureAwait(false);
            Datastore.ConfigureWithCredentials(UserName, Password);
        }

        /// <summary>
        /// Request a session token with either the credentials that are stored in this user object or with the provided refresh token. Optionally configures the datastore with the received token and saves it in the user object.
        /// </summary>
        /// <param name="configure">(Optional) Set flag to false if you don't want the Datastore to be configured automatically with the received session token and also don't want to save it in the user object.</param>
        /// <param name="refreshToken">(Optional) refresh token to use for requesting a new session token</param>
        /// <returns>A Dictionary that maps the following keys to their values: "SessionToken", "RefreshToken", "ExpirationDate" (Unix UTC timestamp in ms), "Module" and "Model"</returns>
        /// <exception cref="ApiomatRequestException">When the request fails</exception>
        /// <exception cref="InvalidOperationException">When requesting a session token without a refresh token and the user object has no username or password</exception>
        public async Task<IDictionary<string, string>> RequestSessionTokenAsync(bool configure = true, string refreshToken = null)
        {
            bool allowGuest = refreshToken == null ? false : true;
            InitDatastoreIfNeeded(allowGuest);
            if (!configure)
            {
                return await Datastore.Instance.RequestSessionTokenAsync(refreshToken).ConfigureAwait(false);
            }
            IDictionary<string, string> tokenDict = await Datastore.Instance.RequestSessionTokenAsync(refreshToken).ConfigureAwait(false);
            string sessionToken = tokenDict["SessionToken"];
            if ( sessionToken == null )
            {
                throw new ApiomatRequestException( Status.NO_TOKEN_RECEIVED );
            }
            SessionToken = sessionToken;
            Datastore.ConfigureWithSessionToken(sessionToken);
            return tokenDict;
        }

        /// <summary>
        /// Returns a list of objects of this class filtered by the given query from server.
        /// The size of the resultset is limited to an installation specific value ('maxResults') and defaults to 1000. 
        /// Use limit and offset to return all results if the expected size is larger.
        /// </summary>
        /// <param name="query">A query filtering the results in SQL style (see http://www.apiomat.com/docs/concepts/query-language/)</param>
        /// <returns>A list of objects of this class filtered by the given query from server</returns>
        /// <exception cref="Exception">When something fails</exception>
        public static async Task<IList<User>> GetUsersAsync(string query = null) 
        {
            User o = new  User();
            return await Datastore.Instance.LoadFromServerAsync<User>(o.ModuleName, o.SimpleName, query, false, false, Datastore.Instance.GetOfflineUsageForType(typeof(User))).ConfigureAwait(false);
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
        public static async Task<IList<User>> GetUsersAsync(string query, bool withReferencedHrefs) 
        {
            User o = new  User();
            return await Datastore.Instance.LoadFromServerAsync<User>(o.ModuleName, o.SimpleName, query, withReferencedHrefs, false, Datastore.Instance.GetOfflineUsageForType(typeof(User))).ConfigureAwait(false);
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
        public static async Task<IList<User>> GetUsersAsync(string query, bool withReferencedHrefs, bool usePersistentStorage) 
        {
            User o = new  User();
            return await Datastore.Instance.LoadFromServerAsync<User>(o.ModuleName, o.SimpleName, query, withReferencedHrefs, false, usePersistentStorage).ConfigureAwait(false);
        }
        
        /// <summary>
        /// Returns the count of objects of this class filtered by the given query from the server.
        /// </summary>
        /// <param name="query">A query filtering the results in SQL style (see http://www.apiomat.com/docs/concepts/query-language/)</param>
        /// <returns>The (filtered) count of objects of this class</returns>
        /// <exception cref="ApiomatRequestException">When the request fails</exception>
        public static async Task<long> GetUsersCountAsync( string query = null )
        {
            User o = new User();
            string countHref = Datastore.Instance.CreateModelHref( o.ModuleName, o.SimpleName ) + "/count";
            return await Datastore.Instance.CountFromServerAsync(countHref, query, false, false).ConfigureAwait(false);
        }


            public DateTime? DateOfBirth
        {
            get
            {
                JToken jt = Data["dateOfBirth"];
                if(jt != null)
                {
                    DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    DateTime date = epoch.AddMilliseconds((jt.ToObject<double>()));
                    return date.ToLocalTime();
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value != null)
                {
                    DateTime dateToSet = (DateTime)value;
                    Data["dateOfBirth"] = JValue.Parse(JsonConvert.SerializeObject((long)(dateToSet.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds), Formatting.None));
                }
                else
                {
                    Data["dateOfBirth"] = null;
                }
            }
        }


            public IDictionary<string, string> DynamicAttributes
        {
            get
            {
                return Data.GetOrDefault<IDictionary<string, string>>("dynamicAttributes");
            }

            set
            {
                Data["dynamicAttributes"] = JObject.Parse(JsonConvert.SerializeObject(value));
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

            public double LocLatitude
        {
            get
            {
                JArray loc = Data["loc"] as JArray;
                if (loc == null)
                {
                    return default(double);
                }
                else
                {
                    return double.Parse (loc.ElementAt (0).ToObject<string>(), CultureInfo.InvariantCulture);
                }
            }
            set
            {
                JArray ja = Data["loc"] as JArray;
                if ( ja == null)
                {
                    ja = new JArray( ) {null, null};
                }
                ja.RemoveAt(0);
                ja.Insert(0, JValue.FromObject(value));
                Data["loc"] = ja;
            }
        }
        public double LocLongitude
        {
            get
            {
                JArray loc = Data["loc"] as JArray;
                if (loc == null)
                {
                    return default(double);
                }
                else
                {
                    return double.Parse (loc.ElementAt (1).ToObject<string>(), CultureInfo.InvariantCulture);
                }
            }
            set
            {
                JArray ja = Data["loc"] as JArray;
                if ( ja == null)
                {
                    ja = new JArray( ) {null, null};
                }
                ja.RemoveAt(1);
                ja.Insert(1, JValue.FromObject(value));
                Data["loc"] = ja;
            }
        }

            public string Password
        {
            get
            {
                return Data.GetOrDefault<string>("password");
            }

            set
            {
                Data["password"] = JToken.Parse(JsonConvert.SerializeObject(value));
            }
        }

            public string Salt
        {
            get
            {
                return Data.GetOrDefault<string>("salt");
            }

            set
            {
                Data["salt"] = JToken.Parse(JsonConvert.SerializeObject(value));
            }
        }

            public string SessionToken
        {
            get
            {
                return Data.GetOrDefault<string>("sessionToken");
            }

            set
            {
                Data["sessionToken"] = JToken.Parse(JsonConvert.SerializeObject(value));
            }
        }

            public string UserName
        {
            get
            {
                return Data.GetOrDefault<string>("userName");
            }

            set
            {
                Data["userName"] = JToken.Parse(JsonConvert.SerializeObject(value));
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
        /// Note 1: This ONLY removes objects fetched with getUsersAsync(), which doesn't include loaded referenced objects!
        /// Note 2: The SDK doesn't contain a query parser, so you need to pass the same query as in the fetch request.
        ///  Also, if an object was included in the result of multiple queries, the object doesn't get removed
        /// </summary>
        /// <returns>The number of removed objects</returns>
        /// <param name="query">The query you used when fetching the object collection</param>
        /// <param name="withReferencedHrefs">Use true or false depending on what you used when fetching the object collection</param>
        public static int RemoveFromStorage( string query = null, bool withReferencedHrefs = false )
        {
            User o = new User();
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
            User o = new User();
            return Datastore.Instance.RemoveAllFromStorage( o.ModuleName, o.SimpleName );
        }
        
        /// <summary>
        /// Starts a Countly session with the default analytics host, app key and SDK version.
        /// </summary>
        public static async Task StartCountlySessionAsync()
        {
            await StartCountlySessionAsync (User.AnalyticsHost, User.AnalyticsAppKey, User.SdkVersion).ConfigureAwait(false);
        }

        /// <summary>
        /// Starts a Countly session.
        /// </summary>
        /// <param name="analyticsHost">Analytics host.</param>
        /// <param name="analyticsAppKey">Analytics app key.</param>
        /// <param name="analyticsAppVersion">Analytics app version (e.g. SDK version).</param>
        public static async Task StartCountlySessionAsync(string analyticsHost, string analyticsAppKey, string analyticsAppVersion)
        {
            await Countly.StartSession (analyticsHost, analyticsAppKey, analyticsAppVersion, FileSystem.Current).ConfigureAwait(false);
        }

        public static async Task EndCountlySessionAsync()
        {
            await Countly.EndSession().ConfigureAwait(false);
        }
    }
}
