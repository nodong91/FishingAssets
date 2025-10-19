using UnityEngine;
using UnityEngine.UI;
using static Data_Manager;

public class Skill_Tool_Slot : MonoBehaviour
{
    public Image iconImage;
    public TMPro.TMP_Text skillAmount;
    public Custom_Button button;
    public SkillStatus Status { get; set; }
    void Start()
    {
        button.SetButton(ButtonAction);
    }

    void ButtonAction()
    {

    }

    public void SetSlot(SkillStatus _Status)
    {
        Status = _Status;
        iconImage.gameObject.SetActive(_Status.icon != null);
        if (_Status.icon == null)
            return;
        
        Sprite iconSprite = Singleton_Data.INSTANCE.Dict_Sprite[_Status.icon];
        iconImage.sprite = iconSprite;
    }
}
