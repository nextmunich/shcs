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
using Newtonsoft.Json.Linq;

namespace Com.Apiomat.Frontend.Helper
{
    public class JsonHelper
    {
        public JsonHelper()
        {
        }

        public static List<string> GetHrefListFromJsonArrayWithAomModels(string jsonArrayString)
        {
            return GetHrefListFromJsonArrayWithAomModels(JArray.Parse(jsonArrayString));
        }

        public static List<string> GetHrefListFromJsonArrayWithAomModels(JArray ja)
        {
            List<string> result = new List<string>();
            List<JObject> aomModelsAsJsonObject = GetJsonObjectListFromJsonArrayWithAomModels(ja);
            JToken hrefToken;
            foreach (JObject jo in aomModelsAsJsonObject)
            {
                if (jo.TryGetValue("href", out hrefToken))
                {
                    result.Add(hrefToken.ToString());
                }
            }
            return result;
        }

        public static List<JObject> GetJsonObjectListFromJsonArrayWithAomModels(JArray ja)
        {
            List<JObject> result = new List<JObject>();

            string currentObjectJson;
            JObject currentObject;
            for (int i = 0; i < ja.Count; i++)
            {
                currentObjectJson = ja[i].ToString();
                currentObject = JObject.Parse(currentObjectJson);
                result.Add(currentObject);
            }

            return result;
        }

        public static IDictionary<string, string> GetHrefLMDictionaryFromJsonArrayWithAomModels(JArray ja)
        {
            IDictionary<string, string> result = new Dictionary<string, string>();

            List<JObject> aomModelsAsJsonObject = GetJsonObjectListFromJsonArrayWithAomModels(ja);
            string currentActualHref;
            string currentId;
            JToken currActHrefToken;
            JToken lastModAtToken;
            foreach (JObject jo in aomModelsAsJsonObject)
            {

                jo.TryGetValue("href", out currActHrefToken);
                jo.TryGetValue("lastModifiedAt", out lastModAtToken);
                if (currActHrefToken != null && lastModAtToken != null)
                {
                    currentActualHref = currActHrefToken.ToString();
                    currentId = currentActualHref.Substring(currentActualHref.LastIndexOf("/") + 1);
                    result.Add(currentId, lastModAtToken.ToString());
                }
            }

            return result;
        }
        public static string ToStringFromJObjectList(IList<JObject> objList)
        {
            string output = "[";
            foreach (var obj in objList)
            {
                output += obj.ToString() + ",";
            }
            output = output.Remove(output.Length - 1);
            output += "]";
            return output;
        }

        public static bool StringJsonArrayContains(JArray ja, string str)
        {
            for (int i = 0; i < ja.Count; i++)
            {
                if (str.Equals(ja[i].ToString()))
                {
                    return true;
                }
            }
            return false;
        }
    }
}

