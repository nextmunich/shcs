using System;
using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class PlaceOutdoorCamera : GazeableButton
{
    public GameObject OutdoorPrefab;
    

    protected override bool OnPointSelected(Vector3 point)
    {
        var camera = FindObjectOfType<Camera>();
        var cameraManager = FindObjectOfType<CameraManager>();
        
        var outdoorCamera = Instantiate(OutdoorPrefab, point, new Quaternion(), cameraManager.transform);
        var lightShafts = outdoorCamera.GetComponentInChildren<LightShafts>();
        lightShafts.m_Cameras = new Camera[] { camera };

        var vectorToIndoorCamera = camera.transform.position - outdoorCamera.transform.position;
        vectorToIndoorCamera.y = 0;

        var rotation = Quaternion.LookRotation(vectorToIndoorCamera, camera.transform.up);
        outdoorCamera.transform.rotation = rotation;

        var spotlight = outdoorCamera.GetComponentInChildren<Light>();
        spotlight.color = GetNextCameraSightColor();

        ProcessService.GetInstance().AddCameraToOrder(CameraModelType.Outdoor);

        return true;
    }
}
