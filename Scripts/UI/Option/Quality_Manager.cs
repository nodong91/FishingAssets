using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Data_Manager;

public class Quality_Manager : MonoBehaviour
{
    public TMP_Dropdown qualityDropdown; // Assuming you are using TMPro for the dropdown, otherwise use UnityEngine.UI.Dropdown
    public TMP_Dropdown resolutionDropdown; // Assuming you are using TMPro for the dropdown, otherwise use UnityEngine.UI.Dropdown
    public TMP_Dropdown frameRateDropdown;
    public Toggle fullScreenToggle; // Assuming you have a toggle for fullscreen mode

    int resolutionIndex = 0;
    private List<Resolution> resolutionList = new List<Resolution>();
    public int frameRate = 60; // Desired frame rate
    private List<int> frameList = new List<int>();

    public TMP_Text currentQualityText; // Text to display the current quality level

    public void SetStart()
    {
        SetQualityDropbox();
        SetResolutionDropbox();
        SetFrameRateDropbox();

        qualityDropdown.onValueChanged.AddListener(SetQualityLevel);
        resolutionDropdown.onValueChanged.AddListener(SetResolution);
        frameRateDropdown.onValueChanged.AddListener(SetFrameRate);

        fullScreenToggle.onValueChanged.AddListener(ToggleFullScreen);

        Data_Option optionData = Option_Manager.current.optionData;
        ToggleFullScreen(optionData.fullScreen);
        //SetQualityLevel(optionData.qualityLevel);
        SetResolution(optionData.resolutionIndex);
        SetFrameRate(optionData.frameRateIndex);
        DebugText();
    }

    void SetQualityDropbox()
    {
        string[] qualityOptions = QualitySettings.names;
        qualityDropdown.ClearOptions();
        qualityDropdown.AddOptions(new List<string>(qualityOptions));
        int currentQualityIndex = QualitySettings.GetQualityLevel();
        qualityDropdown.value = currentQualityIndex;
        qualityDropdown.RefreshShownValue();
    }

    void SetQualityLevel(int _levelIndex)
    {
        QualitySettings.SetQualityLevel(_levelIndex, false);
        qualityDropdown.value = _levelIndex;
        Debug.LogWarning(GetCurrentQualityLevel());
    }

    void SetResolutionDropbox()
    {
        resolutionList.Clear();
        resolutionDropdown.ClearOptions();

        HashSet<string> options = new HashSet<string>();
        Resolution[] resolutions = Screen.resolutions;
        int currentResolutionIndex = -1;
        string debugTest = "";
        for (int i = 0; i < resolutions.Length; i++)
        {
            int resolutionWidth = resolutions[i].width;
            int resolutionHeight = (int)(resolutionWidth * (9f / 16f));
            if (resolutions[i].height == resolutionHeight)
            {
                Resolution resolution = new Resolution
                {
                    width = resolutionWidth,
                    height = resolutionHeight
                };
                if (resolutionList.Contains(resolution) == false)
                {
                    debugTest += $"{resolution.width} x {resolution.height}, \n";
                    resolutionList.Add(resolution);
                }
                string option = resolutionWidth + " x " + resolutionHeight;
                options.Add(option);

                currentResolutionIndex++;
            }
        }
        currentQualityText.text = debugTest;
        resolutionDropdown.AddOptions(new List<string>(options));
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetResolution(int _resolutionIndex)
    {
        resolutionIndex = _resolutionIndex;
        resolutionDropdown.value = _resolutionIndex;
        Resolution selectedResolution = resolutionList[_resolutionIndex];
        Screen.SetResolution(selectedResolution.width, selectedResolution.height, fullScreenToggle.isOn);
        DebugText();
    }

    void ToggleFullScreen(bool _isOn)
    {
        fullScreenToggle.isOn = _isOn;
        Screen.fullScreen = _isOn;
        DebugText();
    }

    public void DecreaseQualityLevel()
    {
        QualitySettings.DecreaseLevel(false);
    }

    void SetFrameRateDropbox()
    {
        frameList.Clear();
        frameRateDropdown.ClearOptions();
        HashSet<string> options = new HashSet<string>();
        int currentResolutionIndex = -1;
        for (int i = 30; i <= 120; i += 30)
        {
            frameList.Add(i);
            options.Add(i.ToString() + "Hz");
            currentResolutionIndex++; // Store the index of the current frame rate
        }
        frameRateDropdown.AddOptions(new List<string>(options));
        frameRateDropdown.value = currentResolutionIndex;
        frameRateDropdown.RefreshShownValue();
    }

    void SetFrameRate(int _frameIndex)
    {
        frameRateDropdown.value = _frameIndex;
        Application.targetFrameRate = frameList[_frameIndex];
    }

    void DebugText()
    {
        Resolution selectedResolution = resolutionList[resolutionIndex];
        string debugText = $"{selectedResolution.width} x {selectedResolution.height}:{resolutionList.Count}({resolutionIndex}), Fullscreen: {fullScreenToggle.isOn}";
        debugText += $", Frame Rate: {Application.targetFrameRate}Hz";
        debugText += $", Quality: {GetCurrentQualityLevel()}";
        currentQualityText.text = debugText;
    }

    public string GetCurrentQualityLevel()
    {
        return QualitySettings.names[QualitySettings.GetQualityLevel()];
    }
}
