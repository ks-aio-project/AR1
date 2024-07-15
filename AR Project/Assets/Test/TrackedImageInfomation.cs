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

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            //CheckForPlanes(trackedImage);
            if (trackedImage.trackingState != TrackingState.Tracking)
            {
                if (trackedImage.referenceImage.name == "room1")
                {
                    Vector3 offset = new Vector3(0, Camera.main.transform.position.y - 5, 5);

                    GameObject parent = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    parent.transform.localScale = Vector3.one * 0.01f;
                    parent.transform.position = Camera.main.transform.position + offset;
                    parent.AddComponent<ARAnchor>();

                    GameObject objtt = Instantiate(arObjectPrefab, Camera.main.transform.position + offset, Quaternion.identity);
                    objtt.transform.parent = parent.transform;

                    trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
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