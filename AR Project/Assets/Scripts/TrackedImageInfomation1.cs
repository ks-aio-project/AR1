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
        // ���Ӱ� Ʈ��ŷ�� �̹����� ���� ó��
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            CreateOrUpdateARObject(trackedImage);
        }

        // ������Ʈ�� �̹����� ���� ó��
        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
        }

        // ���ŵ� �̹����� ���� ó��
        foreach (ARTrackedImage trackedImage in eventArgs.removed)
        {
            // Ʈ��ŷ���� ����� �̹����� ó������ ���� (�̹����� ���°� ������Ʈ�Ǵ� ���� ��ġ�� ������ ���̹Ƿ�)
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
