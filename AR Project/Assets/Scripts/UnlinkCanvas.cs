using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlinkCanvas : MonoBehaviour
{
    Transform parent;

    readonly Vector3 offset = new Vector3(0, 0.25f, 0);

    void Start()
    {
        parent = transform.parent;

        transform.parent = null;
    }

    void Update()
    {
        if (parent == null)
        {
            Destroy(gameObject);
        }

        transform.position = parent.transform.position + offset;
       }
}