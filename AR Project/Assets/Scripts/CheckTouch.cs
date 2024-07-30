using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using static UnityEngine.GraphicsBuffer;

public class CheckTouch : MonoBehaviour
{
    [SerializeField]
    private ARRaycastManager raycastManager;
    [SerializeField]
    private ARPlaneManager arPlaneManager;

    [SerializeField]
    private GameObject TextCanvas;
        
    private List<ARRaycastHit> hitList = new List<ARRaycastHit>();

    private GameObject lastTouchObject;

    public class User
    {
        public string userId;
        public string id;
        public string title;
        public string body;
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider != null && hit.collider.CompareTag("Touchable"))
                    {
                        Vector3 offset = new Vector3(0, -0.3f, 0.1f);

                        TextCanvas.GetComponentInChildren<TextMeshProUGUI>().text = "";
                        TextCanvas.transform.position = hit.collider.transform.position + offset;

                        if(TextCanvas.activeSelf && hit.collider.gameObject == lastTouchObject)
                        {
                            TextCanvas.SetActive(false);
                            return;
                        }

                        // 오브젝트 터치
                        switch (hit.collider.name)
                        {
                            case "air":
                                GetRequestFun("https://jsonplaceholder.typicode.com/posts/1");
                                break;
                            case "tv":
                                GetRequestFun("https://jsonplaceholder.typicode.com/posts/2");
                                break;
                            case "light":
                                Debug.Log($"KKS Light Touch");
                                break;
                        }
                        //lastTouchObject = hit.collider.gameObject;
                        //// 플레인과 동시 터치 방지
                        //return;
                    }
                }

                //if (raycastManager.Raycast(Input.GetTouch(0).position, hitList, TrackableType.PlaneWithinPolygon))
                //{
                //    // 플레인 터치
                //    Debug.Log($"KKS Touch Ray Plane");
                //    var hitPose = hitList[0].pose;

                //    Vector3 spawnPosition = hit.point;

                //    GetComponent<ObjectsController>().CreateOrDestroy("room1", hitPose);
                //}
            }
        }
    }

    public void GetRequestFun(string url)
    {
        TextCanvas.SetActive(false);
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
                //Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                Debug.Log($"KKS web Error : {webRequest.error}");
                TextCanvas.transform.GetComponentInChildren<TextMeshProUGUI>().text = webRequest.error;
            }
            else
            {
                TextCanvas.SetActive(true);
                Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                User user = JsonUtility.FromJson<User>(webRequest.downloadHandler.text);

                Debug.Log($"KKS web success / UserID : {user.userId} / ID : {user.id} / Title : {user.title} / Body : {user.body}");
                // 변환된 데이터 출력

                TextCanvas.transform.GetComponentInChildren<TextMeshProUGUI>().text =
                    $"UserID : {user.userId}\n" +
                    $"ID : {user.id}\n" +
                    $"Title : {user.title}\n" +
                    $"Body : {user.body}\n";
                TextCanvas.transform.rotation = Quaternion.LookRotation(TextCanvas.transform.position - Camera.main.transform.position);

                //TextCanvas.transform.Rotate(0, 180, 0);
            }
        }
    }
}