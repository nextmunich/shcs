using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartCameraObject : MonoBehaviour {

	// Use this for initialization
	public float Red;
	public float Green;
	public float Blue;

	public bool AlreadyColored = false;

	public bool IsRotateable = false;

	public CameraHead Head;

	private int rotationAngle = 0;

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
}
