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
        GUILayout.Label("이 창이 나타나지 않게 하려면 Editor 폴더내 WelcomeWindow.cs를 삭제\n" +
            "빌드시 인풋 타입에 대한 다이얼로그가 나오면 '예'\n" +
            "인풋타입은 반드시 Both여야함\n" +
            "인식할 이미지의 정보는 Assets/ReferenceImageLibrary에 있음.\n" +
            "전반적인 코드는 ImageTrackObject오브젝트를 확인" +
            "TrackedImageInfomation1.cs에서 이미지가 인식되었을 때, 특정 오브젝트 생성\n" +
            "그 외 부가적인 설정 및 터치인식 등은 CreatePlaceObject.cs에서 관리\n" +
            "Scripts/Global/GlobalVariable.cs는 전역 변수를 담고 있는 스크립트\n" +
            "각 Prefab별로 코드를 가지고 있는 경우가 있음.\n" +
            "Scripts/TextShow.cs에서 각종 기기들의 정보들을 표현함.\n" +
            "안드로이드 네이티브 연동은 AndroidCommunicate.cs에서 관리\n" +
            "AndroidCommunicate를 가진 오브젝트의 이름이 변경 될 경우 안드로이드에서도 호출시 변경해줘야함\n" +
            "안드로이드에 해당 내용 주석있음", EditorStyles.wordWrappedLabel);

        if (GUILayout.Button("Close"))
        {
            this.Close();
        }
    }
}
