using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using TMPro;
using System.Collections;

public class TrackedImageInfomation1 : MonoBehaviour
{
    public ARTrackedImageManager trackedImageManager;
    public GameObject[] arObjectPrefab;

    public GameObject placeListCanvas;

    [HideInInspector]
    public GameObject createdPrefab;

    [HideInInspector]
    public string currentTrackingObjectName;

    bool delay;

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
        if(!delay)
        {
            // 새롭게 트래킹된 이미지에 대해 처리
            foreach (ARTrackedImage trackedImage in eventArgs.added)
            {
                CreateOrUpdateARObject(trackedImage);
            }

            // 업데이트된 이미지에 대해 처리
            foreach (ARTrackedImage trackedImage in eventArgs.updated)
            {
                CreateOrUpdateARObject(trackedImage);
            }

            // 제거된 이미지에 대해 처리
            foreach (ARTrackedImage trackedImage in eventArgs.removed)
            {
            }
        }
    }

    IEnumerator TrackingDelay()
    {
        yield return new WaitForSeconds(1f);

        delay = false;
    }

    void StartCoroutineTrackingDelay()
    {
        delay = true;
        StartCoroutine(TrackingDelay());
    }

    void CreateOrUpdateARObject(ARTrackedImage trackedImage)
    {
        // 이미지 트래킹시
        if (trackedImage.referenceImage.name == "room1")
        {
            StartCoroutineTrackingDelay();

            if (currentTrackingObjectName == "room1")
                return;

            if (createdPrefab != null)
            {
                //Destroy(createdPrefab);
                createdPrefab.SetActive(false);
            }

            Vector3 spawnPosition = Camera.main.transform.position + GlobalVariable.Instance.room_offset;

            GameObject spawnedObject = Instantiate(arObjectPrefab[0]);
            spawnedObject.transform.position = spawnPosition;
            spawnedObject.transform.Rotate(0f, 180f, 0f);
            createdPrefab = spawnedObject;

            // 2차년도 부분
            //placeListCanvas.SetActive(true);

            currentTrackingObjectName = "room1";
        }
        else if(trackedImage.referenceImage.name == "distribution_box")
        {
            StartCoroutineTrackingDelay();

            if (currentTrackingObjectName == "distribution_box")
                return;

            if (createdPrefab != null)
            {
                //Destroy(createdPrefab);
                createdPrefab.SetActive(false);
            }

            Vector3 spawnPosition = Camera.main.transform.position + GlobalVariable.Instance.distribution_box_offset;

            GameObject spawnedObject = Instantiate(arObjectPrefab[1]);
            spawnedObject.transform.position = spawnPosition;
            createdPrefab = spawnedObject;

            placeListCanvas.SetActive(false);

            currentTrackingObjectName = "distribution_box";
        }
    }
}