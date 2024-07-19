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

    [HideInInspector]
    public GameObject createdPrefab;
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
        if (trackedImage.referenceImage.name == "room1")
        {
            Vector3 offset = new Vector3(3f, -1.5f, 0.5f);
            //GameObject obj = Instantiate(arObjectPrefab[0]);

            Vector3 spawnPosition = trackedImage.transform.position;
            GameObject spawnedObject = Instantiate(arObjectPrefab[0]);
            spawnedObject.transform.position = spawnPosition + offset;
            spawnedObject.transform.Rotate(0f, 180f, 0f);

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
                case "btn 1":
                    createdPrefab.transform.Rotate(1f, 0, 0);
                    break;
                case "btn 2":
                    createdPrefab.transform.Rotate(-1f, 0, 0);
                    break;
                case "btn 3":
                    createdPrefab.transform.Rotate(0, 1, 0);
                    break;
                case "btn 4":
                    createdPrefab.transform.Rotate(0, -1, 0);
                    break;
                case "btn 5":
                    createdPrefab.transform.Rotate(0, 0, 1);
                    break;
                case "btn 6":
                    createdPrefab.transform.Rotate(0, 0, 0);
                    break;
            }
        }
        text.GetComponent<TextMeshProUGUI>().text = 
            $"Position\n" +
            $"X: {createdPrefab.transform.position.x}\n" +
            $"Y: {createdPrefab.transform.position.y}\n" +
            $"Z: {createdPrefab.transform.position.z}\n" +
            $"Rotation\n" +
            $"X: {createdPrefab.transform.rotation.x}\n" +
            $"Y: {createdPrefab.transform.rotation.y}\n" +
            $"Z: {createdPrefab.transform.rotation.z}";
    }
}
