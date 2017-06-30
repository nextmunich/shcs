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
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Com.Apiomat.Frontend.Basics;
using Com.Apiomat.Frontend.Extensions;
using System.Net.Http;
using Com.Apiomat.Frontend.Helper;
using System.Diagnostics;

namespace Com.Apiomat.Frontend
{
	/// <summary>
	/// This class defines the base class of all classes for frontend developers. All data is stored in a JSON data object except the HREF of an incance of this class, originally containing the type of this class.
	/// </summary>
	public abstract class AbstractClientDataModel
	{
		#region Enums

		public enum ObjectState
		{
			CREATED, PERSISTING, PERSISTED, LOCAL_PERSISTED, DELETING, DELETED, LOCAL_DELETED
		}

		#endregion
		#region Private data

		// The representation of the data of an instance of this class as JSON object
		private JObject _data;
		private string _href;

		#endregion
		#region Public properties

		internal JObject Data { get { return _data; } private set { _data = value; } }

		/// <summary>
		/// Gets the HREF of this class instance.
		/// </summary>
		/// <value>The HREF of this data class instance, NULL if it was created but not saved yet</value>
		public string Href { get { return _href; } private set { _href = value; } }

		public ObjectState CurrentState { get; set; }

		/// <summary>
		/// Property for the foreign id for this object.
		/// A foreign id is a NON apiomat id (like facebook/twitter id).
		/// Contains an empty string by default.
		/// </summary>
		/// <value>The foreign ID</value>
		public string ForeignId
		{
			get
			{
				return Data.GetOrDefault<string>("foreignId", string.Empty, true);
			}
			set
			{
				Data["foreignId"] = value;
			}
		}

		/// <summary>
		/// Returns a set of all role names allowed to grant privileges on this object
		/// Contains an empty HashSet<string> by default.
		/// </summary>
		/// <value>The set of all roles allowed to grant privileges on this object</value>
		public ISet<string> AllowedRolesGrant
		{
			get
			{
				return GetAllowedRoles("allowedRolesGrant");
			}
			set
			{
				SetAllowedRoles("allowedRolesGrant", value);
			}
		}

		/// <summary>
		/// Returns a set of all role names allowed to read this object.
		/// Contains an empty HashSet<string> by default.
		/// </summary>
		/// <value>The set of all roles allowed to read this object</value>
		public ISet<string> AllowedRolesRead
		{
			get
			{
				return GetAllowedRoles("allowedRolesRead");
			}
			set
			{
				SetAllowedRoles("allowedRolesRead", value);
			}
		}

