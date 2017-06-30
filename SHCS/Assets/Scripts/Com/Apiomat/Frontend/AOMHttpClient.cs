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
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Com.Apiomat.Frontend;
using Com.Apiomat.Frontend.Basics;
using System.IO;
using System.Net.Http;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Reflection;
using System.Diagnostics;

namespace Com.Apiomat.Frontend
{
	public class AOMHttpClient
	{

		#region Private data

		protected static readonly string AOM_HEADER_APIKEY = "X-apiomat-apikey";
		protected static readonly string AOM_HEADER_SDKVERION = "X-apiomat-sdkVersion";
		protected static readonly string AOM_HEADER_SYSTEM = "X-apiomat-system";
		protected static readonly string AOM_HEADER_FULLUPDATE = "X-apiomat-fullupdate";
		protected static readonly string AOM_HEADER_DELTA = "X-apiomat-delta";
		protected static readonly string AOM_HEADER_DELTADELETED = "X-apiomat-delta-deleted";

		private readonly string _baseUrl;
		private readonly string _apiKey;
		private readonly string _userName;
		private readonly string _password;
		private readonly string _system;
		private readonly Datastore.AuthType _authType;
		private readonly string _sessionToken;
		private int _clientTimeout = 15000;

		protected static CookieContainer cookieContainer = new CookieContainer();

		#endregion

		#region Public data

		public int ClientTimeout { get { return _clientTimeout; } set { _clientTimeout = value; } }

		internal Dictionary<string, string> CustomHeaders { get; set; }

		#endregion

		#region Constructors

		public AOMHttpClient(string baseUrl, string apiKey, string userName, string password, string system,
							   Datastore.AuthType authType, string sessionToken)
		{
			_baseUrl = baseUrl;
			_apiKey = apiKey;
			_userName = userName;
			_password = password;
			_system = system;
			_authType = authType;
			_sessionToken = sessionToken;
		}

		#endregion

		#region internal methods

		protected internal virtual async Task<AOMClientResponse<string>> SendActualRequestAsync(HttpMethod method, Uri url, byte[] postEntity,
			bool asOctetStream, IList<HttpStatusCode> expectedCodes, string eTag, string deltaSync)
		{
			return await SendActualRequestAsync (method, url, new ByteContainer(postEntity), asOctetStream, expectedCodes, eTag, deltaSync).ConfigureAwait(false);
		}

		protected internal virtual async Task<AOMClientResponse<string>> SendActualRequestAsync(HttpMethod method, Uri url, Stream postEntity,
			bool asOctetStream, IList<HttpStatusCode> expectedCodes, string eTag, string deltaSync)
		{
			return await SendActualRequestAsync (method, url, new StreamContainer(postEntity), asOctetStream, expectedCodes, eTag, deltaSync).ConfigureAwait(false);
		}

