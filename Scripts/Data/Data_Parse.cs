using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Data_Parse : MonoBehaviour
{
#if UNITY_EDITOR
    [Header(" [ EDITOR ] ")]
    public List<Object> ResourceFolders = new List<Object>();

    public virtual void DataSetting()
    {
        sprites = new List<Sprite>();
        audioClip = new List<AudioClip>();
        CSV_Data = new List<TextAsset>();
        npcData = new List<Data_NPC>();
        shipData = new List<Data_Ship>();
        eventData = new List<Data_Event_Start>();
        dialogData = new List<Data_Dialog>();

        if (ResourceFolders.Count == 0)
        {
            Debug.LogError("DataFolders 폴더 필요");
            return;
        }

        string[] paths = new string[ResourceFolders.Count];
        for (int i = 0; i < ResourceFolders.Count; i++)
        {
            paths[i] = AssetDatabase.GetAssetPath(ResourceFolders[i]);
            Debug.LogWarning("File paths : " + paths[i]);
        }

        string[] assets = AssetDatabase.FindAssets("t: Prefab", paths);
        // 데이터 추가
        for (int i = 0; i < assets.Length; i++)
        {
            //var data = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(assets[i]), typeof(Unit_Animation));
            //if (data as Unit_Animation)
            //{
            //    Unit_Animation addData = data as Unit_Animation;
            //    units.Add(addData);
            //    EditorUtility.SetDirty(data);
            //}
            //data = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(assets[i]), typeof(Skill_Set));
            //if (data as Skill_Set)
            //{
            //    Skill_Set addData = data as Skill_Set;
            //    skillSet.Add(addData);
            //    EditorUtility.SetDirty(data);
            //}
        }

        //assets = AssetDatabase.FindAssets("t: Prefab", paths);
        // 데이터 추가
        //for (int i = 0; i < assets.Length; i++)
        //{
        //    var data = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(assets[i]), typeof(Skill_Set));
        //    //if (data as Unit_AI)
        //    //{
        //    //    Unit_AI addData = data as Unit_AI;
        //    //    units.Add(addData);
        //    //    EditorUtility.SetDirty(data);
        //    //}
        //    //else 
        //    if (data as Skill_Set)
        //    {
        //        Skill_Set addData = data as Skill_Set;
        //        skillSet.Add(addData);
        //        EditorUtility.SetDirty(data);
        //    }
        //}

        assets = AssetDatabase.FindAssets("t: Sprite", paths);
        // 데이터 추가
        for (int i = 0; i < assets.Length; i++)
        {
            var data = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(assets[i]), typeof(Sprite));
            Sprite addData = data as Sprite;
            sprites.Add(addData);
            EditorUtility.SetDirty(data);
        }

        assets = AssetDatabase.FindAssets("t: AudioClip", paths);
        for (int i = 0; i < assets.Length; i++)
        {
            var data = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(assets[i]), typeof(AudioClip));
            AudioClip addData = data as AudioClip;
            audioClip.Add(addData);
            EditorUtility.SetDirty(data);
        }

        assets = AssetDatabase.FindAssets("t: ScriptableObject", paths);
        for (int i = 0; i < assets.Length; i++)
        {
            var data = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(assets[i]), typeof(ScriptableObject));
            if (data as Data_Ship)
            {
                Data_Ship temp = data as Data_Ship;
                shipData.Add(temp);
            }
            else if (data as Data_NPC)
            {
                Data_NPC temp = data as Data_NPC;
                npcData.Add(temp);
            }
            else if (data as Data_Event_Start)
            {
                Data_Event_Start temp = data as Data_Event_Start;
                eventData.Add(temp);
            }
            else if (data as Data_Dialog)
            {
                Data_Dialog temp = data as Data_Dialog;
                dialogData.Add(temp);
            }
            EditorUtility.SetDirty(data);
        }

        assets = AssetDatabase.FindAssets("t: TextAsset", paths);// CSV
        for (int i = 0; i < assets.Length; i++)
        {
            var data = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(assets[i]), typeof(TextAsset));
            TextAsset addData = data as TextAsset;
            CSV_Data.Add(addData);
            EditorUtility.SetDirty(data);
        }
    }

    //void SetCSVData()
    //{
    //    if (CSVFolders.Count == 0)
    //    {
    //        Debug.LogError("CSVFolders 폴더 필요");
    //        return;
    //    }

    //    string[] paths = new string[CSVFolders.Count];
    //    for (int i = 0; i < CSVFolders.Count; i++)
    //    {
    //        paths[i] = AssetDatabase.GetAssetPath(CSVFolders[i]);
    //        Debug.LogWarning("File paths : " + paths[i]);
    //    }

    //    string[] assets = AssetDatabase.FindAssets("t: TextAsset", paths);// CSV
    //    for (int i = 0; i < assets.Length; i++)
    //    {
    //        var data = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(assets[i]), typeof(TextAsset));
    //        TextAsset addData = data as TextAsset;
    //        CSV_Data.Add(addData);
    //        EditorUtility.SetDirty(data);
    //    }
    //}

    //==================================================================================
    // Parse
    //==================================================================================

    public static int Parse_Int(string _str)
    {
        if (int.TryParse(_str, out int value))
            return value;
        return 0;
    }

    public static float Parse_Float(string _str)
    {
        if (float.TryParse(_str, out float value))
            return value;
        return 0.0f;
    }

    public static Color HexToColor(string hex)
    {
        ColorUtility.TryParseHtmlString(hex, out Color color);
        return color;
    }

    //public Unit_Animation FindUnit(string _str)
    //{
    //    for (int i = 0; i < units.Count; i++)
    //    {
    //        if (_str.Equals(units[i].ID))
    //            return units[i];
    //    }
    //    return null;
    //}

    //public Sprite FindSprite(string _str)
    //{
    //    for (int i = 0; i < sprites.Count; i++)
    //    {
    //        if (_str.Equals(sprites[i].name))
    //            return sprites[i];
    //    }
    //    Debug.LogError("해당 파일이 없음");
    //    return null;
    //}

    public AudioClip Parse_AudioClip(string _str)
    {
        for (int i = 0; i < audioClip.Count; i++)
        {
            if (_str.Contains(audioClip[i].name))
                return audioClip[i];
        }
        Debug.LogError($"{_str} : 해당 파일이 없음");
        return null;
    }

    public Vector2Int[] Parse_Vector2IntArray(string _str)
    {
        if (_str.Contains(";") == false)
            return new Vector2Int[0];

        //string temp = "-1;0/0;0/1;0/0;0";
        string[] substrings = _str.Split('/');
        Vector2Int[] vectorArray = new Vector2Int[substrings.Length];
        for (int i = 0; i < substrings.Length; i++)
        {
            Vector2Int vector = Parse_Vector2Int(substrings[i]);
            vectorArray[i] = vector;
        }
        return vectorArray;
    }

    public static Vector2Int Parse_Vector2Int(string _str)
    {
        if (_str.Contains(";") == false)
            return default;

        string[] subStrings = _str.Split(';');
        int[] subInt = new int[subStrings.Length];
        for (int i = 0; i < subStrings.Length; i++)
        {
            int index = int.Parse(subStrings[i]);
            subInt[i] = index;
        }
        return new Vector2Int(subInt[0], subInt[1]);
    }

    public Vector2 Parse_Vector2(string _str)
    {
        if (_str.Contains(";") == false)
            return default;

        string[] subStrings = _str.Split(';');
        float[] subFloat = new float[subStrings.Length];
        for (int i = 0; i < subStrings.Length; i++)
        {
            float index = float.Parse(subStrings[i]);
            subFloat[i] = index;
        }
        return new Vector2(subFloat[0], subFloat[1]);
    }

    public Vector3 Parse_Vector3(string _str)
    {
        if (_str.Contains(";") == false)
            return default;

        string[] subStrings = _str.Split(';');
        float[] subInt = new float[subStrings.Length];
        for (int i = 0; i < subStrings.Length; i++)
        {
            float index = float.Parse(subStrings[i]);
            subInt[i] = index;
        }
        return new Vector3(subInt[0], subInt[1], subInt[2]);
    }

    public string[] Parse_IDArray(string _str)
    {
        if (_str.Length == 0)
            return new string[0];

        //string temp = "-1;0/0;0/1;0/0;0";
        string[] substrings = _str.Split('/');
        return substrings;
    }
#endif

    [Header(" [ Resource ] ")]
    /* 구글 스플레트 시트에서 "파일 - 다운로드 - 쉼표로 구분된 값" 으로 저장*/
    public List<TextAsset> CSV_Data = new List<TextAsset>();
    public List<AudioClip> audioClip = new List<AudioClip>();
    public List<Sprite> sprites = new List<Sprite>();
    public List<Data_NPC> npcData = new List<Data_NPC>();
    public List<Data_Ship> shipData = new List<Data_Ship>();
    public List<Data_Event_Start> eventData = new List<Data_Event_Start>();
    public List<Data_Dialog> dialogData = new List<Data_Dialog>();
}
