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
using System.Collections.Generic;

namespace Com.Apiomat.Frontend.Helper
{
    public static class ModelHelper
    {
        /// <summary>
        /// Method returns true if the given list already contains the HREF
        /// </summary>
        /// <returns><c>true</c> if href contained, <c>false</c> otherwise.</returns>
        /// <param name="list">List to search through</param>
        /// <param name="href">HREF for which we search</param>
        public static bool ContainsHref<T>(IList<T> list, string href) where T : AbstractClientDataModel
        {
            bool containsHref = false;
            foreach (AbstractClientDataModel model in list)
            {
                if (model.Href.Equals(href))
                {
                    containsHref = true;
                    break;
                }
            }
            return containsHref;
        }


        public static string GetIdFromModel(AbstractClientDataModel model)
        {
            string href = model.Href;
            return href.Substring(href.LastIndexOf("/") + 1);
        }

        /// <summary>
        /// Returns the index of an object in a list if it's contained in that list
        /// </summary>
        /// <typeparam name="T">A class extending AbstractClientDataModel</typeparam>
        /// <param name="list">the list that's supposed to contain the object</param>
        /// <param name="href">The href of the object that's supposed to be in the list</param>
        /// <returns>the index of the object in the list, or -1 if the object is not in the list</returns>
        public static int ObjectAtIndex<T>(IList<T> list, string href) where T : AbstractClientDataModel
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Href.Equals(href))
                {
                    return i;
                }
            }
            return -1;
        }
    }
}

