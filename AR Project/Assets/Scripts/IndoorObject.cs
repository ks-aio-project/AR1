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
}
