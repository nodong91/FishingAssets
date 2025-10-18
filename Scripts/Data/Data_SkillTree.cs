using System.Collections.Generic;
using UnityEngine;
using static Data_Manager;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(Data_SkillTree))]
public class DataSkillTree_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        GUIStyle fontStyle = new GUIStyle(GUI.skin.button);
        fontStyle.fontSize = 15;
        fontStyle.normal.textColor = Color.yellow;

        Data_SkillTree Inspector = target as Data_SkillTree;
        if (GUILayout.Button("Data Parse", fontStyle, GUILayout.Height(30f)))
        {
            Inspector.UpdateData();
            EditorUtility.SetDirty(Inspector);
        }
        GUILayout.Space(10f);
        base.OnInspectorGUI();
    }
}
#endif
[CreateAssetMenu(fileName = "Data_SkillTree", menuName = "Scriptable Objects/Data_SkillTree")]
public class Data_SkillTree : ScriptableObject
{
    public TextAsset csvData;
    public Vector2Int skillMapSize, startSlot;
    public List<SkillStatus> skillList = new List<SkillStatus>();
    Dictionary<Vector2Int, SkillStatus> dictSkills = new Dictionary<Vector2Int, SkillStatus>();
    public void UpdateData()
    {
        skillList = new List<SkillStatus>();
        SetFish(csvData);
    }

    void SetFish(TextAsset _textAsset)
    {
        dictSkills = new Dictionary<Vector2Int, SkillStatus>();
        skillList = new List<SkillStatus>();
        string[] data = _textAsset.text.Split(new char[] { '\n' });
        for (int i = 1; i < data.Length; i++)// 첫째 라인 빼고 리스팅
        {
            string[] elements = data[i].Split(new char[] { ',' });

            SetStatus setAddStatus = new SetStatus
            {
                catchRadius = Data_Parse.Parse_Float(elements[6]),// 물고기를 잡는 범위
                catchSpeed = Data_Parse.Parse_Float(elements[7]),// 낚시대가 물고기를 향해 이동하는 속도
                catchPower = Data_Parse.Parse_Float(elements[8]),// 낚시대의 힘
                catchMaxHealth = Data_Parse.Parse_Float(elements[9]),// 낚시대의 최대 체력
                catchAttakSpeed = Data_Parse.Parse_Float(elements[10]),// 물고기를 공격하는 빈도

                shipSpeed = Data_Parse.Parse_Float(elements[11]),// 배의 이동 속도
                maxWeight = Data_Parse.Parse_Float(elements[12]),// 인벤토리 중량
                maxEnergy = Data_Parse.Parse_Float(elements[13]),// 연료통 크기
                efficient = Data_Parse.Parse_Float(elements[14]),// 에너지 효율

                maxBoxSize = Data_Parse.Parse_Vector2Int(elements[15]),// 인벤토리 크기
                shipHealth = Data_Parse.Parse_Int(elements[16]),// 배 체력
                freshness = Data_Parse.Parse_Float(elements[17]),// 신선도 유지 - 꼭 필요한가??????  

                luckFish = Data_Parse.Parse_Float(elements[18]),// 희귀 물고기 확률
                fishAmount = Data_Parse.Parse_Int(elements[19]),// 낚시 횟수 증가
                fishPrice = Data_Parse.Parse_Float(elements[20]),// 판매 물고기 가격 증가
            };

            SkillStatus tempData = new SkillStatus
            {
                name = elements[0],
                map = Data_Parse.Parse_Vector2Int(elements[1]),
                description = elements[2],
                addStatusString = elements[3],
                icon = elements[4],
                price = int.Parse(elements[5]),
                addStatus = setAddStatus,
            };
            if (dictSkills.ContainsKey(tempData.map) == true)
            {
                Debug.LogError($"같은 위치에 스킬이 존재함 : {dictSkills[tempData.map].name} -> {tempData.name}");
                break;
            }
            else if (tempData.map.x >= skillMapSize.x || tempData.map.y >= skillMapSize.y)
            {
                Debug.LogError($"위치 범위를 넘김 : {tempData.name}");
            }
            dictSkills[tempData.map] = tempData;
        }

        for (int y = 0; y < skillMapSize.y; y++)
        {
            for (int x = 0; x < skillMapSize.x; x++)
            {
                SkillStatus temp = new SkillStatus();
                Vector2Int map = new Vector2Int(x, y);
                if (dictSkills.ContainsKey(map) == true)
                {
                    temp = dictSkills[map];
                }
                skillList.Add(temp);
            }
        }
    }
}
