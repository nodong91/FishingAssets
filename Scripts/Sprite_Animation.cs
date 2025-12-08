using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Sprite_Animation : MonoBehaviour, IPointerClickHandler
{
    public StaticOpenCanvas.CanvasStruct[] canvasStructs;
    public CanvasGroup canvasGroup;
    public Image setImage;
    public Sprite[] sprites;

    public void OnPointerClick(PointerEventData eventData)
    {
        StopAllCoroutines();
        canvasGroup.gameObject.SetActive(false);
    }

    public void OpenCanvas(bool _open)
    {
        if (_open == true)
            StartCoroutine(AnimateSprite());
        StartCoroutine(StaticOpenCanvas.OpenCanvas(canvasStructs, _open));
    }

    IEnumerator AnimateSprite()
    {
        int index = 0;
        while (true)
        {
            setImage.sprite = sprites[index];
            index = (index + 1) % sprites.Length;
            yield return new WaitForSeconds(0.1f);
        }
    }
}
