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

    [SerializeField]
    private ARAnchorManager anchorManager;

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

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            //CheckForPlanes(trackedImage);
            if (trackedImage.trackingState != TrackingState.Tracking)
            {
                if (trackedImage.referenceImage.name == "room1")
                {
                    if (planeManager.trackables.count > 0)
                    {
                        ARPlane closestPlane = GetClosestPlane(trackedImage.transform.position);
                        if (closestPlane != null)
                        {
                            Debug.Log($"Closest Plane: {closestPlane.trackableId}");
                            Vector3 offset = new Vector3(0, Camera.main.transform.position.y - 5, 5);

                            GameObject anchor = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            anchor.transform.localScale = Vector3.one * 0.01f;
                            anchor.transform.position = closestPlane.transform.position;

                            anchor.GetComponent<Renderer>().enabled = false;
                            anchor.AddComponent<ARAnchor>();

                            GameObject objtt = Instantiate(arObjectPrefab, Camera.main.transform.position + offset, Quaternion.identity);
                            objtt.transform.parent = anchor.transform;
                        }
                        else
                        {
                            return;
                        }
                    }
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

    ARPlane GetClosestPlane(Vector3 imagePosition)
    {
        ARPlane closestPlane = null;
        float closestDistance = float.MaxValue;

        foreach (ARPlane plane in planeManager.trackables)
        {
            float distance = Vector3.Distance(imagePosition, plane.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPlane = plane;
            }
        }

        return closestPlane;
    }
}