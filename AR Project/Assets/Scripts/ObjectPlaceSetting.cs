using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ObjectPlaceSetting : MonoBehaviour
{
    public GameObject ceiling;
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    Vector3 newPosition = hit.point;
                    newPosition.y = ceiling.transform.position.y - 0.5f; // y√‡ ∞Ì¡§
                    transform.position = newPosition;
                }
            }
        }
    }
}
