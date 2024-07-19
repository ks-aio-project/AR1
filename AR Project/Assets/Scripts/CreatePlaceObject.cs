using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatePlaceObject : MonoBehaviour
{
    public List<GameObject> placeableObjects;

    public void OnCreateButtonClick(GameObject btn)
    {
        GameObject prefab = GetComponent<TrackedImageInfomation1>().createdPrefab;

        if(prefab != null)
        {
            switch (btn.name)
            {
                case "obj1":
                    GameObject p1 = Instantiate(placeableObjects[0]);
                    p1.transform.localScale = Vector3.one;
                    p1.transform.position = prefab.GetComponent<IndoorObject>().placeableObject.transform.position;
                    p1.transform.position += new Vector3(0, -0.1f, 0);

                    p1.AddComponent<ObjectPlaceSetting>();
                    p1.GetComponent<ObjectPlaceSetting>().ceiling = prefab.GetComponent<IndoorObject>().placeableObject;

                    btn.transform.root.gameObject.SetActive(false);
                    break;
                case "obj2":
                    break;
                case "obj3":
                    break;
                case "obj4":
                    break;
            }
        }
    }
}
