using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using System;
using UnityEngine.XR.Interaction.Toolkit.UI;
using UnityEngine.UI;
using Unity.VisualScripting;
using static System.Net.Mime.MediaTypeNames;
using Image = UnityEngine.UI.Image;

public class TextShow : MonoBehaviour
{
    //float distanceRange = 5f;
    public List<GameObject> inputFields;

    public GameObject canvas;
    public GameObject defaultText, eventText;

    public GameObject defaultPanel, eventPanel, historyPanel, asPanel;
    public GameObject button_explane, button_event, button_hisotry, button_as;

    int textIndex = 0;

    [HideInInspector]
    public bool isVisible = false;

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

    public class TestIdentity
    {
        public string category;
        public string deviceName;
        public string modelNmae;
        public string useElecWeek;
        public string eventHistory;
    }

    public class ASHistory
    {
        public string nameNumber;
        public string brandName;
        public string ASHistorys;
    }

    public void SetVisible()
    {
        isVisible = !isVisible;

        canvas.SetActive(isVisible);
    }

    //void Update()
    //{
    //    float distance = Vector3.Distance(Camera.main.transform.position, transform.position);

    //    if (distance < distanceRange)
    //    {
    //        if (!canvas.activeSelf)
    //        {
    //            canvas.SetActive(true);
    //        }
    //    }
    //    else if (distance > distanceRange)
    //    {
    //        canvas.SetActive(false);
    //    }
    //}

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
                        GetRequestFun("https://jsonplaceholder.typicode.com/posts/1");
                        break;
                    case 2:
                        GetRequestFun("https://jsonplaceholder.typicode.com/posts/2");
                        break;
                    case 3:
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
                        GetRequestFun("https://jsonplaceholder.typicode.com/posts/1");
                        break;
                    case 2:
                        GetRequestFun("https://jsonplaceholder.typicode.com/posts/2");
                        break;
                    case 3:
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
                if(transform.name == "Air")
                {
                    if (textIndex == 0)
                    {
                        //TestIdentity Identity = JsonUtility.FromJson<TestIdentity>(webRequest.downloadHandler.text);
                        TestIdentity test1 = new();

                        test1.category = "�ó����";
                        test1.deviceName = "�ý��� ������";
                        test1.modelNmae = "�Ｚ �ý��� ������";
                        test1.useElecWeek = "2023. 08. 05";

                        string json = JsonUtility.ToJson(test1);

                        TestIdentity Identity = JsonUtility.FromJson<TestIdentity>(json);

                        // ��ȯ�� ������ ���
                        canvas.SetActive(true);
                        canvas.transform.LookAt(Camera.main.transform);
                        canvas.transform.Rotate(0, 180, 0);

                        defaultText.GetComponent<TextMeshProUGUI>().text =
                            $"{Identity.category}\n" +
                            $"��ġ�� : {Identity.deviceName}\n" +
                            $"�𵨸� : {Identity.modelNmae}\n" +
                            $"��ġ�ñ� : {Identity.useElecWeek}";
                    }
                    else if (textIndex == 1)
                    {
                        eventText.GetComponent<TextMeshProUGUI>().text =
                            "Event �߻� �̷� (�ֱ� 12��)\n" +
                            "2023. 08. 05. 13:55 ���� ON\n" +
                            "2023. 08. 05. 13:55 �ù���\n" +
                            "2023. 08. 05. 13:55 �µ� ���� 24��\n" +
                            "2023. 08. 05. 13:55 �µ� ���� 23��\n" +
                            "2023. 08. 05. 13:55 �µ� ���� 22��\n" +
                            "2023. 08. 05. 13:55 �µ� ���� 21��\n" +
                            "2023. 08. 05. 13:55 �µ� ���� 20��\n" +
                            "2023. 08. 05. 13:55 �µ� ���� 19��\n" +
                            "2023. 08. 05. 13:56 �µ� ���� 18��\n" +
                            "2023. 08. 05. 17:45 ���� OFF\n" +
                            "2023. 08. 06. 13:55 ���� ON\n" +
                            "2023. 08. 06. 13:55 �ù���\n" +
                            "2023. 08. 06. 13:55 �µ� ���� 24��\n" +
                            "2023. 08. 06. 13:55 �µ� ���� 23��\n" +
                            "2023. 08. 06. 13:55 �µ� ���� 22��\n" +
                            "2023. 08. 06. 13:55 �µ� ���� 21��\n" +
                            "2023. 08. 06. 13:55 �µ� ���� 20��\n" +
                            "2023. 08. 06. 13:55 �µ� ���� 19��\n" +
                            "2023. 08. 06. 13:56 �µ� ���� 18��\n" +
                            "2023. 08. 06. 17:45 ���� OFF";
                    }
                    else
                    {
                        ASHistory test1 = new();

                        test1.nameNumber = "�Ｚ �ý��� ������ / A301-1";
                        test1.brandName = "�Ｚ";
                        test1.ASHistorys = "2024-08-01 �ÿ����� ���� - �ø� ����\n" +
                            "2024-08-02 ���� ���� ���� - �����\n" +
                            "2024-08-03 �۵� �ȵ� �����ڵ� E407 - �ǿܱ� �з� ���� ���� ����";

                        string json = JsonUtility.ToJson(test1);

                        ASHistory Identity = JsonUtility.FromJson<ASHistory>(json);

                        // ��ȯ�� ������ ���
                        canvas.SetActive(true);
                        canvas.transform.LookAt(Camera.main.transform);
                        canvas.transform.Rotate(0, 180, 0);

                        inputFields[0].GetComponent<TMP_InputField>().text = Identity.nameNumber;
                        inputFields[1].GetComponent<TMP_InputField>().text = Identity.brandName;
                        inputFields[2].GetComponent<TMP_InputField>().text = Identity.ASHistorys;
                    }
                }
                else if(transform.name == "tv")
                {
                    if (textIndex == 0)
                    {
                        //TestIdentity Identity = JsonUtility.FromJson<TestIdentity>(webRequest.downloadHandler.text);
                        TestIdentity test1 = new();

                        test1.category = "TV";
                        test1.deviceName = "����Ʈ TV";
                        test1.modelNmae = "�Ｚ ����Ʈ TV";
                        test1.useElecWeek = "2023. 08. 05";

                        string json = JsonUtility.ToJson(test1);

                        TestIdentity Identity = JsonUtility.FromJson<TestIdentity>(json);

                        // ��ȯ�� ������ ���
                        canvas.SetActive(true);
                        canvas.transform.LookAt(Camera.main.transform);
                        canvas.transform.Rotate(0, 180, 0);

                        defaultText.GetComponent<TextMeshProUGUI>().text =
                            $"{Identity.category}\n" +
                            $"��ġ�� : {Identity.deviceName}\n" +
                            $"�𵨸� : {Identity.modelNmae}\n" +
                            $"��ġ�ñ� : {Identity.useElecWeek}";
                    }
                    else if (textIndex == 1)
                    {
                        eventText.GetComponent<TextMeshProUGUI>().text =
                            "Event �߻� �̷� (�ֱ� 12��)\n" +
                            "2023. 08. 05. 13:55 ���� ON\n" +
                            "2023. 08. 05. 13:55 �ù���\n" +
                            "2023. 08. 05. 13:55 �µ� ���� 24��\n" +
                            "2023. 08. 05. 13:55 �µ� ���� 23��\n" +
                            "2023. 08. 05. 13:55 �µ� ���� 22��\n" +
                            "2023. 08. 05. 13:55 �µ� ���� 21��\n" +
                            "2023. 08. 05. 13:55 �µ� ���� 20��\n" +
                            "2023. 08. 05. 13:55 �µ� ���� 19��\n" +
                            "2023. 08. 05. 13:56 �µ� ���� 18��\n" +
                            "2023. 08. 05. 17:45 ���� OFF\n" +
                            "2023. 08. 06. 13:55 ���� ON\n" +
                            "2023. 08. 06. 13:55 �ù���\n" +
                            "2023. 08. 06. 13:55 �µ� ���� 24��\n" +
                            "2023. 08. 06. 13:55 �µ� ���� 23��\n" +
                            "2023. 08. 06. 13:55 �µ� ���� 22��\n" +
                            "2023. 08. 06. 13:55 �µ� ���� 21��\n" +
                            "2023. 08. 06. 13:55 �µ� ���� 20��\n" +
                            "2023. 08. 06. 13:55 �µ� ���� 19��\n" +
                            "2023. 08. 06. 13:56 �µ� ���� 18��\n" +
                            "2023. 08. 06. 17:45 ���� OFF";
                    }
                    else
                    {
                        ObjectIdentity Identity = JsonUtility.FromJson<ObjectIdentity>(webRequest.downloadHandler.text);

                        // ��ȯ�� ������ ���
                        canvas.SetActive(true);
                        canvas.transform.LookAt(Camera.main.transform);
                        canvas.transform.Rotate(0, 180, 0);

                        inputFields[0].GetComponent<TMP_InputField>().text = Identity.userId;
                        inputFields[1].GetComponent<TMP_InputField>().text = Identity.id;
                        inputFields[2].GetComponent<TMP_InputField>().text = Identity.title;
                        inputFields[3].GetComponent<TMP_InputField>().text = Identity.body;
                    }
                }


            }

            //if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            //{
            //    //Debug.LogError(pages[page] + ": Error: " + webRequest.error);
            //}
            //else
            //{
            //    Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
            //    ObjectIdentity Identity = JsonUtility.FromJson<ObjectIdentity>(webRequest.downloadHandler.text);

            //    // ��ȯ�� ������ ���
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

        defaultPanel.SetActive(false);
        eventPanel.SetActive(false);
        historyPanel.SetActive(false);
        asPanel.SetActive(false);

        button_explane.GetComponent<Image>().color = Color.white;
        button_event.GetComponent<Image>().color = Color.white;
        button_hisotry.GetComponent<Image>().color = Color.white;
        button_as.GetComponent<Image>().color = Color.white;

        switch (textIndex)
        {
            case 0:
                GetRestAPICall();
                button_explane.GetComponent<Image>().color = Color.yellow;
                defaultPanel.SetActive(true);
                break;
            case 1:
                GetRestAPICall();
                button_event.GetComponent<Image>().color = Color.yellow;
                eventPanel.SetActive(true);
                break;
            case 2:
                GetRestAPICall();
                button_hisotry.GetComponent<Image>().color = Color.yellow;
                historyPanel.SetActive(true);
                break;
            case 3:
                button_as.GetComponent<Image>().color = Color.yellow;
                asPanel.SetActive(true);
                
                DateTime today = DateTime.Now;
                
                // ���� ��¥ �ٷ� �Է�
                asPanel.transform.GetChild(1).GetComponent<TMP_InputField>().text = today.ToString("yyyy-MM-dd");
                break;
        }
    }

    public void RequestAS()
    {

    }
}
