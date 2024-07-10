using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class FixTest : MonoBehaviour
{
    private ARAnchor anchor;

    public TextMeshProUGUI tmp;


    private void Update()
    {
        if(tmp != null)
        {
            tmp.text = $"X : {transform.position.x}\n" +
                $"Y : {transform.position.y}\n" +
                $"Z : {transform.position.z}";
        }

        Debug.Log("KKS FixTest is True");
    }

    public void SetAnchor(ARAnchor anchor)
    {
        this.anchor = anchor;
    }

    public void SetPositionAndRotation(Vector3 position, Quaternion rotation)
    {
        if (anchor != null)
        {
            anchor.transform.position = position;
            anchor.transform.rotation = rotation;
        }
        else
        {
            transform.position = position;
            transform.rotation = rotation;
        }
    }

    public void SetTMP(TextMeshProUGUI _tmp)
    {
        tmp = _tmp;
    }
}
