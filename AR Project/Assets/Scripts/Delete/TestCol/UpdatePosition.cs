using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������
/// </summary>
public class UpdatePosition : MonoBehaviour
{
    void Update()
    {
        Debug.Log($"Update Pos Camera : {Camera.main.transform.position}");
        Debug.Log($"Update Pos {transform.name} : {transform.position}");
    }
}
