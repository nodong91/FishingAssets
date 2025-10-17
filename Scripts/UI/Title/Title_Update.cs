using UnityEngine;

public class Title_Update : MonoBehaviour
{
    public Custom_Button closeButton;
    public GameObject updatePanel;
    public TMPro.TMP_Text volume;
    [TextArea]
    public string versionString, buildString;
    public TMPro.TMP_Text versionText;
    public TMPro.TMP_Text buildText;

    void Start()
    {
        closeButton.SetButton(CloseButton);
        SetString();
        volume.text = versionString;
    }

    void SetString()
    {
        versionText.text = $"Updata - {versionString}";
        buildText.text = buildString;
    }

    void CloseButton()
    {
        updatePanel.SetActive(false);
    }
}
