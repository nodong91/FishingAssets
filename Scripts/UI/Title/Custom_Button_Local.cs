using UnityEngine;

public class Custom_Button_Local : MonoBehaviour
{
    public TMPro.TMP_Text buttonLocalation;
    public string buttonID;

    void Start()
    {
        Option_Manager.current.optionControl.onValueChange += ChangeLanguage;
    }

    void ChangeLanguage()
    {
        buttonLocalation.text = Singleton_Data.INSTANCE.GetLanguage(buttonID);
    }
}
