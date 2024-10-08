using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.XR.ARFoundation;

public class AndroidCommunicate : MonoBehaviour
{
    public FirebaseInit firebaseInit;
    AndroidJavaObject _pluginInstance;

    [System.Serializable]
    public class Alert
    {
        public string category; // ī�װ� (�˸�)
        public string alert_seq; // �˸��� �︰ ��� �̸�
        public string alert_location; // ��� ��ġ
        public string alert_type; // �˸� ���� (ȭ�� ���� ����...)
        public string alert_time; // ��� �︰ �ð�
        public string currentAlert; // �︰ ����
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            Debug.Log("Application Focused (Resumed)");
            // ���ø����̼��� Resume ���¿� ���� �� ������ �ڵ�
            if (_pluginInstance != null)
            {
                _pluginInstance.Call("getSaveSendData");
            }
        }
        else
        {
            Debug.Log("Application Lost Focus (Might be Paused)");
            // ���ø����̼��� ��Ŀ���� �Ҿ��� �� ������ �ڵ�
        }
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

    // �� �޼���� Android���� Unity�� �����͸� ������ �� ȣ��˴ϴ�.
    public void OnApiResponseReceived(string data)
    {
        Debug.Log($"kks getData : {data}");
        Debug.Log($"kks 1234");
        Alert alert = JsonUtility.FromJson<Alert>(data);
        Debug.Log($"kks alert.currentAlert : {alert.currentAlert}");
        if (alert.category == "notification")
        {
            firebaseInit.JsonNotification(alert.alert_seq, alert.alert_location, alert.alert_type, alert.alert_time, alert.currentAlert);
        }
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