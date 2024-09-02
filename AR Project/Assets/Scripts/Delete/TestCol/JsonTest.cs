using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Android;

public class JsonTest : MonoBehaviour
{
    private AndroidJavaObject activity;

    void Start()
    {
        Debug.Log("kks start");
#if UNITY_ANDROID && !UNITY_EDITOR
        StartForegroundService();
#endif
    }
    private void StartForegroundService()
    {
        Debug.Log("kks StartForegroundService");
        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            if (activity == null)
            {
                Debug.LogError("kks activity is null!");
                return;
            }

            // Intent ��ü ����
            using (var serviceClass = new AndroidJavaClass("com.example.foregroundservice.MyForegroundService"))
            {
                using (var intent = new AndroidJavaObject("android.content.Intent", activity, serviceClass.Call<AndroidJavaObject>("getClass")))
                {
                    if (intent == null)
                    {
                        Debug.LogError("kks intent is null!");
                        return;
                    }

                    // startService�� activity �ν��Ͻ��� ���� ȣ���ؾ� ��
                    activity.Call("startService", intent);
                }
            }
        }
    }

    void OnApplicationQuit()
    {
        Debug.Log("kks OnApplicationQuit");
#if UNITY_ANDROID && !UNITY_EDITOR
        StopForegroundService();
#endif
    }

    private void StopForegroundService()
    {
        Debug.Log("kks StopForegroundService");
        if (activity != null)
        {
            using (var intent = new AndroidJavaObject("android.content.Intent", activity, new AndroidJavaClass("com.example.foregroundservice.MyForegroundService")))
            {
                // stopService ���� activity �ν��Ͻ��� ���� ȣ��
                activity.Call("stopService", intent);
            }
        }
    }
}