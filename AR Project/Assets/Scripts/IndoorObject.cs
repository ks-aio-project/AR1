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
}
