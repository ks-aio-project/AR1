using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class TextShow : MonoBehaviour
{
    float distanceRange = 4f;
    public List<GameObject> inputFields;

    public GameObject canvas;
    public GameObject tmp;

    int textIndex = 0;

    public class ObjectIdentity
    {
        public string userId;
        public string id;
        public string title;
        public string body;

        //public string objectID;
        //public string companyID;
        //public string asHistory;
        //public string body;
    }

    void Update()
    {
        float distance = Vector3.Distance(Camera.main.transform.position, transform.position);

        if (distance < distanceRange)
        {
            if (!canvas.activeSelf)
            {
                GetRestAPICall();
            }
        }
        else if (distance > distanceRange)
        {
            canvas.SetActive(false);
        }
    }

    private void GetRestAPICall()
    {
        switch (transform.name)
        {
            case "Air":
                switch(textIndex)
                {
                    case 0:
                        GetRequestFun("https://jsonplaceholder.typicode.com/posts/1");
                        break;
                    case 1:
                        GetRequestFun("https://jsonplaceholder.typicode.com/posts/2");
                        break;
                    case 2:
                        break;
                }
                break;
            case "tv":
                switch (textIndex)
                {
                    case 0:
                        GetRequestFun("https://jsonplaceholder.typicode.com/posts/1");
                        break;
                    case 1:
                        GetRequestFun("https://jsonplaceholder.typicode.com/posts/2");
                        break;
                    case 2:
                        break;
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

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                ObjectIdentity Identity = JsonUtility.FromJson<ObjectIdentity>(webRequest.downloadHandler.text);

                // 변환된 데이터 출력
                canvas.SetActive(true);
                canvas.transform.LookAt(Camera.main.transform);
                canvas.transform.Rotate(0, 180, 0);

                inputFields[0].GetComponent<TMP_InputField>().text = Identity.userId;
                inputFields[1].GetComponent<TMP_InputField>().text = Identity.id;
                inputFields[2].GetComponent<TMP_InputField>().text = Identity.title;
                inputFields[3].GetComponent<TMP_InputField>().text = Identity.body;
            }

            //if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            //{
            //    //Debug.LogError(pages[page] + ": Error: " + webRequest.error);
            //}
            //else
            //{
            //    Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
            //    ObjectIdentity Identity = JsonUtility.FromJson<ObjectIdentity>(webRequest.downloadHandler.text);

            //    // 변환된 데이터 출력
            //    canvas.SetActive(true);
            //    canvas.transform.LookAt(Camera.main.transform);
            //    canvas.transform.Rotate(0, 180, 0);

            //    inputFields[0].GetComponent<TMP_InputField>().text = Identity.userId;
            //    inputFields[1].GetComponent<TMP_InputField>().text = Identity.id;
            //    inputFields[2].GetComponent<TMP_InputField>().text = Identity.title;
            //    inputFields[3].GetComponent<TMP_InputField>().text = Identity.body;
                
            //    //tmp.GetComponent<TextMeshProUGUI>().text =
            //    //    $"UserID : {Identity.userId}\n" +
            //    //    $"ID : {Identity.id}\n" +
            //    //    $"Title : {Identity.title}\n" +
            //    //    $"Body : {Identity.body}\n";
            //}
        }
    }

    public void ChangeTextIndex(int value)
    {
        textIndex = value;

        GetRestAPICall();
    }
}
