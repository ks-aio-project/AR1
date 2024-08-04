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
            Debug.LogError("placeCanvas �� �Ҵ�");
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
            // ���Ӱ� Ʈ��ŷ�� �̹����� ���� ó��
            foreach (ARTrackedImage trackedImage in eventArgs.added)
            {
                CreateOrUpdateARObject(trackedImage);
            }

            // ������Ʈ�� �̹����� ���� ó��
            foreach (ARTrackedImage trackedImage in eventArgs.updated)
            {
                CreateOrUpdateARObject(trackedImage);
            }

            // ���ŵ� �̹����� ���� ó��
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
        // �̹��� Ʈ��ŷ��
        if (trackedImage.referenceImage.name == "room1")
        {
            StartCoroutineTrackingDelay();

            if (currentTrackingObjectName == "room1")
                return;

            if(createdPrefab != null)
            {
                //Destroy(createdPrefab);
                createdPrefab.SetActive(false);
            }

            Vector3 offset = new Vector3(2f, -1.25f, 0.5f);
            // ���� Vector3 offset = new Vector3(2f, -1.5f, 0.5f);
            Vector3 spawnPosition = trackedImage.transform.position;

            GameObject spawnedObject = Instantiate(arObjectPrefab[0]);
            // ���� spawnedObject.transform.position = spawnPosition + offset;
            spawnedObject.transform.position = Camera.main.transform.position + offset;
            spawnedObject.transform.Rotate(0f, 180f, 0f);
            createdPrefab = spawnedObject;

            placeListCanvas.SetActive(true);

            currentTrackingObjectName = "room1";
        }
        else if(trackedImage.referenceImage.name == "distribution_box")
        {
            StartCoroutineTrackingDelay();

            if (currentTrackingObjectName == "distribution_box")
                return;

            if(createdPrefab != null)
            {
                //Destroy(createdPrefab);
                createdPrefab.SetActive(false);
            }

            Vector3 offset = new Vector3(0f, 0f, -5f);
            Vector3 spawnPosition = Camera.main.transform.position + offset;

            GameObject spawnedObject = Instantiate(arObjectPrefab[1]);
            spawnedObject.transform.position = spawnPosition;
            //spawnedObject.transform.Rotate(0f, 0f, 0f);
            createdPrefab = spawnedObject;

            placeListCanvas.SetActive(false);

            currentTrackingObjectName = "distribution_box";
        }
    }
}
