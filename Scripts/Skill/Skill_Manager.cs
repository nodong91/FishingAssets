using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using static Data_Manager;

public class Skill_Manager : MonoBehaviour
{
    public StaticOpenCanvas.CanvasStruct[] canvasStructs;
    public Custom_Button resetButton;// 스킬 초기화 버튼
    public TMPro.TMP_Text activeDescription;

    public Data_SkillTree skillTreeData;
    public Canvas canvas;
    public CanvasGroup canvasGroup;
    public Custom_Button closeButton;
    public RectTransform slotParent;
    public Skill_Slot slot;
    private Vector2Int skillMap;
    public float slotSize = 50f; // Size of each skill slot
    public RectTransform instParent;

    public Skill_Slot startSlot;
    public Skill_Slot[,] allSlot;

    public SetStatus addStatus;// 추가 스탯
    public List<Vector2Int> enableSlotLIst = new List<Vector2Int>();// 활성화된 슬롯 리스트
    public AnimationCurve openingCurve;
    SkillStruct[,] statusStructs;

    public Skill_Infomation infomation;

    public void SetStart()
    {
        UpdateData();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = Camera_Manager.current.UICamera;

        closeButton.SetButton(CloseCanvas);
        resetButton.SetButton(SkillReset);

        activeDescription.text = Singleton_Data.INSTANCE.GetLanguage(Const_ETC._activeSkill);
        OpenCanvas(false);
    }
    public void OpenCanvas(bool _open)
    {
        StartCoroutine(StaticOpenCanvas.OpenCanvas(canvasStructs, _open));
    }

    public void CloseCanvas()
    {
        OpenCanvas(false);
        Static_JsonManager.SaveEnableSkillData(Const_Save._enableSkill, enableSlotLIst);// 활성화 된 스킬 저장
        Game_Manager.current.GetLanding.BackButton();
    }

    void LoadData()
    {
        skillMap = skillTreeData.skillMapSize;
        statusStructs = new SkillStruct[skillMap.x, skillMap.y];

        // 스킬 트리 불러오기
        int index = 0;
        for (int y = 0; y < skillMap.y; y++)
        {
            for (int x = 0; x < skillMap.x; x++)
            {
                string id = skillTreeData.skillList[index];
                if (string.IsNullOrEmpty(id))
                {
                    statusStructs[x, y] = new SkillStruct();
                    statusStructs[x, y].addStatus = new SetStatus();
                }
                else
                {
                    statusStructs[x, y] = Singleton_Data.INSTANCE.Dict_Skill[id];
                }
                index++;
            }
        }

        // 활성화된 스킬 불러오기
        if (Static_JsonManager.TryLoadEnableSkillData(Const_Save._enableSkill, out List<Vector2Int> _enableSlotLIst))
        {
            enableSlotLIst = _enableSlotLIst;
        }
    }

    void SettingLoadSlot()
    {
        for (int i = 0; i < enableSlotLIst.Count; i++)
        {
            // 슬롯 활성화
            SetSlot(enableSlotLIst[i]);
        }
    }

    public void UpdateData()
    {
        LoadData();// 데이타 불러오기
        SetParent();
        allSlot = new Skill_Slot[skillMap.x, skillMap.y];
        for (int y = 0; y < skillMap.y; y++)
        {
            for (int x = 0; x < skillMap.x; x++)
            {
                Skill_Slot inst = Instantiate(slot, instParent);
                inst.slotNode = new Vector2Int(x, y);
                inst.name = inst.slotNode.ToString();
                inst.SetHide(true);
                inst.deleSlotAction = ActiveSkill;
                inst.deleSlotPosition = infomation.SetPosition;

                SkillStruct skill = statusStructs[x, y];
                inst.Skill = skill;
                inst.SetStart();
                inst.SetNearBySlot(skillMap);   // 근처 슬롯 설정

                allSlot[x, y] = inst;
            }
        }
        Vector2Int startIndex = skillTreeData.startSlot;
        startSlot = allSlot[startIndex.x, startIndex.y];

        startSlot.startSlot = true;
        startSlot.SetHide(false);// 활성화
        startSlot.boxImage.gameObject.SetActive(true);

        SettingLoadSlot();
        EnableSkill();
    }

