using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MainIint : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(LoadAB.Instance.LoadUIName("load"));
    }
}
