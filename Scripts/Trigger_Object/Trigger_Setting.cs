using UnityEngine;

public class Trigger_Setting : MonoBehaviour
{
    public delegate void DeleTriggerAction();
    public DeleTriggerAction deleTriggerAction;
    public DeleTriggerAction deleTriggerEnter;

    Sprite icon;
    public Sprite GetIconSprite
    {
        get { return icon; }
        set { icon = value; }
    }

    public void EnterTrigger()
    {
        deleTriggerEnter?.Invoke();
    }

    public void TriggerAction()
    {
        Debug.LogWarning($"TriggerAction 호출됨 : {gameObject.name}");
        // 트리거 기능 활성화
        deleTriggerAction?.Invoke();
    }
}
