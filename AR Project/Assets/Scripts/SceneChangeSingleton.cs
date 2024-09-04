using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChangeSingleton : MonoBehaviour
{
    // �̱��� �ν��Ͻ��� ������ ���� ����
    private static SceneChangeSingleton _instance;

    public bool sceneChangeAble = true;
    public string changeName = "";

    // �̱��� �ν��Ͻ��� ������ �� �ִ� �Ӽ�
    public static SceneChangeSingleton Instance
    {
        get
        {
            if (_instance == null)
            {
                // �ν��Ͻ��� ������ �˻�
                _instance = FindObjectOfType<SceneChangeSingleton>();

                // �׷��� ������ ���� ����
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("SingletonExample");
                    _instance = singletonObject.AddComponent<SceneChangeSingleton>();
                }
            }
            return _instance;
        }
    }

    // Awake�� ����Ƽ�� �����ֱ⿡�� ���� ���� �����
    private void Awake()
    {
        // �̱��� �ν��Ͻ��� ������ �ڽ��� �Ҵ�
        if (_instance == null)
        {
            _instance = this;

            // �ٸ� ������ �Ѿ�� �ı����� �ʵ��� ����
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // �ν��Ͻ��� �̹� �����ϸ� ���� ������ ��ü�� �ı�
            if (_instance != this)
            {
                Destroy(gameObject);
            }
        }
    }

    // �̱����� ����� �����ϴ��� �׽�Ʈ�� �޼���
    public void DoSomething()
    {
        Debug.Log("SingletonExample is working.");
    }
}