using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ObjectInitPosition : MonoBehaviour
{
    public GameObject room1, room2, room3;

    public TextMeshProUGUI tmp;
    
    [SerializeField]
    private ARTrackedImageManager arTrackedImageManager;

    private void OnEnable()
    {
        if (arTrackedImageManager != null)
        {
            arTrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
        }
    }

    private void OnDisable()
    {
        if (arTrackedImageManager != null)
        {
            arTrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
        }
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            // 새로운 이미지가 추가되었을 때 처리할 로직
            Debug.Log($"New image added: {trackedImage.referenceImage.name}");

            room1.SetActive(true);
            room1.transform.position = Camera.main.transform.position;
        }

        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            // 기존 이미지가 업데이트되었을 때 처리할 로직
            Debug.Log($"Image updated: {trackedImage.referenceImage.name}");
        }

        foreach (ARTrackedImage trackedImage in eventArgs.removed)
        {
            // 이미지가 제거되었을 때 처리할 로직
            Debug.Log($"Image removed: {trackedImage.referenceImage.name}");
        }
    }
}