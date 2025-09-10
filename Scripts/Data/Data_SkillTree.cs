using System.Collections.Generic;
using UnityEngine;
using static Data_Manager;

[CreateAssetMenu(fileName = "Data_SkillTree", menuName = "Scriptable Objects/Data_SkillTree")]
public class Data_SkillTree : ScriptableObject
{
    public Vector2Int skillMapSize, startSlot;
    public List<SkillStatus> skillList = new List<SkillStatus>();
}
