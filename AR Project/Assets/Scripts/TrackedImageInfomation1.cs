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

    private Dictionary<string, GameObject> spawnedObjects = new Dictionary<string, GameObject>();

    GameObject createdPrefab;
    public GameObject text;

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
        if(trackedImage.referenceImage.name == "room1")
        {
            Vector3 offset = new Vector3(5f, -1f, -0.5f);
            //GameObject obj = Instantiate(arObjectPrefab[0]);

            Vector3 forwardDirection = Camera.main.transform.forward;
            Vector3 spawnPosition = trackedImage.transform.position;
            GameObject spawnedObject = Instantiate(arObjectPrefab[0]);
            spawnedObject.transform.position = spawnPosition;
            spawnedObject.transform.position -= spawnedObject.transform.GetChild(0).position;

            createdPrefab = spawnedObject;
        }
    }

    public void OnBtnClick(GameObject btn)
    {
        if (createdPrefab)
        {
            switch (btn.name)
            {
                case "Button":
                    createdPrefab.transform.Translate(-0.1f, 0, 0);
                    break;
                case "Button (1)":
                    createdPrefab.transform.Translate(0, -0.1f, 0);
                    break;
                case "Button (2)":
                    createdPrefab.transform.Translate(0, 0, -0.1f);
                    break;
                case "Button (3)":
                    createdPrefab.transform.Translate(0.1f, 0, 0);
                    break;
                case "Button (4)":
                    createdPrefab.transform.Translate(0, 0.1f, 0);
                    break;
                case "Button (5)":
                    createdPrefab.transform.Translate(0, 0, 0.1f);
                    break;
            }
        }
        text.GetComponent<TextMeshProUGUI>().text = 
            $"X: {createdPrefab.transform.position.x}\n" +
            $"Y: {createdPrefab.transform.position.y}\n" +
            $"Z: {createdPrefab.transform.position.z}";
    }
}
