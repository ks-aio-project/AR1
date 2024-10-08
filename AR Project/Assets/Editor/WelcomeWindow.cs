using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class WelcomeWindow : EditorWindow
{
    private const string FirstRunKey = "FirstRunCompleted";

    private int currentTextIndex = 0; // ���� �ؽ�Ʈ�� �ε����� ������ ����
    private string[] texts = new string[]  // ���� �ؽ�Ʈ�� �迭�� ����
    {
        "�� â�� ��Ÿ���� �ʰ� �Ϸ��� Editor ������ WelcomeWindow.cs�� ����\n" +
        "����� ��ǲ Ÿ�Կ� ���� ���̾�αװ� ������ '��'\n" +
        "��ǲŸ���� �ݵ�� Both������\n" +
        "�ν��� �̹����� ������ Assets/ReferenceImageLibrary�� ����.\n" +
        "�������� �ڵ�� ImageTrackObject������Ʈ�� Ȯ��" +
        "TrackedImageInfomation1.cs���� �̹����� �νĵǾ��� ��, Ư�� ������Ʈ ����\n" +
        "�� �� �ΰ����� ���� �� ��ġ�ν� ���� CreatePlaceObject.cs���� ����\n" +
        "Scripts/Global/GlobalVariable.cs�� ���� ������ ��� �ִ� ��ũ��Ʈ\n" +
        "�� Prefab���� �ڵ带 ������ �ִ� ��찡 ����.\n" +
        "Scripts/TextShow.cs���� ���� ������ �������� ǥ����.\n" +
        "�ȵ���̵� ����Ƽ�� ������ AndroidCommunicate.cs���� ����\n" +
        "AndroidCommunicate�� ���� ������Ʈ�� �̸��� ���� �� ��� �ȵ���̵忡���� ȣ��� �����������\n" +
        "�ȵ���̵忡 �ش� ���� �ּ�����",

        "�����ͺ��̽� ȣ���� GetInstalledEquipment �ڷ�ƾ �Լ��� ����\n" +
        "��� Documents/fast_api/fast_api_server.py�� Ȯ���� ��\n" +
        "���� �ڵ�� uvicorn fast_api_server:app --host 0.0.0.0 --port 9080 --reload",

        "�ȵ���̵� ���� ��δ� Documents/GitHub/AR_PlugIn/UnityPlug"
    };

    static WelcomeWindow()
    {
        // ����Ƽ �����Ͱ� ���۵� �� ȣ��˴ϴ�.
        EditorApplication.update += ShowWindowOnStart;
    }

    private static void ShowWindowOnStart()
    {
        // �˾�â�� �� ���� �����ֵ��� �մϴ�.
        EditorApplication.update -= ShowWindowOnStart;
        // ���� ���� ���� Ȯ��
        //ShowWindow();
        if (!EditorPrefs.GetBool(FirstRunKey, false))
        {
            // ���� ������ ��� �˾�â�� ǥ���ϰ� �÷��׸� �����մϴ�.
            ShowWindow();
            EditorPrefs.SetBool(FirstRunKey, true);
        }
    }

    [MenuItem("Window/������Ʈ ���� â")]
    public static void ShowWindow()
    {
        // �����츦 ǥ���մϴ�.
        var window = GetWindow<WelcomeWindow>("Welcome");
        window.minSize = new Vector2(300, 150);
    }

    private void OnGUI()
    {
        GUILayout.Label($"AR Project [{currentTextIndex + 1}/{texts.Length}]", EditorStyles.boldLabel);

        // ���� �ε����� �´� �ؽ�Ʈ�� ǥ��
        GUILayout.Label($"{texts[currentTextIndex]}", EditorStyles.wordWrappedLabel);

        GUILayout.Space(10); // ��ư ������ �ֱ� ���� ���� �߰�

        GUILayout.BeginHorizontal(); // ���� ��ư�� ���� ��ư�� ���η� ��ġ

        // ���� ��ư
        if (GUILayout.Button("����"))
        {
            if (currentTextIndex > 0)
            {
                currentTextIndex--; // �ε����� ���ҽ��� ���� �ؽ�Ʈ�� �̵�
            }
        }

        // ���� ��ư
        if (GUILayout.Button("����"))
        {
            if (currentTextIndex < texts.Length - 1)
            {
                currentTextIndex++; // �ε����� �������� ���� �ؽ�Ʈ�� �̵�
            }
        }

        GUILayout.EndHorizontal();

        GUILayout.Space(10); // ��ư ������ �ֱ� ���� ���� �߰�

        // â �ݱ� ��ư
        if (GUILayout.Button("Close"))
        {
            this.Close();
        }
    }
}
