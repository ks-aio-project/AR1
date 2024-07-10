using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;
using System.Collections.Generic;
using System;
using UnityEditor.Rendering;
using TMPro;

public class TrackedImageInfomation : MonoBehaviour
{
    [SerializeField]
    private ARPlaneManager planeManager;

    public ARTrackedImageManager trackedImageManager;
    public GameObject arObjectPrefab;

    [SerializeField]
    private TextMeshProUGUI tmp;

    [SerializeField]
    private GameObject[] spawnedObjectPrefabs;
    private Dictionary<string, GameObject> spawnedObjects;

    public Mesh cubeMesh;

    void OnEnable() => trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;

    void OnDisable() => trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;

    private void CheckForPlanes(ARTrackedImage trackedImage)
    {
        Vector3 imagePosition = trackedImage.transform.position;
        float detectionRadius = 1f;

        foreach (ARPlane plane in planeManager.trackables)
        {

            Debug.Log($"Plane detected near the tracked image '{trackedImage.referenceImage.name}' at position {plane.center}");

            if (trackedImage.trackingState != TrackingState.Tracking)
            {
                if (trackedImage.referenceImage.name == "room1")
                {
                    GameObject objtt = Instantiate(arObjectPrefab, trackedImage.transform.position, Quaternion.identity);
                    objtt.AddComponent<ARAnchor>();
                    //objtt.GetComponent<ParentTest>().SetPosAndRot(anchorTest.transform.position, Quaternion.identity);

                    tmp.text = $"KKS transform.position : {transform.position}" +
                        $"transform.rotation : {transform.rotation}";

                    Debug.Log($"KKS objtt NAME : {objtt.name}");

                }
            }
            //    if (Vector3.Distance(plane.center, imagePosition) < detectionRadius)
            //    {
            //        Debug.Log($"Plane detected near the tracked image '{trackedImage.referenceImage.name}' at position {plane.center}");

            //        if (trackedImage.trackingState != TrackingState.Tracking)
            //        {
            //            if (trackedImage.referenceImage.name == "room1")
            //            {
            //                GameObject objtt = Instantiate(arObjectPrefab, trackedImage.transform.position, Quaternion.Euler(new Vector3(270, 0, 0)));
            //                objtt.AddComponent<ARAnchor>();
            //                //objtt.GetComponent<ParentTest>().SetPosAndRot(anchorTest.transform.position, Quaternion.identity);

            //                tmp.text = $"KKS transform.position : {transform.position}" +
            //                    $"transform.rotation : {transform.rotation}";

            //                Debug.Log($"KKS objtt NAME : {objtt.name}");

            //            }
            //        }
            //    }
            //    else
            //    {
            //        Debug.Log($"KKS plane : {plane.center} / image : {imagePosition} / dis : {Vector3.Distance(plane.center, imagePosition)}");
            //    }
            }
        }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            //CheckForPlanes(trackedImage);
            if (trackedImage.trackingState != TrackingState.Tracking)
            {
                if (trackedImage.referenceImage.name == "room1")
                {
                    GetComponent<CallBackTest>().CreateObject(trackedImage.transform.position);
                    //Vector3 offset = new Vector3(0, Camera.main.transform.position.y - 5, 5);
                    //GameObject objtt = Instantiate(arObjectPrefab, Camera.main.transform.position + offset, Quaternion.Euler(new Vector3(0, 180, 0)));
                    //objtt.transform.localScale = Vector3.one * 0.4f;
                    //GameObject objtt = Instantiate(arObjectPrefab, Camera.main.transform.position + offset, Quaternion.Euler(new Vector3(-90, 0, 0)));
                    //objtt.AddComponent<ARAnchor>();
                    //objtt.GetComponent<ParentTest>().SetPosAndRot(anchorTest.transform.position, Quaternion.identity);

                    //tmp.text = $"KKS transform.position : {transform.position}" +
                    //    $"transform.rotation : {transform.rotation}";
                    //Debug.Log($"KKS objtt NAME : {objtt.name}");
                }
            }
        }

        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            return;
        }

        foreach (ARTrackedImage trackedImage in eventArgs.removed)
        {
            Debug.Log($"KKS Log eventArgs.removed");
        }
    }
}