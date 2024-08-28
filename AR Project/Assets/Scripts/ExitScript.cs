using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitScript : MonoBehaviour
{
    public float duration = 1.0f; // ���� ��ȯ�� �ɸ��� �ð�

    void Start()
    {
        GetComponent<Renderer>().material = Instantiate(GetComponent<Renderer>().material);
        gameObject.SetActive(false);
    }

    public void StartExit()
    {
        gameObject.SetActive(true);
        StartCoroutine(LerpColor());
    }

    private IEnumerator LerpColor()
    {
        while (true)
        {
            // ���������� ������� ��ȭ
            yield return StartCoroutine(ChangeColor(Color.red, Color.white, duration));

            // ������� ���������� ��ȭ
            yield return StartCoroutine(ChangeColor(Color.white, Color.red, duration));
        }
    }

    private IEnumerator ChangeColor(Color startColor, Color endColor, float duration)
    {
        float time = 0;

        while (time < duration)
        {
            // �ð��� ���� ������ ���� ����
            GetComponent<Renderer>().material.color = Color.Lerp(startColor, endColor, time / duration);
            time += Time.deltaTime;

            // �� ������ ���
            yield return null;
        }

        // ������ ���� ���� (���� ����)
        GetComponent<Renderer>().material.color = endColor;
    }
}
