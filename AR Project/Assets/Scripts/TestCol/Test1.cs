using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class Test1 : MonoBehaviour
{
    bool trigger = false;

    private void Start()
    {
        GetComponentInParent<UIConnetor>().pairs.Add(gameObject, trigger);
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.transform != transform.parent)
        {
            trigger = true;

            if (GetComponentInParent<UIConnetor>().pairs.ContainsKey(gameObject))
            {
                GetComponentInParent<UIConnetor>().pairs[gameObject] = trigger;
            }

            Debug.Log($"KKS triggerStay : {gameObject.name} / {trigger} / other : {other.name}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform != transform.parent)
        {
            trigger = false;

            if (GetComponentInParent<UIConnetor>().pairs.ContainsKey(gameObject))
            {
                GetComponentInParent<UIConnetor>().pairs[gameObject] = trigger;
            }
            Debug.Log($"KKS triggerExit : {gameObject.name} / {trigger} / other : {other.name}");
        }
    }
}
