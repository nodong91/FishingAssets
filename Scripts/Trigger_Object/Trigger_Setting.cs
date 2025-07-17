using UnityEngine;

public class Trigger_Setting : MonoBehaviour
{
    public enum TriggerType
    {
        Fishing,// ³¬½Ã
        Landing,// ºÎµÎ
    }
    public TriggerType triggerType;

    public virtual Sprite GetIconSprite
    {
        get { return null; }
    }

    public Trigger_Fish GetTriggerFish
    {
        get { return this as Trigger_Fish; }
    }

    public Trigger_Landing GetTriggerLanding
    {
        get { return this as Trigger_Landing; }
    }
}
