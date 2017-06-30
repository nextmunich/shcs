using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class CameraManager : MonoBehaviour {

    

	// Use this for initialization
	void Start () {
        GazeManager.Instance.FocusedObjectChanged += GazeManagerInstance_FocusedObjectChanged;
	}

    // Update is called once per frame
    void Update () {
		
	}

    
    
    private void GazeManagerInstance_FocusedObjectChanged(GameObject previousObject, GameObject newObject)
    {
        var indoorCamera = newObject.GetComponent<IndoorCamera>();
        if (indoorCamera != null && !indoorCamera.GetComponent<Rigidbody>())
        {
            var rigidBody = indoorCamera.gameObject.AddComponent<Rigidbody>();
            rigidBody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }
    }
}
