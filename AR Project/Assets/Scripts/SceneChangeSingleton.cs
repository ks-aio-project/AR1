using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChangeSingleton : MonoBehaviour
{
    // 싱글톤 인스턴스를 저장할 정적 변수
    private static SceneChangeSingleton _instance;

    public bool sceneChangeAble = true;
    public string changeName = "";

    // 싱글톤 인스턴스에 접근할 수 있는 속성
    public static SceneChangeSingleton Instance
    {
        get
        {
            if (_instance == null)
            {
                // 인스턴스가 없으면 검색
                _instance = FindObjectOfType<SceneChangeSingleton>();

                // 그래도 없으면 새로 생성
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("SingletonExample");
                    _instance = singletonObject.AddComponent<SceneChangeSingleton>();
                }
            }
            return _instance;
        }
    }

    // Awake는 유니티의 생명주기에서 가장 먼저 실행됨
    private void Awake()
    {
        // 싱글톤 인스턴스가 없으면 자신을 할당
        if (_instance == null)
        {
            _instance = this;

            // 다른 씬으로 넘어가도 파괴되지 않도록 설정
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // 인스턴스가 이미 존재하면 새로 생성된 객체를 파괴
            if (_instance != this)
            {
                Destroy(gameObject);
            }
        }
    }

    // 싱글톤이 제대로 동작하는지 테스트할 메서드
    public void DoSomething()
    {
        Debug.Log("SingletonExample is working.");
    }
}