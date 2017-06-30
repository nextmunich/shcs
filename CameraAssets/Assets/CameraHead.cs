using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHead : MonoBehaviour
{

    [System.NonSerialized]
    public int TargetedRotationAngle = 180;
    public int RotationSpeed = 4;
    public bool IsRotetable = false;
    public bool RotateAroundZAxis = false;

    [System.NonSerialized]
    public bool RotateOnlyOneDirection = false;
    private bool directionPositive = true;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (IsRotetable)
        {
            int angle = (int)GetRotationAngle();

            if (directionPositive)
            {
                if (angle != TargetedRotationAngle / 2 || RotateOnlyOneDirection)
                {
                    SetRotationAngle(TargetedRotationAngle * Time.deltaTime / 10);
                }
                else
                {
                    directionPositive = false;
                }
            }
            else
            {
                if (angle != 360 - TargetedRotationAngle / 2)
                {
                    SetRotationAngle(TargetedRotationAngle * Time.deltaTime / -10);
                }
                else
                {
                    directionPositive = true;
                }
            }
        }
    }

    private float GetRotationAngle()
    {
        if (RotateAroundZAxis)
        {
            Debug.Log(gameObject.transform.eulerAngles.z);
        }
        else
        {
            Debug.Log(gameObject.transform.eulerAngles.y);
        }
        Debug.Log(directionPositive);

        if (RotateAroundZAxis)
        {
            return gameObject.transform.eulerAngles.z;
        }
        else
        {
            return gameObject.transform.eulerAngles.y;
        }
    }

    private void SetRotationAngle(float angle)
    {
        if (RotateAroundZAxis)
        {
            gameObject.transform.Rotate(0, 0, angle);
        }
        else
        {
            gameObject.transform.Rotate(0, angle, 0);
        }
    }
}
