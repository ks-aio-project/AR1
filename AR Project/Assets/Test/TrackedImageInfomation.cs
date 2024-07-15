using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;

public class TrackedImageInfomation : MonoBehaviour
{
    public ARTrackedImageManager trackedImageManager;
    public GameObject arObjectPrefab;
    private GameObject spawnedObject;
    Vector3 initPosition;
    Quaternion initRotation;

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
                initPosition = trackedImage.transform.position;
                initRotation = trackedImage.transform.rotation;

                spawnedObject = Instantiate(arObjectPrefab, initPosition, initRotation);
            }
            else
            {
                spawnedObject.transform.position = initPosition;
                //spawnedObject.transform.rotation = trackedImage.transform.rotation;
            }
        }

        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            if (spawnedObject != null)
            {
                spawnedObject.transform.position = initPosition;
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