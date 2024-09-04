using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Touch = UnityEngine.Touch;

public class CreatePlaceObject : MonoBehaviour
{
    public List<GameObject> placeableObjects;
    public List<GameObject> originalPlaceableObjects;

    bool placeMode = false;

    GameObject ceiling;

    GameObject placeingObject;

    GameObject buttonCanvas;

    int placeID = int.MaxValue;

    // ������ ������Ʈ ���� ����� ���� �� ������ ����� ����Ʈ�� �߰�
    List<GameObject> placedNewObjects = new List<GameObject>();

    public GameObject imageCanvas;
    public List<Texture> imageList;


    public TextMeshProUGUI testText;

    /// <summary>
    /// ������ ������Ʈ ��� �ı� (����)
    /// </summary>
    void ResetObjects()
    {
        for (int i = placedNewObjects.Count - 1; i >= 0; i--)
        {
            Destroy(placedNewObjects[i]);
            placedNewObjects.RemoveAt(i);
        }
    }

    void Update()
    {
        if(GetComponent<TrackedImageInfomation1>().createdPrefab != null && GetComponent<TrackedImageInfomation1>().currentTrackingObjectName == "room1")
        {
            ceiling = GetComponent<TrackedImageInfomation1>().createdPrefab.GetComponent<IndoorObject>().placeableObject;
        }

        // ��ġ
        if (Input.touchCount > 0)
        {
            // UI ��ġ�� ��ȯ
            if(EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                return;
            }

            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit[] hits = Physics.RaycastAll(ray);

                // ��ġ����϶�
                if (hits.Length > 0)
                {
                    PlaceModeTouch(hits);                    
                }

                // ��ġ ��� �ƴ� ��
                if (!placeMode)
                {
                    Debug.Log("KKS ��ġ ��ġ��� X");
                    for (int i = 0; i < hits.Length; i++)
                    {
                        for(int j = 0; j < hits.Length; j++)
                        {
                            if (hits[j].collider.CompareTag("ObjectStatusButton"))
                            {
                                hits[j].collider.transform.GetComponent<Button>().onClick.Invoke();
                                return;
                            }
                        }

                        if (hits[i].collider.CompareTag("UIButton"))
                        {
                            if (hits[i].collider.name == "Button_Delete")
                            {
                                placedNewObjects.Remove(hits[i].collider.transform.root.gameObject);
                                Destroy(hits[i].collider.transform.root.gameObject);
                                break;
                            }

                            if (hits[i].collider.name.Equals("CloseFloorPlan Button"))
                            {
                                OnClickCloseImageCanvasButton(false);
                                //imageCanvas.SetActive(false);
                            }
                        }

                        if (hits[i].collider.CompareTag("Touchable"))
                        {
                            if (hits[i].collider.GetComponent<TextShow>())
                            {
                                hits[i].collider.GetComponent<TextShow>().SetVisible();
                                break;
                            }
                        }

                        if (hits[i].collider.CompareTag("PlacedObject"))
                        {
                            // Active ���°� ������� false�� �����Ǿ� �ִ� ��쿡�� true���� �־� ��.
                            if (!hits[i].collider.transform.GetComponentInChildren<Canvas>(true).gameObject.activeSelf)
                            {
                                hits[i].collider.transform.GetComponentInChildren<Canvas>(true).gameObject.SetActive(true);
                                Transform canvas = hits[i].collider.transform.GetComponentInChildren<Canvas>().transform;
                                canvas.rotation = Quaternion.LookRotation(canvas.position - Camera.main.transform.position);
                            }
                            else
                            {
                                hits[i].collider.transform.GetComponentInChildren<Canvas>(true).gameObject.SetActive(false);
                            }
                        }

                        Debug.Log("KKS ���������� Ȯ�� ���� �õ�");
                        if (hits[i].collider.name.Contains("distribution_box_"))
                        {
                            Debug.Log("KKS ������ Ȯ��");

                            OnClickCloseImageCanvasButton(true);
                            //imageCanvas.SetActive(true);
                            imageCanvas.transform.position = Camera.main.transform.position + GlobalVariable.Instance.imageCanvas_offset;
                            switch (hits[i].collider.name)
                            {
                                case "distribution_box_101":
                                    imageCanvas.GetComponentInChildren<RawImage>(true).texture = imageList[0];
                                    break;
                                case "distribution_box_102":
                                    imageCanvas.GetComponentInChildren<RawImage>(true).texture = imageList[1];
                                    break;
                                case "distribution_box_103":
                                    imageCanvas.GetComponentInChildren<RawImage>(true).texture = imageList[2];
                                    break;
                                case "distribution_box_104":
                                    imageCanvas.GetComponentInChildren<RawImage>(true).texture = imageList[3];
                                    break;
                                case "distribution_box_105":
                                    imageCanvas.GetComponentInChildren<RawImage>(true).texture = imageList[0];
                                    break;
                                case "distribution_box_106":
                                    imageCanvas.GetComponentInChildren<RawImage>(true).texture = imageList[1];
                                    break;
                                case "distribution_box_107":
                                    imageCanvas.GetComponentInChildren<RawImage>(true).texture = imageList[2];
                                    break;
                                case "distribution_box_201":
                                    imageCanvas.GetComponentInChildren<RawImage>(true).texture = imageList[3];
                                    break;
                                case "distribution_box_202":
                                    imageCanvas.GetComponentInChildren<RawImage>(true).texture = imageList[0];
                                    break;
                                case "distribution_box_203":
                                    imageCanvas.GetComponentInChildren<RawImage>(true).texture = imageList[1];
                                    break;
                                case "distribution_box_204":
                                    imageCanvas.GetComponentInChildren<RawImage>(true).texture = imageList[2];
                                    break;
                                case "distribution_box_205":
                                    imageCanvas.GetComponentInChildren<RawImage>(true).texture = imageList[3];
                                    break;
                                case "distribution_box_206":
                                    imageCanvas.GetComponentInChildren<RawImage>(true).texture = imageList[0];
                                    break;
                                case "distribution_box_207":
                                    imageCanvas.GetComponentInChildren<RawImage>(true).texture = imageList[1];
                                    break;
                            }
                        }
                    }
                }
            }
        }
    }

    private void PlaceModeTouch(RaycastHit[] hits)
    {
        if (placeMode)
        {
            bool touched = false;
            // ��ġ�� ������ ui�� ������ ui �켱 ����
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider.CompareTag("UIButton"))
                {
                    touched = true;

                    if (hits[i].collider.name == "Button_OK")
                    {
                        GameObject newPlaceObject = Instantiate(originalPlaceableObjects[placeID]);
                        placedNewObjects.Add(newPlaceObject);

                        newPlaceObject.transform.position = placeingObject.transform.position;
                        newPlaceObject.transform.rotation = placeingObject.transform.rotation;
                        newPlaceObject.transform.localScale = placeingObject.transform.localScale;
                        newPlaceObject.transform.parent = GetComponent<TrackedImageInfomation1>().createdPrefab.transform;

                        EndPlaceMode();
                    }
                    else if (hits[i].collider.name == "Button_Cancel")
                    {
                        EndPlaceMode();
                    }
                    else if (hits[i].collider.name == "Button_Rotate")
                    {
                        switch (placeID)
                        {
                            case 0:
                                placeingObject.transform.Rotate(0, 30, 0);
                                break;
                            case 2:
                                placeingObject.transform.Rotate(0, 0, 30);
                                break;
                        }
                    }
                    break;
                }
            }

            if (touched) return;

            // ��ġ ������ UI�� ������ ���
            for (int i = 0; i < hits.Length; i++)
            {
                if (EventSystem.current.IsPointerOverGameObject() == false)
                {
                    Vector3 previousPosition;
                    Quaternion previousRotation;
                    Vector3 newPosition;
                    if (placeID == 1)
                    {
                        if (hits[i].collider.name.Contains("Wall"))
                        {
                            previousPosition = placeingObject.transform.position;
                            previousRotation = placeingObject.transform.rotation;
                            switch (hits[i].collider.name)
                            {
                                case "Back Wall":
                                    newPosition = hits[i].point;
                                    newPosition.z = hits[i].collider.transform.position.z;
                                    placeingObject.transform.position = newPosition;
                                    placeingObject.transform.rotation = Quaternion.Euler(0, 180, 0);
                                    break;
                                case "Front Wall":
                                    newPosition = hits[i].point;
                                    newPosition.z = hits[i].collider.transform.position.z;
                                    placeingObject.transform.position = newPosition;
                                    placeingObject.transform.rotation = Quaternion.Euler(0, 0, 0);
                                    break;
                                case "Left Wall":
                                    newPosition = hits[i].point;
                                    newPosition.x = hits[i].collider.transform.position.x;
                                    placeingObject.transform.position = newPosition;
                                    placeingObject.transform.rotation = Quaternion.Euler(0, 90, 0);
                                    break;
                                case "Right Wall":
                                    newPosition = hits[i].point;
                                    newPosition.x = hits[i].collider.transform.position.x;
                                    placeingObject.transform.position = newPosition;
                                    placeingObject.transform.rotation = Quaternion.Euler(0, 270, 0);
                                    break;
                            }
                            break;
                        }
                    }
                    else
                    {
                        if (hits[i].collider.name == "Ceiling")
                        {
                            previousPosition = placeingObject.transform.position;
                            previousRotation = placeingObject.transform.rotation;
                            if (placeID == 2)
                            {
                                newPosition = hits[i].point;
                                newPosition.y = ceiling.transform.position.y; // y�� ����
                                placeingObject.transform.position = newPosition;
                            }
                            else
                            {
                                newPosition = hits[i].point;
                                newPosition.y = ceiling.transform.position.y; // y�� ����
                                placeingObject.transform.position = newPosition;
                            }
                            break;
                        }
                    }
                }
            }
        }
    }

    public void OnClickCloseImageCanvasButton(bool value)
    {
        imageCanvas.SetActive(value);
        StartCoroutine(SendPostRequest());
    }

    // ������ POST ��û�� ������ �޼���
    IEnumerator SendPostRequest()
    {
        Debug.Log($"kks sendPosrtRequest");
        string url = "http://192.168.1.139/api/alarm/release";

        // seq ���� ������ JSON ������
        string jsonData = $"{{\"seq\" : {GetComponent<FirebaseInit>().alert_seq}}}";
        Debug.Log($"kks alert_seq : {GetComponent<FirebaseInit>().alert_seq}");

        // ��û ����
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        // Content-Type ��� ����
        request.SetRequestHeader("Content-Type", "application/json");

        // ��û ������
        yield return request.SendWebRequest();

        // ��û ��� ó��
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Response: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Error: " + request.error);
        }
    }

    public void RoomItemListSet(GameObject obj)
    {
        switch(obj.name)
        {
            case "Button ListHide":
                GetComponent<TrackedImageInfomation1>().placeListShowButton.SetActive(true);
                GetComponent<TrackedImageInfomation1>().placeListCanvas.transform.GetChild(0).gameObject.SetActive(false);
                //GetComponent<FirebaseInit>().LocalNotification("notification", "�˸�", "ȭ��/4��/4010ȣ");
                obj.SetActive(false);
                break;
            case "Button ListShow":
                GetComponent<TrackedImageInfomation1>().placeListHideButton.SetActive(true);
                GetComponent<TrackedImageInfomation1>().placeListCanvas.transform.GetChild(0).gameObject.SetActive(true);
                obj.SetActive(false);
                break;
        }
    }

    /// <summary>
    /// ������Ʈ ��ġ ����� ȣ���ϴ� �Լ�
    /// </summary>
    private void EndPlaceMode()
    {
        Destroy(placeingObject);
        placeingObject = null;
        buttonCanvas.SetActive(true);
        placeMode = false;
        placeID = int.MaxValue;
    }

    /// <summary>
    /// OnClick �̺�Ʈ�� ��ϵǾ� ����
    /// </summary>
    /// <param name="btn"></param>
    public void OnCreateButtonClick(GameObject btn)
    {
        if(buttonCanvas == null)
        {
            buttonCanvas = btn.transform.root.gameObject;
        }

        GameObject prefab = GetComponent<TrackedImageInfomation1>().createdPrefab;

        if (GetComponent<TrackedImageInfomation1>().currentTrackingObjectName == "room1")
        {
            if (prefab != null)
            {
                switch (btn.name)
                {
                    case "AirConditioner":
                        placeID = 0;
                        placeingObject = Instantiate(placeableObjects[placeID]);
                        placeingObject.transform.position = prefab.GetComponent<IndoorObject>().placeableObject.transform.position;
                        break;
                    case "TV":
                        placeID = 1;
                        placeingObject = Instantiate(placeableObjects[placeID]);
                        placeingObject.transform.position = prefab.GetComponent<IndoorObject>().wallObject.transform.position;
                        break;
                    case "FluorescentLight":
                        placeID = 2;
                        placeingObject = Instantiate(placeableObjects[placeID]);
                        placeingObject.transform.position = prefab.GetComponent<IndoorObject>().placeableObject.transform.position;
                        break;
                    case "reset":
                        ResetObjects();
                        break;
                }

                if(btn.name != "reset")
                {
                    buttonCanvas.SetActive(false);
                    placeMode = true;
                }
            }
        }
    }
}
