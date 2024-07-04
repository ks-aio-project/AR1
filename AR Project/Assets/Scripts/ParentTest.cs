using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ParentTest : MonoBehaviour
{

    private void Start()
    {
        //transform.position = Camera.main.transform.position;
        Debug.Log($"Scale : {transform.localScale}");
    }

    void Update()
    {
        //    transform.SetParent(null);
        //    if (GetComponent<ARTrackedImage>())
        //    {
        //        Destroy(GetComponent<ARTrackedImage>());
        //    }

        //    Debug.Log($"position : {transform.position}");
    }
}
