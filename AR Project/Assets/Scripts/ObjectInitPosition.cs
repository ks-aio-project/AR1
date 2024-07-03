using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ObjectInitPosition : MonoBehaviour
{
    public GameObject room1, room2, room3;

    public TextMeshProUGUI tmp;
    DefaultObserverEventHandler[] observerEventHandlers;

    int count = 0;

    private void Start()
    {
        // Scene에서 모든 DefaultObserverEventHandler를 찾아서 가져옴
        observerEventHandlers = FindObjectsOfType<DefaultObserverEventHandler>();

        // 각 DefaultObserverEventHandler에 대해 구독 설정
        foreach (var observer in observerEventHandlers)
        {
            observer.OnTargetFound.AddListener(() => OnTargetFoundHandler(observer.transform));
        }
    }

    // OnTargetFound 이벤트 발생 시 호출될 메서드
    private void OnTargetFoundHandler(Transform observer)
    {
        Debug.Log("Target Found!");
        
        count++;

        tmp.text = $"Target Found / Count : {count}";

        switch(observer.transform.name)
        {
            case "ImageTarget1":
            room1.SetActive(true);
            room1.transform.position = Camera.main.transform.position;
            room2.SetActive(false);
            break;
            case "ImageTarget2":
            room1.SetActive(false);
            room2.SetActive(true);
            room2.transform.position = Camera.main.transform.position;
            break;
        }
    }

    private void OnDestroy()
    {
        // 각 DefaultObserverEventHandler에서 구독 해제
        foreach (var observer in observerEventHandlers)
        {
            observer.OnTargetFound.RemoveListener(() => OnTargetFoundHandler(observer.transform));
        }
    }
}