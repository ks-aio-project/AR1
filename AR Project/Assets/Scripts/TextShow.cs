using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class TextShow : MonoBehaviour
{
    public GameObject canvas;
    public GameObject tmp;

    int textIndex = 0;

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

        if (distance < 3f)
        {
            if (!canvas.activeSelf)
            {
                GetRestAPICall();
            }
        }
        else if (distance > 3f)
        {
            canvas.SetActive(false);
        }
    }

    private void GetRestAPICall()
    {
        switch (transform.name)
        {
            case "Air":
                if (textIndex == 0)
                {
                    GetRequestFun("https://jsonplaceholder.typicode.com/posts/1");
                }
                else
                {
                    GetRequestFun("https://jsonplaceholder.typicode.com/posts/2");
                }
                break;
            case "tv":
                if (textIndex == 0)
                {
                    GetRequestFun("https://jsonplaceholder.typicode.com/posts/3");
                }
                else
                {
                    GetRequestFun("https://jsonplaceholder.typicode.com/posts/4");
                }
                break;
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
            }
            else
            {
                Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                User user = JsonUtility.FromJson<User>(webRequest.downloadHandler.text);

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

    public void ChangeTextIndex(int value)
    {
        textIndex = value;

        GetRestAPICall();
    }
}
