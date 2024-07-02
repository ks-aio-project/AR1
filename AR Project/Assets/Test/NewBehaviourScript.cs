using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public GameObject tmp;

    private void Start() {
        string _roomOffset = $"pos:{transform.position}\nrot:{transform.rotation}";
        tmp.GetComponent<TextMeshProUGUI>().text = _roomOffset;
    }

    private void Update() {
        transform.rotation = Quaternion.identity;
    }
}
