using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class CreatePlaceObject : MonoBehaviour
{
    public List<GameObject> placeableObjects;
    public List<GameObject> originalPlaceableObjects;

    bool placeMode = false;

    public GameObject ceiling;

    GameObject placeingObject;

    GameObject buttonCanvas;

    int placeID = int.MaxValue;

    public GameObject testTMP;

    void Update()
    {
        if(GetComponent<TrackedImageInfomation1>().createdPrefab != null)
        {
            ceiling = GetComponent<TrackedImageInfomation1>().createdPrefab.GetComponent<IndoorObject>().placeableObject;
        }

        if (Input.touchCount > 0 && placeMode)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit[] hits = Physics.RaycastAll(ray);
                for (int i = 0; i < hits.Length; i++)
                {
                    if (placeMode && hits[i].collider.CompareTag("UIButton"))
                    {
                        if (hits[i].collider.name == "Button_OK")
                        {
                            testTMP.GetComponent<TextMeshProUGUI>().text = "Button_OK";
                            GameObject newPlaceObject = Instantiate(originalPlaceableObjects[placeID]);

                            newPlaceObject.transform.position = placeingObject.transform.position;
                            newPlaceObject.transform.rotation = placeingObject.transform.rotation;
                            newPlaceObject.transform.localScale = placeingObject.transform.localScale;

                            EndPlaceMode();
                        }
                        else if (hits[i].collider.name == "Button_Cancel")
                        {
                            testTMP.GetComponent<TextMeshProUGUI>().text = "Button_Cancel";
                            EndPlaceMode();
                        }
                        else if (hits[i].collider.name == "Button_Rotate")
                        {
                            testTMP.GetComponent<TextMeshProUGUI>().text = "Button_Rotate";
                            placeingObject.transform.Rotate(0, 0, 45);
                            placeingObject.GetComponent<UIConnetor>().button_ok.transform.Rotate(0, 0, -45);
                            placeingObject.GetComponent<UIConnetor>().button_cancel.transform.Rotate(0, 0, -45);
                            placeingObject.GetComponent<UIConnetor>().button_rotation.transform.Rotate(0, 0, -45);
                        }
                        break;
                    }
                }

                for (int i = 0; i < hits.Length; i++)
                {
                    if(placeID == 1)
                    {
                        Debug.Log($"KKS placeID is 1");
                        if (hits[i].collider.name.Contains("Wall"))
                        {
                            testTMP.GetComponent<TextMeshProUGUI>().text = "placeID 1 & Touch Wall";
                            Vector3 newPosition;
                            switch (hits[i].collider.name)
                            {
                                case "Back Wall":
                                    testTMP.GetComponent<TextMeshProUGUI>().text = $"hit : {hits[i].point}";
                                    newPosition = hits[i].point;
                                    newPosition.z = hits[i].collider.transform.position.z;
                                    placeingObject.transform.position = newPosition;
                                    placeingObject.transform.rotation = Quaternion.Euler(0, 180, 0);
                                    break;
                                case "Front Wall":
                                    testTMP.GetComponent<TextMeshProUGUI>().text = $"hit : {hits[i].point}";
                                    newPosition = hits[i].point;
                                    newPosition.z = hits[i].collider.transform.position.z;
                                    placeingObject.transform.position = newPosition;
                                    placeingObject.transform.rotation = Quaternion.Euler(0, 0, 0);
                                    break;
                                case "Left Wall":
                                    testTMP.GetComponent<TextMeshProUGUI>().text = $"hit : {hits[i].point}";
                                    newPosition = hits[i].point;
                                    newPosition.x = hits[i].collider.transform.position.x;
                                    placeingObject.transform.position = newPosition;
                                    placeingObject.transform.rotation = Quaternion.Euler(0, 90, 0);
                                    break;
                                case "Right Wall":
                                    testTMP.GetComponent<TextMeshProUGUI>().text = $"hit : {hits[i].point}";
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
                            if (placeID == 2)
                            {
                                testTMP.GetComponent<TextMeshProUGUI>().text = "placeID 2 & Touch Ceiling";
                                Vector3 newPosition = hits[i].point;
                                newPosition.y = ceiling.transform.position.y; // y축 고정
                                placeingObject.transform.position = newPosition;
                            }
                            else
                            {
                                testTMP.GetComponent<TextMeshProUGUI>().text = "placeID 0 & Touch Ceiling";
                                Vector3 newPosition = hits[i].point;
                                newPosition.y = ceiling.transform.position.y; // y축 고정
                                placeingObject.transform.position = newPosition;
                            }
                            break;
                        }
                    }
                }
            }
        }
    }

    private void EndPlaceMode()
    {
        Destroy(placeingObject);
        placeingObject = null;
        buttonCanvas.SetActive(true);
        placeMode = false;
        placeID = int.MaxValue;
    }

    public void OnCreateButtonClick(GameObject btn)
    {
        if(buttonCanvas == null)
        {
            buttonCanvas = btn.transform.root.gameObject;
        }

        GameObject prefab = GetComponent<TrackedImageInfomation1>().createdPrefab;

        if(prefab != null)
        {
            switch (btn.name)
            {
                case "obj1":
                    placeingObject = Instantiate(placeableObjects[0]);
                    placeingObject.transform.position = prefab.GetComponent<IndoorObject>().placeableObject.transform.position;

                    testTMP.GetComponent<TextMeshProUGUI>().text = "placeID = 0";

                    placeID = 0;

                    buttonCanvas.SetActive(false);
                    placeMode = true;
                    break;
                case "obj2":
                    placeingObject = Instantiate(placeableObjects[1]);
                    placeingObject.transform.position = prefab.GetComponent<IndoorObject>().placeableObject.transform.position;

                    testTMP.GetComponent<TextMeshProUGUI>().text = "placeID = 1";

                    placeID = 1;

                    buttonCanvas.SetActive(false);
                    placeMode = true;
                    break;
                case "obj3":
                    placeingObject = Instantiate(placeableObjects[2]);
                    placeingObject.transform.position = prefab.GetComponent<IndoorObject>().placeableObject.transform.position;

                    testTMP.GetComponent<TextMeshProUGUI>().text = "placeID = 2";

                    placeID = 2;

                    buttonCanvas.SetActive(false);
                    placeMode = true;
                    break;
                case "obj4":
                    break;
            }
        }
    }

    private void ObjectControl(string value)
    {
        switch (value)
        {
            case "rotate":
                placeingObject.transform.Rotate(0, 0, 45);
                break;
        }
    }

    /// <summary>
    /// 배치, 취소 2개 항목을 위한 함수
    /// </summary>
    /// <param name="value">Ok / Cancel</param>
    public void ObjectPlace(string value)
    {
        switch(value)
        {
            case "ok":
                GameObject newPlaceObject = Instantiate(originalPlaceableObjects[placeID]);

                newPlaceObject.transform.position = placeingObject.transform.position;
                newPlaceObject.transform.rotation = placeingObject.transform.rotation;
                newPlaceObject.transform.localScale = placeingObject.transform.localScale;
                break;
        }

        EndPlaceMode();
        Destroy(placeingObject);
        placeingObject = null;
        buttonCanvas.SetActive(true);
        placeMode = false;

        placeID = int.MaxValue;
    }
}