		/// <summary>
		/// Returns a set of all role names allowed to write this object
		/// Contains an empty HashSet<string> by default.
		/// </summary>
		/// <value>The set of all roles allowed to write this object</value>
		public ISet<string> AllowedRolesWrite
		{
			get
			{
				return GetAllowedRoles("allowedRolesWrite");
			}
			set
			{
				SetAllowedRoles("allowedRolesWrite", value);
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the access to resources is / should be restricted by the defined roles for this object
		/// </summary>
		/// <value><c>true</c> if access to resource is / should be restricted; <c>false</c> otherwise.</value>
		public bool RestrictResourceAccess
		{
			get
			{
				return Data.GetOrDefault<bool>("restrictResourceAccess");
			}
			set
			{
				Data["restrictResourceAccess"] = value;
			}
		}

		/// <summary>
		/// Returns the name of the app that this class belongs to
		/// </summary>
		/// <value>The name of the app that this class belongs to</value>
		/// <exception cref="KeyNotFoundException">If the key app name couldn't be found in the object's data</exception>
		public string AppName
		{
			get
			{
				JToken jt = Data["applicationName"];
				if (jt == null)
				{
					throw new KeyNotFoundException("App name couldn't be found in the object's data");
				}
				return Data["applicationName"].ToObject<string>();
			}
		}

		/// <summary>
		/// Returns the date when this object was created on the server in the format "milliseconds since epoch".
		/// To get a DateTime use the method GetCreatedAt().
		/// Contains 0 by default.
		/// </summary>
		/// <value>Milliseconds since epoch from when the object was created.</value>
		public long CreatedAt
		{
			get
			{
				return Data.GetOrDefault<long>("createdAt");
			}
		}

		/// <summary>
		/// Returns the date when this object was modified the last time on the server in the format "milliseconds since epoch".
		/// To get a DateTime use the method GetLastModiefiedAt().
		/// Contains 0 by default.
		/// </summary>
		/// <value>Milliseconds since epoch from when the object was modified the last time.</value>
		public long LastModifiedAt
		{
			get
			{
				return Data.GetOrDefault<long>("lastModifiedAt");
			}
		}

		/// <summary>
		/// Contains the ID of the object. Null if the object was created, but not saved yet.
		/// </summary>
		/// <value>The ID of the object</value>
		public string Id
		{
			get
			{
				string id = Data.GetOrDefault<string>("id");
				if (string.IsNullOrEmpty(id))
				{
					// extract from HREF
					id = Href == null ? null : Href.Substring(Href.LastIndexOf("/") + 1);
				}
				return id;
			}
		}

		public bool Offline
		{
			// return this.data.optBoolean("isOffline", false);
			get
			{
				return Data.GetOrDefault<bool>("isOffline", false);
			}
			set
			{
				Data["isOffline"] = value;
			}
		}

		public string System { get { return User.System; } }

		/// <summary>
		/// Returns the name of the module that this class belongs to.
		/// </summary>
		/// <value>The name of the module which this class belongs to</value>
		public abstract string ModuleName { get; }

		/// <summary>
		/// Contains the HREFs of all referenced class instances.
		/// Null by default.
		/// </summary>
		/// <value>The reference model hrefs.</value>
		public IDictionary<string, IList<string>> RefModelHrefs
		{
			get
			{
				return Data.GetOrDefault<IDictionary<string, IList<string>>>("referencedHrefs");
			}
		}

		/// <summary>
		/// Returns the simple class name
		/// </summary>
		/// <value>The simple class name</value>
		public abstract string SimpleName { get; }

		#endregion
		#region Private properties

		/// <summary>
		/// Returns the unique type of this class to get identified via REST interface
		/// </summary>
		/// <value>The the unique type of this class to get identified via REST interface</value>
		private string @Type
		{
			get
			{
				return this.ModuleName + "$" + this.SimpleName;
			}
		}

		#endregion
		#region Constructors

		public AbstractClientDataModel()
		{
			Data = new JObject();
			Data["@type"] = this.Type;
			if ((this.SimpleName.Equals("MemberModel") || this.SimpleName.Equals("User")) && ModuleName.Equals("Basics"))
			{
				Data["dynamicAttributes"] = JToken.FromObject(new Dictionary<string, string>());
			}
		}

		#endregion
		#region Public methods

		public AbstractClientDataModel FromJson(string modelJson)
		{
			Data = JObject.Parse(modelJson);
			_href = Data.GetOrDefault<string>("href", string.Empty, true);
			return this;
		}

		/// <summary>
		/// Encodes this class instance as a JSON string; used to communicate with the REST interface
		/// </summary>
		/// <returns>The class instance as JSON string</returns>
		public string ToJson()
		{
			string json;
			if (string.IsNullOrEmpty(Href) == false)
			{
				Data["id"] = Href.Substring(Href.LastIndexOf("/") + 1);
				json = JsonConvert.SerializeObject(Data);
				Data.Remove("id");
			}
			else
			{
				json = JsonConvert.SerializeObject(Data);
			}
			return json;
		}

		/// <summary>
		/// Deletes this object from the storage (cache and persistent storage)
		/// </summary>
		/// <returns></returns>
		public bool DeleteFromStorage()
		{
			return Datastore.Instance.DeleteObjectFromStorage(this.Href, false);
		}

		/// <summary>
		/// Deletes the class instance on the server
		/// </summary>
		/// <exception cref="ApiomatRequestException">When the request fails</exception>
		public async Task DeleteAsync()
		{
			await DeleteAsync(Datastore.Instance.GetOfflineUsageForType(this.GetType()));
		}

		/// <summary>
		/// Deletes the class instance on the server
		/// </summary>
		/// <exception cref="ApiomatRequestException">When the request fails</exception>
		public async Task DeleteAsync(bool usePersistentStorage)
		{
			if (Datastore.Instance.CheckObjectState && (CurrentState.Equals(ObjectState.PERSISTING) || CurrentState.Equals(ObjectState.DELETING)))
			{
				throw new InvalidOperationException("Object is in persisting or deleting process. Please try again later");
			}
			bool wasLocalDelete = false;
			CurrentState = ObjectState.DELETING;
			try
			{
				bool isRef = false;
				if (Datastore.Instance.SendOffline(HttpMethod.Delete))
				{
					Datastore.Instance.OfflineHandler.AddTask(HttpMethod.Delete, Href, this, null, isRef, usePersistentStorage);
					wasLocalDelete = true;
				}
				else
				{
					await Datastore.Instance.DeleteOnServerAsync(Href, isRef, usePersistentStorage).ConfigureAwait(false);
				}
			}
			finally
			{
				CurrentState = wasLocalDelete ? ObjectState.LOCAL_DELETED : ObjectState.DELETED;
			}
		}

		/// <summary>
		/// Returns HREFs of a referenced class instance, given by its name
		/// </summary>
		/// <returns>HREFs of a referenced class instance</returns>
		/// <param name="name">The name of the referenced class instance</param>
		public IList<string> GetRefModelHrefsForName(string name)
		{
			IDictionary<string, IList<string>> referencedHrefs = RefModelHrefs;
			if (referencedHrefs == null || referencedHrefs.ContainsKey(name) == false)
			{
				return null;
			}
			return referencedHrefs[name];
		}


		/// <summary>
		/// Loads (updates) this class instance with server values.
		/// The href parameter is optional, only use it when loading a class instance that doesn't already contain a href (when it wasn't sent/loaded before).
		/// </summary>
		/// <param name="href">The href of this class instance</param>
		/// <exception cref="ApiomatRequestException">When the request fails</exception>
		/// <exception cref="InvalidOperationException">When the object is in persisting process</exception>
		public async Task LoadAsync(string href = null)
		{
			await LoadAsync(href, false, Datastore.Instance.GetOfflineUsageForType(this.GetType())).ConfigureAwait(false);
		}
		/// <summary>
		/// Loads (updates) this class instance with server values.
		/// The href parameter is optional, only use it when loading a class instance that doesn't already contain a href (when it wasn't sent/loaded before).
		/// </summary>
		/// <param name="href">The href of this class instance</param>
		/// <exception cref="ApiomatRequestException">When the request fails</exception>
		/// <exception cref="InvalidOperationException">When the object is in persisting process</exception>
		public async Task LoadAsync(bool usePersistentStorage)
		{
			await LoadAsync(null, false, Datastore.Instance.GetOfflineUsageForType(this.GetType())).ConfigureAwait(false);
		}
		/// <summary>
		/// Loads (updates) this class instance with server values.
		/// The href parameter is optional, only use it when loading a class instance that doesn't already contain a href (when it wasn't sent/loaded before).
		/// </summary>
		/// <param name="href">The href of this class instance</param>
		/// <param name="usePersistentStorage">whether persistent storage will be used</param>
		/// <exception cref="ApiomatRequestException">When the request fails</exception>
		/// <exception cref="InvalidOperationException">When the object is in persisting process</exception>
		public async Task LoadAsync(string href, bool usePersistentStorage)
		{
			await LoadAsync(href, false, usePersistentStorage).ConfigureAwait(false);
		}

		/// <summary>
		/// Loads (updates) this class instance with server values.
		/// The href parameter is optional, only use it when loading a class instance that doesn't already contain a href (when it wasn't sent/loaded before).
		/// </summary>
		/// <param name="href">The href of this class instance</param>
		/// <param name="isCacheThenNetworkFirstCall"></param>
		/// <param name="usePersistentStorage">whether persistent storage will be used</param>
		/// <exception cref="ApiomatRequestException">When the request fails</exception>
		/// <exception cref="InvalidOperationException">When the object is in persisting process</exception>
		private async Task LoadAsync(string href, bool isCacheThenNetworkFirstCall, bool usePersistentStorage)
		{
			if (Datastore.Instance.CheckObjectState && (CurrentState.Equals(ObjectState.PERSISTING) || CurrentState.Equals(ObjectState.DELETING)))
			{
				throw new InvalidOperationException("Object is in persisting or deleting process. Please try again later");
			}
			bool withReferencedHrefs = false;
			bool isRef = false;
			await Datastore.Instance.LoadFromServerAsync(this, href == null ? Href : href, withReferencedHrefs, isRef, isCacheThenNetworkFirstCall, usePersistentStorage).ConfigureAwait(false);
			// Set href only if was given
			if (string.IsNullOrEmpty(href) == false)
			{
				Href = Data.GetOrDefault<string>("href");
			}
		}
		public virtual async Task<string> SaveAsync(bool loadAfterwards = true)
		{
			return await SaveAsync(loadAfterwards, Datastore.Instance.GetOfflineUsageForType(this.GetType())).ConfigureAwait(false);
		}

		/// <summary>
		/// Saves this class instance. It is - based on its HREF - automatically determined, if this class instance exists on the server, leading to an update, or not, leading to an post command.
		/// </summary>
		/// <param name="loadAfterwards">Indicates whether after saving the object, the local object should be loaded with the values from the server (on the first save, new values like createdAt and href get added on the server)</param>
		/// <returns>The Href of the persisted (saved or updated) object</returns>
		/// <exception cref="ApiomatRequestException">When the request fails</exception>
		public virtual async Task<string> SaveAsync(bool loadAfterwards, bool usePersistentStorage)
		{
			if (Datastore.Instance.CheckObjectState && (CurrentState.Equals(ObjectState.PERSISTING) || CurrentState.Equals(ObjectState.DELETING)))
			{
				throw new InvalidOperationException("Object is in persisting or deleting process. Please try again later");
			}
			bool wasLocalSave = false;
			CurrentState = ObjectState.PERSISTING;
			try
			{
				if (this.Href == null)
				{
					if (Datastore.Instance.SendOffline(HttpMethod.Post))
					{
						string sendHREF =
							Datastore.Instance.CreateModelHref(ModuleName, SimpleName);
						string location =
							Datastore.Instance.OfflineHandler
								.AddTask(HttpMethod.Post, sendHREF, this, null, false, usePersistentStorage);
						wasLocalSave = true;
						if (location != null)
						{
							this.Href = location;
						}
					}
					else
					{
						string location = await Datastore.Instance.PostOnServerAsync(this, false, usePersistentStorage).ConfigureAwait(false);
						this.Href = location;
					}
				}
				else
				{
					if (Datastore.Instance.SendOffline(HttpMethod.Put))
					{
						/* update lastmodified */
						try
						{
							this.Data["lastModifiedAt"] = DateTimeHelper.GetCurrentTimeMillis();
						}
						catch (JsonException ex)
						{
							Debug.WriteLine("During storing the queued object to offline storage" +
								"the lastmodified attribute couldn't be updated due to a JSONException." +
								"It will be stored nevertheless.", ex);
						}
						Datastore.Instance.OfflineHandler
							.AddTask(HttpMethod.Put, Href, this, null, false, usePersistentStorage);
						wasLocalSave = true;
					}
					else
					{
						await Datastore.Instance.UpdateOnServerAsync(this, false, usePersistentStorage).ConfigureAwait(false);
					}
				}
				this.Offline = wasLocalSave;
				if (wasLocalSave == false)
				{
					if (loadAfterwards)
					{
						/* Object state needs to be changed before loading */
						CurrentState = ObjectState.PERSISTED;
						await LoadAsync(Href, usePersistentStorage).ConfigureAwait(false);
					}
				}
				return this.Href;
			}
			finally
			{
				CurrentState = wasLocalSave ? ObjectState.LOCAL_PERSISTED : ObjectState.PERSISTED;
			}
		}

		/// <summary>
		/// Returns the date when this object was created on the server.
		/// </summary>
		/// <returns>The date when this object was created on the server. Null if the object didn't get saved yet<see cref="System.DateTime"/>.</returns>
		public DateTime? GetCreatedAt()
		{
			try
			{
				return new DateTime(CreatedAt * TimeSpan.TicksPerMillisecond);
			}
			catch (NullReferenceException)
			{
				return null;
			}
		}

		/// <summary>
		/// Returns the date when this object was modified the last time on the server
		/// </summary>
		/// <returns>The date when this object was modified the last time on the server. Null if the object didn't get modified yet or didn't get saved after modification<see cref="System.DateTime"/>.</returns>
		public DateTime? GetLastModifiedAt()
		{
			try
			{
				return new DateTime(LastModifiedAt * TimeSpan.TicksPerMillisecond);
			}
			catch (NullReferenceException)
			{
				return null;
			}
		}

		#endregion
		#region Internal methods

		internal static IList<T> FromJArray<T>(JArray jsonArray) where T : AbstractClientDataModel
		{
			IList<T> result = new List<T>();
			foreach (JToken jt in jsonArray)
			{
				//              result.Add (jt.ToObject<T> ()); // doesn't populate the inherited attributes
				T t = (T)Activator.CreateInstance(typeof(T));
				t.FromJObject<T>((JObject)jt);
				result.Add(t);
			}
			return result;
		}

		internal T FromJObject<T>(JObject dataJson) where T : AbstractClientDataModel
		{
			Data = dataJson;
			Href = Data.GetOrDefault<string>("href");
			return (T)this;
		}

		#endregion
		#region Private methods

		private ISet<string> GetAllowedRoles(string accessType)
		{
			return Data.GetOrDefault<ISet<string>>(accessType, new HashSet<string>(), true);
		}

		private void SetAllowedRoles(string accessType, ISet<string> roles)
		{
			Data[accessType] = JToken.FromObject(roles);
		}

		#endregion
	}
}
