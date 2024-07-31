using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class CreatePlaceObject : MonoBehaviour
{
    public List<GameObject> placeableObjects;
    public List<GameObject> originalPlaceableObjects;

    bool placeMode = false;

    GameObject ceiling;

    GameObject placeingObject;

    GameObject buttonCanvas;

    int placeID = int.MaxValue;

    // 생성된 오브젝트 리셋 기능이 생길 수 있음에 대비해 리스트에 추가
    List<GameObject> placedNewObjects = new List<GameObject>();

    public TextMeshProUGUI textTMP;
    public GameObject imageCanvas;
    public List<Texture> imageList;

    /// <summary>
    /// 생성된 오브젝트 모두 파괴 (리셋)
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

        // 터치
        if (Input.touchCount > 0)
        {
            // UI 터치시 반환
            if(EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                return;
            }

            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit[] hits = Physics.RaycastAll(ray);
                bool touched = false;

                // 배치모드일때
                if(placeMode)
                {
                    // 터치한 구간에 ui가 있을시 ui 우선 순위
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

                    // 터치 구간에 UI가 없었을 경우
                    for (int i = 0; i < hits.Length; i++)
                    {
                        if (EventSystem.current.IsPointerOverGameObject() == false)
                        {
                            if (placeID == 1)
                            {
                                if (hits[i].collider.name.Contains("Wall"))
                                {
                                    Vector3 newPosition;
                                    Vector3 previousPosition = placeingObject.transform.position;
                                    Quaternion previousRotation = placeingObject.transform.rotation;
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
                                    Vector3 previousPosition = placeingObject.transform.position;
                                    Quaternion previousRotation = placeingObject.transform.rotation;
                                    if (placeID == 2)
                                    {
                                        Vector3 newPosition = hits[i].point;
                                        newPosition.y = ceiling.transform.position.y; // y축 고정
                                        placeingObject.transform.position = newPosition;
                                    }
                                    else
                                    {
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
                // 배치 모드 아닐 때
                else
                {
                    Debug.Log("KKS 터치 배치모드 X");
                    for (int i = 0; i < hits.Length; i++)
                    {
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
                                imageCanvas.SetActive(false);
                            }
                        }

                        if (hits[i].collider.CompareTag("PlacedObject"))
                        {
                            // Active 상태가 실행부터 false로 설정되어 있는 경우에는 true값을 넣어 줌.
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

                        Debug.Log("KKS 배전반인지 확인 진입 시도");
                        if (hits[i].collider.name.Contains("distribution_box_"))
                        {
                            Debug.Log("KKS 배전반 확인");

                            imageCanvas.SetActive(true);
                            switch (hits[i].collider.name)
                            {
                                case "distribution_box_1":
                                    imageCanvas.GetComponentInChildren<RawImage>(true).texture = imageList[0];
                                    textTMP.text = "distribution_box_1";
                                    break;
                                case "distribution_box_2":
                                    imageCanvas.GetComponentInChildren<RawImage>(true).texture = imageList[1];
                                    textTMP.text = "distribution_box_2";
                                    break;
                                case "distribution_box_3":
                                    imageCanvas.GetComponentInChildren<RawImage>(true).texture = imageList[2];
                                    textTMP.text = "distribution_box_3";
                                    break;
                                case "distribution_box_4":
                                    imageCanvas.GetComponentInChildren<RawImage>(true).texture = imageList[3];
                                    textTMP.text = "distribution_box_4";
                                    break;
                                case "distribution_box_5":
                                    textTMP.text = "distribution_box_5";
                                    break;
                                case "distribution_box_6":
                                    textTMP.text = "distribution_box_6";
                                    break;
                                case "distribution_box_7":
                                    textTMP.text = "distribution_box_7";
                                    break;
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 오브젝트 배치 종료시 호출하는 함수
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
    /// OnClick 이벤트에 등록되어 있음
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
