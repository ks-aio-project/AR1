using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class TrackedImageInfomation1 : MonoBehaviour
{
    public ARTrackedImageManager trackedImageManager;
    public GameObject[] arObjectPrefab;

    private Dictionary<string, GameObject> spawnedObjects = new Dictionary<string, GameObject>();

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
            // 트래킹에서 사라진 이미지는 처리하지 않음 (이미지의 상태가 업데이트되는 동안 위치를 유지할 것이므로)
        }
    }

    void CreateOrUpdateARObject(ARTrackedImage trackedImage)
    {
        if(trackedImage.referenceImage.name == "room1")
        {
            Vector3 offset = new Vector3(0, -1f, 1.5f);
            GameObject obj = Instantiate(arObjectPrefab[0]);
            obj.transform.position = trackedImage.transform.position + offset;
        }
    }
}
