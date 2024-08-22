using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using TMPro;
using System.Collections;
using Unity.VisualScripting;

public class TrackedImageInfomation1 : MonoBehaviour
{
    public ARTrackedImageManager trackedImageManager;
    public GameObject[] arObjectPrefab;

    public GameObject placeListCanvas;

    [HideInInspector]
    public GameObject createdPrefab;

    [HideInInspector]
    public string currentTrackingObjectName;

    [HideInInspector]
    public string currentForward = "";

    bool delay;

    public Vector3 trackedPosition = new();
    public Quaternion trackedRotation = new();

    List<GameObject> objs = new();


    private void Start()
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.transform.localScale = new Vector3(0.1f, 0.5f, 0.5f);
        obj.name = "+x";
        obj.GetComponent<Renderer>().enabled = false;
        objs.Add(obj);

        obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.transform.localScale = new Vector3(0.1f, 0.5f, 0.5f);
        obj.name = "-x";
        obj.GetComponent<Renderer>().enabled = false;
        objs.Add(obj);

        obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.transform.localScale = new Vector3(0.5f, 0.1f, 0.5f);
        obj.name = "+y";
        obj.GetComponent<Renderer>().enabled = false;
        objs.Add(obj);

        obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.transform.localScale = new Vector3(0.5f, 0.1f, 0.5f);
        obj.name = "-y";
        obj.GetComponent<Renderer>().enabled = false;
        objs.Add(obj);

        obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.transform.localScale = new Vector3(0.5f, 0.5f, 0.1f);
        obj.name = "+z";
        obj.GetComponent<Renderer>().enabled = false;
        objs.Add(obj);

        obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.transform.localScale = new Vector3(0.5f, 0.5f, 0.1f);
        obj.name = "-z";
        obj.GetComponent<Renderer>().enabled = false;
        objs.Add(obj);

        Input.compass.enabled = true; // 나침반

        if (placeListCanvas == null)
        {
            Debug.LogError("placeCanvas 미 할당");
        }

        placeListCanvas.SetActive(false);
    }
    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            currentForward = hit.transform.name;
            Debug.Log($"currentForward : {currentForward}");
        }

        foreach (GameObject i in objs)
        {
            switch (i.name)
            {
                case "+x":
                    i.transform.position = Camera.main.transform.position + new Vector3(0.25f, 0, 0);
                    break;
                case "-x":
                    i.transform.position = Camera.main.transform.position + new Vector3(-0.25f, 0, 0);
                    break;
                case "+y":
                    i.transform.position = Camera.main.transform.position + new Vector3(0, 0.25f, 0);
                    break;
                case "-y":
                    i.transform.position = Camera.main.transform.position + new Vector3(0, -0.25f, 0);
                    break;
                case "+z":
                    i.transform.position = Camera.main.transform.position + new Vector3(0, 0, 0.25f);
                    break;
                case "-z":
                    i.transform.position = Camera.main.transform.position + new Vector3(0, 0, -0.25f);
                    break;
            }
        }
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
            // 새롭게 트래킹된 이미지에 대해 처리
            foreach (ARTrackedImage trackedImage in eventArgs.added)
            {
                CreateOrUpdateARObject(trackedImage);
            }

            // 업데이트된 이미지에 대해 처리
            foreach (ARTrackedImage trackedImage in eventArgs.updated)
            {
                CreateOrUpdateARObject(trackedImage);
            }

            // 제거된 이미지에 대해 처리
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

    void CurrentForwardRotation(string _forward, string image_name, GameObject obj)
    {
        if(image_name.Equals("room1"))
        {
            switch (_forward)
            {
                case "+x":
                    obj.transform.rotation = Quaternion.Euler(0, 180, 0);
                    //GetComponent<CreatePlaceObject>().testText.text = $"parent Rot : {obj.transform.parent.rotation}\n180";
                    break;
                case "-x":
                    obj.transform.rotation = Quaternion.Euler(0, -180, 0);
                    //GetComponent<CreatePlaceObject>().testText.text = $"parent Rot : {obj.transform.parent.rotation}\n-180";
                    break;
                case "+y":
                    obj.transform.rotation = Quaternion.Euler(0, 135, 0);
                    //GetComponent<CreatePlaceObject>().testText.text = $"parent Rot : {obj.transform.parent.rotation}\n135";
                    break;
                case "-y":
                    obj.transform.rotation = Quaternion.Euler(0, 90, 0);
                    //GetComponent<CreatePlaceObject>().testText.text = $"parent Rot : {obj.transform.parent.rotation}\n90";
                    break;
                case "+z":
                    obj.transform.rotation = Quaternion.Euler(0, 225, 0);
                    //GetComponent<CreatePlaceObject>().testText.text = $"parent Rot : {obj.transform.parent.rotation}\n225";
                    break;
                case "-z":
                    obj.transform.rotation = Quaternion.Euler(0, 270, 0);
                    //GetComponent<CreatePlaceObject>().testText.text = $"parent Rot : {obj.transform.parent.rotation}\n270";
                    break;
            }
        }
    }

    void CreateOrUpdateARObject(ARTrackedImage trackedImage)
    {
        Vector3 cameraForward = Camera.main.transform.forward; // 현재 카메라가 바라보는 방향

        // 이미지 트래킹시
        if (trackedImage.referenceImage.name == "room1")
        {
            // 0807
            if (trackedImage.trackingState == TrackingState.Tracking)
            {
                if (currentTrackingObjectName == "room1")
                {
                    return;
                }

                if (createdPrefab != null)
                {
                    Destroy(createdPrefab);
                }
                
                trackedPosition = trackedImage.transform.position;
                Vector3 spawnPosition = trackedImage.transform.position + GlobalVariable.Instance.room_offset;

                GameObject spawnedObject = Instantiate(arObjectPrefab[0]);
                spawnedObject.transform.position = spawnPosition;
                spawnedObject.transform.eulerAngles = new Vector3(0, Camera.main.transform.eulerAngles.y - Camera.main.transform.eulerAngles.y, 0);
                //CurrentForwardRotation(currentForward, "room1", spawnedObject);

                createdPrefab = spawnedObject;

                // 2차년도 부분
                //placeListCanvas.SetActive(true);

                currentTrackingObjectName = "room1";
                Debug.Log($"kks room1");
                Debug.Log($"kks spawn object rotation : {spawnedObject.transform.eulerAngles}");
                Debug.Log($"kks cameraRotation : {Camera.main.transform.eulerAngles}");
                GetComponent<CreatePlaceObject>().testText.text = $"room1\n" +
                    $"ROT : {spawnedObject.transform.eulerAngles}";
            }
            else if (trackedImage.referenceImage.name == "distribution_box" || trackedImage.referenceImage.name == "distribution_box1" || trackedImage.referenceImage.name == "distribution_box2")
            {
                StartCoroutineTrackingDelay();

                if (currentTrackingObjectName == "distribution_box")
                    return;

                if (createdPrefab != null)
                {
                    Destroy(createdPrefab);
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