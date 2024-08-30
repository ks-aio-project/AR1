using Firebase;
using Firebase.Extensions;
using Firebase.Messaging;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Notifications.Android;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FirebaseInit : MonoBehaviour
{
    string CHANNEL_ID = "myChannel";
    int apiLevel;
    public GameObject notificationCanvas, notificationImage, notificationPanel, notificationFloorPlanImage, notificationText;
    public List<Texture> notificationTextures;

    public List<Texture> floorPlanImages;

    public string abc;

    public List<Button> floorButtons;

    Dictionary<string, int> notificationImageKeyValue = new Dictionary<string, int>();

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
        Debug.Log($"KKS : Start");
#if UNITY_ANDROID
        Debug.Log($"KKS : Push");
        InitializeAndroidLocalPush();
        Debug.Log($"KKS : FCM");
        InitializeFCM();
#endif

        Debug.Log($"KKS abc ? : {abc}");
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
                Debug.Log("btn ok");
                //btn.GetComponent<Button>().Select();
                //btn.GetComponent<Image>().color = new Color(195, 195, 195, 255);
                notificationFloorPlanImage.GetComponent<RawImage>().texture = floorPlanImages[i];
                Debug.Log($"notificationFloorPlanImage.GetComponent<RawImage>().texture : {notificationFloorPlanImage.GetComponent<RawImage>().texture}");
                Debug.Log($"floorPlanImages[i] : {floorPlanImages[i].name}");
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

        notificationText.GetComponent<TextMeshProUGUI>().text = $"발생 : {bodySplit[0]}\n" +
            $"\n장소 : {bodySplit[1]} / {bodySplit[2]}";

        // 어플리케이션 활성화 중이면 바로 도면 띄움
        if (Application.isFocused)
        {
            // x층, xx층을 분리
            int floor = int.Parse(bodySplit[1].Split("층")[0]);

            notificationFloorPlanImage.GetComponent<RawImage>().texture = floorPlanImages[floor - 1];

            notificationPanel.SetActive(true);
            notificationFloorPlanImage.SetActive(true);
            notificationFloorPlanImage.GetComponent<RawImage>().texture = floorPlanImages[0];

            floorButtons[floor - 1].GetComponent<Button>().Select();
            EventSystem.current.SetSelectedGameObject(floorButtons[floor - 1].gameObject);
        }


        switch (bodySplit[0])
        {
            case "화재":                
                notificationImage.GetComponent<RawImage>().texture = notificationTextures[notificationImageKeyValue[bodySplit[0]]];

                if (GetComponent<TrackedImageInfomation1>().currentTrackingObjectName == "room1")
                {
                    GetComponent<TrackedImageInfomation1>().createdPrefab.GetComponent<IndoorObject>().exitObject.GetComponent<ExitScript>().StartExit();
                }
                break;

            case "누수":
                notificationImage.GetComponent<RawImage>().texture = notificationTextures[notificationImageKeyValue[bodySplit[0]]];
                break;

            case "누전":
                notificationImage.GetComponent<RawImage>().texture = notificationTextures[notificationImageKeyValue[bodySplit[0]]];
                break;

            case "데이터 통신 오류":
                notificationImage.GetComponent<RawImage>().texture = notificationTextures[notificationImageKeyValue[bodySplit[0]]];
                break;

            case "냉난방 에너지 손실 감지":
                notificationImage.GetComponent<RawImage>().texture = notificationTextures[notificationImageKeyValue[bodySplit[0]]];
                break;
        }

        var notification = new AndroidNotification();
        notification.SmallIcon = "icon_0";
        notification.Title = title;
        notification.Text = body;
        abc = body;
        Debug.Log($"KKS abc is : {abc}");

        if (apiLevel >= 26)
        {
            AndroidNotificationCenter.SendNotification(notification, CHANNEL_ID);
        }
        else
        {
            Debug.LogError("Android 8.0 이상의 디바이스에서만 푸시 알림이 정상적으로 표시됩니다.");
        }
    }

    public void TestNotification()
    {
        string type = "";
        string title = "";
        string body = "";

        type = "notification";
        title = "알림";
        body = "화재/4층/4010호";

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
            Debug.LogError("Android 8.0 이상의 디바이스에서만 푸시 알림이 정상적으로 표시됩니다.");
        }
    }
}
