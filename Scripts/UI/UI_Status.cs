using UnityEngine;
using UnityEngine.EventSystems;
using static Data_Manager;
using static UI_Main;

public class UI_Status : MonoBehaviour, IPointerClickHandler
{
    public StaticOpenCanvas.CanvasStruct[] canvasStructs;

    [Header("[ 스테이터스 ]")]
    public SetStatus totalStatus;
    public TMPro.TMP_Text maxSpeedText, maxWeightText, maxEnergyText, maxBoxSizeText, freshnessText;
    public TMPro.TMP_Text fishingAreaText, LodPowerText, ReelingSpeedText, ReelingAccText, HitPointText, HitSpeedText;

    private void Start()
    {

        OpenCanvas(false);
    }

    public void OpenCanvas(bool _open)
    {
        StaticOpenCanvas.deleEndOpen = null;
        StartCoroutine(StaticOpenCanvas.OpenCanvas(canvasStructs, _open));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OpenCanvas(false);
    }
}
