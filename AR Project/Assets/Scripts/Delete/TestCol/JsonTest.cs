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

    // �� �޼���� Android���� Unity�� �����͸� ������ �� ȣ��˴ϴ�.
    public void OnApiResponseReceived(string jsonData)
    {
        // jsonData�� ó���ϴ� ���� �ۼ�
        Debug.Log("API Response received: " + jsonData);

        // JSON �����͸� ���ϴ� ���·� ��ȯ�Ͽ� ����� �� �ֽ��ϴ�.
        // ��: JSON �Ľ�, UI ������Ʈ ��
        ProcessApiResponse(jsonData);
    }

    private void ProcessApiResponse(string jsonData)
    {
        Debug.Log($"kks jsonData : {jsonData}");
        // jsonData�� �Ľ��ϰų� ������ ������ ��ȯ
        // ���� ���, JSON ��ƿ��Ƽ�� ����Ͽ� �����͸� �Ľ��� �� �ֽ��ϴ�.
        // MyData data = JsonUtility.FromJson<MyData>(jsonData);

        // �����͸� Ȱ���� ���� ���� �߰�
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