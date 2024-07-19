using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIConnetor : MonoBehaviour
{
    public GameObject button_ok, button_cancel;

    private void Update()
    {
        button_ok.transform.rotation = Quaternion.LookRotation(button_ok.transform.position - Camera.main.transform.position);
        button_cancel.transform.rotation = Quaternion.LookRotation(button_cancel.transform.position - Camera.main.transform.position);

    }
}
