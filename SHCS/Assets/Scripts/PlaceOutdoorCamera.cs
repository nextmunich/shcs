using System;
using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class PlaceOutdoorCamera : GazeableButton {

    public GameObject OutdoorPrefab;

    protected override void OnPointSelected(Vector3 point)
    {
        Instantiate(OutdoorPrefab, point, new Quaternion());
    }
}
