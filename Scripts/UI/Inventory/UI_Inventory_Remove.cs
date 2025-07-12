using UnityEngine;
using UnityEngine.UI;

public class UI_Inventory_Remove : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public Button cancelButton;
    public Button doneButton;

    public delegate void Dele_Remove();
    public Dele_Remove deleRemove;

    void Start()
    {
        cancelButton.onClick.AddListener(CancelButton);
        doneButton.onClick.AddListener(DoneButton);
    }

    public void OpenRemovePopup()
    {
        canvasGroup.gameObject.SetActive(true);
    }

    public void CancelButton()
    {
        canvasGroup.gameObject.SetActive(false);
    }

    void DoneButton()
    {
        deleRemove?.Invoke();
        canvasGroup.gameObject.SetActive(false);
    }
}
