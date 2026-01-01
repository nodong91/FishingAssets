using UnityEngine;

public class Custom_Button_Local : MonoBehaviour
{
    public TMPro.TMP_Text buttonLocalation;
    public string buttonID;

    private void Start()
    {
        Option_Manager.current.langageDelegate += ChangeLanguage;
        ChangeLanguage();
    }

    public void ChangeLanguage()
    {
        buttonLocalation.text = Singleton_Data.INSTANCE.GetLanguage(buttonID);
    }
}
