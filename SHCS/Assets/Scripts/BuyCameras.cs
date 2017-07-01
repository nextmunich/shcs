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

    protected override void OnPointSelected(Vector3 point)
    {
        ProcessService.GetInstance().CompleteOrder(this, (success) =>
        {
            //TODO: Remove all cameras from scene
        });
    }
}
