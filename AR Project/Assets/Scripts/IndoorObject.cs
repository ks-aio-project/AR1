using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndoorObject : MonoBehaviour
{
    [Header("��ġ ������ ������Ʈ")]
    public List<GameObject> touchableObjects = new List<GameObject>();

    [Space(10f)]

    [Header("��ġ ������ ������Ʈ")]
    public GameObject placeableObject;
    public GameObject wallObject;

    [Header("������Ʈ�� �Ҵ�� ��ư")]
    public GameObject button_air_explane, button_air_history;
    public GameObject button_tv_explane, button_tv_history;
}