		/// <summary>
		/// Sends the actual request.
		/// </summary>
		/// <returns>The actual request.</returns>
		/// <param name="method">Method.</param>
		/// <param name="url">URL.</param>
		/// <param name="postEntity">Post entity.</param>
		/// <param name="asOctetStream">As octet stream.</param>
		/// <param name="expectedCodes">Expected codes.</param>
		/// <param name="returnedStatusCode">Returned status code.</param>
		/// <param name="eTag">ETAG.</param>
		/// <param name="deltaSync">Delta sync (unused for now).</param>
        protected internal virtual async Task<AOMClientResponse<string>> SendActualRequestAsync(HttpMethod method, Uri url, IDataContainer postEntity,
																									   bool asOctetStream, IList<HttpStatusCode> expectedCodes, string eTag, string deltaSync)
		{
			AOMClientResponse<string> clientResponse = null;
			string resultStr = null;
			/*use httpclient with automatic decompression*/
			using (var client = CreateHttpClient())
			{
				client.DefaultRequestHeaders.Clear();
				IList<HttpMethod> allowedMethods =
					new List<HttpMethod>() { HttpMethod.Get, HttpMethod.Post, HttpMethod.Put, HttpMethod.Delete };


				if (allowedMethods.Contains(method) == false)
				{
					throw new InvalidOperationException("method is not allowed");
				}
				HttpRequestMessage reqMessage = new HttpRequestMessage();
				Dictionary<string, string> standardHeaders = GetStandardHeaders(eTag, true, deltaSync);
				reqMessage.RequestUri = url;
				reqMessage.Method = method;
				foreach (KeyValuePair<string, string> entry in standardHeaders)
				{
					if (entry.Key.Equals("if-none-match") && !entry.Value.Contains("-gzip"))
					{
						reqMessage.Headers.TryAddWithoutValidation(entry.Key, "\"" + entry.Value + "\"");
					}
					else
					{
						reqMessage.Headers.Add(entry.Key, entry.Value);
					}

				}
				if (reqMessage.Headers.Contains(HttpRequestHeader.IfModifiedSince.ToString()) || reqMessage.Headers.Contains(HttpRequestHeader.IfNoneMatch.ToString()))
				{
					reqMessage.Headers.Add(HttpRequestHeader.AcceptEncoding.ToString(), "");
				}
				if (method == HttpMethod.Put)
				{
					reqMessage.Headers.Add(AOMHttpClient.AOM_HEADER_FULLUPDATE, "true");
				}
				List<HttpMethod> methodsWithContent = new List<HttpMethod>() { HttpMethod.Post, HttpMethod.Put };
                if (methodsWithContent.Contains(method) && postEntity != null)
				{
					if (asOctetStream)
					{
						reqMessage.Content = postEntity.ToHttpContent();
					}
					else
					{
						/* Cast to int and access of Size is OK, because in the non-octetStream case, it's not static data but just some JSON, so the length won't exceed Int32.MaxValue */
						string encString = Encoding.UTF8.GetString(await postEntity.ToByteArrayAsync().ConfigureAwait(false), 0, (int)postEntity.Size);
						reqMessage.Content = new StringContent(encString, Encoding.UTF8, "application/json");
					}
				}

				/* All headers are set, check them (if method is overwritten in subclassed AOMHttpClient) */
				CheckRequestHeaders(reqMessage);

				client.Timeout = TimeSpan.FromMilliseconds(this._clientTimeout);
				HttpResponseMessage response = null;
				try
				{
					response = await client.SendAsync(reqMessage).ConfigureAwait(false);
				}
				catch (ArgumentException e)
				{ // malformed uri
					throw new ApiomatRequestException(Status.WRONG_URI_SYNTAX, HttpStatusCode.OK, e);
				}
				catch (TaskCanceledException e)
				{ //Timeout
					Debug.WriteLine("Connection timeout" + e.ToString());
					throw new ApiomatRequestException(Status.REQUEST_TIMEOUT, HttpStatusCode.OK, e);
				}
				catch (HttpRequestException e)
				{ //other request failures
					Debug.WriteLine("HttpRequest failed" + e.ToString());
					throw new ApiomatRequestException(Status.IO_EXCEPTION, HttpStatusCode.OK, e);
				}
				catch (AggregateException e)
				{
					if (e.InnerException != null && e.InnerException.GetType ().GetTypeInfo ().IsAssignableFrom (typeof(TaskCanceledException).GetTypeInfo ()))
					{
						//Do something on timeout
						throw new ApiomatRequestException (Status.REQUEST_TIMEOUT, HttpStatusCode.OK, e);
					} else
					{
						throw e;
					}

				}
				catch (Exception e)
				{ //any other exception
					throw new ApiomatRequestException(Status.IO_EXCEPTION, HttpStatusCode.OK, e);
				}


				bool errorOccurred = false;
				var statusCode = response.StatusCode;
				if ((int)statusCode >= 400)
				{
					/* If error is 401 or 407, ErrorStream won't be set, so checking it for != null won't work later */
					errorOccurred = true;
				}
				clientResponse = new AOMClientResponse<string>();
				clientResponse.ReturnedStatusCode = (int)statusCode;

				if (expectedCodes != null && expectedCodes.Contains(statusCode))
				{

					// Get etag from header		
					if (response.Headers.ETag != null)
					{
						clientResponse.ETag = response.Headers.ETag.Tag;
					}

					// Result string for post methods is the location in header (HREF)
					if (HttpMethod.Post.Equals(method))
					{
						if (response.Headers.Location != null)
						{
							resultStr = response.Headers.Location.ToString ();
						}
					}
					else
					{
						resultStr = await response.Content.ReadAsStringAsync();
					}
				}
				else
				{
					/* Doesn't have to be an HTTP error, maybe just the statusCode whas not the one as expected.
				 * So only put the response body in the exception if an HTTP error occurred. */
					if (errorOccurred)
					{
						string errorStr = await response.Content.ReadAsStringAsync();
						throw new ApiomatRequestException((int)statusCode, (int)expectedCodes[0], "An error occurred during sending the request", errorStr);
					}
					else
					{
						throw new ApiomatRequestException(Status.GetStatusForCode((int)statusCode), expectedCodes[0]);
					}
				}

				if (response != null)
				{
					CheckResponseHeaders(response);
					/* Set delta deleted header value to atomic reference, so that callers can access it */
					if (deltaSync != null && response.Headers.Contains(AOM_HEADER_DELTADELETED))
					{
						string datSync = "";
						foreach (var x in response.Headers.GetValues(AOMHttpClient.AOM_HEADER_DELTADELETED))
						{
							datSync += x;
						}

						clientResponse.DeltaSync = datSync;
					}
				}

			}
			CheckResponseString(resultStr);
			clientResponse.ResponseObject = resultStr;
			return clientResponse;
		}

