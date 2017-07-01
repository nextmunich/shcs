using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlaceIndoorCamera : GazeableButton
{
    public GameObject IndoorPrefab;
    

    protected override bool OnPointSelected(Vector3 point)
    {
        var camera = FindObjectOfType<Camera>();
        var cameraManager = FindObjectOfType<CameraManager>();

        var indoorCamera = Instantiate(IndoorPrefab, point, new Quaternion(), cameraManager.transform);
        var lightShafts = indoorCamera.GetComponentInChildren<LightShafts>();
        lightShafts.m_Cameras = new Camera[] { camera };

        var vectorToIndoorCamera = camera.transform.position - indoorCamera.transform.position;
        vectorToIndoorCamera.y = 0;

        var rotation = Quaternion.LookRotation(vectorToIndoorCamera, camera.transform.up);
        indoorCamera.transform.rotation = rotation;

        var spotlight = indoorCamera.GetComponentInChildren<Light>();
        spotlight.color = GetNextCameraSightColor();

        ProcessService.GetInstance().AddCameraToOrder(CameraModelType.Indoor);

        return true;
    }
}
