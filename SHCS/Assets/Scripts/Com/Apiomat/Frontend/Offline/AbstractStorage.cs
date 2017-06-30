/* Copyright (c) 2011 - 2016, Apinauten GmbH
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without modification,
 * are permitted provided that the following conditions are met:
 * 
 * * Redistributions of source code must retain the above copyright notice, this
 * list of conditions and the following disclaimer.
 * * Redistributions in binary form must reproduce the above copyright notice,
 * this list of conditions and the following disclaimer in the documentation
 * and/or other materials provided with the distribution.
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
 * THIS FILE IS GENERATED AUTOMATICALLY. DON'T MODIFY IT. */
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Com.Apiomat.Frontend.External;

namespace Com.Apiomat.Frontend.Offline
{
    public abstract class AbstractStorage
    {
        #region Private data

        private static AOMConcurrentDictionary<string, string> eTagDict = new AOMConcurrentDictionary<string, string>();

        #endregion

        #region Constructors

        public AbstractStorage()
        {
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Images get saved with /images/<id>, but requested with /images/<id>.img, which doesn't match
        /// Note: other files also get requested with .img.
        /// </summary>
        /// <returns>The image.</returns>
        /// <param name="href">Href.</param>
        private string AddImg(string href)
        {
            if (href.Contains(".img"))
            {
                return href;
            }
            string result = href;
            string imgEnding = ".img";
            if (href.Contains("?"))
            {
                string firstPart = href.Substring(0, href.IndexOf("?"));
                string lastPart = href.Substring(href.IndexOf("?"));
                result = firstPart + imgEnding + lastPart;
            }
            else
            {
                result += imgEnding;
            }
            return result;
        }

        #endregion

        #region Internal methods

        // Protected + private methods ============================================

        /// <summary>
        ///  Note: Ignoring queries / not saving query responses is important, because otherwise if an object gets modified offline, it might be included in the wrong queries.
        ///  Considering queries for offline storage can only be used when there's a query engine in the SDK
        ///  Note: don't cut off params for images, because they contain crop infos that mustn't get lost when storing in cache / persistent
        /// </summary>
        /// <param name="href"></param>
        /// <returns>The href without URL parameters</returns>
        internal static string CutOffParameters(string href)
        {
            if (href.Contains("?") && !href.Contains("/data/images/"))
            {
                return href.Substring(0, href.IndexOf("?"));
            }
            return href;
        }
        #endregion

        #region Public methods

        public static string Hash(string message)
        {
            /* In case SHA-1 or MD5 can't be found, the messageDigest can be null */
            IDigest digest = GetNewHashAlgorithm();
            if (digest == null)
            {
                return message;
            }
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            digest.BlockUpdate(messageBytes, 0, messageBytes.Length);
            byte[] result = new byte[digest.GetDigestSize()];
            digest.DoFinal(result, 0);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in result)
            {
                sb.Append(string.Format("{0:X2}", b));
            }
            return sb.ToString();
        }

        public abstract bool StoreObject(string href, string json, HttpMethod httpMethod);

        public virtual bool StoreBinary(string href, byte[] binaryData, HttpMethod httpMethod)
        {
            /* SQLite supports blob, but to reuse the existing object table, we convert the bytearray to hex text */
            string binaryDataString = Convert.ToBase64String(binaryData);
            href = AddImg(href);
            return StoreObject(href, binaryDataString, httpMethod);
        }

        public abstract bool StoreCollection(string requestUrl, string json);

        public abstract string GetStoredObject(string href);

        public virtual byte[] GetStoredBinary(string href)
        {
            Debug.WriteLine("trying to get stored binary for href: " + href);
            href = AddImg(href);
            string binaryDataString = GetStoredObject(href);
            if (binaryDataString == null)
            {
                return null;
            }

            return Convert.FromBase64String(binaryDataString);
        }

        /// <summary>
        /// Returns the stored collection for the url. Returns null if nothing found.
        /// </summary>
        /// <param name="url">the url to get the collection for</param>
        /// <returns>The stored collection for the url. Null if nothing found.</returns>
        public abstract string GetStoredCollection(string url);

        internal abstract IList<CollectionStorageContainer> GetAllCollections ();

        /// <summary>
        /// Returns the timestamp (in ms) of the last update of the collection associated with the url
        /// </summary>
        /// <param name="url"></param>
        /// <returns>The timestamp (in ms) of the last update of the collection associated with the url, null if the collection wasn't found</returns>
        internal abstract long? LoadCollectionLastUpdate(string url);

