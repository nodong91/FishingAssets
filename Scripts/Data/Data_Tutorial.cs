using UnityEngine;

[CreateAssetMenu(fileName = "Data_Tutorial", menuName = "Scriptable Objects/Data_Tutorial")]
public class Data_Tutorial : ScriptableObject
{
    public string id;
    public Data_NPC npc;
    public int dialogIndex;
    public float timeScale;
    
    [System.Serializable]
    public struct TutorialComment
    {
        public string comment;
        public int commentSize;
        public float intervalTime;
        public Vector2 commentPosition;
        public Vector2 boxPosition;
        public Vector2 boxSize;
    }
    public TutorialComment[] tutorialComment;
}
