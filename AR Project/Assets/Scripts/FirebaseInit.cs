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

    public int alert_seq;
    
    public List<Button> floorButtons;

    Dictionary<string, int> notificationImageKeyValue = new Dictionary<string, int>();

    Texture notificationFloorPlanImageTexture;

    private void Awake()
    {
        notificationFloorPlanImageTexture = notificationFloorPlanImage.GetComponent<RawImage>().texture;
    }

    void Start()
    {
        for (int i = 0; i < notificationTextures.Count; i++)
        {
            switch (notificationTextures[i].name)
            {
                case "fire":
                    notificationImageKeyValue.Add("화재", i);
                    break;
                case "water":
                    notificationImageKeyValue.Add("누수", i);
                    break;
                case "data":
                    notificationImageKeyValue.Add("데이터 통신 오류", i);
                    break;
                case "electric":
                    notificationImageKeyValue.Add("누전", i);
                    break;
                case "air":
                    notificationImageKeyValue.Add("냉난방 에너지 손실 감지", i);
                    break;
            }
        }
#if UNITY_ANDROID
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
                notificationFloorPlanImageTexture = floorPlanImages[i];
            }
        }
    }

    public void NotificationClick()
    {
        Debug.Log($"KKS : Enter NotiClick");
        notificationImage.SetActive(false);
        notificationPanel.SetActive(true);
        notificationFloorPlanImage.SetActive(true);
        notificationFloorPlanImageTexture = floorPlanImages[0];
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

        notificationText.GetComponent<TextMeshProUGUI>().text = $"알림 : {bodySplit[0]}\n" +
            $"위치 : {bodySplit[1]}\n" +
            $"일시 : {bodySplit[2]}";

        // 어플리케이션 활성화 중이면 바로 도면 띄움
        if (Application.isFocused)
        {
            // x층, xx층을 분리
            int floor = int.Parse(bodySplit[1].Split("층")[0]);

            notificationFloorPlanImageTexture = floorPlanImages[floor - 1];

            notificationPanel.SetActive(true);
            notificationFloorPlanImage.SetActive(true);
            notificationFloorPlanImageTexture = floorPlanImages[0];

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
            Debug.LogError("Android 8.0 이상의 디바이스에서만 푸시 알림이 정상적으로 표시됩니다.");
        }
    }

    private void SetNotificationImage(string value)
    {
        switch (value)
        {
            case "화재":
                notificationImage.GetComponent<RawImage>().texture = notificationTextures[0];

                Debug.Log($"kks 화재 room1 ? : {GetComponent<TrackedImageInfomation1>().currentTrackingObjectName}");
                if (GetComponent<TrackedImageInfomation1>().currentTrackingObjectName == "room1")
                {
                    GetComponent<TrackedImageInfomation1>().createdPrefab.GetComponent<IndoorObject>().exitObject.GetComponent<ExitScript>().StartExit();
                }
                break;

            case "누수":
                notificationImage.GetComponent<RawImage>().texture = notificationTextures[1];
                break;

            case "누전":
                notificationImage.GetComponent<RawImage>().texture = notificationTextures[2];
                break;

            case "데이터 통신 오류":
                notificationImage.GetComponent<RawImage>().texture = notificationTextures[3];
                break;

            case "냉난방 에너지 손실 감지":
                notificationImage.GetComponent<RawImage>().texture = notificationTextures[4];
                break;
        }
    }

    // 서버에 POST 요청을 보내는 메서드
    IEnumerator SendPostRequest()
    {
        Debug.Log($"kks sendPostRequest");
        string url = "http://192.168.1.139/api/alarm/release";

        // seq 값을 포함한 JSON 데이터
        string jsonData = $"{{\"seq\" : {alert_seq}}}";
        Debug.Log($"kks alert_seq : {alert_seq}");

        // 요청 생성
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        // Content-Type 헤더 설정
        request.SetRequestHeader("Content-Type", "application/json");

        // 요청 보내기
        yield return request.SendWebRequest();

        // 요청 결과 처리
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Response: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Error: " + request.error);
        }
    }

    /// <summary>
    /// 안드로이드에서 수신 된 데이터로 노티피케이션 표출
    /// </summary>
    /// <param name="_type">알림 유형 (화재, 누전, 누수)</param>
    /// <param name="_location">감지기 위치 (ex. 3층 회의실)</param>
    /// <param name="_time">감지된 일시</param>
    public void JsonNotification(string _type, int seq, string _location, string _time)
    {
        alert_seq = seq;

        if (!notificationCanvas.activeSelf)
        {
            notificationCanvas.SetActive(true);
        }

        _time = _time.Replace("T", " ");
        notificationText.GetComponent<TextMeshProUGUI>().text = $"알림 : {_type}\n위치 : {_location}\n시간 : {_time}";

        Debug.Log($"kks Notofi : 알림 : {_type} 위치 : {_location} 시간 : {_time}");

        SetNotificationImage(_type);
        // 어플리케이션 활성화 중이면 바로 도면 띄움
        if (Application.isFocused)
        {
            // x층, xx층을 분리
            string location_floor = _location.Split("층")[0];

            // xx실 (추후 도면이 완성되면 적용할 것)
            string location_detail = _location.Split("층")[1];
            int floor = int.Parse(location_floor);

            floorButtons[floor - 1].Select();
            EventSystem.current.SetSelectedGameObject(floorButtons[floor - 1].gameObject);
            notificationFloorPlanImageTexture = floorPlanImages[floor - 1];

            notificationPanel.SetActive(true);
            notificationFloorPlanImage.SetActive(true);
            notificationFloorPlanImageTexture = floorPlanImages[0];
        }

        notificationImage.GetComponent<RawImage>().color = Color.red;

        if (GetComponent<TrackedImageInfomation1>().currentTrackingObjectName == "room1")
        {
            GetComponent<TrackedImageInfomation1>().createdPrefab.GetComponent<IndoorObject>().exitObject.GetComponent<ExitScript>().StartExit();
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

        notificationText.GetComponent<TextMeshProUGUI>().text = $"발생 : {bodySplit[0]}\n" +
            $"\n장소 : {bodySplit[1]} / {bodySplit[2]}";

        // 어플리케이션 활성화 중이면 바로 도면 띄움
        if (Application.isFocused)
        {
            // x층, xx층을 분리
            int floor = int.Parse(bodySplit[1].Split("층")[0]);

            floorButtons[floor - 1].Select();
            EventSystem.current.SetSelectedGameObject(floorButtons[floor - 1].gameObject);
            notificationFloorPlanImageTexture = floorPlanImages[floor - 1];

            notificationPanel.SetActive(true);
            notificationFloorPlanImage.SetActive(true);
            notificationFloorPlanImageTexture = floorPlanImages[0];
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
            Debug.LogError("Android 8.0 이상의 디바이스에서만 푸시 알림이 정상적으로 표시됩니다.");
        }
    }
}
