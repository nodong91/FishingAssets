using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Option_Control : MonoBehaviour
{
    public Toggle fpsToggle;
    public bool GetFPS { get { return fpsToggle.isOn; } }
    public FPSCounter fpsCanvas;
    public Toggle shakeToggle;

    public TMPro.TMP_Dropdown language;
    public bool GetShake { get { return shakeToggle.isOn; } }

    public void SetStart()
    {
        language.ClearOptions();
        HashSet<string> options = new HashSet<string>();
        int count = (int)Singleton_Data.LanguageType.Count;
        for (int i = 0; i < count; i++)
        {
            string option = string.Empty;
            switch (i)
            {
                case 0:
                    option = "English";
                    break;
                case 1:
                    option = "ÇÑ±¹¾î";
                    break;
                case 2:
                    option = "ìíÜâåÞ";
                    break;
                case 3:
                    option = "ñéÙþ";
                    break;
            }
            options.Add(option);
        }
        language.AddOptions(new List<string>(options));
        language.onValueChanged.AddListener(OnValueChange);

        Data_Manager.Data_Option optionData = Option_Manager.current.optionData;
        language.value = optionData.language;
        fpsToggle.onValueChanged.AddListener(SetFPS);
        fpsToggle.isOn = optionData.setFPS;

        shakeToggle.onValueChanged.AddListener(SetFPS);
        shakeToggle.isOn = optionData.shake;
    }

    void OnValueChange(int _index)
    {
        language.value = _index;
        Singleton_Data.LanguageType languageType = (Singleton_Data.LanguageType)_index;
        Singleton_Data.INSTANCE.languageType = languageType;
        Option_Manager.current.optionLanguage.SetStart();
    }

    void SetFPS(bool _open)
    {
        fpsCanvas.gameObject.SetActive(_open);
    }
}
