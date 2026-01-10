using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Sprite_Animation : MonoBehaviour, IPointerClickHandler
{
    public StaticOpenCanvas.CanvasStruct[] canvasStructs;
    public CanvasGroup canvasGroup;
    public int currentIndex;
    public Image setImage;
    //public Sprite[] sprites;
    public TMPro.TMP_Text infoText;
    //public string[] strings;
    Coroutine animateCoroutine;
    [System.Serializable]
    public struct TutorialStruct
    {
        public Sprite sprite;
        public string info;
    }
    public TutorialStruct[] tutorialStructs;

    public void OnPointerClick(PointerEventData eventData)
    {
        StopAllCoroutines();
        currentIndex++;
        if (currentIndex < tutorialStructs.Length)
        {
            SetTutorial(currentIndex);
        }
        else
        {
            OpenCanvas(false);
        }
    }

    public void OpenCanvas(bool _open)
    {
        if (_open == true)
        {
            currentIndex = 0;
            SetTutorial(0);
        }
        StartCoroutine(StaticOpenCanvas.OpenCanvas(canvasStructs, _open));
    }

    void SetTutorial(int _index)
    {
        setImage.sprite = tutorialStructs[_index].sprite;
        infoText.text = Singleton_Data.INSTANCE.GetLanguage(tutorialStructs[_index].info);
    }
}
