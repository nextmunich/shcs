using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorableObject : MonoBehaviour {

	public float Red = 1;
	public float Green = 1;
	public float Blue = 1;
	public bool FixedColor = false;

	// Use this for initialization
	void Start () {
		GetComponent<Renderer> ().material.color = new Color (Red, Green, Blue);
	}
	
	// Update is called once per frame
	void Update () {
		if (!FixedColor) {
			GetComponent<Renderer> ().material.color = new Color (Red, Green, Blue);
		}
	}
}
