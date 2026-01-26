using UnityEngine;

public class UI_Landing_Infomation : MonoBehaviour
{
    public TMPro.TMP_Text m_Text;

    public void SetStart(string _text)
    {
        m_Text.text = _text;
    }
}
