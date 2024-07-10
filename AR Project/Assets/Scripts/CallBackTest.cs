using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallBackTest : MonoBehaviour
{
    [SerializeField]
    private GameObject arObjectPrefab;
    bool _init = false;

    Vector3 holdPosition;
    Quaternion holdRotation;

    List<GameObject> objList = new List<GameObject>();

    public void CreateObject(Vector3 position)
    {
        if (!_init)
        {
            _init = true;
        }
    }

    private void Update()
    {
        if (objList.Count == 0 && _init)
        {
            Vector3 offset = new Vector3(0, -15, 0);
            GameObject objtt = Instantiate(arObjectPrefab, Camera.main.transform.position, Quaternion.identity);
            objtt.transform.localScale = Vector3.one;

            //holdPosition = objtt.transform.position;
            //holdRotation = objtt.transform.rotation;

            objList.Add(objtt);

            _init = false;
        }

        //if (objList.Count > 0)
        //{
        //    objList[0].transform.SetPositionAndRotation(holdPosition, holdRotation);
        //}
    }

    public void OnBtn()
    {
        Vector3 offset = new Vector3(0, 0, -5);
        GameObject objtt = Instantiate(arObjectPrefab, Camera.main.transform.position, Quaternion.Euler(new Vector3(0, 0, 0)));
    }
}
