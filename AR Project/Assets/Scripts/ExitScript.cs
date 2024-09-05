using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitScript : MonoBehaviour
{
    public float duration = 1.0f; // ���� ��ȯ�� �ɸ��� �ð�
    public bool isInit = false;

    Material _material;

    void Start()
    {
        _material = GetComponent<Renderer>().material;

        _material = Instantiate(_material);
        //gameObject.SetActive(false);
    }

    public void StartExit()
    {
        Debug.Log("kks StartExit!");
        isInit = true;
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
            _material.color = Color.Lerp(startColor, endColor, time / duration);
            time += Time.deltaTime;

            // �� ������ ���
            yield return null;
        }

        // ������ ���� ���� (���� ����)
        _material.color = endColor;
    }
}
