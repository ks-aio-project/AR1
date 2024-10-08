using Firebase;
using Firebase.Extensions;
using Firebase.Messaging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using TMPro;
using Unity.Notifications.Android;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FirebaseInit : MonoBehaviour
{
    string CHANNEL_ID = "myChannel";
    int apiLevel;
    public GameObject notificationCanvas, notificationImage, notificationPanel, notificationFloorPlanImage, notificationText;
    public List<Texture> notificationTextures;

    public List<Texture> floorPlanImages;

    public string alert_seq;
    
    public List<Button> floorButtons;

    Dictionary<string, int> notificationImageKeyValue = new Dictionary<string, int>();


    void Start()
    {
        for (int i = 0; i < notificationTextures.Count; i++)
        {
            switch (notificationTextures[i].name)
            {
                case "fire":
                    notificationImageKeyValue.Add("ȭ��", i);
                    break;
                case "water":
                    notificationImageKeyValue.Add("����", i);
                    break;
                case "data":
                    notificationImageKeyValue.Add("������ ��� ����", i);
                    break;
                case "electric":
                    notificationImageKeyValue.Add("����", i);
                    break;
                case "air":
                    notificationImageKeyValue.Add("�ó��� ������ �ս� ����", i);
                    break;
            }
        }
#if UNITY_ANDROID && !UNITY_EDITOR
        InitializeAndroidLocalPush();
        InitializeFCM();
#endif
    }

    public void InitializeAndroidLocalPush()
    {
        Debug.Log($"KKS : Enter Push");
        string androidInfo = SystemInfo.operatingSystem;
        Debug.Log("androidInfo: " + androidInfo);
        apiLevel = int.Parse(androidInfo.Substring(androidInfo.IndexOf("-") + 1, 2));
        Debug.Log("apiLevel: " + apiLevel);

        if (apiLevel >= 33 &&
            !Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
        {
            Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
        }

        if (apiLevel >= 26)
        {
            var channel = new AndroidNotificationChannel()
            {
                Id = CHANNEL_ID,
                Name = "test",
                Importance = Importance.High,
                Description = "for test",
            };
            AndroidNotificationCenter.RegisterNotificationChannel(channel);
        }
    }

    public void InitializeFCM()
    {
        Debug.Log($"KKS : Enter FCM");
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                Debug.Log("Google Play version OK");

                FirebaseMessaging.TokenReceived += OnTokenReceived;
                FirebaseMessaging.MessageReceived += OnMessageReceived;
                FirebaseMessaging.RequestPermissionAsync().ContinueWithOnMainThread(task => {
                    Debug.Log("push permission: " + task.Status.ToString());
                });
            }
            else
            {
                Debug.LogError(string.Format(
                    "Could not resolve all Firebase dependencies: {0}",
                    dependencyStatus
                ));
            }
        });
    }

    public void FloorChangeButtonClick(GameObject btn)
    {
        Debug.Log("button click");
        for(int i =  0; i < floorButtons.Count; i++)
        {
            if (floorButtons[i].gameObject == btn)
            {
                Debug.Log($"kks if floorButtons?");
                notificationFloorPlanImage.GetComponent<RawImage>().texture = floorPlanImages[i];
                Debug.Log($"kks if texture? : {floorPlanImages[i].name}");
            }
        }
    }

    public void NotificationClick()
    {
        Debug.Log($"KKS : Enter NotiClick");
        notificationImage.SetActive(false);
        notificationPanel.SetActive(true);
        notificationFloorPlanImage.SetActive(true);
        notificationFloorPlanImage.GetComponent<RawImage>().texture = floorPlanImages[0];
    }

    public void NotificationClose()
    {
        Debug.Log($"KKS : NotiClose");
        notificationImage.SetActive(true);
        notificationCanvas.SetActive(false);
        notificationPanel.SetActive(false);
        StartCoroutine(SendPostRequest());
    }

    public void OnTokenReceived(object sender, TokenReceivedEventArgs token)
    {
        Debug.Log("OnTokenReceived: " + token.Token);
    }

    public void OnMessageReceived(object sender, MessageReceivedEventArgs e)
    {
        Debug.Log($"KKS : OnMessageReceived");
        string type = "";
        string title = "";
        string body = "";

        // for notification message
        if (e.Message.Notification != null)
        {
            type = "notification";
            title = e.Message.Notification.Title;
            body = e.Message.Notification.Body;
        }
        // for data message
        else if (e.Message.Data.Count > 0)
        {
            type = "data";
            title = e.Message.Data["title"];
            body = e.Message.Data["body"];

            foreach (var pair in e.Message.Data)
            {
                Debug.Log($"Message data:{pair.Key}: {pair.Value}");
            }
        }
        Debug.Log("message type: " + type + ", title: " + title + ", body: " + body);

        if(!notificationCanvas.activeSelf)
        {
            notificationCanvas.SetActive(true);
        }

        string[] bodySplit = body.Split("/");

        notificationText.GetComponent<TextMeshProUGUI>().text = $"�˸� : {bodySplit[0]}\n" +
            $"��ġ : {bodySplit[1]}\n" +
            $"�Ͻ� : {bodySplit[2]}";

        // ���ø����̼� Ȱ��ȭ ���̸� �ٷ� ���� ���
        if (Application.isFocused)
        {
            // x��, xx���� �и�
            int floor = int.Parse(bodySplit[1].Split("��")[0]);

            notificationPanel.SetActive(true);
            notificationFloorPlanImage.SetActive(true);
            notificationFloorPlanImage.GetComponent<RawImage>().texture = floorPlanImages[0];

            floorButtons[floor - 1].GetComponent<Button>().Select();
            EventSystem.current.SetSelectedGameObject(floorButtons[floor - 1].gameObject);
        }

        SetNotificationImage(bodySplit[0]);

        var notification = new AndroidNotification();
        notification.SmallIcon = "icon_0";
        notification.Title = title;
        notification.Text = body;

        if (apiLevel >= 26)
        {
            AndroidNotificationCenter.SendNotification(notification, CHANNEL_ID);
        }
        else
        {
            Debug.LogError("Android 8.0 �̻��� ����̽������� Ǫ�� �˸��� ���������� ǥ�õ˴ϴ�.");
        }
    }

    private void SetNotificationImage(string value)
    {
        switch (value)
        {
            case "ȭ��":
                notificationImage.GetComponent<RawImage>().texture = notificationTextures[0];

                Debug.Log($"kks ȭ�� room1 ? : {GetComponent<TrackedImageInfomation1>().currentTrackingObjectName}");
                if (GetComponent<TrackedImageInfomation1>().currentTrackingObjectName == "room1")
                {
                    GetComponent<TrackedImageInfomation1>().createdPrefab.GetComponent<IndoorObject>().exitObject.GetComponent<ExitScript>().StartExit();
                }
                break;

            case "����":
                notificationImage.GetComponent<RawImage>().texture = notificationTextures[1];
                break;

            case "����":
                notificationImage.GetComponent<RawImage>().texture = notificationTextures[2];
                break;

            case "������ ��� ����":
                notificationImage.GetComponent<RawImage>().texture = notificationTextures[3];
                break;

            case "�ó��� ������ �ս� ����":
                notificationImage.GetComponent<RawImage>().texture = notificationTextures[4];
                break;
        }
    }

    // ������ POST ��û�� ������ �޼���
    IEnumerator SendPostRequest()
    {
        if (GlobalVariable.Instance.currentAlert != "")
        {
            string url = $"http://192.168.1.155:9080/realtime/reset-sensor/?device_name={GlobalVariable.Instance.alert_seq}&sensor_column={GlobalVariable.Instance.currentAlert}";

            UnityWebRequest request = UnityWebRequest.Put(url, ""); // �ٵ� �ʿ� ���ٸ� �� ���ڿ��� �Ӵϴ�.

            yield return request.SendWebRequest();

            // ��û�� ���� ���� Ȯ��
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("PUT request completed successfully!");
                Debug.Log("Response: " + request.downloadHandler.text);
                GlobalVariable.Instance.alert_seq = "";
                GlobalVariable.Instance.currentAlert = "";
            }
            else
            {
                Debug.Log("PUT request failed: " + request.error);
            }

        }

    }

    /// <summary>
    /// �ȵ���̵忡�� ���� �� �����ͷ� ��Ƽ�����̼� ǥ��
    /// </summary>
    /// <param name="_seq">�˸��� �︰ ��� �̸�</param>
    /// <param name="_location">��� ��ġ</param>
    /// <param name="_type">�˸� ���� (ȭ�� ���� ����...)</param>
    /// <param name="_time">��� �︰ �ð�</param>
    /// <param name="_currentAlert">�︰ ����</param>
    public void JsonNotification(string _seq, string _location, string _type, string _time, string _currentAlert)
    {
        if(GlobalVariable.Instance.currentAlert == "")
        {
            GlobalVariable.Instance.currentAlert = _currentAlert;
            GlobalVariable.Instance.alert_seq = _seq;

            if (!notificationCanvas.activeSelf)
            {
                notificationCanvas.SetActive(true);
            }

            _time = _time.Replace("T", " ");
            notificationText.GetComponent<TextMeshProUGUI>().text = $"�˸� : {_type}\n��ġ : {_location}\n�ð� : {_time}";

            Debug.Log($"kks Notofi : �˸� : {_type} ��ġ : {_location} �ð� : {_time}");

            SetNotificationImage(_type);
            // ���ø����̼� Ȱ��ȭ ���̸� �ٷ� ���� ���
            if (Application.isFocused)
            {
                // x��, xx���� �и�
                string location_floor = _location[0] + "��";

                // xx�� (���� ������ �ϼ��Ǹ� ������ ��)
                string location_detail = _location[0] + "��";
                int floor = int.Parse(location_floor);

                floorButtons[floor - 1].Select();
                EventSystem.current.SetSelectedGameObject(floorButtons[floor - 1].gameObject);

                notificationPanel.SetActive(true);
                notificationFloorPlanImage.SetActive(true);
                notificationFloorPlanImage.GetComponent<RawImage>().texture = floorPlanImages[0];
            }

            notificationImage.GetComponent<RawImage>().color = Color.red;

            if (GetComponent<TrackedImageInfomation1>().currentTrackingObjectName == "room1")
            {
                GetComponent<TrackedImageInfomation1>().createdPrefab.GetComponent<IndoorObject>().exitObject.GetComponent<ExitScript>().StartExit();
            }
        }
    }

    public void LocalNotification(string _type, string _title, string _body)
    {
        string type = _type;
        string title = _title;
        string body = _body;

        if (!notificationCanvas.activeSelf)
        {
            notificationCanvas.SetActive(true);
        }

        string[] bodySplit = body.Split("/");

        notificationText.GetComponent<TextMeshProUGUI>().text = $"�߻� : {bodySplit[0]}\n" +
            $"\n��� : {bodySplit[1]} / {bodySplit[2]}";

        // ���ø����̼� Ȱ��ȭ ���̸� �ٷ� ���� ���
        if (Application.isFocused)
        {
            // x��, xx���� �и�
            int floor = int.Parse(bodySplit[1].Split("��")[0]);

            floorButtons[floor - 1].Select();
            EventSystem.current.SetSelectedGameObject(floorButtons[floor - 1].gameObject);
            notificationFloorPlanImage.GetComponent<RawImage>().texture = floorPlanImages[floor - 1];

            notificationPanel.SetActive(true);
            notificationFloorPlanImage.SetActive(true);
            notificationFloorPlanImage.GetComponent<RawImage>().texture = floorPlanImages[0];
        }

        notificationImage.GetComponent<RawImage>().color = Color.red;

        if (GetComponent<TrackedImageInfomation1>().currentTrackingObjectName == "room1")
        {
            GetComponent<TrackedImageInfomation1>().createdPrefab.GetComponent<IndoorObject>().exitObject.GetComponent<ExitScript>().StartExit();
        }

        var notification = new AndroidNotification();
        notification.SmallIcon = "icon_0";
        notification.Title = title;
        notification.Text = body;

        if (apiLevel >= 26)
        {
            AndroidNotificationCenter.SendNotification(notification, CHANNEL_ID);
        }
        else
        {
            Debug.LogError("Android 8.0 �̻��� ����̽������� Ǫ�� �˸��� ���������� ǥ�õ˴ϴ�.");
        }
    }
}
