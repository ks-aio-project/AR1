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

    private Quaternion initialCameraRotation;  // ����̽��� �ʱ� ȸ������ ������ ����
    private bool isCalibrated = false;  // �ʱ� ������ �����Ǿ����� ����
    private LineRenderer lineRenderer;

    private void Start()
    {
        var rotationAngles = Camera.main.transform.rotation;
        Debug.Log($"kks start camera Rotation X: {rotationAngles.x}��, Y: {rotationAngles.y}��, Z: {rotationAngles.z}��");

        // ��ħ�� Ȱ��ȭ
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

        Input.compass.enabled = true; // ��ħ��

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


        // ����̽��� �ʱ� ȸ������ ����
        initialCameraRotation = Camera.main.transform.rotation;

        if (placeListCanvas == null)
        {
            Debug.LogError("placeCanvas �� �Ҵ�");
        }
        //placeListCanvas.SetActive(false);
    }

    // ������ �������� �ȹ���(��, �ϵ�, ��, ����, ��, ����, ��, �ϼ�) ���
    string GetDirectionFromAngle(float angle)
    {
        if (angle >= 337.5f || angle < 22.5f)
            return "�� (N)";
        else if (angle >= 22.5f && angle < 67.5f)
            return "�ϵ� (NE)";
        else if (angle >= 67.5f && angle < 112.5f)
            return "�� (E)";
        else if (angle >= 112.5f && angle < 157.5f)
            return "���� (SE)";
        else if (angle >= 157.5f && angle < 202.5f)
            return "�� (S)";
        else if (angle >= 202.5f && angle < 247.5f)
            return "���� (SW)";
        else if (angle >= 247.5f && angle < 292.5f)
            return "�� (W)";
        else
            return "�ϼ� (NW)";
    }

    private void Update()
    {
        // �������κ����� ���� (0~360��)
        northAngle = Input.compass.trueHeading;

        // ȭ�鿡 ���� ���� ���
        //Debug.Log("KKS ���� ���� : " + northAngle + "��");

        // ī�޶��� ���� ȸ�� �������� Y��(����̽��� ����) ȸ������ ������
        float cameraYAngle = Camera.main.transform.eulerAngles.y;

        // +Z���� �ٶ󺸴� ���� (����̽��� ������ �������� �󸶳� ȸ���ߴ���)
        float deviceDirection = (northAngle + cameraYAngle) % 360;

        // �ȹ����� ��ȯ
        string direction = GetDirectionFromAngle(deviceDirection);
        //Debug.Log("Z���� �ٶ󺸴� ����: " + direction);

        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            currentForward = hit.transform.name;
        }

        // ��ħ���� Ȱ��ȭ�ǰ� �ʱ� ������ ������ �� ����
        if (Input.compass.enabled && !isCalibrated)
        {
            // �ʱ� ����̽� ȸ������ �����Ͽ� ����
            initialCameraRotation = Camera.main.transform.rotation;
            isCalibrated = true;  // ���� �Ϸ�
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
        // ���Ӱ� Ʈ��ŷ�� �̹����� ���� ó��
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            CreateOrUpdateARObject(trackedImage);
        }

        // ������Ʈ�� �̹����� ���� ó��
        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            CreateOrUpdateARObject(trackedImage);
        }

        // ���ŵ� �̹����� ���� ó��
        foreach (ARTrackedImage trackedImage in eventArgs.removed)
        {
        }
    }

    void CreateOrUpdateARObject(ARTrackedImage trackedImage)
    {
        Vector3 cameraForward = Camera.main.transform.forward; // ���� ī�޶� �ٶ󺸴� ����

        // �̹��� Ʈ��ŷ��
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

                // ī�޶��� ȸ�� �� ��������
                Vector3 rotationAngles = Camera.main.transform.rotation.eulerAngles;
                Debug.Log($"kks Camera Rotation X: {rotationAngles.x}��, Y: {rotationAngles.y}��, Z: {rotationAngles.z}��");

                // TrackedImage ��ġ
                Vector3 trackedPosition = trackedImage.transform.position;

                // �� ������Ʈ ����
                GameObject spawnedObject = Instantiate(arObjectPrefab[0]);

                // A���� B(TrackedImage)�� ���ϴ� ���� ���
                Vector3 directionToB = trackedPosition - objs[0].transform.position;
                directionToB.Normalize();  // ���� ���� ����ȭ

                // A�� forward ����
                Vector3 forwardA = objs[0].transform.forward;

                // �ﰢ�Լ��� ����� ���� ��� (Cosine ��Ģ)
                float dotProduct = Vector3.Dot(forwardA, directionToB);  // ���� ���
                float magnitudeA = forwardA.magnitude;
                float magnitudeB = directionToB.magnitude;
                float cosTheta = dotProduct / (magnitudeA * magnitudeB);  // ���� ���ϱ� ���� Cosine
                float angle = Mathf.Acos(cosTheta) * Mathf.Rad2Deg;  // ���ȿ��� ������ ��ȯ

                // ��ȣ�� �����ϱ� ���� ���� (Cross Product)
                Vector3 cross = Vector3.Cross(forwardA, directionToB);
                if (cross.y < 0)
                {
                    angle = -angle;  // ���� ���̸� �ð� �������� ȸ���� ����
                }

                //// 360�� ������ ����
                //if (angle < 0)
                //{
                //    angle += 360;
                //}

                // 90���� ���� ó��
                float snappedAngle = Mathf.Round(angle / 90) * 90;
                Debug.Log($"kks Snapped Angle: {snappedAngle}");

                // ���� ī�޶� Y�� ȸ�� ������ �ﰢ�Լ��� ���� ���
                float cameraYRotation = Camera.main.transform.eulerAngles.y;

                // 1. objs[0]���� TrackedImage�� ���ϴ� ����
                Vector3 directionFromObjToTracked = trackedImage.transform.position - objs[0].transform.position;
                directionFromObjToTracked.Normalize();

                // 2. ī�޶󿡼� TrackedImage�� ���ϴ� ����
                Vector3 directionFromCameraToTracked = trackedImage.transform.position - Camera.main.transform.position;
                directionFromCameraToTracked.Normalize();

                // 3. �� ���� ������ ���� ��� (���� ���)
                dotProduct = Vector3.Dot(directionFromObjToTracked, directionFromCameraToTracked);
                float angleBetweenObjAndCamera = Mathf.Acos(dotProduct) * Mathf.Rad2Deg;  // ���� -> ���� ��ȯ

                Debug.Log($"kks angleBetweenObjAndCamera : {angleBetweenObjAndCamera}");

                // ������ ����� ��ȣ ���� (�ð�/�ݽð�)
                Vector3 crossProduct = Vector3.Cross(directionFromObjToTracked, directionFromCameraToTracked);
                if (crossProduct.y < 0)
                {
                    angleBetweenObjAndCamera = -angleBetweenObjAndCamera;  // �ð� ������ �� ������ ����
                }

                // 360�� ���� ����
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

                // ������Ʈ ��ġ ���� (ī�޶� �������� 2m)
                spawnedObject.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2f + Camera.main.transform.right * -0.8f + Camera.main.transform.up * -1.5f;

                Debug.Log($"kks currentForwards : {currentForward}");
                // ������Ʈ�� �����̼� ����
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

                // 2���⵵ �κ�
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