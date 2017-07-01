using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GazeableButton : MonoBehaviour, IFocusable, IInputClickHandler
{

    private static int _nextColorIndex;
    private static readonly Color[] _colors = new Color[] { Color.blue, Color.red, Color.green, Color.cyan, Color.magenta };

    private Vector3 _oldScale;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    protected static Color GetNextCameraSightColor()
    {
        return _colors[_nextColorIndex++ % _colors.Length];
    }

    protected void CloseMenu()
    {
        var menu = FindObjectOfType<Menu>();
        menu.CloseMenu();
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
        if (OnPointSelected(lastTapLocation))
        {
            CloseMenu();
        }
    }

    protected abstract bool OnPointSelected(Vector3 point);
}
