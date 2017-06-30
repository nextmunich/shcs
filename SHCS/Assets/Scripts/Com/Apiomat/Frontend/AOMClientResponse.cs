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

namespace Com.Apiomat.Frontend
{
	public class AOMClientResponse<T>
	{
		private int _returnedStatusCode;
		private string _eTag;
		private string _deltaSync;
		private bool _notModified;
		private T _responseObj;

		public AOMClientResponse()
		{
		}

		public AOMClientResponse(int returnedStatusCode, string eTag, string deltaSync, T responseObj, bool notModified)
		{
			this._returnedStatusCode = returnedStatusCode;
			this._eTag = eTag;
			this._deltaSync = deltaSync;
			this._responseObj = responseObj;
			this._notModified = notModified;
		}

		public int ReturnedStatusCode { get { return _returnedStatusCode; } set { _returnedStatusCode = value; } }

		public string ETag { get { return _eTag; } set { _eTag = value; } }

		public string DeltaSync { get { return _deltaSync; } set { _deltaSync = value; } }

		public bool NotModified { get { return _notModified; } set { _notModified = value; } }

		public T ResponseObject { get { return _responseObj; } set { _responseObj = value; } }
	}
}

