using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class JsonTest : MonoBehaviour
{
    // ������ �ȵ���̵� �÷����� ĳ�̿�.
    private AndroidJavaObject activityContext = null;
    private AndroidJavaClass javaClass = null;
    private AndroidJavaObject javaClassInstance = null;


    void Start()
    {

        // context������ ���� ���� ����Ƽ�� Activityĳ��.
        // context�� �ȵ���̵� Activity ���°� �̶�� �����ϸ� �ɵ�.
        // using���� �ȿ� �߰�ȣ�� ���� �� �޸𸮿��� Ȯ���ϰ� ������ �����̶�� ��.
        using (AndroidJavaClass activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");

        }

        // �ۼ��� �ڹ� �ڵ��� ������ �ִ� ��Ű���� + .Ŭ���� �̸�
        using (javaClass = new AndroidJavaClass("kr.allione.mylibrary"))
        {
            if (javaClass != null)
            {
                // �̷��� �ν��Ͻ��� ���ϸ� �۵� �ȵǴ°� Ȯ����.
                // �� �ȵǴ°��� �ƽô� �� �����ø� ��� �޾��ּ���Ф�
                javaClassInstance = javaClass.CallStatic<AndroidJavaObject>("instance");

                // �ڹ� �ڵ��ʿ� Context�� �����Ͽ� ����.
                javaClassInstance.Call("setContext", activityContext);
            }

        }

        // �ȵ���̵� �ڵ� �����ϱ�.
        callJava();

    }

    // �ȵ���̵� �ڵ� �����ϱ�.
    void callJava()
    {
        //-----------------�޼��� �Լ� ���� ��û.-------------------------------

        //System.Object[] objs = new System.Object[2];

        //// �޼����� ���� ������Ʈ �̸�.
        //objs[0] = "M";

        //// ������Ʈ�� ���Ե� ��ũ��Ʈ�� �� OutPutLog�̶�� �Լ��� �����ش޶�� ������ �뵵.
        //objs[1] = "OutPutLog";

        //// �ȵ���̵� �÷����ο��� TestLog�Լ��� �����ϰ�, �μ��� objs�� ��������.
        //// �� �������� ����Ƽ�� �����͵� �ȵ���̵�� �ѱ� �� ����.
        //javaClassInstance.Call("TestLog", objs);

        //-------------------�佺Ʈ ���� ���� ��û-----------------------------

        javaClassInstance.Call("ShowToast");

    }





    // �ȵ���̵� ��Ʃ������� sendMessage�� �μ��� �����Ƿ� �� �Լ����� ���� ������ �μ��� �־����.
    // �ȵ���̵� �ʿ��� �����ϴ°��̹Ƿ� ���� ����Ƽ �������� ���Ե� �ʿ䰡 ����.
    // �� ���� �ڵ��� ��� (����Ƽ)Start�Լ� -> (�ȵ���̵�)TestLog�Լ� -> (����Ƽ)OutPutLog�Լ� ������ ���� ��.
    void OutPutLog(string msg)
    {
        t.text = msg;

        Debug.Log(msg);
    }


}
//    public class TestIdentity
//    {
//        public string category;
//        public string deviceName;
//        public string modelNmae;
//        public string useElecWeek;
//        public string eventHistory;
//    }

//    void Start()
//    {
//        TestIdentity test1 = new();

//        test1.category = "airConditioner";
//        test1.deviceName = "�ý��� ������";
//        test1.modelNmae = "�Ｚ �ý��� ������";
//        test1.useElecWeek = "2023. 08. 05";
//        test1.eventHistory = "-";

//        string json = JsonUtility.ToJson(test1);

//        TestIdentity Identity = JsonUtility.FromJson<TestIdentity>(json);

//        GetComponent<TextMeshProUGUI>().text =
//            $"��ġ�� : {Identity.deviceName}\n" +
//            $"�𵨸� : {Identity.modelNmae}\n" +
//            $"��ġ�ñ� : {Identity.useElecWeek}" +
//            $"Event �߻� �̷� (�ֱ� 12��)\n" +
//            $"{Identity.eventHistory}";
//    }
//}
