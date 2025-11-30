using UnityEditor;
using UnityEngine;
using static Data_Dialog;
using static Data_Event;

namespace P01.Editor
{
    public class Editor_ScriptableObject : EditorWindow
    {
        [MenuItem("Graphics Tool/07. Editor_ScriptableObject")]
        public static void OpenWindow()
        {
            Editor_ScriptableObject window = EditorWindow.GetWindow<Editor_ScriptableObject>("Editor_ScriptableObject");
            window.minSize = new Vector2(500f, 200f);
            window.Show();
        }

        string dataPath = "Assets/01_Resources/Datas/Data_ScriptableObject/";
        string path;
        string id = "ID";
        int endLabel = 0;
        public enum PathType
        {
            None,
            Data_Event,

        }
        public PathType pathType = PathType.None;
        public enum DataType
        {
            Normal,
            Start,
            Reward
        }
        public DataType dataType;

        SerializedObject targetObject;
        [SerializeField] TextStruct eventStruct;
        [SerializeField] EventSelect[] eventSelect;

        private void OnEnable()
        {
            targetObject = new SerializedObject(this);
            eventStruct = default(TextStruct);
            eventSelect = new EventSelect[0];
        }
        Vector2 scrollPosition = Vector2.zero;
        void OnGUI()
        {
            pathType = (PathType)EditorGUILayout.EnumPopup("데이터 타입", pathType, GUILayout.Height(30f));
            path = $"{dataPath}{pathType}/{id}/";
            EditorGUILayout.LabelField("Path", path);
            id = EditorGUILayout.TextField("이벤트 ID", id);
            endLabel = EditorGUILayout.IntField("엔드 라벨", endLabel);
            // 데이터 내용
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            targetObject.Update();
            SerializedProperty property = targetObject.FindProperty("eventStruct");
            EditorGUILayout.PropertyField(property, new GUIContent(property.displayName));
            property = targetObject.FindProperty("eventSelect");
            EditorGUILayout.PropertyField(property, new GUIContent(property.displayName));
            targetObject.ApplyModifiedProperties();
            EditorGUILayout.EndScrollView();

            //EditorGUILayout.ObjectField("", mat, typeof(Material), true, GUILayout.Width(setWidth), GUILayout.Height(setHight));
            GUIStyle fontStyle = new GUIStyle(GUI.skin.button);
            fontStyle.fontSize = 15;
            fontStyle.normal.textColor = Color.yellow;
            if (GUILayout.Button("Create", fontStyle, GUILayout.Height(35)))
            {
                SetScriptableObejct(id);
            }
        }

        void SetScriptableObejct(string _id)
        {
            if (string.IsNullOrEmpty(_id))
                return;

            Data_Event inst = InstanceScriptableObejct(_id);
            if (inst == null) return;

            //inst.eventStruct = 
            EditorUtility.SetDirty(inst);
            EditorGUIUtility.PingObject(inst);// 생성된 오브젝트 추적
        }

        Data_Event InstanceScriptableObejct(string _id)
        {
            switch (dataType)
            {
                case DataType.Normal:

                    break;

                case DataType.Start:

                    break;

                case DataType.Reward:

                    break;
            }
            // 저장
            P01_Utility.FindFolder(path);
            string fileName = $"{_id.Trim()}_{endLabel.ToString("D2")}.asset";

            //파일이 있는지 확인
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(path + fileName);
            if (fileInfo.Exists == true)
            {
                // 처리
                if (EditorUtility.DisplayDialog("파일이 있는지 확인", "같은 이름의 파일이 있음", "OK", "Cancel"))
                {
                    Data_Event inst = ScriptableObject.CreateInstance<Data_Event>();
                    inst.eventStruct = eventStruct;
                    inst.eventSelect = eventSelect;

                    AssetDatabase.CreateAsset(inst, path + fileName);
                    AssetDatabase.Refresh();
                    return inst;
                }
            }
            return null;
        }
    }
}