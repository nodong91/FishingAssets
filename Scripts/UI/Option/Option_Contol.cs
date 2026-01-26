using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Option_Control : MonoBehaviour
{
    public Toggle fpsToggle;
    public bool GetFPS { get { return fpsToggle.isOn; } }
    public FPSCounter fpsCanvas;
    public Toggle shakeToggle;
    public Toggle cursorLockToggle;
    public TMPro.TMP_Dropdown language;
    public bool fps = false;
    public bool shake = false;
    public bool cursorLock = false;
    public bool GetShake { get { return shakeToggle.isOn; } }
    public bool GetCursor { get { return cursorLockToggle.isOn; } }

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

        fps = optionData.setFPS;
        fpsToggle.onValueChanged.AddListener(SetFPS);
        fpsToggle.isOn = fps;
        SetFPS(fps);

        shake = optionData.shake;
        shakeToggle.onValueChanged.AddListener(SetShake);
        shakeToggle.isOn = shake;

        cursorLock = optionData.cursorLock;
        cursorLockToggle.onValueChanged.AddListener(SetCursorLock);
        cursorLockToggle.isOn = cursorLock;
        SetCursorLock(cursorLock);

        OnValueChange(optionData.language);
    }

    void OnValueChange(int _index)
    {
        language.value = _index;
        Singleton_Data.LanguageType languageType = (Singleton_Data.LanguageType)_index;
        Singleton_Data.INSTANCE.languageType = languageType;
        Option_Manager.current.langageDelegate?.Invoke();
    }

    void SetFPS(bool _open)
    {
        fpsCanvas.gameObject.SetActive(_open);
    }

    void SetShake(bool _shake)
    {
        shake = _shake;
    }

    void SetCursorLock(bool _lock)
    {
        cursorLock = _lock;
        Cursor_Manager.current.CusorLock(cursorLock);
    }
}
