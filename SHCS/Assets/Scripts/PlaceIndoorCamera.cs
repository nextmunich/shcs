using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlaceIndoorCamera : GazeableButton
{
    public GameObject IndoorPrefab;

    protected override void OnPointSelected(Vector3 point)
    {
        Instantiate(IndoorPrefab, point, new Quaternion());
        ProcessService.GetInstance().AddCameraToOrder(CameraModelType.Indoor);
    }
}
