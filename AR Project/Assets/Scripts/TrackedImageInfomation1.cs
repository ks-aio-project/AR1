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
using System.Net;

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

    public Vector3 trackedPosition = new();
    public Quaternion trackedRotation = new();

    List<GameObject> objs = new();

    float northAngle;

    private Quaternion initialCameraRotation;  // 디바이스의 초기 회전값을 저장할 변수
    private bool isCalibrated = false;  // 초기 방향이 설정되었는지 여부
    private LineRenderer lineRenderer;

    private void Start()
    {
        var rotationAngles = Camera.main.transform.rotation;
        Debug.Log($"kks start camera Rotation X: {rotationAngles.x}°, Y: {rotationAngles.y}°, Z: {rotationAngles.z}°");

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
        }

        // 나침반이 활성화되고 초기 방향이 설정된 후 실행
        if (Input.compass.enabled && !isCalibrated)
        {
            // 초기 디바이스 회전값을 보정하여 설정
            initialCameraRotation = Camera.main.transform.rotation;
            isCalibrated = true;  // 보정 완료
        }

        //foreach (GameObject i in objs)
        //{
        //    switch (i.name)
        //    {
        //        case "+x":
        //            i.transform.position = Camera.main.transform.position + new Vector3(0.25f, 0, 0);
        //            break;
        //        case "-x":
        //            i.transform.position = Camera.main.transform.position + new Vector3(-0.25f, 0, 0);
        //            break;
        //        case "+y":
        //            i.transform.position = Camera.main.transform.position + new Vector3(0, 0.25f, 0);
        //            break;
        //        case "-y":
        //            i.transform.position = Camera.main.transform.position + new Vector3(0, -0.25f, 0);
        //            break;
        //        case "+z":
        //            i.transform.position = Camera.main.transform.position + new Vector3(0, 0, 0.25f);
        //            break;
        //        case "-z":
        //            i.transform.position = Camera.main.transform.position + new Vector3(0, 0, -0.25f);
        //            break;
        //    }
        //}
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

                // 카메라의 회전 값 가져오기
                Vector3 rotationAngles = Camera.main.transform.rotation.eulerAngles;
                Debug.Log($"kks Camera Rotation X: {rotationAngles.x}°, Y: {rotationAngles.y}°, Z: {rotationAngles.z}°");

                // TrackedImage 위치
                Vector3 trackedPosition = trackedImage.transform.position;

                // 새 오브젝트 생성
                GameObject spawnedObject = Instantiate(arObjectPrefab[0]);

                // A에서 B(TrackedImage)로 향하는 벡터 계산
                Vector3 directionToB = trackedPosition - objs[0].transform.position;
                directionToB.Normalize();  // 방향 벡터 정규화

                // A의 forward 벡터
                Vector3 forwardA = objs[0].transform.forward;

                // 삼각함수를 사용한 각도 계산 (Cosine 법칙)
                float dotProduct = Vector3.Dot(forwardA, directionToB);  // 내적 계산
                float magnitudeA = forwardA.magnitude;
                float magnitudeB = directionToB.magnitude;
                float cosTheta = dotProduct / (magnitudeA * magnitudeB);  // 각도 구하기 위한 Cosine
                float angle = Mathf.Acos(cosTheta) * Mathf.Rad2Deg;  // 라디안에서 각도로 변환

                // 부호를 결정하기 위한 외적 (Cross Product)
                Vector3 cross = Vector3.Cross(forwardA, directionToB);
                if (cross.y < 0)
                {
                    angle = -angle;  // 음의 값이면 시계 방향으로 회전한 각도
                }

                //// 360도 범위로 보정
                //if (angle < 0)
                //{
                //    angle += 360;
                //}

                // 90도로 스냅 처리
                float snappedAngle = Mathf.Round(angle / 90) * 90;
                Debug.Log($"kks Snapped Angle: {snappedAngle}");

                // 현재 카메라 Y축 회전 각도를 삼각함수를 통해 계산
                float cameraYRotation = Camera.main.transform.eulerAngles.y;

                // 1. objs[0]에서 TrackedImage로 향하는 벡터
                Vector3 directionFromObjToTracked = trackedImage.transform.position - objs[0].transform.position;
                directionFromObjToTracked.Normalize();

                // 2. 카메라에서 TrackedImage로 향하는 벡터
                Vector3 directionFromCameraToTracked = trackedImage.transform.position - Camera.main.transform.position;
                directionFromCameraToTracked.Normalize();

                // 3. 두 벡터 사이의 각도 계산 (내적 사용)
                dotProduct = Vector3.Dot(directionFromObjToTracked, directionFromCameraToTracked);
                float angleBetweenObjAndCamera = Mathf.Acos(dotProduct) * Mathf.Rad2Deg;  // 라디안 -> 각도 변환

                Debug.Log($"kks angleBetweenObjAndCamera : {angleBetweenObjAndCamera}");

                // 외적을 사용해 부호 결정 (시계/반시계)
                Vector3 crossProduct = Vector3.Cross(directionFromObjToTracked, directionFromCameraToTracked);
                if (crossProduct.y < 0)
                {
                    angleBetweenObjAndCamera = -angleBetweenObjAndCamera;  // 시계 방향일 때 음수로 만듦
                }

                // 360도 범위 보정
                //if (angleBetweenObjAndCamera < 0)
                //{
                //    angleBetweenObjAndCamera += 360;
                //}

                float result = 0;

                if (cameraYRotation < 90)
                {
                    result = 90f - cameraYRotation;
                }
                else if (cameraYRotation < 180)
                {
                    result = 180f - cameraYRotation;
                }
                else if (cameraYRotation < 270)
                {
                    result = 270f - cameraYRotation;
                }
                else
                {
                    result = 360f - cameraYRotation;
                }

                Debug.Log($"kks result : {result}");

                // 오브젝트 위치 조정 (카메라 앞쪽으로 2m)
                spawnedObject.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2f + Camera.main.transform.right * -0.8f + Camera.main.transform.up * -1.5f;

                Debug.Log($"kks currentForwards : {currentForward}");
                // 오브젝트의 로테이션 설정
                if (result > 50 && angleBetweenObjAndCamera < 50)
                {
                    spawnedObject.transform.rotation = Quaternion.Euler(0f, snappedAngle + 180 - result, 0f);
                }
                else if (result > 50 && angleBetweenObjAndCamera > 50)
                {
                    spawnedObject.transform.rotation = Quaternion.Euler(0f, snappedAngle + 270 - result, 0f);
                }
                else
                {
                    spawnedObject.transform.rotation = Quaternion.Euler(0f, snappedAngle + 90 - result, 0f);
                }

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