using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GazeableButton : MonoBehaviour, IFocusable, IInputClickHandler
{

    private Vector3 _oldScale;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    public void OnFocusEnter()
    {
        _oldScale = this.transform.localScale;
        this.transform.localScale = this.transform.localScale * 1.2f;
    }

    public void OnFocusExit()
    {
        this.transform.localScale = _oldScale;
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        var lastTapLocation = FindObjectOfType<Menu>().LastTapLocation;
        OnPointSelected(lastTapLocation);
    }

    protected abstract void OnPointSelected(Vector3 point);
}
