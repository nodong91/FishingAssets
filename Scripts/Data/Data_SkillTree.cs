using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data_SkillTree", menuName = "Scriptable Objects/Data_SkillTree")]
public class Data_SkillTree : ScriptableObject
{
    public Vector2Int skillMapSize, startSlot;
    public List<string> skillList = new List<string>();
}
