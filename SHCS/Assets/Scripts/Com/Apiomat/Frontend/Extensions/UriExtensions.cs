using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Apiomat.Frontend.Extensions
{
	public static class UriExtensions
	{
		public static Uri AddParameterToUri(this Uri url, string paramName, string paramValue)
		{
			var uriBuilder = new UriBuilder(url);
			var qry = uriBuilder.Query;
			/* escape the input values*/
			paramName = Uri.EscapeDataString(paramName);
			paramValue = Uri.EscapeDataString(paramValue);
			if (qry.Contains("?" + paramName + "=") || qry.Contains("&" + paramName + "="))
			{ /* parameter is already contained -> replace it*/
				/*check whether first or other arg (to ensure that paramname is a paramname not value)*/
				var startIdx = qry.IndexOf("?" + paramName + "=");
				startIdx = startIdx == -1 ? qry.IndexOf("&" + paramName + "=") : startIdx;
				startIdx += 2 + paramName.Length; // find position of first char of the old value
				var endIdx = qry.IndexOf('&', startIdx); //find endposition of old value 
				var oldValue = endIdx < 0 ? qry.Substring(startIdx) : qry.Substring(startIdx, endIdx - startIdx);
				qry = qry.Replace(oldValue, paramValue);
			}
			else
			{//not already contained -> append it normally
				/* the question mark will be automatically added to query*/
				qry = string.IsNullOrWhiteSpace(qry) ? "" : qry + "&";
				qry += paramName + "=" + paramValue;
			}
			if (qry[0].Equals('?'))
			{
				qry = qry.Remove(0, 1);
			}
			uriBuilder.Query = qry;
			url = uriBuilder.Uri;
			return uriBuilder.Uri;
		}
	}
}
