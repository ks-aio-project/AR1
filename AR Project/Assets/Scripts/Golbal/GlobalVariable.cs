using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalVariable
{
    private static GlobalVariable instance;

    public Vector3 distribution_box_offset = new Vector3(0f, -0.2f, 1f);
    public Vector3 imageCanvas_offset = new Vector3(0f, 0f, 3f);
    public Vector3 room_offset = new Vector3(2f, -1.25f, 0.5f);

    public static GlobalVariable Instance
    {
        get
        {
            if (null == instance)
            {
                instance = new GlobalVariable();
            }
            return instance;
        }
    }
}