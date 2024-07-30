using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class Test1 : MonoBehaviour
{
    bool trigger = false;

    UIConnetor uiConnetor;

    private void Start()
    {
        uiConnetor = GetComponentInParent<UIConnetor>();
        uiConnetor.pairs.Add(gameObject, trigger);
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.transform != transform.parent)
        {
            trigger = true;

            uiConnetor.pairs[gameObject] = trigger;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform != transform.parent)
        {
            trigger = false;

            uiConnetor.pairs[gameObject] = trigger;
        }
    }
}
