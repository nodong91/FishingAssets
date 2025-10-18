using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using static Data_Manager;


#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(Skill_Manager))]
public class Editor_Skill_Manager : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Space(10f);

        GUIStyle fontStyle = new GUIStyle(GUI.skin.button);
        fontStyle.fontSize = 15;
        fontStyle.normal.textColor = Color.yellow;

        Skill_Manager Inspector = target as Skill_Manager;
        if (GUILayout.Button("UpdateData", fontStyle, GUILayout.Height(30f)))
        {
            //Inspector.UpdateData();
            //EditorUtility.SetDirty(Inspector);
        }
    }
}
#endif

public class Skill_Manager : MonoBehaviour
{
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
    SkillStatus[,] statusStructs;

    public Skill_Infomation infomation;
    const string activeSkill = "FX_0003";
    const string saveEnableData = "Skill_Enabled";// 활성화 된 스킬 저장

    public void SetStart()
    {
        UpdateData();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = Camera_Manager.current.UICamera;

        closeButton.SetButton(CloseCanvas);
        resetButton.SetButton(SkillReset);

        OpenCanvas(false);
    }

    public void OpenCanvas(bool _open)
    {
        canvasGroup.alpha = _open ? 1f : 0f;
        canvasGroup.interactable = _open;
        canvasGroup.blocksRaycasts = _open;
    }

    void CloseCanvas()
    {
        OpenCanvas(false);
        Game_Manager.current.GetLanding.BackButton();
    }

    void LoadData()
    {
        skillMap = skillTreeData.skillMapSize;
        statusStructs = new SkillStatus[skillMap.x, skillMap.y];

        // 스킬 트리 불러오기
        int index = 0;
        for (int y = 0; y < skillMap.y; y++)
        {
            for (int x = 0; x < skillMap.x; x++)
            {
                statusStructs[x, y] = skillTreeData.skillList[index];
                index++;
            }
        }

        // 활성화된 스킬 불러오기
        if (Static_JsonManager.TryLoadEnableSkillData(saveEnableData, out List<Vector2Int> _enableSlotLIst))
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
                inst.deleSlotAction = AddSlot;
                inst.deleSlotPosition = infomation.SetPosition;

                SkillStatus status = statusStructs[x, y];
                inst.Status = status;
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
   
    void AddSlot(Vector2Int _addNode)// 스킬 활성화
    {
        enableSlotLIst.Add(_addNode);
        SetSlot(_addNode);
        Singleton_Audio.INSTANCE.Audio_FX(activeSkill);
        Static_JsonManager.SaveEnableSkillData(saveEnableData, enableSlotLIst);// 활성화 된 스킬 저장
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
        // 스탯 추가
        addStatus.AddStatus(slot.Status.addStatus);
        Game_Manager.current.AddStatus();
    }

    public Custom_Button resetButton;// 스킬 초기화 버튼

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
            addStatus.AddStatus(slot.Status.addStatus, -1);
            Game_Manager.current.AddStatus();
        }
        enableSlotLIst.Clear();
        Static_JsonManager.SaveEnableSkillData(saveEnableData, enableSlotLIst);// 활성화 된 스킬 저장
    }
}
