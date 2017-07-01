using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SmartCameraObject : MonoBehaviour, IFocusable, IInputClickHandler {

	// Use this for initialization
	public float Red;
	public float Green;
	public float Blue;

	public bool AlreadyColored = false;

	public bool IsRotateable = false;

	public CameraHead Head;

	private int rotationAngle = 0;

    private bool _isFocused;
    private Vector3 _oldScale;


    public bool IsFocused
    {
        get
        {
            return _isFocused;
        }
    }


    void Start () {
		
	}

	// Update is called once per frame
	void Update () {
		Head.IsRotetable = IsRotateable;

		if (!AlreadyColored) {
			var objs = FindObjectsOfType<ColorableObject> ();
			foreach (var o in objs) {
				o.Red = Red;
				o.Green = Green;
				o.Blue = Blue;
				o.AlreadyColored = false;
				AlreadyColored = true;
			}
		}
	}

	public void SetRotationAngle(int degree){
		if (IsRotateable) {
			rotationAngle = degree;
		}
    }

    public void OnFocusEnter()
    {
        _isFocused = true;
        _oldScale = this.transform.localScale;
        this.transform.localScale = this.transform.localScale * 1.2f;
    }

    public void OnFocusExit()
    {
        this.transform.localScale = _oldScale;
        _isFocused = false;
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        Destroy(gameObject);
    }
}
