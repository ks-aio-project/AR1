using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTextMeshSort : MonoBehaviour
{
    TextMesh textMesh;

    private void Start()
    {
        textMesh = GetComponent<TextMesh>();

    }

    public string AddLineBreaks(string text, int interval)
    {
        for (int i = interval; i < text.Length; i += interval + 1) // +1 to account for the inserted newline
        {
            text = text.Insert(i, "\n");
        }
        return text;
    }
}
