using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ObjectFastenPosition : MonoBehaviour
{
    public GameObject obj;

    public void onClick() {
        obj.SetActive(true);
        obj.transform.position = Camera.main.transform.position;
    }
}
