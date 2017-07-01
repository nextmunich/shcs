using HoloToolkit.Unity.SpatialMapping;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR.WSA.Input;

public class Menu : MonoBehaviour {

    private GestureRecognizer _gestureRecognizer;

    private Vector3 _lastTapLocation;


    public Vector3 LastTapLocation
    {
        get
        {
            return _lastTapLocation;
        }
    }


	// Use this for initialization
	void Start () {
        _gestureRecognizer = new GestureRecognizer();
        _gestureRecognizer.TappedEvent += OnTappedEvent;
        _gestureRecognizer.SetRecognizableGestures(GestureSettings.Tap);

        StartGestureRecognizer();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnDestroy()
    {
        StopGestureRecognizer();
        if (_gestureRecognizer != null)
        {
            _gestureRecognizer.TappedEvent -= OnTappedEvent;
            _gestureRecognizer.Dispose();
        }
    }




    /// <summary>
    /// Make sure the gesture recognizer is off, then start it.
    /// Otherwise, leave it alone because it's already in the desired state.
    /// </summary>
    public void StartGestureRecognizer()
    {
        if (_gestureRecognizer != null && !_gestureRecognizer.IsCapturingGestures())
        {
            _gestureRecognizer.StartCapturingGestures();
        }
    }

    /// <summary>
    /// Make sure the gesture recognizer is on, then stop it.
    /// Otherwise, leave it alone because it's already in the desired state.
    /// </summary>
    public void StopGestureRecognizer()
    {
        if (_gestureRecognizer != null && _gestureRecognizer.IsCapturingGestures())
        {
            _gestureRecognizer.StopCapturingGestures();
        }
    }


    private void OnTappedEvent(InteractionSourceKind source, int tapCount, Ray headRay)
    {
        var isAnyCameraFocused = false;
        var cameras = FindObjectsOfType<SmartCameraObject>();
        foreach (var camera in cameras)
        {
            if (camera.IsFocused)
            {
                isAnyCameraFocused = true;
                break;
            }
        }

        RaycastHit hitInfo;
        if (!isAnyCameraFocused && Physics.Raycast(headRay.origin, headRay.direction, out hitInfo, 30.0f, SpatialMappingManager.Instance.LayerMask))
        {
            _lastTapLocation = hitInfo.point;

            var camera = FindObjectOfType<Camera>();
            var cameraTransform = camera.transform;

            this.transform.position = _lastTapLocation;
            this.transform.LookAt(cameraTransform);
        }
    }
}
