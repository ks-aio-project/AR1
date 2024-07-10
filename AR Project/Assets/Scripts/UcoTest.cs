using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UcoTest : MonoBehaviour
{
    Vector3 oldPosition;
    Vector3 newPosition;

    private void Start()
    {
        oldPosition = transform.position;
    }

    void Update()
    {
        if (oldPosition != transform.position)
        {
            newPosition = oldPosition;
        }

        if(newPosition != null)
        {

        }
    }
}
