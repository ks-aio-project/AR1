using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class ObjectFastenPosition : MonoBehaviour
{
    Vector3 initPosition;

    void Start()
    {
        initPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = initPosition;
        transform.rotation = Quaternion.Euler(0,0,0);
    }
}
