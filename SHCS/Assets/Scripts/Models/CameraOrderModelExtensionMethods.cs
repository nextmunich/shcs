using UnityEngine;
using System.Collections.Generic;

public static class CameraOrderModelExtensionMethods
{
    public static Dictionary<string, object> GetPostDictionary(this CameraOrderModel cameraOrder, string appName)
    {
        var data = new Dictionary<string, object>();
        data.Add("@type", string.Format("{0}${1}", appName, cameraOrder.GetType().Name));
        data.Add("cameraOrderItems", cameraOrder.CameraOrderItems);
        data.Add("client", cameraOrder.Client);
        return data;
    }
}
