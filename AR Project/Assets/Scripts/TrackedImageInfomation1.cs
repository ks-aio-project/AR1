using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using TMPro;
using System.Collections;
using Unity.VisualScripting;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARCore;

public class TrackedImageInfomation1 : MonoBehaviour
{
    public XROrigin xrOrigin;

    public ARTrackedImageManager trackedImageManager;
    public GameObject[] arObjectPrefab;

    public GameObject placeListCanvas;
    public GameObject placeListHideButton;
    public GameObject placeListShowButton;

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

    float northAngle;

    private Quaternion initialCameraRotation;  // 디바이스의 초기 회전값을 저장할 변수
    private bool isCalibrated = false;  // 초기 방향이 설정되었는지 여부

    private void Start()
    {
        // 나침반 활성화
        Input.compass.enabled = true;

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


        // 디바이스의 초기 회전값을 저장
        initialCameraRotation = Camera.main.transform.rotation;

        if (placeListCanvas == null)
        {
            Debug.LogError("placeCanvas 미 할당");
        }
        //placeListCanvas.SetActive(false);
    }

    // 각도를 기준으로 팔방위(북, 북동, 동, 남동, 남, 남서, 서, 북서) 계산
    string GetDirectionFromAngle(float angle)
    {
        if (angle >= 337.5f || angle < 22.5f)
            return "북 (N)";
        else if (angle >= 22.5f && angle < 67.5f)
            return "북동 (NE)";
        else if (angle >= 67.5f && angle < 112.5f)
            return "동 (E)";
        else if (angle >= 112.5f && angle < 157.5f)
            return "남동 (SE)";
        else if (angle >= 157.5f && angle < 202.5f)
            return "남 (S)";
        else if (angle >= 202.5f && angle < 247.5f)
            return "남서 (SW)";
        else if (angle >= 247.5f && angle < 292.5f)
            return "서 (W)";
        else
            return "북서 (NW)";
    }

    private void Update()
    {
        // 북쪽으로부터의 각도 (0~360도)
        northAngle = Input.compass.trueHeading;

        // 화면에 북쪽 각도 출력
        //Debug.Log("KKS 북쪽 각도 : " + northAngle + "°");

        // 카메라의 현재 회전 각도에서 Y축(디바이스의 방향) 회전값을 가져옴
        float cameraYAngle = Camera.main.transform.eulerAngles.y;

        // +Z축이 바라보는 방향 (디바이스가 북쪽을 기준으로 얼마나 회전했는지)
        float deviceDirection = (northAngle + cameraYAngle) % 360;

        // 팔방위로 변환
        string direction = GetDirectionFromAngle(deviceDirection);
        //Debug.Log("Z축이 바라보는 방향: " + direction);

        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            currentForward = hit.transform.name;
            //Debug.Log($"kks init currentForward : {currentForward}");
            //Debug.Log($"kks init cameraRotation : {Camera.main.transform.eulerAngles}");
            //Debug.Log($"kks init cameraforward : {Camera.main.transform.forward}");
        }

