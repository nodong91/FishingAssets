using P01.Editor;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;

public class Editor_CreatePrefab : EditorWindow
{
    [MenuItem("Graphics Tool/07. Editor_CreatePrefab")]
    public static void OpenWindow()
    {
        Editor_CreatePrefab window = EditorWindow.GetWindow<Editor_CreatePrefab>("Editor_CreatePrefab");
        window.minSize = new Vector2(500f, 200f);
        window.Show();
    }

    public GameObject objectToConvert; // 프리팹으로 만들 오브젝트 (씬에서 드래그해 할당)
    private string savePath = "Assets/NewPrefabs/"; // 저장할 기본 경로

    void OnGUI()
    {

        GUILayout.Label("Create Prefab Asset", EditorStyles.boldLabel);

        // 씬의 GameObject를 EditorWindow 필드에 할당할 수 있도록 ObjectField 사용
        objectToConvert = (GameObject)EditorGUILayout.ObjectField("Source GameObject", objectToConvert, typeof(GameObject), true);

        // 저장 경로 입력 필드
        savePath = EditorGUILayout.TextField("Save Path", savePath);

        if (GUILayout.Button("Create Prefab"))
        {
            if (objectToConvert != null)
            {
                CreatePrefab();
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Source GameObject is not assigned.", "OK");
            }
        }
    }

    void CreatePrefab()
    {
        // 폴더가 없으면 생성
        P01_Utility.FindFolder(savePath);
        string localPath = savePath + objectToConvert.name + ".prefab";
        localPath = AssetDatabase.GenerateUniqueAssetPath(localPath); // 중복 이름 방지

        // PrefabUtility.SaveAsPrefabAsset를 사용하여 GameObject를 프리팹 에셋으로 저장
        // 이 함수는 씬 오브젝트를 프로젝트 에셋으로 만들고 원본 오브젝트를 인스턴스로 유지합니다.
        GameObject newPrefab = PrefabUtility.SaveAsPrefabAsset(objectToConvert, localPath);

        if (newPrefab != null)
        {
            EditorUtility.DisplayDialog("Success", "Prefab created at: " + localPath, "OK");
            // 프로젝트 창에서 새로 생성된 프리팹을 하이라이트
            Selection.activeObject = newPrefab;
            EditorGUIUtility.PingObject(newPrefab);
        }
        else
        {
            EditorUtility.DisplayDialog("Error", "Failed to create prefab.", "OK");
        }
    }
}
