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

using Newtonsoft.Json.Linq;

namespace Com.Apiomat.Frontend.Extensions
{
	public static class JsonExtensions
	{
		/// <summary>
		/// Gets the value from the JObject that's associated with the key and returns the deserialized object.
		/// If the found value is of a different type than the type given in the type parameter, the given or determined default value will be returned.
		/// </summary>
		/// <returns>The value if it was found, otherwise the given or determined default value of the type of the value. If the found value is null, it depends on the parameter defaultForNull.</returns>
		/// <param name="key">The key to look for in the JObject</param>
		/// <param name="defaultValue">Optional default value. If not set, the default value for the type gets used (e.g. 0 for int, null for string and other reference types). See http://msdn.microsoft.com/en-us/library/83fhsxwc.aspx for all value type default values.</param>
		/// <param name="defaultForNull">If set to true, the method will also return the given defaultValue if the value was found but is null (only applies when the generic type parameter is a reference type or Nullable). If set to false, null will be returned for reference types and Nullables, for value types the default value will be determined.</param>
		/// <typeparam name="T">The type of the value to get from the JObject</typeparam>
		public static T GetOrDefault<T>(this JObject jObject, string key, T defaultValue = default(T), bool defaultForNull = false)
		{
			JToken jt;
			if (jObject.TryGetValue(key, out jt) == false)
			{
				return defaultValue;
			}
			// The found value can be a JSON null. Don't deserialize in that case.
			// When not deserializing, use given defaultValue if requested by caller, or if not requested: determined default value, because that defaults to null for all reference types. For value types null can't be returned so the determined default value is the only value that makes sense.
			if (jt.Type == JTokenType.Null)
			{
				return defaultForNull ? defaultValue : default(T);
			}
			try
			{
				return jt.ToObject<T>();
			}
			catch (Exception) // If the found value can't be deserialized to T
			{
				return defaultValue;
			}
		}
	}
}

