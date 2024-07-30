using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class WelcomeWindow : EditorWindow
{
    private const string FirstRunKey = "FirstRunCompleted";
    static WelcomeWindow()
    {
        // 유니티 에디터가 시작될 때 호출됩니다.
        EditorApplication.update += ShowWindowOnStart;
    }

    private static void ShowWindowOnStart()
    {
        // 팝업창을 한 번만 보여주도록 합니다.
        EditorApplication.update -= ShowWindowOnStart;
        // 최초 실행 여부 확인
        if (!EditorPrefs.GetBool(FirstRunKey, false))
        {
            // 최초 실행인 경우 팝업창을 표시하고 플래그를 설정합니다.
            ShowWindow();
            EditorPrefs.SetBool(FirstRunKey, true);
        }
    }

    [MenuItem("Window/프로젝트 설명 창")]
    public static void ShowWindow()
    {
        // 윈도우를 표시합니다.
        var window = GetWindow<WelcomeWindow>("Welcome");
        window.minSize = new Vector2(250, 150);
    }

    private void OnGUI()
    {
        // 팝업창의 내용을 작성합니다.
        GUILayout.Label("AR Project", EditorStyles.boldLabel);
        GUILayout.Label("이 창이 나타나지 않게 하려면 Editor 폴더내 WelcomeWindow.cs를 삭제", EditorStyles.wordWrappedLabel);
        GUILayout.Label("빌드시 Active Input Handling가 Both이고, Input System Package를 사용하라고 나타나면 무시 할 것.", EditorStyles.wordWrappedLabel);
        GUILayout.Label("인풋타입은 반드시 Both여야함", EditorStyles.wordWrappedLabel);

        if (GUILayout.Button("Close"))
        {
            this.Close();
        }
    }
}
