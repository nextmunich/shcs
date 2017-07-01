using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class CameraManager : MonoBehaviour {

    

	// Use this for initialization
	void Start () {
        
	}

    // Update is called once per frame
    void Update () {
		
	}

    
    public void RemoveCameras()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

    }
    
}
