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
        var camera = FindObjectOfType<Camera>();

        var indoorCamera = Instantiate(IndoorPrefab, point, new Quaternion());
        var lightShafts = indoorCamera.GetComponentInChildren<LightShafts>();
        lightShafts.m_Cameras = new Camera[] { camera };

        /*indoorCamera.transform.LookAt(camera.transform);*/
    }
}
