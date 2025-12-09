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
            setImage.sprite = tutorialStructs[currentIndex].sprite;
            infoText.text = tutorialStructs[currentIndex].info;
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
            setImage.sprite = tutorialStructs[currentIndex].sprite;
            infoText.text = tutorialStructs[currentIndex].info;
        }
        //if (animateCoroutine != null)
        //    StopCoroutine(animateCoroutine);
        //if (_open == true)
        //    animateCoroutine = StartCoroutine(AnimateSprite());
        StartCoroutine(StaticOpenCanvas.OpenCanvas(canvasStructs, _open));
    }

    //IEnumerator AnimateSprite()
    //{
    //    int index = 0;
    //    while (true)
    //    {
    //        setImage.sprite = sprites[index];
    //        index = (index + 1) % sprites.Length;
    //        yield return new WaitForSeconds(0.1f);
    //    }
    //}
}
