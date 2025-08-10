using TMPro;
using UnityEngine;
using static Data_Manager;

public class Quality_Manager : MonoBehaviour
{
    public TMP_Dropdown qualityDropdown; // Assuming you are using TMPro for the dropdown, otherwise use UnityEngine.UI.Dropdown

    public void SetStart()
    {
        qualityDropdown.onValueChanged.AddListener(SetQualityLevel);

        Data_Option optionData = Option_Manager.current.optionData;
        int currentQualityIndex = optionData.qualityLevel;
        SetQualityLevel(optionData.qualityLevel);
    }

    public void SetQualityLevel(int _levelIndex)
    {
        QualitySettings.SetQualityLevel(_levelIndex, false);
        qualityDropdown.value = _levelIndex;
        Debug.LogWarning(GetCurrentQualityLevel());
    }

    public void IncreaseQualityLevel()
    {
        QualitySettings.IncreaseLevel(false);
    }

    public void DecreaseQualityLevel()
    {
        QualitySettings.DecreaseLevel(false);
    }

    public string GetCurrentQualityLevel()
    {
        return QualitySettings.names[QualitySettings.GetQualityLevel()];
    }
}
