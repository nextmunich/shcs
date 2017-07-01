using System;
using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class PlaceOutdoorCamera : GazeableButton
{
    public GameObject OutdoorPrefab;

    protected override void OnPointSelected(Vector3 point)
    {
        var camera = FindObjectOfType<Camera>();

        var outdoorCamera = Instantiate(OutdoorPrefab, point, new Quaternion());
        var lightShafts = outdoorCamera.GetComponentInChildren<LightShafts>();
        lightShafts.m_Cameras = new Camera[] { camera };

        /*outdoorCamera.transform.LookAt(camera.transform);
        outdoorCamera.transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);*/

        ProcessService.GetInstance().AddCameraToOrder(CameraModelType.Outdoor);
    }
}
