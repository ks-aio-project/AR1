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

    public Vector3 trackedPosition = new();
    public Quaternion trackedRotation = new();
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
        Vector3 cameraForward = Camera.main.transform.forward; // ���� ī�޶� �ٶ󺸴� ����

        // ī�޶� �ٶ󺸴� ���� ����� ���� ã��
        float angleToX = Vector3.Angle(cameraForward, Vector3.right);
        float angleToY = Vector3.Angle(cameraForward, Vector3.up);
        float angleToZ = Vector3.Angle(cameraForward, Vector3.forward);

        // �̹��� Ʈ��ŷ��
        if (trackedImage.referenceImage.name == "room1")
        {
            // 0807
            if(trackedImage.trackingState == TrackingState.Tracking)
            {
                if (currentTrackingObjectName == "room1")
                    return;

                if (createdPrefab != null)
                {
                    //Destroy(createdPrefab);
                    createdPrefab.SetActive(false);
                }

                trackedPosition = trackedImage.transform.position;
                GetComponent<CreatePlaceObject>().testText.text = $"camera : {Camera.main.transform.position}\n" +
                    $"image : {trackedImage.transform.position}";
                Vector3 spawnPosition = trackedImage.transform.position + GlobalVariable.Instance.room_offset;

                GameObject spawnedObject = Instantiate(arObjectPrefab[0]);
                spawnedObject.transform.position = spawnPosition;
                spawnedObject.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);

                createdPrefab = spawnedObject;

                // 2���⵵ �κ�
                //placeListCanvas.SetActive(true);

                currentTrackingObjectName = "room1";
            }
            else if (trackedImage.referenceImage.name == "distribution_box" || trackedImage.referenceImage.name == "distribution_box1" || trackedImage.referenceImage.name == "distribution_box2")
            {
                StartCoroutineTrackingDelay();

                if (currentTrackingObjectName == "distribution_box")
                    return;

                if (createdPrefab != null)
                {
                    //Destroy(createdPrefab);
                    createdPrefab.SetActive(false);
                }

                //Vector3 spawnPosition = Camera.main.transform.position + GlobalVariable.Instance.distribution_box_offset;
                Vector3 spawnPosition = trackedImage.transform.position + GlobalVariable.Instance.distribution_box_offset;

                GameObject spawnedObject = Instantiate(arObjectPrefab[1]);
                //spawnedObject.transform.rotation = Quaternion.identity;
                spawnedObject.transform.position = spawnPosition;
                createdPrefab = spawnedObject;

                placeListCanvas.SetActive(false);

                currentTrackingObjectName = "distribution_box";
            }
        }
    }
}