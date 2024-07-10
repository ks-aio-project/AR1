using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ParentTest : MonoBehaviour
{
    public Vector3 pos;
    public Quaternion rot;

    void Update()
    {
        if(transform.parent.name == "room Parent")
        {
            transform.position = transform.parent.position;
            Debug.Log($"Parent : {transform.parent}");
        }

        if (pos != null)
        {
            transform.SetPositionAndRotation(pos, rot);
        }
    }

    public void SetPosAndRot(Vector3 _pos, Quaternion _rot)
    {
        pos = _pos; rot = _rot;
    }
}
