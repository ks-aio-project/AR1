using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class WelcomeWindow : EditorWindow
{
    private const string FirstRunKey = "FirstRunCompleted";
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
        window.minSize = new Vector2(250, 150);
    }

    private void OnGUI()
    {
        // �˾�â�� ������ �ۼ��մϴ�.
        GUILayout.Label("AR Project", EditorStyles.boldLabel);
        GUILayout.Label("�� â�� ��Ÿ���� �ʰ� �Ϸ��� Editor ������ WelcomeWindow.cs�� ����", EditorStyles.wordWrappedLabel);
        GUILayout.Label("����� Active Input Handling�� Both�̰�, Input System Package�� ����϶�� ��Ÿ���� ���� �� ��.", EditorStyles.wordWrappedLabel);
        GUILayout.Label("��ǲŸ���� �ݵ�� Both������", EditorStyles.wordWrappedLabel);

        if (GUILayout.Button("Close"))
        {
            this.Close();
        }
    }
}
