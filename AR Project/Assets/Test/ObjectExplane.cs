using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ObjectExplane : MonoBehaviour
{
    public class User
    {
        public int userId;
        public int id;
        public string title;
        public string body;
    }

    [SerializeField]
    ARRaycastManager arRaycastManager;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    public GameObject descriptionPanel;
    public Camera arCamera;
    public Vector3 offset = new Vector3(0.1f, 0, 0);

    void Start()
    {
        descriptionPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                Debug.Log("kks Touch");
                Ray ray = arCamera.ScreenPointToRay(touch.position);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider != null && hit.collider.CompareTag("Touchable"))
                    {
                        ShowDescription(hit.collider.gameObject);
                    }
                }
            }
        }
    }

    private void ShowDescription(GameObject touchedObject)
    {
        if (touchedObject.name.Contains("Room1"))
        {
            GetRequestFun("https://jsonplaceholder.typicode.com/posts/1");
        }
        else if (touchedObject.name.Contains("FloorLight"))
        {
            GetRequestFun("https://jsonplaceholder.typicode.com/posts");
        }

        Vector3 worldPosition = touchedObject.transform.position + offset;
        descriptionPanel.transform.position = worldPosition;
        descriptionPanel.transform.LookAt(arCamera.transform);
        descriptionPanel.transform.Rotate(0, 180, 0);
        descriptionPanel.SetActive(true);
    }

    public void GetRequestFun(string url)
    {
        StartCoroutine(GetRequest(url));
    }

    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                if(webRequest.error.Length > 20)
                {
                    descriptionPanel.GetComponent<TextMesh>().text =
                        descriptionPanel.GetComponent<TestTextMeshSort>().AddLineBreaks(webRequest.error, 20);
                }
                else
                {
                    descriptionPanel.GetComponent<TextMesh>().text = webRequest.error;
                }
            }
            else
            {
                Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                if (webRequest.downloadHandler.text.Length > 20)
                {
                    User user = JsonUtility.FromJson<User>(webRequest.downloadHandler.text);

                    // 변환된 데이터 출력
                    descriptionPanel.GetComponent<TextMesh>().text = $"UserID : {user.userId}\n" +
                        $"ID : {user.id}\n" +
                        $"Title : {user.title}\n" +
                        $"Body : {user.body}\n";
                }
                else
                {
                    descriptionPanel.GetComponent<TextMesh>().text = webRequest.downloadHandler.text;
                }
            }
        }
    }
}
