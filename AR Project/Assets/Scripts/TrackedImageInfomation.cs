using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;

public class TrackedImageInfomation : MonoBehaviour
{
    public ARTrackedImageManager trackedImageManager;
    public GameObject arObjectPrefab;
    private GameObject spawnedObject;

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
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            if (spawnedObject == null)
            {
                spawnedObject = Instantiate(arObjectPrefab, trackedImage.transform.position, trackedImage.transform.rotation);
            }
            else
            {
                spawnedObject.transform.position = trackedImage.transform.position;
                spawnedObject.transform.rotation = trackedImage.transform.rotation;
            }
        }

        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            if (spawnedObject != null)
            {
                spawnedObject.transform.position = trackedImage.transform.position;
                spawnedObject.transform.rotation = trackedImage.transform.rotation;
            }
        }

        foreach (ARTrackedImage trackedImage in eventArgs.removed)
        {
            if (spawnedObject != null)
            {
                Destroy(spawnedObject);
            }
        }
    }
}