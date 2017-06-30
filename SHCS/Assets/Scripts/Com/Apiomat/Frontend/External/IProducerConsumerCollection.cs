// IConcurrentCollection.cs
//
// Copyright (c) 2008 Jérémie "Garuma" Laval
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
//
using System;
using System.Collections;
using System.Collections.Generic;

namespace Com.Apiomat.Frontend.External
{
	/// <summary>
	/// This Interface was taken from mosa-project <br/>
	/// see: https://github.com/mosa/Mono-Class-Libraries/blob/master/mcs/class/corlib/System.Collections.Concurrent/IProducerConsumerCollection.cs
	/// For license information, take a look at the LICENSE file contained in this folder
	/// (or see https://github.com/mosa/MOSA-Project/wiki/License )
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IProducerConsumerCollection<T> : IEnumerable<T>, ICollection, IEnumerable
	{
		bool TryAdd (T item);
		bool TryTake (out T item);
		T[] ToArray ();
		void CopyTo (T[] array, int index);
	}
}