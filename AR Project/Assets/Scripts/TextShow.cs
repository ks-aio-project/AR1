using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static CheckTouch;
using UnityEngine.Networking;

public class TextShow : MonoBehaviour
{
    public GameObject canvas;
    public GameObject tmp;

    public class User
    {
        public string userId;
        public string id;
        public string title;
        public string body;
    }

    void Update()
    {
        float distance = Vector3.Distance(Camera.main.transform.position, transform.position);
        Debug.Log($"KKS Camera to Touchable Object Dis : {distance}");

        if (distance < 3f)
        {
            if (!canvas.activeSelf)
            {
                switch (transform.name)
                {
                    case "Air":
                        GetRequestFun("https://jsonplaceholder.typicode.com/posts/1");
                        break;
                    case "tv":
                        GetRequestFun("https://jsonplaceholder.typicode.com/posts/2");
                        break;
                }
            }
        }
        else if (distance > 3f)
        {
            canvas.SetActive(false);
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
            }
            else
            {
                Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                User user = JsonUtility.FromJson<User>(webRequest.downloadHandler.text);

                Debug.Log($"KKS web success / UserID : {user.userId} / ID : {user.id} / Title : {user.title} / Body : {user.body}");
                // 변환된 데이터 출력
                canvas.SetActive(true);
                canvas.transform.LookAt(Camera.main.transform);
                canvas.transform.Rotate(0, 180, 0);

                tmp.GetComponent<TextMeshProUGUI>().text =
                    $"UserID : {user.userId}\n" +
                    $"ID : {user.id}\n" +
                    $"Title : {user.title}\n" +
                    $"Body : {user.body}\n";
            }
        }
    }
}