    void SetParent()
    {
        if (instParent != null)
            DestroyImmediate(instParent.gameObject);

        instParent = new GameObject("[ PARENT ]").AddComponent<RectTransform>();
        instParent.SetParent(slotParent);
        instParent.anchoredPosition = Vector2.zero;
        instParent.localScale = Vector2.one;
        SetGrid();
    }

    void SetGrid()
    {
        GridLayoutGroup grid = instParent.gameObject.AddComponent<GridLayoutGroup>();
        grid.cellSize = Vector2.one * slotSize;
        grid.spacing = Vector2.one * slotSize * 0.1f;
        grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
        grid.startAxis = GridLayoutGroup.Axis.Horizontal;
        grid.childAlignment = TextAnchor.UpperLeft;
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = skillMap.x;
    }

    void ActiveSkill(Vector2Int _addNode)// 스킬 활성화
    {
        enableSlotLIst.Add(_addNode);
        SetSlot(_addNode);
        Singleton_Audio.INSTANCE.Audio_FX(Const_Audio._activeSkill);

        EnableSkill();
    }

    void SetSlot(Vector2Int _addNode)
    {
        Skill_Slot slot = allSlot[_addNode.x, _addNode.y];
        slot.EnableSlot(true);// 활성화
        for (int i = 0; i < slot.nearbySlot.Count; i++)// 주변 슬롯 열기
        {
            Skill_Slot near = allSlot[slot.nearbySlot[i].x, slot.nearbySlot[i].y];
            near.SetHide(false, slot.transform.position);
        }
        AddSkill(slot.Skill);
    }

    void AddSkill(SkillStruct _skill)
    {
        switch (_skill.skillType)
        {
            case SkillStruct.SkillType.AddStatus:
                // 스탯 추가
                Debug.LogWarning(_skill.id);
                addStatus.AddStatus(_skill.addStatus);
                Game_Manager.current.AddStatus();
                break;

            case SkillStruct.SkillType.ShipUnlocked:
                ShipUnlock(_skill.id);
                break;

            case SkillStruct.SkillType.Etc:

                break;
        }
    }

    void ShipUnlock(string _id)
    {
        Data_Ship data_Ship = Singleton_Data.INSTANCE.Dict_Ship[_id];
        Debug.LogWarning($"{_id} : {data_Ship.shipName}");
        Game_Manager.current.GetChangeShip.AddShip(data_Ship);
    }

    void SkillReset()
    {
        for (int i = 0; i < enableSlotLIst.Count; i++)
        {
            Skill_Slot slot = allSlot[enableSlotLIst[i].x, enableSlotLIst[i].y];
            slot.ResetSlot();// 비활성화
            for (int j = 0; j < slot.nearbySlot.Count; j++)// 주변 슬롯 열기
            {
                Skill_Slot near = allSlot[slot.nearbySlot[j].x, slot.nearbySlot[j].y];
                near.ResetSlot();
            }
            // 스탯 제거
            addStatus.AddStatus(slot.Skill.addStatus, -1);
            Game_Manager.current.AddStatus();
        }
        enableSlotLIst.Clear();
        Static_JsonManager.SaveEnableSkillData(Const_Save._enableSkill, enableSlotLIst);// 활성화 된 스킬 리셋 저장
    }




    public List<string> enableList = new List<string>();
    void EnableSkill()
    {
        enableList.Clear();
        for (int i = 0; i < enableSlotLIst.Count; i++)
        {
            Skill_Slot slot = allSlot[enableSlotLIst[i].x, enableSlotLIst[i].y];

            // 스탯 제거
            string skillID = slot.Skill.id;
            enableList.Add(skillID);
        }
    }
}
