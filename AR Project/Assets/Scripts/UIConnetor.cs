using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIConnetor : MonoBehaviour
{
    public GameObject button_ok, button_cancel, button_rotation;

    private void Update()
    {
        button_ok.transform.rotation = Quaternion.LookRotation(button_ok.transform.position - Camera.main.transform.position);
        button_cancel.transform.rotation = Quaternion.LookRotation(button_cancel.transform.position - Camera.main.transform.position);
        button_rotation.transform.rotation = Quaternion.LookRotation(button_rotation.transform.position - Camera.main.transform.position);
    }
}
