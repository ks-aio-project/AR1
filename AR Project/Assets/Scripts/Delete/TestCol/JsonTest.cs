using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Android;

public class JsonTest : MonoBehaviour
{
    AndroidJavaObject _pluginInstance;

    private void Awake()
    {
        Application.runInBackground = true;
        var pluginClass = new AndroidJavaClass("kr.allione.mylibrary.UnityPlug");

        _pluginInstance = pluginClass.CallStatic<AndroidJavaObject>("instance");

        Debug.Log("kks Awake");
    }

    private void Start()
    {
        _pluginInstance.Call("unitySendMessage", gameObject.name, "CallByAndroid", "Hello Android Toast");
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
    public void OnApiResponseReceived(string jsonData)
    {
        // jsonData를 처리하는 로직 작성
        Debug.Log("API Response received: " + jsonData);

        // JSON 데이터를 원하는 형태로 변환하여 사용할 수 있습니다.
        // 예: JSON 파싱, UI 업데이트 등
        ProcessApiResponse(jsonData);
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