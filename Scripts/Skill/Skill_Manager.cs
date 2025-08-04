using UnityEngine;
using NUnit.Framework;
using System.Collections.Generic;
using System.Collections;




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
    public RectTransform slotParent;
    public Skill_Slot slot;
    public Vector2Int skillMap;
    public RectTransform instParent;
    public Canvas canvas;

    public Skill_Slot startSlot;
    public Skill_Slot[,] allSlot;

    public void UpdateData()
    {
        SetParent();

        allSlot = new Skill_Slot[skillMap.x, skillMap.y];
        for (int y = 0; y < skillMap.y; y++)
        {
            for (int x = 0; x < skillMap.x; x++)
            {
                Skill_Slot inst = Instantiate(slot, instParent);
                inst.rect.anchoredPosition = new Vector2(x, y) * 60f;
                inst.slotNode = new Vector2Int(x, y);
                inst.name = inst.slotNode.ToString();
                inst.hide = true;
                allSlot[x, y] = inst;
            }
        }

        foreach (Skill_Slot slot in allSlot)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    int slotX = x + slot.slotNode.x;
                    int slotY = y + slot.slotNode.y;
                    if (slotX >= 0 && slotX < skillMap.x && slotY >= 0 && slotY < skillMap.y)
                    {
                        if (x == 0 || y == 0)
                            slot.nearbySlot.Add(allSlot[slotX, slotY]);
                    }
                }
            }
        }
    }

    void SetParent()
    {
        if (instParent != null)
            DestroyImmediate(instParent.gameObject);

        instParent = new GameObject("[ PARENT ]").AddComponent<RectTransform>();
        instParent.SetParent(slotParent);
        instParent.anchoredPosition = Vector2.zero;
        instParent.localScale = Vector2.one;
    }

    void Start()
    {
        StartCoroutine(iejfjejf());
    }

    IEnumerator iejfjejf()
    {
        yield return null;
        startSlot.startSlot = true;
        startSlot.hide = false;
        startSlot.boxImage.gameObject.SetActive(true);
        //startSlot.CheckNearbySlot();
    }
}
