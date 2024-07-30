using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndoorObject : MonoBehaviour
{
    [Header("터치 가능한 오브젝트")]
    public List<GameObject> touchableObjects = new List<GameObject>();

    [Space(10f)]

    [Header("배치 가능한 오브젝트")]
    public GameObject placeableObject;
    public GameObject wallObject;

    [Header("오브젝트에 할당된 버튼")]
    public GameObject button_air_explane, button_air_history;
    public GameObject button_tv_explane, button_tv_history;
}
