using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UI_Main : MonoBehaviour
{
    [System.Serializable]
    public class CanvasStruct
    {
        public enum MoveDirection
        {
            Up, Down, Left, Right, Alpha
        }
        public MoveDirection direction;
        public RectTransform rect;
        [HideInInspector]
        public CanvasGroup canvasGroup;
        [HideInInspector]
        public Vector2 targetPosition;

        public void SetCanvasStruct()
        {
            Vector2 sizeDelta = rect.sizeDelta;
            switch (direction)
            {
                case MoveDirection.Up:
                    sizeDelta.x = 0f;
                    break;
                case MoveDirection.Down:
                    sizeDelta.x = 0f;
                    sizeDelta.y *= -1f;
                    break;
                case MoveDirection.Left:
                    sizeDelta.x *= -1f;
                    sizeDelta.y = 0f;
                    break;
                case MoveDirection.Right:
                    sizeDelta.y = 0f;
                    break;
                case MoveDirection.Alpha:

                    break;
            }
            targetPosition = sizeDelta;
            canvasGroup = rect.GetComponent<CanvasGroup>();
        }
    }

    public static IEnumerator OpenCanvasMoving(CanvasStruct[] _canvasStructs, bool _open)
    {
        float _speed = 10f;
        for (int i = 0; i < _canvasStructs.Length; i++)
        {
            CanvasStruct canvas = _canvasStructs[i];
            if (_open == true)
                canvas.rect.gameObject.SetActive(true);
            canvas.SetCanvasStruct();
        }

        float normalize = 0f;
        while (normalize < 1f)
        {
            normalize += Time.deltaTime * _speed;
            for (int i = 0; i < _canvasStructs.Length; i++)
            {
                float setLerp = _open == true ? normalize : 1f - normalize;
                Vector2 rectPosition = Vector2.Lerp(_canvasStructs[i].targetPosition, Vector2.zero, setLerp);
                _canvasStructs[i].rect.anchoredPosition = rectPosition;
                if (_canvasStructs[i].canvasGroup != null)
                {
                    _canvasStructs[i].canvasGroup.alpha = setLerp;
                }
            }
            yield return null;
        }

        for (int i = 0; i < _canvasStructs.Length; i++)
        {
            if (_open == false)
                _canvasStructs[i].rect.gameObject.SetActive(false);
        }
    }
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

    public CanvasStruct[] canvasStructs;

    public Canvas cameraCanvas;
    public TMPro.TMP_Text warnningText;
    Coroutine textActing;

    public void SetStart()
    {
        inventoryButton.onClick.AddListener(InventoryButton);
        fishGuideButton.onClick.AddListener(FishGuideButton);
        fishingButton.onClick.AddListener(FishingButton);

        SetCameraCanvas();
        Debug.LogWarning("menuState");
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
        bool onInventory= (menuState & MenuState.Inventory) != 0;
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
        StartCoroutine(OpenCanvasMoving(canvasStructs, _open));
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
