using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class ProcessService
{
    private BackendService _backendService = new BackendService();
    private DataService _dataService = new DataService();

    private static ProcessService _instance = null;

    public static ProcessService GetInstance()
    {
        if (_instance == null)
        {
            _instance = new ProcessService();
        }
        return _instance;
    }

    public IEnumerator Sync(MonoBehaviour monoBehaviour)
    {
        yield return monoBehaviour.StartCoroutine(_backendService.FetchClients((clients) =>
        {
            if (clients == null)
            {
                clients = new List<ClientModel>();
            }

            _dataService.Clients = clients;
            Debug.LogError(string.Format("{0} clients fetched", clients.Count));
        }));

        yield return monoBehaviour.StartCoroutine(_backendService.FetchCameras((cameras) =>
        {
            if (cameras == null)
            {
                cameras = new List<CameraModel>();
            }

            _dataService.Cameras = cameras;
            Debug.LogError(string.Format("{0} cameras fetched", cameras.Count));
        }));
    }

    public ClientModel DefaultClient
    {
        get
        {
            if (_dataService.Clients.Count > 0)
            {
                return _dataService.Clients[0];
            }

            return null;
        }
    }

    public IList<CameraModel> Cameras
    {
        get
        {
            return _dataService.Cameras;
        }
    }

    public void StartNewOrder()
    {
        var order = new CameraOrderModel();
        order.Client = DefaultClient.Id;
        order.CameraOrderItems = new Dictionary<string, string>();
        _dataService.Order = order;
    }

    public void AddCameraToOrder(CameraModelType cameraModel)
    {
        if (_dataService.Order == null)
        {
            StartNewOrder();
        }

        var camera = _dataService.GetCameraForModel(cameraModel);

        if (camera == null)
        {
            Debug.LogError("Could not find camera for that model");
            return;
        }

        if (_dataService.Order.CameraOrderItems.ContainsKey(camera.Id))
        {
            var numberOfCameras = _dataService.Order.CameraOrderItems[camera.Id];
            var numberOfCamerasInteger = int.Parse(numberOfCameras);
            numberOfCameras = string.Format("{0}", numberOfCamerasInteger + 1);
        }
        else
        {
            _dataService.Order.CameraOrderItems.Add(camera.Id, string.Format("{0}", 1));
        }

        Debug.LogError("Camera added");
    }

    public void RemoveCameraFromOrder(CameraModelType cameraModel)
    {
        if (_dataService.Order == null)
        {
            Debug.LogError("Please start a new order first");
            return;

        }

        var camera = _dataService.GetCameraForModel(cameraModel);

        if (camera == null)
        {
            Debug.LogError("Could not find camera for that model");
            return;
        }

        if (_dataService.Order.CameraOrderItems.ContainsKey(camera.Id))
        {
            var numberOfCameras = _dataService.Order.CameraOrderItems[camera.Id];
            var numberOfCamerasInteger = int.Parse(numberOfCameras);
            if (numberOfCamerasInteger == 1)
            {
                _dataService.Order.CameraOrderItems.Remove(camera.Id);
            }
            else
            {
                numberOfCameras = string.Format("{0}", numberOfCamerasInteger - 1);
            }
        }
        else
        {
            Debug.LogError("Could not find any of those cameras in the order");
        }
    }

    public void CompleteOrder(Action<bool> action)
    {
        if (_dataService.Order != null)
        {
            _backendService.PostOrder(_dataService.Order, (success) =>
            {
                if (success)
                {
                    _dataService.Order = null;
                    Debug.LogError("Order placed");
                }
                else
                {
                    Debug.LogError("Error while placing order");
                }

                action(success);
            });
        }
        else
        {
            Debug.LogError("There is no uncompleted order");
            action(false);
        }
    }
}