		/// <summary>
		/// Sends the actual resource request.
		/// </summary>
		/// <returns>The actual resource request.</returns>
		/// <param name="href">Href.</param>
		/// <param name="eTag">ETag.</param>
		protected internal virtual async Task<AOMClientResponse<byte[]>> SendActualResourceRequest(string href, string eTag)
		{
			AOMClientResponse<byte[]> clientResponse = null;
			using (var client = CreateHttpClient())
			{
				HttpResponseMessage response = null;
				try
				{
					HttpRequestMessage reqMessage = new HttpRequestMessage();

					Uri url = new Uri(href);
					reqMessage.RequestUri = url;
					if (this._userName != null && this._password != null)
					{
						reqMessage.Headers.Authorization = GetAuthHeader(this._authType);
					}
					reqMessage.Headers.Add(AOM_HEADER_APIKEY, this._apiKey);
					reqMessage.Headers.Add(AOM_HEADER_SDKVERION, User.SdkVersion);
					reqMessage.Headers.Add(AOM_HEADER_SYSTEM, this._system);

					if (string.IsNullOrWhiteSpace(eTag) == false)
					{
						reqMessage.Headers.IfNoneMatch.Add(new EntityTagHeaderValue(eTag));
					}
					reqMessage.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("identity")); //For AOM-2122

					/* All headers are set, check them (if method is overwritten in subclassed AOMHttpClient) */
					CheckRequestHeaders(reqMessage);
					client.Timeout = TimeSpan.FromMilliseconds(this._clientTimeout);

					/* executes the request */
					response = await client.SendAsync(reqMessage).ConfigureAwait(false);

					clientResponse = new AOMClientResponse<byte[]>(0, eTag, null, null, false);
					int statusCode = (int)response.StatusCode;

					clientResponse.ReturnedStatusCode = statusCode;
					if (response.IsSuccessStatusCode && response.Content != null)
					{
						clientResponse.ResponseObject = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
						if (response.Headers.ETag != null)
						{ // new etag will be set, if input-etag was not null (but maybe empty) 
							clientResponse.ETag = response.Headers.ETag.Tag;
						}
					}
					else if (((int)HttpStatusCode.NotModified == statusCode))
					{
						clientResponse.NotModified = true;
					}
					else
					{
						string errorStr = await response.Content.ReadAsStringAsync();
						throw new ApiomatRequestException(statusCode, HttpStatusCode.OK, "An error occurred during sending a resource request", errorStr);
					}
				}
				catch (ArgumentException e)
				{ // malformed uri
					throw new ApiomatRequestException(Status.WRONG_URI_SYNTAX, HttpStatusCode.OK, e);
				}
				catch (TaskCanceledException e)
				{ //Timeout
					Debug.WriteLine("Connection timeout" + e.ToString());
					throw e;//new ApiomatRequestException(Status.IO_EXCEPTION, HttpStatusCode.OK, e.Message);
				}
				catch (HttpRequestException e)
				{ //other request failures
					Debug.WriteLine("HttpRequest failed" + e.ToString());
					throw new ApiomatRequestException(Status.IO_EXCEPTION, HttpStatusCode.OK, e);
				}
				catch (AggregateException e)
				{
					if (e.InnerException != null && e.InnerException.GetType().GetTypeInfo().IsAssignableFrom(typeof(TaskCanceledException).GetTypeInfo()))
					{
						//Do something on timeout
						throw e.InnerException;
					}
				}
				catch (ApiomatRequestException e)
				{
					throw e;
				}
				catch (Exception e)
				{ //any other exception
					throw new ApiomatRequestException(Status.IO_EXCEPTION, HttpStatusCode.OK, e);
				}
				finally
				{
					if (response != null)
					{
						CheckResponseHeaders(response);
					}
				}
			}

			return clientResponse;
		}

