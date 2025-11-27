using System.Collections.Generic;
using UnityEngine;
//#if UNITY_EDITOR
//using UnityEditor;

//[CustomEditor(typeof(Loading_Hint))]
//public class Loading_Hint_Editor : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        GUIStyle fontStyle = new GUIStyle(GUI.skin.button);
//        fontStyle.fontSize = 15;
//        fontStyle.normal.textColor = Color.yellow;

//        Loading_Hint Inspector = target as Loading_Hint;
//        if (GUILayout.Button("움직이는 글자 세팅", fontStyle, GUILayout.Height(30f)))
//        {
//            Inspector.UpdateData();
//            EditorUtility.SetDirty(Inspector);
//        }
//        base.OnInspectorGUI();
//    }
//}
//#endif

public class Loading_Hint : MonoBehaviour
{
    private List<string> hintStrings = new List<string>();
    public TMPro.TMP_Text hintText;

    public void SetStart()
    {
        hintStrings.Clear();
        foreach (var child in Singleton_Data.INSTANCE.Dict_Language)
        {
            if (child.Key.Contains("ht_"))// 힌트만 추출
            {
                hintStrings.Add(child.Key);
            }
        }
    }

    public void SetHint()
    {
        if (Option_Manager.current == null)
        {
            hintText.text = "";
            return;
        }
        string key = hintStrings[Random.Range(0, hintStrings.Count)];
        hintText.text = Singleton_Data.INSTANCE.GetLanguage(key);
    }
}
