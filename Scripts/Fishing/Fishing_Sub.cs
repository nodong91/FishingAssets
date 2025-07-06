using UnityEngine;
using UnityEngine.UI;

public class Fishing_Sub : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public Image gageImage;
    public float fillAmount;
    public int keyCode;

    public delegate void DeleEndGame(Fishing_Manager.FishingState _state);
    public DeleEndGame deleEndGame;

    public virtual void SetStart()
    {
        canvasGroup.gameObject.SetActive(false);
        gageImage.material = Instantiate(gageImage.material);
    }

    public virtual void StartGame()
    {
        canvasGroup.gameObject.SetActive(true);
    }

    public void EndGame()
    {
        deleEndGame?.Invoke(Fishing_Manager.FishingState.Complate);// 끝 (보스라면 다시 메인으로)
        canvasGroup.gameObject.SetActive(false);
    }

    public virtual void Action_Left()
    {
      
    }

    public virtual void Action_Right()
    {
       
    }
}
