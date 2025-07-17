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

    public static IEnumerator OpenCanvasMoving(CanvasStruct[] _canvasStructs, bool _open, float _speed = 10f)
    {
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

    public Button inventoryButton;
    public Button fishGuideButton;
    public Button statusButton;
    public Button outButton;

    public CanvasStruct[] canvasStructs;
    bool _open = false;

    public void SetStart()
    {
        inventoryButton.onClick.AddListener(InventoryButton);
        fishGuideButton.onClick.AddListener(FishGuideButton);
        statusButton.onClick.AddListener(StatusButton);
        outButton.onClick.AddListener(OutButton);
    }

    void InventoryButton()
    {
        Game_Manager.current.inventory.OpenCanvas(true);
    }

    void FishGuideButton()
    {
        Game_Manager.current.fishGuide.OpenCanvas(true);
    }

    void StatusButton()
    {
        Game_Manager.current.statusUI.OpenCanvas(true);
    }

    void OutButton()
    {
        _open = !_open;
        OpenCanvas(_open);
    }

    public void OpenCanvas(bool _open)
    {
        StartCoroutine(OpenCanvasMoving(canvasStructs, _open, 10f));
    }
}