		protected internal virtual async Task<JObject> SendActualTokenRequestAsync()
		{
			string appName = this._baseUrl.Substring(this._baseUrl.LastIndexOf("/") + 1);
			IList<KeyValuePair<string, string>> nameValuePairs = new List<KeyValuePair<string, string>>();

			nameValuePairs.Add(new KeyValuePair<string, string>("grant_type", "aom_user"));
			nameValuePairs.Add(new KeyValuePair<string, string>("client_id", appName));
			nameValuePairs.Add(new KeyValuePair<string, string>("client_secret", this._apiKey));
			nameValuePairs.Add(new KeyValuePair<string, string>("scope", "read write"));
			nameValuePairs.Add(new KeyValuePair<string, string>("username", this._userName));
			nameValuePairs.Add(new KeyValuePair<string, string>("password", this._password));
			nameValuePairs.Add(new KeyValuePair<string, string>("app", appName));
			nameValuePairs.Add(new KeyValuePair<string, string>("system", this._system));
			return await SendActualTokenRequestAsync(nameValuePairs).ConfigureAwait(false);
		}

		protected internal virtual async Task<JObject> SendActualTokenRequestAsync(string refreshToken)// throws ApiomatRequestException
		{
			string appName = this._baseUrl.Substring(this._baseUrl.LastIndexOf('/') + 1);
			IList<KeyValuePair<string, string>> nameValuePairs = new List<KeyValuePair<string, string>>();
			nameValuePairs.Add(new KeyValuePair<string, string>("grant_type", "refresh_token"));
			nameValuePairs.Add(new KeyValuePair<string, string>("client_id", appName));
			nameValuePairs.Add(new KeyValuePair<string, string>("client_secret", this._apiKey));
			nameValuePairs.Add(new KeyValuePair<string, string>("refresh_token", refreshToken));
			return await SendActualTokenRequestAsync(nameValuePairs).ConfigureAwait(false);
		}

		/// <summary>
		/// Returns a map with token info for either the user that this Datastore has been configured with or the refresh token
		/// </summary>
		/// <param name="nameValuePairs">A list of NameValuePairs to build the url encoded form parameters</param>
		/// <returns>A JsonObject with token info for either the user that this Datastore has been configured with or the refresh token</returns>
		/// <exception cref="ApiomatRequestException">When the request fails</exception>
		protected internal virtual async Task<JObject> SendActualTokenRequestAsync(IList<KeyValuePair<string, string>> nameValuePairs)
		{
			JObject resultJsonObject = null;
			string responseString = null;

			int expectedCode = (int)HttpStatusCode.OK;
			using (var client = CreateHttpClient())
			{
				var content = new FormUrlEncodedContent(nameValuePairs);
				HttpResponseMessage response = null;
				try
				{
					Uri url =
						new Uri(this._baseUrl.Substring(0, this._baseUrl.IndexOf("yambas") + 6) + "/oauth/token");
					HttpRequestMessage reqMessage = new HttpRequestMessage(HttpMethod.Post, url);
					reqMessage.Content = content;
					reqMessage.Headers.Add(AOM_HEADER_SYSTEM, this._system);
					/* All headers are set, check them (if method is overwritten in subclassed AOMHttpClient) */
					CheckRequestHeaders(reqMessage);

					/* executes the request */
					/* IOException is thrown for example when status code is 803 */

					response = await client.SendAsync(reqMessage).ConfigureAwait(false);
					int statusCode = (int)response.StatusCode;

					var stream = await response.Content.ReadAsStreamAsync();
					using (StreamReader streamReader = new StreamReader(stream))
					{
						/* creating a GZIPInputStream when status code is 304 not modified throws an ioexception */
						responseString = await streamReader.ReadToEndAsync().ConfigureAwait(false);
					}
					if (statusCode != expectedCode)
					{
						throw new ApiomatRequestException(statusCode, expectedCode, "An error occurred during sending the token request", responseString);
					}
				}
				catch (ArgumentException e)
				{ // malformed uri
					throw new ApiomatRequestException(Status.WRONG_URI_SYNTAX, HttpStatusCode.OK, e);
				}
				catch (ApiomatRequestException e)
				{
					throw e;
				}
				catch (Exception e)
				{ //any other exception
					throw new ApiomatRequestException(Status.IO_EXCEPTION, HttpStatusCode.OK, e);
				}
				finally
				{
					if (response != null)
					{
						CheckResponseHeaders(response);
					}
				}
			}

			CheckResponseString(responseString);

			/* Get tokens and date from json and put them into a map */
			resultJsonObject = JObject.Parse(responseString);
			return resultJsonObject;
		}

		#endregion

		#region Public methods

