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
            Inspector.UpdateData();
            EditorUtility.SetDirty(Inspector);
        }
    }
}
#endif

public class Skill_Manager : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public RectTransform slotParent;
    public Skill_Slot slot;
    public Vector2Int skillMap;
    public float slotSize = 50f; // Size of each skill slot
    public RectTransform instParent;
    public Custom_Button closeButton;

    public Skill_Slot startSlot;
    public Skill_Slot[,] allSlot;

    // 스탯 추가
    public SetStatus defaultStatus, addStatus, setStatus;// 기본 스탯
    public List<Vector2Int> enableSlotLIst = new List<Vector2Int>();// 활성화된 슬롯 리스트
    public AnimationCurve openingCurve;
    StatusStruct[,] statusStructs;

    public Skill_Infomation infomation;
    public void SetStart()
    {
        UpdateData();
        closeButton.SetButton(CloseCanvas);
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
        statusStructs = new StatusStruct[skillMap.x, skillMap.y];
        if (Static_JsonManager.TryLoadSkillData("SkillMap", out List<StatusStruct> _statusStructs))
        {
            int index = 0;
            for (int y = 0; y < skillMap.y; y++)
            {
                for (int x = 0; x < skillMap.x; x++)
                {
                    statusStructs[x, y] = _statusStructs[index];
                    index++;
                }
            }
        }
        else
        {
            Debug.LogError("Failed to load skill data.");
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

                StatusStruct status = statusStructs[x, y];
                inst.status = status;
                inst.SetStart();
                inst.SetNearBySlot(skillMap);   // 근처 슬롯 설정
                inst.openingCurve = openingCurve; // 애니메이션 곡선 설정
                allSlot[x, y] = inst;
            }
        }
        startSlot = allSlot[skillMap.x / 2, skillMap.y / 2];

        startSlot.startSlot = true;
        startSlot.SetHide(false);
        startSlot.boxImage.gameObject.SetActive(true);
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

    public void AddSlot(Vector2Int _addNode)// 스킬 슬롯 추가
    {
        enableSlotLIst.Add(_addNode);
        Skill_Slot slot = allSlot[_addNode.x, _addNode.y];
        for (int i = 0; i < slot.nearbySlot.Count; i++)// 주변 슬롯 열기
        {
            Skill_Slot near = allSlot[slot.nearbySlot[i].x, slot.nearbySlot[i].y];
            near.SetHide(false, slot.transform.position);
        }
        AddStatuts(slot.status);

        setStatus.SettingStatus(defaultStatus);
        setStatus.AddStatus(addStatus);
    }

    void AddStatuts(StatusStruct _status)
    {
        //if (_status.setStatus == null || _status.setStatus.Count == 0)
        //    return;

        for (int i = 0; i < _status.setStatus.Count; i++)
        {
            Debug.LogWarning($"{_status.setStatus} {_status.setStatus[i].value}");
            switch (_status.setStatus[i].statusType)
            {
                case StatusStruct.StatusType.CatchRadius:
                    addStatus.catchRadius += _status.setStatus[i].value;
                    break;
                case StatusStruct.StatusType.CatchSpeed:
                    addStatus.catchSpeed += _status.setStatus[i].value;
                    break;
                case StatusStruct.StatusType.CatchPower:
                    addStatus.catchPower += _status.setStatus[i].value;
                    break;
                case StatusStruct.StatusType.CatchHealth:
                    addStatus.catchHealth += _status.setStatus[i].value;
                    break;
                case StatusStruct.StatusType.CatchAttakSpeed:
                    addStatus.catchAttakSpeed += _status.setStatus[i].value;
                    break;
                case StatusStruct.StatusType.ShipSpeed:
                    addStatus.shipSpeed += _status.setStatus[i].value;
                    break;
                case StatusStruct.StatusType.MaxWeight:
                    addStatus.maxWeight += _status.setStatus[i].value;
                    break;
                case StatusStruct.StatusType.MaxEnergy:
                    addStatus.maxEnergy += _status.setStatus[i].value;
                    break;
                case StatusStruct.StatusType.MaxBoxSize:
                    addStatus.maxBoxSize += new Vector2Int((int)_status.setStatus[i].value, (int)_status.setStatus[i].value);
                    break;
                case StatusStruct.StatusType.Freshness:
                    addStatus.freshness += _status.setStatus[i].value;
                    break;
            }
        }
    }
}