        /// <summary>
        /// Removes the object's LastModified value from the ETag Cache, as well as all ETag values of the collections the object was part of.
		/// Override to remove from actual storage.
        /// </summary>
        /// <returns><c>false</c>to indicate that no actual object was deleted</returns>
        /// <param name="url">URL</param>
        public virtual bool RemoveObject(string url)
        {
            RemoveObjectLastModified (url);
            return false;
        }

        /// <summary>
        /// Removes the collection's ETag from the ETag Cache.
		/// Override to remove from actual storage.
        /// </summary>
        /// <returns>0 to indicate that no actual objects were deleted</returns>
        /// <param name="url">URL</param>
        public virtual int RemoveCollection(string url)
        {
            RemoveCollectionETag(url);
            return 0;
        }

		/// <summary>
		/// Removes the ETags of all objects of a class, as well as all ETag values of the collections the object was part of.
		/// Override to remove from actual storage.
		/// </summary>
		/// <returns>0 to indicate that no actual objects were deleted</returns>
		/// <param name="modelHref">Model HREF.</param>
		internal virtual int RemoveAllObjects (string modelHref)
		{
			List<string> matchingKeys = eTagDict.AsParallel ()
				.Select (kvPair => kvPair.Key.StartsWith (modelHref) ? kvPair.Key : null)
				.Where(key => key != null)
				.ToList ();
			foreach (string matchingKey in matchingKeys)
			{
				/* Removes the object LastModified as well as the ETags of collections that contain the object */
				RemoveObjectLastModified (matchingKey);
			}
			return 0;
		}

        /* Clearing actual storage is implemented in sub classes.
         * But it should always also lead to clearing the ETag storage.
         */
        public virtual int Clear()
        {
            ClearETagStorage();
            return Int32.MinValue;
        }

        // ETag

        public bool StoreObjectLastModified(string url, string lastModified)
        {
            string cutUrl = CutOffParameters(url);
            // We currently don't need to differentiate between object lastModified and collection ETag
            return StoreCollectionETag(cutUrl, lastModified);
        }

        public bool StoreCollectionETag(string url, string eTag)
        {
            eTagDict[url] = eTag;
            return true;
        }

        public string LoadObjectLastModified(string url)
        {
            string cutUrl = CutOffParameters(url);
            // We currently don't need to differentiate between object lastModified and collection ETag
            return LoadCollectionETag(cutUrl);
        }

        public string LoadCollectionETag(string url)
        {
            string outValue;
            if (eTagDict.TryGetValue(url, out  outValue))
            {
                return outValue;
            }
            return null;
        }

        public bool RemoveObjectLastModified(string url)
        {
            string cutUrl = CutOffParameters (url);
            /* An object could be part of several collections, so when the LastModified gets deleted
             * (which happens when the object gets deleted), the collections' ETag needs to be deleted as well.
             * -> Find out which collections contain the object
             */
			IList<CollectionStorageContainer> collectionList = GetAllCollections ();
			foreach (CollectionStorageContainer csc in collectionList)
			{
				bool matchingCollectionElementFound = JArray.Parse (csc.Body).AsParallel ()
					.Select (jToken => ((JValue)jToken).ToString ())
					.Any (dataModelHref => dataModelHref == cutUrl);
				if (matchingCollectionElementFound)
				{
					RemoveCollectionETag (csc.URL);
				}
			}
            /* Then remove the LastModified of the object itself */
            // We currently don't need to differentiate between object lastModified and collection ETag
            return RemoveCollectionETag(cutUrl);
        }

        public bool RemoveCollectionETag(string url)
        {
            if (eTagDict.ContainsKey (url) == false)
            {
                return false;
            } else
            {
                eTagDict.Remove (url);
                /* return false if deletion wasn't successful */
                return eTagDict.ContainsKey (url) == false;
            }
        }

        public void ClearETagStorage()
        {
            eTagDict.Clear();
        }

        public static IDigest GetNewHashAlgorithm()
        {
            IDigest digest = null;
            try
            {
                digest = new Sha1Digest();
            }
            catch (Exception)
            {
                try
                {
                    //there's no managed version of md5, try to get an unmanaged one
                    digest = new MD5Digest();
                }
                catch (Exception e1)
                {
                    Debug.WriteLine(e1.ToString());
                    /* Everywhere where the hashAlgo gets used, the messages won't be hashed! */
                    digest = null;
                }
            }
            return digest;
        }
        #endregion
    }
}

