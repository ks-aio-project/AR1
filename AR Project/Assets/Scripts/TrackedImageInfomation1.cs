using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using TMPro;

public class TrackedImageInfomation1 : MonoBehaviour
{
    public ARTrackedImageManager trackedImageManager;
    public GameObject[] arObjectPrefab;

    public GameObject placeListCanvas;

    [HideInInspector]
    public GameObject createdPrefab;

    [HideInInspector]
    public string currentTrackingObjectName;

    private void Start()
    {
        if(placeListCanvas == null)
        {
            Debug.LogError("placeCanvas 미 할당");
        }

        placeListCanvas.SetActive(false);
    }

    void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        // 새롭게 트래킹된 이미지에 대해 처리
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            CreateOrUpdateARObject(trackedImage);
        }

        // 업데이트된 이미지에 대해 처리
        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
        }

        // 제거된 이미지에 대해 처리
        foreach (ARTrackedImage trackedImage in eventArgs.removed)
        {
        }
    }

    void CreateOrUpdateARObject(ARTrackedImage trackedImage)
    {
        // 이미지 트래킹시
        if (trackedImage.referenceImage.name == "room1")
        {
            if (currentTrackingObjectName == "room1")
                return;

            if(createdPrefab != null)
            {
                createdPrefab.SetActive(false);
            }

            Vector3 offset = new Vector3(2f, -1.5f, 0.5f);
            Vector3 spawnPosition = trackedImage.transform.position;

            GameObject spawnedObject = Instantiate(arObjectPrefab[0]);
            spawnedObject.transform.position = spawnPosition + offset;
            spawnedObject.transform.Rotate(0f, 180f, 0f);
            createdPrefab = spawnedObject;

            placeListCanvas.SetActive(true);
            
            currentTrackingObjectName = "room1";
        }
        else if(trackedImage.referenceImage.name == "distribution_box" || trackedImage.referenceImage.name == "distribution_box_mini")
        {
            if (currentTrackingObjectName == "distribution_box")
                return;

            if(createdPrefab != null)
            {
                createdPrefab.SetActive(false);
            }

            Vector3 offset = new Vector3(0f, 0f, 5f);
            Vector3 spawnPosition = trackedImage.transform.position + offset;

            GameObject spawnedObject = Instantiate(arObjectPrefab[1]);
            spawnedObject.transform.position = spawnPosition;
            //spawnedObject.transform.Rotate(0f, 0f, 0f);
            createdPrefab = spawnedObject;

            placeListCanvas.SetActive(false);

            currentTrackingObjectName = "distribution_box";
        }
    }
}