		/// <summary>
		/// Check if ApiOmat service is reachable
		/// The request will timeout after x ms or if connection cannot be established in y ms (values can be changed)
		/// </summary>
		/// <returns>true if service is available otherwise false</returns>
		public async virtual Task<bool> IsServiceAvailable()
		{
			bool isAvailable = false;
			try
			{
				Uri tmpUrl = new Uri(this._baseUrl);
				Uri url = new Uri(tmpUrl, "/yambas/rest");
				using (var client = new HttpClient())
				{
					client.Timeout = new TimeSpan(this._clientTimeout);

					HttpResponseMessage response = await client.GetAsync(url).ConfigureAwait(false);

					isAvailable = response.StatusCode == HttpStatusCode.OK;
					await response.Content.ReadAsStringAsync().ConfigureAwait(false);
				}
			}
			catch (Exception)
			{
				isAvailable = false;
			}
			return isAvailable;
		}


		#endregion

		#region Private methods

		private Dictionary<string, string> GetStandardHeaders(string eTag, bool useEncoding, string deltaSync)
		{
			Dictionary<string, string> headerMap = new Dictionary<string, string>();

			headerMap.Add("Accept", "application/json");
			headerMap.Add(AOM_HEADER_APIKEY, this._apiKey);
			headerMap.Add(AOM_HEADER_SDKVERION, User.SdkVersion);
			headerMap.Add(AOM_HEADER_SYSTEM, this._system);
			if (this._authType != Datastore.AuthType.GUEST)
			{
				headerMap.Add("Authorization", GetAuthHeader(this._authType).ToString());
			}
			if (useEncoding)
			{
				headerMap.Add(HttpRequestHeader.AcceptEncoding.ToString(), "gzip");
			}
			if (!string.IsNullOrWhiteSpace(eTag))
			{
				headerMap.Add("if-none-match", eTag);
			}
			if (!string.IsNullOrWhiteSpace(deltaSync))
			{
				deltaSync = deltaSync.Replace('\r', ' ');
				deltaSync = deltaSync.Replace('\n', ' ');
				headerMap.Add(AOM_HEADER_DELTA, deltaSync);
			}
			if (CustomHeaders != null && CustomHeaders.Count > 0)
			{
				foreach (string key in CustomHeaders.Keys)
				{
					headerMap.Add (key, CustomHeaders[key]);
				}
			}
			return headerMap;
		}

		private AuthenticationHeaderValue GetAuthHeader(Datastore.AuthType authType)
		{
			switch (authType)
			{
				case Datastore.AuthType.USERNAME_PASSWORD:
					if (string.IsNullOrEmpty(_userName) || string.IsNullOrEmpty(_password))
					{
						throw new NullReferenceException("userName or password is null or empty");
					}
					string credentials = _userName + ":" + _password;

					string encoded = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(credentials));
					return new AuthenticationHeaderValue("Basic", encoded);
				case Datastore.AuthType.OAUTH2_TOKEN:
					if (string.IsNullOrEmpty(_sessionToken))
					{
						throw new NullReferenceException("userName or password is null or empty");
					}
					return new AuthenticationHeaderValue("Bearer", _sessionToken);
				default:
					return null;
			}
		}

		private static string Base64Encode(string plainText)
		{
			var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
			return System.Convert.ToBase64String(plainTextBytes);
		}


		/* Override this if you need the info. */
		protected virtual void CheckRequestHeaders(HttpRequestMessage request)
		{
		}

		/* Override this if you need the info. */
		protected virtual void CheckResponseHeaders(HttpResponseMessage response)
		{
		}

		/* Override this if you need the info. */
		protected virtual void CheckResponseString(string responseString)
		{
		}

		/// <summary>
		/// Creates the http client with automatic decompression enabled for GZip and Deflate, adds a Cookiecontainer
		/// </summary>
		/// <returns>The http client.</returns>
		protected virtual HttpClient CreateHttpClient()
		{
			HttpClientHandler handler = new HttpClientHandler();
			/* check if method for setting Automatic Decompression is present, otherwise we will get a MissingMethodException */
			MethodInfo method = handler.GetType().GetRuntimeMethod("set_AutomaticDecompression", new Type[] { typeof(System.Net.DecompressionMethods)});

			/*
			 * if method was found, invoke it. Don't call it directly, because there seems to be an issue in Mono runtime
			 * triggering the MissingMethodException anyway
			 */
			if (method != null)
			{
				Object[] methodParams = new Object[] { System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate };
				method.Invoke(handler, methodParams);
			}
			handler.CookieContainer = cookieContainer;
			return new HttpClient(handler);
		}
		#endregion
	}
}

