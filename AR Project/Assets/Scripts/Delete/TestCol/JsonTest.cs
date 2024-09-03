using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Android;

public class JsonTest : MonoBehaviour
{
    public FirebaseInit firebaseInit;
    AndroidJavaObject _pluginInstance;

    [System.Serializable]
    public class Alert
    {
        public string category;
        public string alert_type;
        public string alert_location;
        public string alert_time;
    }


    private void Awake()
    {
        Application.runInBackground = true;
        var pluginClass = new AndroidJavaClass("kr.allione.mylibrary.UnityPlug");

        _pluginInstance = pluginClass.CallStatic<AndroidJavaObject>("instance");

        Debug.Log("kks Awake");
    }

    private void Start()
    {
        //_pluginInstance.Call("unitySendMessage", gameObject.name, "CallByAndroid", "Hello Android Toast");
        _pluginInstance.Call("startService");

        Debug.Log("kks Start");
    }

    void CallByAndroid(string message)
    {
        _pluginInstance.Call("showToast", message);

        Debug.Log("kks call android");
    }

    public void OnStartService()
    {
        //_pluginInstance.Call("unitySendMessage", gameObject.name, "CallByAndroid", "Hello Android Toast222");
        _pluginInstance.Call("unitySendMessage", gameObject.name, "StartService", "");
        Debug.Log("kks OnStartService");
    }

    // 이 메서드는 Android에서 Unity로 데이터를 전달할 때 호출됩니다.
    public void OnApiResponseReceived(string data)
    {
        Alert alert = JsonUtility.FromJson<Alert>(data);
        if(alert.category == "notification")
        {
            firebaseInit.JsonNotification(alert.alert_type, alert.alert_location, alert.alert_time);
        }
    }

    private void ProcessApiResponse(string jsonData)
    {
        Debug.Log($"kks jsonData : {jsonData}");
        // jsonData를 파싱하거나 데이터 구조로 변환
        // 예를 들어, JSON 유틸리티를 사용하여 데이터를 파싱할 수 있습니다.
        // MyData data = JsonUtility.FromJson<MyData>(jsonData);

        // 데이터를 활용한 게임 로직 추가
    }

    public void StartService()
    {
        Debug.Log("kks call startService");
        _pluginInstance.Call("startService");
    }

    public void StopService()
    {
        _pluginInstance.Call("stopService");
        Debug.Log("kks call stopService");
    }
}