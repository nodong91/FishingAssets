using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Data_Manager;

public class Skill_Tool : MonoBehaviour
{
    public Data_SkillTree data_SkillTree;// 데이터 스크립트에이블
    public GridLayoutGroup skillGridLayout, instSlotGrid;// 슬롯 부모
    public TMPro.TMP_Text skillText, infoText;// 스킬 정보

    public Skill_Tool_Slot baseSlot;// 슬롯
    List<Skill_Tool_Slot> skillSlots = new List<Skill_Tool_Slot>();
    Dictionary<string, Skill_Tool_Slot> dictSlots = new Dictionary<string, Skill_Tool_Slot>();
    List<Skill_Tool_Slot> instSlots = new List<Skill_Tool_Slot>();
    List<SkillStruct> skills = new List<SkillStruct>();

    public Custom_Button openButton, closeButton;
    public GameObject menuObject, skillInfo;
    public Custom_Button saveButton, loadButton, resetButton;
    Skill_Tool_Slot selectSlot, startSlot;
    public Transform startMarker;

    void Start()
    {
        openButton.SetButton(delegate { menuObject.SetActive(true); });
        closeButton.SetButton(delegate { menuObject.SetActive(false); });

        menuObject.SetActive(false);
        skillInfo.SetActive(false);
        savePopup.SetActive(false);

        saveButton.SetButton(delegate { savePopup.SetActive(true); });
        loadButton.SetButton(LoadData);
        resetButton.SetButton(ResetData);

        saveYes.SetButton(delegate { SavePopup(true); });
        saveNo.SetButton(delegate { SavePopup(false); });

        skillGridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        skillGridLayout.constraintCount = data_SkillTree.skillMapSize.x;

        skillSlots = new List<Skill_Tool_Slot>();
        dictSlots = new Dictionary<string, Skill_Tool_Slot>();
        for (int y = 0; y < data_SkillTree.skillMapSize.y; y++)// 스킬 필드 세팅
        {
            for (int x = 0; x < data_SkillTree.skillMapSize.x; x++)
            {
                Skill_Tool_Slot slot = Instantiate(baseSlot, skillGridLayout.transform);
                slot.inst = false;
                slot.clickHandler = ClickHandler;
                Vector2Int map = new Vector2Int(x, y);
                slot.slotNum = map;
                SkillStruct temp = new SkillStruct();
                slot.SetSlot(temp);
                skillSlots.Add(slot);
                if (map == data_SkillTree.startSlot)
                {
                    startSlot = slot;
                }
            }
        }

        instSlots = new List<Skill_Tool_Slot>();
        skills = new List<SkillStruct>();
        foreach (var child in Singleton_Data.INSTANCE.Dict_Skill)// 인스턴트 스킬 세팅
        {
            Skill_Tool_Slot slot = Instantiate(baseSlot, instSlotGrid.transform);
            slot.inst = true;
            slot.clickHandler = ClickHandler;
            slot.SetSlot(child.Value);
            instSlots.Add(slot);
            skills.Add(child.Value);
            dictSlots[child.Key] = slot;
        }

        StartPosition(startSlot.transform.localPosition);// 스타트 포지션
        LoadData();
    }

    void StartPosition(Vector3 _position)
    {
        startMarker.SetParent(startSlot.transform);
        startMarker.localPosition = Vector3.zero;
    }

    void ClickHandler(Skill_Tool_Slot _slot)// 스킬 클릭 시
    {
        if (selectSlot == null)// 스킬 선택
        {
            if (string.IsNullOrEmpty(_slot.Status.id))
                return;

            SelectedSlot(_slot);
            return;
        }
        else if (_slot != selectSlot)// 스킬 놓기
        {
            if (_slot.inst == true)
            {
                selectSlot.selected.SetActive(false);
                SelectedSlot(_slot);
                return;
            }

            SkillStruct status = _slot.Status;
            _slot.SetSlot(selectSlot.Status);
            selectSlot.SetSlot(status);
        }
        else// 같은 슬롯이면
        {
            selectSlot.SetSlot(selectSlot.Status);
        }
        selectSlot = null;
        skillInfo.SetActive(false);
    }

    void SelectedSlot(Skill_Tool_Slot _slot)
    {
        selectSlot = _slot;
        _slot.selected.SetActive(true);
        skillInfo.SetActive(true);
        skillText.text = _slot.Status.name;
        infoText.text = _slot.Status.addStatusString;
    }


    void SaveData()
    {
        data_SkillTree.skillList.Clear();
        foreach (var slot in skillSlots)
        {
            data_SkillTree.skillList.Add(slot.Status.id);
        }

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(data_SkillTree);
#endif
    }

    void LoadData()
    {
        for (int i = 0; i < data_SkillTree.skillList.Count; i++)
        {
            string id = data_SkillTree.skillList[i];
            if (string.IsNullOrEmpty(id))
                continue;

            if (Singleton_Data.INSTANCE.Dict_Skill.ContainsKey(id) == true)
            {
                SkillStruct skill = Singleton_Data.INSTANCE.Dict_Skill[data_SkillTree.skillList[i]];
                skillSlots[i].SetSlot(skill);
                if (skill.id != null && dictSlots.ContainsKey(skill.id) == true)
                    dictSlots[skill.id].gameObject.SetActive(false);
            }
        }
    }

    void ResetData()
    {
        foreach (var slot in skillSlots)
        {
            slot.SetSlot(new SkillStruct());
        }

        for (int i = 0; i < instSlots.Count; i++)
        {
            Skill_Tool_Slot slot = instSlots[i];
            slot.SetSlot(skills[i]);
            Debug.LogWarning(skills[i].id);
        }
    }

    public Custom_Button saveYes, saveNo;
    public GameObject savePopup;

    void SavePopup(bool _yes)
    {
        if (_yes == true)
        {
            SaveData();
        }
        savePopup.SetActive(false);
    }
}
