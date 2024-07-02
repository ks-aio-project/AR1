using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;

public class TrackedImageInfomation : MonoBehaviour
{
    public ARTrackedImageManager trackedImageManager;
    public GameObject objectToPlace; // 생성할 오브젝트

    private bool objectPlaced = false;

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
        foreach (var trackedImage in eventArgs.added)
        {
            if (!objectPlaced && trackedImage.trackingState == TrackingState.Tracking)
            {
                // 트래킹된 이미지의 위치와 방향에 오브젝트 생성
                Instantiate(objectToPlace, trackedImage.transform.position, trackedImage.transform.rotation);
                objectPlaced = true; // 한 번만 생성하도록 플래그 설정
            }
        }
    }
}