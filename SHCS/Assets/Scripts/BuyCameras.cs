using System;
using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class BuyCameras : GazeableButton
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    protected override bool OnPointSelected(Vector3 point)
    {
        ProcessService.GetInstance().CompleteOrder(this, (success) =>
        {
            FindObjectOfType<CameraManager>().RemoveCameras();
        });

        return true;
    }
}
