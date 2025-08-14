using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UI_Main : MonoBehaviour
{

    [Flags]
    public enum MenuState
    {
        Inventory = 1 << 0,
        Fishing = 1 << 1,
        //Status = 1 << 2,

    }
    public MenuState menuState;
    public UI_Time timeUI;
    public UI_Status statusUI;
    public Button inventoryButton;
    public Button fishGuideButton;
    public Button fishingButton;

    public StaticOpenCanvas.CanvasStruct[] canvasStructs;

    public Canvas cameraCanvas;
    public TMPro.TMP_Text warnningText;
    Coroutine textActing;

    public void SetStart()
    {
        inventoryButton.onClick.AddListener(InventoryButton);
        fishGuideButton.onClick.AddListener(FishGuideButton);
        fishingButton.onClick.AddListener(FishingButton);

        SetCameraCanvas();
    }

    void SetCameraCanvas()
    {
        warnningText.alpha = 0f;
        cameraCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        cameraCanvas.worldCamera = Game_Manager.current.cameraManager.UICamera;
    }

    void InventoryButton()
    {
        if ((menuState & MenuState.Inventory) == 0)
        {
            menuState |= MenuState.Inventory;// 넣기
        }
        else
        {
            menuState &= ~MenuState.Inventory;
        }
        bool onInventory = (menuState & MenuState.Inventory) != 0;
        Game_Manager.current.GetInventory.OpenInventory(onInventory);
        statusUI.OpenCanvas(onInventory);
        Debug.LogWarning(menuState);
    }

    void FishGuideButton()
    {
        Game_Manager.current.GetFishGuide.OpenCanvas(true);
    }

    public void OpenCanvas(bool _open)
    {
        StaticOpenCanvas.deleEndOpen = null;
        StartCoroutine(StaticOpenCanvas.OpenCanvas(canvasStructs, _open));
    }

    public void FishingButton()
    {
        string id = "Fs_1001";
        Data_Manager.FishStruct fishStruct = Singleton_Data.INSTANCE.Dict_Fish[id];
        Game_Manager.current.GetFishing.StartGame(fishStruct);
    }
    //===========================================================================================================================
    // 경고 문구
    //===========================================================================================================================

    public void SetWarnningText(string _text)
    {
        warnningText.text = _text;
        if (textActing != null)
            StopCoroutine(textActing);
        textActing = StartCoroutine(TextActing());
    }

    IEnumerator TextActing()
    {
        warnningText.alpha = 1f;
        yield return new WaitForSeconds(1f);
        float normalize = 0f;
        while (normalize < 1f)
        {
            normalize += Time.deltaTime * 5f;
            warnningText.alpha = 1f - normalize;
            yield return null;
        }
    }
}
