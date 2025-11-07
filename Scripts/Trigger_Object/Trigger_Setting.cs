using UnityEngine;

public class Trigger_Setting : MonoBehaviour
{
    public delegate void DeleTriggerAction();
    public DeleTriggerAction deleTriggerAction;

    Sprite icon;
    public Sprite GetIconSprite
    {
        get { return icon; }
        set { icon = value; }
    }

    public void TriggerAction()
    {
        // 트리거 기능 활성화
        deleTriggerAction?.Invoke();
    }
}
