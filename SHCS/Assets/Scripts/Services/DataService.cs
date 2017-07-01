using UnityEngine;
using System.Collections.Generic;

public class DataService
{
    public IList<CameraModel> Cameras { get; set; }

    public IList<ClientModel> Clients { get; set; }

    public CameraOrderModel Order { get; set; }

    public DataService()
    {
        Cameras = new List<CameraModel>();
        Clients = new List<ClientModel>();
    }

    public CameraModel GetCameraForModel(CameraModelType cameraModel)
    {
        foreach (var camera in Cameras)
        {
            if (camera.CameraModelType == cameraModel)
            {
                return camera;
            }
        }

        return null;
    }
}
