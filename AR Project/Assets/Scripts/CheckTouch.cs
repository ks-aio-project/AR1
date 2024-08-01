using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using static UnityEngine.GraphicsBuffer;

public class CheckTouch : MonoBehaviour
{
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
                    if (hit.collider != null && hit.collider.CompareTag("Touchable"))
                    {
                        if(hit.collider.GetComponent<TextShow>())
                        {
                            hit.collider.GetComponent<TextShow>().SetVisible();
                        }
                    }
                }

                //if (raycastManager.Raycast(Input.GetTouch(0).position, hitList, TrackableType.PlaneWithinPolygon))
                //{
                //    // 플레인 터치
                //    Debug.Log($"KKS Touch Ray Plane");
                //    var hitPose = hitList[0].pose;

                //    Vector3 spawnPosition = hit.point;

                //    GetComponent<ObjectsController>().CreateOrDestroy("room1", hitPose);
                //}
            }
        }
    }
}