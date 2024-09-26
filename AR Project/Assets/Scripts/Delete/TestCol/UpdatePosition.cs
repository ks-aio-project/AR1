using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// µð¹ö±ë¿ë
/// </summary>
public class UpdatePosition : MonoBehaviour
{
    void Update()
    {
        Debug.Log($"!@# Update Pos Camera : {Camera.main.transform.position}");
        Debug.Log($"!@# Update Pos {transform.name} : {transform.position}");
        Debug.Log($"!@# Update LocalPos {transform.name} : {transform.localPosition}");
        Debug.Log($"!@# Update {transform.name} Parent : {transform.parent}");
    }
}
