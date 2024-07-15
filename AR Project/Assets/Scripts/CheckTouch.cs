using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using static ObjectExplane;

public class CheckTouch : MonoBehaviour
{
    [SerializeField]
    private ARRaycastManager raycastManager;
    [SerializeField]
    private ARPlaneManager arPlaneManager;

    [SerializeField]
    private GameObject TextCanvas;

    [SerializeField]
    private GameObject testRoom;
    
    private List<ARRaycastHit> hitList = new List<ARRaycastHit>();

    float cameraHeight;

    void Update()
    {
        if (arPlaneManager.trackables.count > 0)
        {
            // 가장 가까운 평면을 찾습니다.
            ARPlane closestPlane = null;
            float closestDistance = float.MaxValue;

            foreach (var plane in arPlaneManager.trackables)
            {
                float distance = Vector3.Distance(Camera.main.transform.position, plane.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPlane = plane;
                }
            }

            if (closestPlane != null)
            {
                // 카메라의 높이를 계산합니다.
                cameraHeight = Camera.main.transform.position.y - closestPlane.transform.position.y;
                
                Debug.Log($"Camera height from ground: {cameraHeight}");
            }
        }

        if (Input.touchCount > 0)
        {
            Debug.Log("KKS Touch");
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider != null && hit.collider.CompareTag("Touchable"))
                    {
                        Debug.Log($"KKS Object Touch : {hit.collider.name}");
                        Vector3 offset = new Vector3(0.1f, 0, 0);

                        TextCanvas.GetComponentInChildren<TextMeshProUGUI>().text = "";
                        TextCanvas.transform.position = hit.collider.transform.position + offset;

                        switch (hit.collider.name)
                        {
                            case "air":
                                GetRequestFun("https://jsonplaceholder.typicode.com/posts/1");
                                break;
                            case "tv":
                                GetRequestFun("https://jsonplaceholder.typicode.com/posts");
                                break;
                            case "light":
                                Debug.Log($"KKS Light Touch");
                                break;
                        }
                        // 플레인과 동시 터치 방지
                        return;
                    }
                }

                if (raycastManager.Raycast(Input.GetTouch(0).position, hitList, TrackableType.PlaneWithinPolygon))
                {
                    Debug.Log($"KKS Touch Ray Plane");
                    var hitPose = hitList[0].pose;
                    GameObject obj = Instantiate(raycastManager.raycastPrefab, hitPose.position, hitPose.rotation);
                    GameObject obj1 = Instantiate(testRoom, hitPose.position, hitPose.rotation);
                }
            }
        }
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
                //Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                Debug.Log($"KKS web Error : {webRequest.error}");
                TextCanvas.transform.GetComponentInChildren<TextMeshProUGUI>().text = webRequest.error;
            }
            else
            {
                Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                if (webRequest.downloadHandler.text.Length > 20)
                {
                    User user = JsonUtility.FromJson<User>(webRequest.downloadHandler.text);

                    Debug.Log($"KKS web success / UserID : {user.userId} / ID : {user.id} / Title : {user.title} / Body : {user.body}");
                    // 변환된 데이터 출력

                    TextCanvas.transform.GetComponentInChildren<TextMeshProUGUI>().text = $"UserID : {user.userId}\n" +
                        $"ID : {user.id}\n" +
                        $"Title : {user.title}\n" +
                        $"Body : {user.body}\n";
                }
                else
                {
                    User user = JsonUtility.FromJson<User>(webRequest.downloadHandler.text);

                    Debug.Log($"KKS web success / UserID : {user.userId} / ID : {user.id} / Title : {user.title} / Body : {user.body}");
                    TextCanvas.transform.GetComponentInChildren<TextMeshProUGUI>().text = webRequest.downloadHandler.text;
                }
            }
        }
    }
}