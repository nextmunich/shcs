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
using System;
using Com.Apiomat.Frontend;

namespace Com.Apiomat.Frontend
{
	public class AOMHttpClientFactory
	{

		private static AOMHttpClient aomHttpClient;
		private static AOMHttpClient backup;
		public static void CreateHttpClient(string baseUrl, string apiKey, string userName, string password, string system, Datastore.AuthType authType, string sessionToken)
		{
			aomHttpClient = new AOMHttpClient(baseUrl, apiKey, userName, password, system, authType, sessionToken);
			backup = aomHttpClient;
		}

		public static AOMHttpClient GetAomHttpClient()
		{
			return aomHttpClient;
		}

		public static void SetAOMHttpClient(AOMHttpClient aomHttpClientReplacement)
		{
			aomHttpClient = aomHttpClientReplacement;
		}

		public static void RestoreAomHttpClient()
		{
			aomHttpClient = backup;
		}
	}
}

