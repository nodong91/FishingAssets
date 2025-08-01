using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_QuestSlot : MonoBehaviour
{
    public Data_Quest questData;
    public TMP_Text title;
    public TMP_Text description;

    public Button acceptButton, cencelButton;

    public void SetQuest(Data_Quest _questData)
    {
        questData = _questData;
        acceptButton.onClick.AddListener(AcceptButton);
        cencelButton.onClick.AddListener(CencelButton);
    }

    void AcceptButton()
    {

    }

    void CencelButton()
    {

    }
}
