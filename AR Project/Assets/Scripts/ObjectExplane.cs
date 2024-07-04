using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ObjectExplane : MonoBehaviour
{
    [SerializeField]
    ARRaycastManager arRaycastManager;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    public GameObject descriptionPanel;
    public Camera arCamera;
    public Vector3 offset = new Vector3(0.1f, 0, 0);

    void Start()
    {
        descriptionPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = arCamera.ScreenPointToRay(touch.position);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider != null && hit.collider.CompareTag("Touchable"))
                    {
                        ShowDescription(hit.collider.gameObject);
                    }
                }
            }
        }
        //if (Input.touchCount > 0)
        //{
        //    Touch touch = Input.GetTouch(0);

        //    Debug.Log("Touch detected!");
        //    if (touch.phase == TouchPhase.Began)
        //    {
        //        Debug.Log("Touch began!");

        //        if (arRaycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
        //        {
        //            Pose hitPose = hits[0].pose;

        //            Ray ray = Camera.main.ScreenPointToRay(touch.position);
        //            RaycastHit hit;

        //            if (Physics.Raycast(ray, out hit))
        //            {
        //                if (hit.collider != null && hit.collider.CompareTag("Touchable"))
        //                {
        //                    Debug.Log("Touched object: " + hit.collider.name);

        //                    OnObjectTouched(hit.collider.gameObject);
        //                }
        //            }
        //        }
        //    }
        //}
    }

    private void ShowDescription(GameObject touchedObject)
    {
        GetRequestFun("https://jsonplaceholder.typicode.com/posts/1");
        Vector3 worldPosition = touchedObject.transform.position + offset;
        descriptionPanel.transform.position = worldPosition;
        descriptionPanel.transform.LookAt(arCamera.transform);
        descriptionPanel.transform.Rotate(0, 180, 0);
        descriptionPanel.SetActive(true);
    }

    public void GetRequestFun(string url)
    {
        StartCoroutine(GetRequest(url));
    }

    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                descriptionPanel.GetComponent<TextMesh>().text = $"{webRequest.error}";
            }
            else
            {
                Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                descriptionPanel.GetComponent<TextMesh>().text = $"{webRequest.downloadHandler.text}";
            }
        }
    }
}
