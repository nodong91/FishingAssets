using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Data_Manager;

public class Skill_Tool : MonoBehaviour
{
    public GridLayoutGroup skillGridLayout, instSlotGrid;
    public Data_SkillTree data_SkillTree;
    public TextAsset csvData;
    public Skill_Tool_Slot baseSlot;
    Dictionary<Vector2Int, SkillStatus> dictSkills = new Dictionary<Vector2Int, SkillStatus>();
    Dictionary<Vector2Int, Skill_Tool_Slot> dictSlots = new Dictionary<Vector2Int, Skill_Tool_Slot>();

    void Start()
    {
        SetFish(csvData);
        skillGridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        skillGridLayout.constraintCount = data_SkillTree.skillMapSize.x;

        for (int y = 0; y < data_SkillTree.skillMapSize.y; y++)
        {
            for (int x = 0; x < data_SkillTree.skillMapSize.x; x++)
            {
                Skill_Tool_Slot slot = Instantiate(baseSlot, skillGridLayout.transform);

                SkillStatus temp = new SkillStatus();
                Vector2Int map = new Vector2Int(x, y);
                if (dictSkills.ContainsKey(map) == true)
                {
                    temp = dictSkills[map];
                }
                else
                {
                    temp = new SkillStatus();
                }
                slot.SetSlot(temp);

                //skillList.Add(temp);
            }
        }
    }

    void SetFish(TextAsset _textAsset)
    {
        dictSkills = new Dictionary<Vector2Int, SkillStatus>();
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
            //if (dictSkills.ContainsKey(tempData.map) == true)
            //{
            //    Debug.LogError($"같은 위치에 스킬이 존재함 : {dictSkills[tempData.map].name} -> {tempData.name}");
            //    break;
            //}
            //else if (tempData.map.x >= data_SkillTree.skillMapSize.x || tempData.map.y >= data_SkillTree.skillMapSize.y)
            //{
            //    Debug.LogError($"위치 범위를 넘김 : {tempData.name}");
            //}

            Skill_Tool_Slot slot = Instantiate(baseSlot, instSlotGrid.transform);
            slot.SetSlot(tempData);
            dictSkills[tempData.map] = tempData;
        }
    }
}
