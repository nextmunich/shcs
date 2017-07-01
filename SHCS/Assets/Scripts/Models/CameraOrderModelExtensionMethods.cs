using UnityEngine;
using System.Collections.Generic;

public static class CameraOrderModelExtensionMethods
{
    public static Dictionary<string, object> GetPostDictionary(this CameraOrderModel cameraOrder)
    {
        var data = new Dictionary<string, object>();
        data.Add("@type", string.Format("{0}${1}", BackendConstants.AppName, BackendConstants.DataModelCameraOrder));
        data.Add("cameraOrderItems", cameraOrder.CameraOrderItems);
        data.Add("client", cameraOrder.Client);
        return data;
    }
}
