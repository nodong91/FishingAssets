using System.Collections;
using UnityEngine;

public static class StaticOpenCanvas
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
    public delegate void DeleEndOpen();
    public static DeleEndOpen deleEndOpen;

    public static IEnumerator OpenCanvas(CanvasStruct[] _canvasStructs, bool _open)
    {
        float _speed = 10f;
        for (int i = 0; i < _canvasStructs.Length; i++)
        {
            _canvasStructs[i].SetCanvasStruct();
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
                    _canvasStructs[i].canvasGroup.interactable = setLerp > 0;
                    _canvasStructs[i].canvasGroup.blocksRaycasts = setLerp > 0;
                }
                else
                {
                    _canvasStructs[i].rect.gameObject.SetActive(setLerp > 0);
                }
            }
            yield return null;
        }
        // 캔버스 닫히고 난후 저장용
        if (_open == false)
        {
            deleEndOpen?.Invoke();
        }
    }
}

