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

    private Quaternion initialCameraRotation;  // ����̽��� �ʱ� ȸ������ ������ ����
    private bool isCalibrated = false;  // �ʱ� ������ �����Ǿ����� ����

    private void Start()
    {
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
            //Debug.Log($"kks init currentForward : {currentForward}");
            //Debug.Log($"kks init cameraRotation : {Camera.main.transform.eulerAngles}");
            //Debug.Log($"kks init cameraforward : {Camera.main.transform.forward}");
        }

        // ��ħ���� Ȱ��ȭ�ǰ� �ʱ� ������ ������ �� ����
        if (Input.compass.enabled && !isCalibrated)
        {
            // �ʱ� ����̽� ȸ������ �����Ͽ� ����
            initialCameraRotation = Camera.main.transform.rotation;
            isCalibrated = true;  // ���� �Ϸ�
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

                Vector3 rotationAngles = Camera.main.transform.rotation.eulerAngles;
                Debug.Log($"kks camera Rotation X: {rotationAngles.x}��, Y: {rotationAngles.y}��, Z: {rotationAngles.z}��");

                trackedPosition = trackedImage.transform.position;
                //Vector3 spawnPosition = trackedImage.transform.position + GlobalVariable.Instance.room_offset;

                GameObject spawnedObject = Instantiate(arObjectPrefab[0]);

                Vector3 directionToB = trackedImage.transform.position - objs[0].transform.position;
                directionToB.Normalize();  // ���� ���͸� ����ȭ

                // A�� forward ����
                Vector3 forwardA = objs[0].transform.forward;

                // A�� forward ���Ϳ� A���� B�� ���� ���� ������ ���� ���
                float angle = Vector3.Angle(forwardA, directionToB);

                // ������ ���� �� A�� ������ ���͸� �������� ���� ��(Cross Product)�� ����Ͽ� ������ ��ȣ�� ���
                Vector3 cross = Vector3.Cross(forwardA, directionToB);
                if (cross.y < 0)
                {
                    angle = -angle;  // ���� ���̸� �ð� �������� ȸ���� ����
                }

                if (angle < 0)
                {
                    angle += 360;
                }

                // 90���� ����� ���� ����� ������ �ݿø��ϰ� �ٽ� 90�� ����
                float snappedAngle = Mathf.Round(angle / 90) * 90;

                // ���� 360������ ū ���� ������ 360���� ����
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

                // ���� ī�޶��� ȸ�� ���� �ʱ� ȸ������ ���̸� ���
                Quaternion currentCameraRotation = Camera.main.transform.rotation;
                Quaternion rotationDifference = Quaternion.Inverse(initialCameraRotation) * currentCameraRotation;

                // ������Ʈ�� ���ʿ� �°� ȸ����Ű��
                //spawnedObject.transform.rotation = Quaternion.Euler(0, -northAngle, 0) * rotationDifference;

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