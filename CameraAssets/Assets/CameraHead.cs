using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHead : MonoBehaviour {

	[System.NonSerialized]
	public int TargetedRotationAngle = 180;
	public int RotationSpeed = 4;
	public bool IsRotetable = false;
	[System.NonSerialized]
	public bool RotateOnlyOneDirection = false;
	private bool directionPositive = true;
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (IsRotetable) {
			Debug.Log (gameObject.transform.eulerAngles.y);
			Debug.Log (directionPositive);
			int yAngle = (int)gameObject.transform.eulerAngles.y;
			if (directionPositive) {
				if (yAngle != TargetedRotationAngle / 2 || RotateOnlyOneDirection) {
					gameObject.transform.Rotate (0, TargetedRotationAngle * Time.deltaTime / 10, 0);
				} else {
					directionPositive = false;
				}
			} else {
				if (yAngle !=  360 - TargetedRotationAngle / 2) {
					gameObject.transform.Rotate (0, TargetedRotationAngle * Time.deltaTime / -10, 0);
				} else {
					directionPositive = true;
				}
			}
		} 
	}
}
