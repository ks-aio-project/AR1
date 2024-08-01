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
        test1.deviceName = "�ý��� ������";
        test1.modelNmae = "�Ｚ �ý��� ������";
        test1.useElecWeek = "2023. 08. 05";
        test1.eventHistory = "-";

        string json = JsonUtility.ToJson(test1);

        TestIdentity Identity = JsonUtility.FromJson<TestIdentity>(json);

        GetComponent<TextMeshProUGUI>().text =
            $"��ġ�� : {Identity.deviceName}\n" +
            $"�𵨸� : {Identity.modelNmae}\n" +
            $"��ġ�ñ� : {Identity.useElecWeek}" +
            $"Event �߻� �̷� (�ֱ� 12��)\n" +
            $"{Identity.eventHistory}";
    }
}
