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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Immutable;
using System.Collections;


namespace Com.Apiomat.Frontend.External
{
	/// <summary>
	/// Inspired by
	/// https://stackoverflow.com/questions/18367839/alternative-to-concurrentdictionary-for-portable-class-library
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	public class AOMConcurrentDictionary<TKey, TValue> :IEnumerable<KeyValuePair<TKey, TValue>>
	{
		private IImmutableDictionary<TKey, TValue> _cache = ImmutableDictionary.Create<TKey, TValue> ();

		/// <summary>
		/// Returns the default value of type U if the key does not exist in the dictionary
		/// </summary>
		public TValue GetOrDefault (TKey key, TValue defaultValue = default(TValue))
		{
			if (_cache.ContainsKey(key))
			{
				try
				{
					return this[key];
				}
				catch (KeyNotFoundException)
				{
					return defaultValue;
				}
			}
			else
			{
				return defaultValue;
			}
		}

		/// <summary>
		/// Returns an existing value U for key T, or creates a new instance of type U using the default constructor, 
		/// adds it to the dictionary and returns it.
		/// </summary>
		public TValue GetOrInsertNew (TKey key)
		{
			TValue value;

			if (_cache.TryGetValue (key, out value))
			{
				return value;
			}

			/* Value not found; create it */
			TValue newValue = default(TValue);
			AddOrUpdate(key, newValue);
			/* return it */
			return newValue;
		}

		public void AddOrUpdate (TKey key, TValue value)
		{
			bool updated = false;
			while (updated == false)
			{
				if(_cache.ContainsKey(key))
				{
					Remove(key);
				}
				IImmutableDictionary<TKey, TValue> oldCache = _cache;
				// Add the new value to the cache
				IImmutableDictionary<TKey, TValue> newCache = oldCache.Add(key, value);
				if (Interlocked.CompareExchange(ref _cache, newCache, oldCache) == oldCache)
				{	// Cache successfully written
					updated = true;
				}
				//try again if failed to write the new cache
			}
		}

		public TValue this [TKey key] {
			get {
				TValue result;
				if (_cache.TryGetValue (key, out result))
				{
					return result;
				}
				throw new KeyNotFoundException ("Key: " + key);
			}
			set {
				AddOrUpdate (key, value);
			}
		}

		public void Remove(TKey key)
		{
			bool updated = false;
			while (updated == false)
			{
				IImmutableDictionary<TKey, TValue> oldCache = _cache;
				// Add the new value to the cache
				IImmutableDictionary<TKey, TValue> newCache = oldCache.Remove(key);
				if (Interlocked.CompareExchange(ref _cache, newCache, oldCache) == oldCache)
				{	// Cache successfully written
					updated = true;
				}
				//try again if failed to write the new cache
			}
		}

		public bool ContainsKey(TKey key)
		{
			return _cache.ContainsKey(key);
		}

		public void Clear ()
		{
			
			_cache = _cache.Clear ();
		}
		

		public bool TryGetValue (TKey key, out TValue value)
		{
			return _cache.TryGetValue (key, out value);
		}

		public int Count
		{
			get { return _cache.Count; }
		}

		public IEnumerable<TKey> Keys
		{
			get { return _cache.Keys; }
		}

		public IEnumerable<TValue> Values
		{
			get { return _cache.Values; }
		}



		#region IEnumerable<KeyValuePair<TKey,TValue>> Members

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return _cache.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _cache.GetEnumerator();
		}

		#endregion
	}
}