        // 나침반이 활성화되고 초기 방향이 설정된 후 실행
        if (Input.compass.enabled && !isCalibrated)
        {
            // 초기 디바이스 회전값을 보정하여 설정
            initialCameraRotation = Camera.main.transform.rotation;
            isCalibrated = true;  // 보정 완료
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
            //if (SceneChangeSingleton.Instance.changeName != "room1")
            //{
            //    SceneChangeSingleton.Instance.sceneChangeAble = true;
            //}

            //if (SceneChangeSingleton.Instance.sceneChangeAble && SceneChangeSingleton.Instance.changeName != "room1")
            //{
            //    SceneChangeSingleton.Instance.sceneChangeAble = false;
            //    SceneChangeSingleton.Instance.changeName = "room1";
            //    SceneManager.LoadScene(0);
            //}
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

                Vector3 rotationAngles = Camera.main.transform.rotation.eulerAngles;
                Debug.Log($"kks camera Rotation X: {rotationAngles.x}°, Y: {rotationAngles.y}°, Z: {rotationAngles.z}°");

                trackedPosition = trackedImage.transform.position;
                //Vector3 spawnPosition = trackedImage.transform.position + GlobalVariable.Instance.room_offset;

                GameObject spawnedObject = Instantiate(arObjectPrefab[0]);

                Vector3 directionToB = trackedImage.transform.position - objs[0].transform.position;
                directionToB.Normalize();  // 방향 벡터를 정규화

                // A의 forward 벡터
                Vector3 forwardA = objs[0].transform.forward;

                // A의 forward 벡터와 A에서 B로 가는 벡터 사이의 각도 계산
                float angle = Vector3.Angle(forwardA, directionToB);

                // 각도를 구할 때 A의 오른쪽 벡터를 기준으로 교차 곱(Cross Product)을 사용하여 각도의 부호를 계산
                Vector3 cross = Vector3.Cross(forwardA, directionToB);
                if (cross.y < 0)
                {
                    angle = -angle;  // 음의 값이면 시계 방향으로 회전한 각도
                }

                if (angle < 0)
                {
                    angle += 360;
                }

                // 90으로 나누어서 가장 가까운 정수로 반올림하고 다시 90을 곱함
                float snappedAngle = Mathf.Round(angle / 90) * 90;

                // 만약 360도보다 큰 값이 나오면 360도로 한정
                if (snappedAngle >= 360)
                {
                    snappedAngle = 0;
                }

                float cameraAngle;

                if (rotationAngles.y < 0)
                {
                    cameraAngle = Mathf.Round(rotationAngles.y % -90);
                }
                else
                {
                    cameraAngle = Mathf.Round(rotationAngles.y % 90);
                }

                Debug.Log($"kks angle : {angle}");
                spawnedObject.transform.position = trackedImage.transform.position + GlobalVariable.Instance.room_offset;
                
                spawnedObject.transform.rotation = snappedAngle == 90 ? Quaternion.Euler(0f, snappedAngle, 0f) : Quaternion.Euler(0f, snappedAngle + 90, 0f);

                Debug.Log($"kks angle : {angle} / snappedAngle : {snappedAngle} / cameraAngle : {cameraAngle} / sum : {snappedAngle}");

                //switch (currentForward)
                //{
                //    case "+x":
                //        Debug.Log("kks forward: +x");
                //        spawnedObject.transform.position = Camera.main.transform.position + GlobalVariable.Instance.room_offset_x;
                //        spawnedObject.transform.position = trackedImage.transform.position + GlobalVariable.Instance.room_offset;
                //        spawnedObject.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                //        break;
                //    case "-x":
                //        Debug.Log("kks forward: -x");
                //        spawnedObject.transform.position = Camera.main.transform.position + GlobalVariable.Instance.room_offset_nx;
                //        spawnedObject.transform.position = trackedImage.transform.position + GlobalVariable.Instance.room_offset;
                //        spawnedObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                //        break;
                //    case "+z":
                //        Debug.Log("kks forward: +z");
                //        spawnedObject.transform.position = Camera.main.transform.position + GlobalVariable.Instance.room_offset_z;
                //        spawnedObject.transform.position = trackedImage.transform.position + GlobalVariable.Instance.room_offset;
                //        spawnedObject.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
                //        break;
                //    case "-z":
                //        Debug.Log("kks forward: -z");
                //        spawnedObject.transform.position = Camera.main.transform.position + GlobalVariable.Instance.room_offset_nz;
                //        spawnedObject.transform.position = trackedImage.transform.position + GlobalVariable.Instance.room_offset;
                //        spawnedObject.transform.rotation = Quaternion.Euler(0f, 270f, 0f);
                //        break;
                //}

                // 현재 카메라의 회전 값과 초기 회전값의 차이를 계산
                Quaternion currentCameraRotation = Camera.main.transform.rotation;
                Quaternion rotationDifference = Quaternion.Inverse(initialCameraRotation) * currentCameraRotation;

                // 오브젝트를 북쪽에 맞게 회전시키기
                //spawnedObject.transform.rotation = Quaternion.Euler(0, -northAngle, 0) * rotationDifference;

                createdPrefab = spawnedObject;

                // 2차년도 부분
                placeListCanvas.SetActive(true);

                currentTrackingObjectName = "room1";
                Debug.Log($"kks room1");
                Debug.Log($"kks spawn object rotation : {spawnedObject.transform.eulerAngles}");
                Debug.Log($"kks cameraRotation : {Camera.main.transform.eulerAngles}");
                //GetComponent<CreatePlaceObject>().testText.text = $"room1\n" +
                //    $"ROT : {spawnedObject.transform.eulerAngles}";
            }
        }
        else if (trackedImage.referenceImage.name == "distribution_box" || trackedImage.referenceImage.name == "distribution_box1"
                || trackedImage.referenceImage.name == "distribution_box2" || trackedImage.referenceImage.name == "distribution_box3"
                || trackedImage.referenceImage.name == "distribution_box4")
        {
            if (trackedImage.trackingState == TrackingState.Tracking)
            {
                if (SceneChangeSingleton.Instance.changeName != "distribution_box")
                {
                    SceneChangeSingleton.Instance.sceneChangeAble = true;
                }

                if (SceneChangeSingleton.Instance.sceneChangeAble && SceneChangeSingleton.Instance.changeName != "distribution_box")
                {
                    SceneChangeSingleton.Instance.sceneChangeAble = false;
                    SceneChangeSingleton.Instance.changeName = "distribution_box";
                    SceneManager.LoadScene(0);
                }
                if (currentTrackingObjectName == "distribution_box")
                {
                    return;
                }

                if (createdPrefab != null)
                {
                    Destroy(createdPrefab);
                }

                Vector3 spawnPosition = Camera.main.transform.position + GlobalVariable.Instance.distribution_box_offset;
                //Vector3 spawnPosition = trackedImage.transform.position + GlobalVariable.Instance.distribution_box_offset;

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