using HoloToolkit.Unity.SpatialMapping;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR.WSA.Input;
using System;

public class Menu : MonoBehaviour {

    private static readonly TimeSpan OneSecond = new TimeSpan(0, 0, 1);

    private GestureRecognizer _gestureRecognizer;

    private Vector3 _lastTapLocation;

    private DateTime _lastMenuClose;


    public Vector3 LastTapLocation
    {
        get
        {
            return _lastTapLocation;
        }
    }

    private bool RenderingEnabled
    {
        set
        {
            var renderers = GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                renderer.enabled = value;
            }
        }
    }


	// Use this for initialization
	void Start () {
        RenderingEnabled = false;

        _gestureRecognizer = new GestureRecognizer();
        _gestureRecognizer.TappedEvent += OnTapEvent;
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
            _gestureRecognizer.TappedEvent -= OnTapEvent;
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


    public void CloseMenu()
    {
        _lastMenuClose = DateTime.UtcNow;
        RenderingEnabled = false;
    }




    private void OnTapEvent(InteractionSourceKind source, int tapCount, Ray headRay)
    {
        if (DateTime.UtcNow - _lastMenuClose < OneSecond)
        {
            return;
        }


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
            RenderingEnabled = true;

            _lastTapLocation = hitInfo.point;

            var camera = FindObjectOfType<Camera>();
            var cameraTransform = camera.transform;

            var vectorToTapLocation = camera.transform.position - _lastTapLocation;

            this.transform.position = _lastTapLocation + 0.3f * vectorToTapLocation.normalized;
            this.transform.LookAt(cameraTransform);
        }
    }
}
