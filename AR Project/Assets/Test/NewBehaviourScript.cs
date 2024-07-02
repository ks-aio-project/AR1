using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public GameObject room;

    private void Awake() {
        GameObject _room = Instantiate(room);
        _room.transform.parent = null;

        var rootPosition = Camera.main.transform.position;
        _room.transform.position = rootPosition;
    }
}