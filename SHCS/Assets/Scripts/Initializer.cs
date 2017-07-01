using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initializer : MonoBehaviour
{
    IEnumerator Start()
    {
        yield return ProcessService.GetInstance().Sync(this);
    }
}
