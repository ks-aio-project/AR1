using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using TMPro;
using System.Collections;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class DeviceData
{
    public string serial;
    public string mac;
    public string ip;
    public int port;
    public string name;
    public int sync;
    public int init;
    public int use;
    public int state;
    public string create;
    public string update;
    public string lasttime;
}

public static class JsonHelper
{
    // JSON 배열 파싱을 도와주는 유틸리티
    public static List<T> FromJson<T>(string json)
    {
        // JSON이 배열 형식이면 배열을 감싸주는 새로운 JSON 구조로 변경
        string newJson = "{ \"Items\": " + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.Items;
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public List<T> Items;
    }
}


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

    public GameObject currentForwardObject;

    Coroutine currentCoroutine;

    public static class JsonHelper
{
    // JSON 배열 파싱을 도와주는 유틸리티
    public static List<T> FromJson<T>(string json)
    {
        string newJson = "{\"Items\":" + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.Items;
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public List<T> Items;
    }
}
    // API 호출 함수
    IEnumerator GetInstalledEquipment(string equipmentName, string location)
    {
        while (true) // 무한 루프
        {
            Debug.Log("kks start Coroutine");
            string url;
            // URL 생성 (API의 엔드포인트에 쿼리 파라미터 추가)
            if (location != "")
            {
                url = $"http://bola.iptime.org:9080/device/searchlocation/?location={location}";
            }
            else
            {
                url = $"http://192.168.1.155:9080/device/all/";
            }

            Debug.Log("kks url : " + url);
            // UnityWebRequest를 사용해 GET 요청 보내기
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                // 요청을 보내고 응답이 올 때까지 기다림
                yield return webRequest.SendWebRequest();

                // 네트워크 에러 체크
                if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError("KKS Error: " + webRequest.error);
                }
                else
                {
                    // 성공적으로 데이터를 받았을 때
                    Debug.Log("KKS Response: " + webRequest.downloadHandler.text);

                    // 받은 데이터를 처리 (필요한 경우 JSON 파싱 등)
                    ProcessResponse(webRequest.downloadHandler.text);
                }
            }
            // 5초 대기
            yield return new WaitForSeconds(5);
        }
    }

    // API에서 받은 응답 데이터를 처리하는 함수 (필요시 구현)
    void ProcessResponse(string jsonResponse)
    {
        // JSON을 Unity에서 다루기 위해 C# 객체로 변환하는 등의 처리
        // 예시: JsonUtility를 사용해 데이터를 파싱할 수 있습니다
        // EquipmentData equipment = JsonUtility.FromJson<EquipmentData>(jsonResponse);
        // 필요한 장치만 출력하거나 사용

        List<DeviceData> devices = JsonConvert.DeserializeObject<List<DeviceData>>(jsonResponse);

        // 필요한 장치만 출력하거나 사용
        foreach (DeviceData device in devices)
        {
            Debug.Log("Name: " + device.name);
            // 필요한 데이터만 사용
            if (device.name.StartsWith("pc"))
            {
                // "pcmain"인 경우 처리
                if (device.name == "pcmain")
                {
                    Debug.Log("Using device: pcmain");

                    GameObject obj = GameObject.Find("pcmain");

                    Material mat = Instantiate(obj.GetComponent<Renderer>().material);

                    mat.color = device.use == 1 ? Color.red : Color.blue;  // 원하는 색상으로 처리
                    obj.GetComponent<Renderer>().material = mat;
                }
                else
                {
                    int deviceNumber = int.Parse(device.name.Substring(2));
                    if (deviceNumber >= 1 && deviceNumber <= 41)
                    {
                        // 여기에 필요한 로직 추가 (예: 장치 처리)
                        Debug.Log("Using device: " + deviceNumber);

                        GameObject obj = GameObject.Find($"pc{deviceNumber}");

                        Material mat = Instantiate(obj.GetComponent<Renderer>().material);

                        mat.color = device.use == 1 ? Color.red : Color.blue;
                        obj.GetComponent<Renderer>().material = mat;
                    }
                }
            }
        }
    }
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
            currentForwardObject = hit.transform.gameObject;
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
        if (trackedImage.referenceImage.name == "8221")
        {
            if (trackedImage.trackingState == TrackingState.Tracking && GlobalVariable.Instance.last_qrcode == "8221")
            {
                if (currentTrackingObjectName == trackedImage.referenceImage.name)
                {
                    return;
                }

                if (createdPrefab != null)
                {
                    Destroy(createdPrefab);
                }

                // 카메라의 회전 값 가져오기
                Vector3 rotationAngles = trackedImage.transform.rotation.eulerAngles;
                //Debug.Log($"kks Camera Rotation X: {rotationAngles.x}°, Y: {rotationAngles.y}°, Z: {rotationAngles.z}°");

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
                if (angle < 0)
                {
                    angle += 360;
                }

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
                if (angleBetweenObjAndCamera < 0)
                {
                    angleBetweenObjAndCamera += 360;
                }

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
                spawnedObject.transform.localRotation = Quaternion.Euler(90, 0, 0);
                spawnedObject.transform.position = trackedImage.transform.position + new Vector3(0, 0, 2f);

                Debug.Log($"kks currentForwards : {currentForward}");
                // 오브젝트의 로테이션 설정
                //if (result > 50 && angleBetweenObjAndCamera < 50)
                //{
                //    spawnedObject.transform.rotation = Quaternion.Euler(0f, snappedAngle + 180 - result, 0f);
                //}
                //else if (result > 50 && angleBetweenObjAndCamera > 50)
                //{
                //    spawnedObject.transform.rotation = Quaternion.Euler(0f, snappedAngle + 270 - result, 0f);
                //}
                //else
                //{
                //    spawnedObject.transform.rotation = Quaternion.Euler(0f, snappedAngle + 90 - result, 0f);
                //}

                createdPrefab = spawnedObject;
                createdPrefab.transform.SetParent(trackedImage.transform);
                //createdPrefab.transform.rotation = Quaternion.Euler(0, 270, 0);
                // 2차년도 부분
                placeListCanvas.SetActive(true);

                currentTrackingObjectName = trackedImage.referenceImage.name;
                Debug.Log($"kks qrcodebox");
                Debug.Log($"kks spawn object rotation : {spawnedObject.transform.eulerAngles}");
                Debug.Log($"kks cameraRotation : {Camera.main.transform.eulerAngles}");
                //GetComponent<CreatePlaceObject>().testText.text = $"room1\n" +
                //    $"ROT : {spawnedObject.transform.eulerAngles}";

                //if (currentCoroutine != null) { StopCoroutine(currentCoroutine); }

                //currentCoroutine = StartCoroutine(GetInstalledEquipment("", "8221"));
            }
        }
        else if (trackedImage.referenceImage.name == "8119")
        {
            if (trackedImage.trackingState == TrackingState.Tracking && GlobalVariable.Instance.last_qrcode == "8119")
            {
                if (currentTrackingObjectName == trackedImage.referenceImage.name)
                {
                    return;
                }

                if (createdPrefab != null)
                {
                    Destroy(createdPrefab);
                }

                // 카메라의 회전 값 가져오기
                Vector3 rotationAngles = trackedImage.transform.rotation.eulerAngles;
                //Debug.Log($"kks Camera Rotation X: {rotationAngles.x}°, Y: {rotationAngles.y}°, Z: {rotationAngles.z}°");

                // TrackedImage 위치
                Vector3 trackedPosition = trackedImage.transform.position;

                // 새 오브젝트 생성
                GameObject spawnedObject = Instantiate(arObjectPrefab[1]);

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
                spawnedObject.transform.position = trackedImage.transform.position;

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
                spawnedObject.transform.rotation = Quaternion.identity;

                createdPrefab = spawnedObject;
                createdPrefab.transform.SetParent(trackedImage.transform);
                // 2차년도 부분
                placeListCanvas.SetActive(true);

                currentTrackingObjectName = trackedImage.referenceImage.name;
                //if(currentCoroutine != null) { StopCoroutine(currentCoroutine); }
                //currentCoroutine = StartCoroutine(GetInstalledEquipment("", "8119"));
            }
        }
        else if (trackedImage.referenceImage.name == "8212-2")
        {
            if (trackedImage.trackingState == TrackingState.Tracking && GlobalVariable.Instance.last_qrcode == "8212-2")
            {
                if (currentTrackingObjectName == trackedImage.referenceImage.name)
                {
                    return;
                }

                if (createdPrefab != null)
                {
                    Destroy(createdPrefab);
                }

                Vector3 spawnPosition = Camera.main.transform.position + GlobalVariable.Instance.distribution_box_offset;
                //Vector3 spawnPosition = trackedImage.transform.position + GlobalVariable.Instance.distribution_box_offset;

                GameObject spawnedObject = Instantiate(arObjectPrefab[2]);
                //spawnedObject.transform.rotation = Quaternion.identity;
                spawnedObject.transform.position = spawnPosition;
                createdPrefab = spawnedObject;

                placeListCanvas.SetActive(false);

                currentTrackingObjectName = trackedImage.referenceImage.name;
            }
        }
        else if (trackedImage.referenceImage.name == "box" && GlobalVariable.Instance.last_qrcode == "box")
        {
            if (trackedImage.trackingState == TrackingState.Tracking)
            {
                if (currentTrackingObjectName == trackedImage.referenceImage.name)
                {
                    return;
                }

                if (createdPrefab != null)
                {
                    Destroy(createdPrefab);
                }

                Vector3 spawnPosition = Camera.main.transform.position + GlobalVariable.Instance.distribution_box_offset;
                //Vector3 spawnPosition = trackedImage.transform.position + GlobalVariable.Instance.distribution_box_offset;

                GameObject spawnedObject = Instantiate(arObjectPrefab[3]);
                //spawnedObject.transform.rotation = Quaternion.identity;
                spawnedObject.transform.position = spawnPosition;
                createdPrefab = spawnedObject;

                placeListCanvas.SetActive(false);

                currentTrackingObjectName = trackedImage.referenceImage.name;
            }
        }
    }
}