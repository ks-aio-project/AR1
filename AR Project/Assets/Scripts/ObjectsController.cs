using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsController : MonoBehaviour
{
    public List<GameObject> objects;

    Dictionary<string, GameObject> objectsDictionary = new Dictionary<string, GameObject>();

    public void CreateOrDestroy(string name, Pose hitPose)
    {
        Debug.Log("KKS CreateOrDestroy");
        if (objectsDictionary.ContainsKey(name))
        {
            Debug.Log("KKS CreateOrDestroy IF");
            GameObject obj = objectsDictionary[name];
            Destroy(obj);

            objectsDictionary.Remove(name);
        }
        else
        {
            Debug.Log("KKS CreateOrDestroy ELSE");
            switch (name)
            {
                case "room1":
                    Debug.Log("KKS CreateOrDestroy ELSE ROOM1");
                    GameObject obj1 = Instantiate(objects[0]);
                    Vector3 offset = new Vector3(obj1.GetComponent<MeshFilter>().mesh.bounds.max.x - obj1.GetComponent<MeshFilter>().mesh.bounds.center.x, 0, -0.5f);
                    obj1.transform.position = hitPose.position + offset;
                    objectsDictionary.Add(name, obj1);
                    Debug.Log($"KKS Camera Pos : {Camera.main.transform.position} / OBJ POS : {obj1.transform.position}");
                    break;
            }
        }
    }
}
