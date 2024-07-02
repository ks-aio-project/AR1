using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ObjectInitPosition : MonoBehaviour
{
    public GameObject room;

    private void Start() {
        var instance = Instantiate(room);
        instance.transform.position = Camera.main.transform.position;
        instance.transform.parent = null;
    }
}