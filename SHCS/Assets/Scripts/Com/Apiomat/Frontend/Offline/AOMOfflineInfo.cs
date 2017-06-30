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
using Com.Apiomat.Frontend.Helper;
using SQLite;
using System;
using System.Net.Http;

namespace Com.Apiomat.Frontend.Offline
{
	/// <summary>
	/// Summary description for Class1
	/// </summary>
	public class AOMOfflineInfo
	{
		HttpMethod _httpMethod;
		string _fileKey;
		long _timestamp;
		string _url;
		Type _type;
		string _localId;
		string _refName;
		bool _isImage;
		bool _usePersistentStorage;
		bool _isRef;

		public AOMOfflineInfo()
		{
		}

		public AOMOfflineInfo(HttpMethod method, string url, string fileKey, Type clazz, string localId, bool isImage, bool isRef, bool usePersistentStorage)
			: this(method, url, fileKey, clazz, localId, null, isImage, isRef, usePersistentStorage)
		{
		}

		public AOMOfflineInfo(HttpMethod httpMethod, string url, string fileKey, Type clazz, string localId,
			string refName, bool isImage, bool isRef, bool usePersistentStorage)
		{
			this._httpMethod = httpMethod;
			this._url = url;
			this._type = clazz;
			this._fileKey = fileKey;
			this._localId = localId;
			this._timestamp = DateTimeHelper.GetCurrentTimeMillis();
			this._refName = refName;
			this._isImage = isImage;
			this._isRef = isRef;
			this._usePersistentStorage = usePersistentStorage;
		}

		[Ignore]
		public HttpMethod Method { get { return _httpMethod; } set { _httpMethod = value; } }
		public string StrMethod { get { return _httpMethod.ToString(); } set { _httpMethod = GetMethodForString(value); } }
		[PrimaryKey]
		public string FileKey { get { return _fileKey; } set { _fileKey = value; } }
		[Indexed]
		public long Timestamp { get { return _timestamp; } set { _timestamp = value; } }
		public string Url { get { return _url; } set { _url = value; } }
		[Ignore]
		public Type Type { get { return _type; } set { _type = value; } }
		public string StrType { get { return _type == null ? null : _type.ToString(); } set { _type = value == null ? null :Type.GetType(value); } }
		public string LocalId { get { return _localId; } set { _localId = value; } }
		public string RefName { get { return _refName; } set { _refName = value; } }
		public bool IsImage { get { return _isImage; } set { _isImage = value; } }
		public bool IsRef { get { return _isRef; } set { _isRef = value; } }
		public bool UsePersistentStorage { get { return _usePersistentStorage; } set { _usePersistentStorage = value; } }

		public override string ToString()
		{
			return "AOMOfflineInfo: fileKey: " + FileKey + " - HTTP: " + Method + " " + Url;
		}

		private HttpMethod GetMethodForString(string httpMethod)
		{
			switch (httpMethod)
			{
				case "GET":
					return HttpMethod.Get;
				case "POST":
					return HttpMethod.Post;
				case "PUT":
					return HttpMethod.Put;
				case "DELETE":
					return HttpMethod.Delete;
				default:
					return null;
			}
		}
	}
}