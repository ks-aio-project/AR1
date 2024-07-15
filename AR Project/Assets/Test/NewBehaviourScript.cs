using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class NewBehaviourScript : MonoBehaviour
{
    public GameObject obj1, obj2, obj3;

    private void Update()
    {
        if (obj1.transform.childCount > 0)
        {
            Debug.Log($"Test Obj1 Child True : {obj1.transform.GetChild(0).transform.name}");
        }
        if (obj2.transform.childCount > 0)
        {
            Debug.Log($"Test Obj2 Child True : {obj2.transform.GetChild(0).transform.name}");

            if (obj2.transform.GetChild(0).childCount > 0)
            {
                Debug.Log($"Test Obj2 Child Child Ture : {obj2.transform.GetChild(0).GetChild(0).name}");
            }
        }
        if (obj3.transform.childCount > 2)
        {
            Debug.Log($"Test Obj3 Child True : {obj3.transform.GetChild(2).transform.name}");
        }
    }
}
