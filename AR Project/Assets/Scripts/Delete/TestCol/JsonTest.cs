using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class JsonTest : MonoBehaviour
{
    public class TestIdentity
    {
        public string category;
        public string deviceName;
        public string modelNmae;
        public string useElecWeek;
        public string eventHistory;
    }

    void Start()
    {
        TestIdentity test1 = new();

        test1.category = "airConditioner";
        test1.deviceName = "시스템 에어컨";
        test1.modelNmae = "삼성 시스템 에어컨";
        test1.useElecWeek = "2023. 08. 05";
        test1.eventHistory = "-";

        string json = JsonUtility.ToJson(test1);

        TestIdentity Identity = JsonUtility.FromJson<TestIdentity>(json);

        GetComponent<TextMeshProUGUI>().text =
            $"장치명 : {Identity.deviceName}\n" +
            $"모델명 : {Identity.modelNmae}\n" +
            $"설치시기 : {Identity.useElecWeek}" +
            $"Event 발생 이력 (최근 12주)\n" +
            $"{Identity.eventHistory}";
    }
}
