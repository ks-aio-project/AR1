using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreatePlaceObject : MonoBehaviour
{
    public List<GameObject> placeableObjects;
    public List<GameObject> originalPlaceableObjects;

    bool placeMode = false;

    public GameObject ceiling;

    GameObject placeingObject;

    GameObject buttonCanvas;

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
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    // 수정해야함.
                    // 터치시 버튼을 우선 수위로
                    if(!hit.collider.CompareTag("UIButton"))
                    {
                        Vector3 newPosition = hit.point;
                        newPosition.y = ceiling.transform.position.y; // y축 고정
                        placeingObject.transform.position = newPosition;
                    }
                }
            }
        }
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

                    placeingObject.GetComponent<UIConnetor>().button_ok.GetComponent<Button>().onClick.AddListener(() => ObjectPlace(0, "ok"));
                    placeingObject.GetComponent<UIConnetor>().button_cancel.GetComponent<Button>().onClick.AddListener(() => ObjectPlace(0, "cancel"));
                    //p1.transform.position += new Vector3(0, 0f, 0);

                    buttonCanvas.SetActive(false);
                    placeMode = true;
                    break;
                case "obj2":
                    break;
                case "obj3":
                    break;
                case "obj4":
                    break;
            }
        }
    }

    public void ObjectPlace(int id, string result)
    {
        switch(result)
        {
            case "ok":
                GameObject newPlaceObject = Instantiate(originalPlaceableObjects[id]);

                newPlaceObject.transform.position = placeingObject.transform.position;
                newPlaceObject.transform.rotation = placeingObject.transform.rotation;
                newPlaceObject.transform.localScale = placeingObject.transform.localScale;
                break;
        }

        Destroy(placeingObject);
        placeingObject = null;
        buttonCanvas.SetActive(true);
        placeMode = false;
    }
}
