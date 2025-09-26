using System.Collections.Generic;
using UnityEngine;

public class TranslateLanguage : MonoBehaviour
{
    public TMPro.TMP_Dropdown dropdown;

    public void SetStart()
    {
        dropdown.ClearOptions();
        HashSet<string> options = new HashSet<string>();
        int count = (int)Singleton_Data.LanguageType.Count;
        for (int i = 0; i < count; i++)
        {
            string option = ((Singleton_Data.LanguageType)i).ToString();
            options.Add(option);
        }
        dropdown.AddOptions(new List<string>(options));
        dropdown.onValueChanged.AddListener(OnValueChange);

        Data_Manager.Data_Option optionData = Option_Manager.current.optionData;
        dropdown.value = optionData.language;
    }

    void OnValueChange(int _index)
    {
        dropdown.value = _index;
        Singleton_Data.LanguageType languageType = (Singleton_Data.LanguageType)_index;
        Singleton_Data.INSTANCE.languageType = languageType;
    }
}
