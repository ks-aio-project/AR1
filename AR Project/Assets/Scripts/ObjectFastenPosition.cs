using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ObjectFastenPosition : MonoBehaviour
{
    Vector3 initPosition;

    private void Awake() {
        initPosition = Camera.main.transform.position;
    }

    private void Update() {
        transform.position = initPosition;
    }
}
