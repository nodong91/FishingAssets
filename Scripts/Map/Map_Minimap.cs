using UnityEngine;

public class Map_Minimap : MonoBehaviour
{
    public RectTransform player;
    public RectTransform lostBox;

    public StaticOpenCanvas.CanvasStruct[] canvasStructs;
    const float percent = 2.3f;
    GameObject setLostBox;
    public void CloseCanvas() => Game_Manager.current.GetMainUI?.CloseCanvas();

    public Custom_Button closeButton;

    public void SetStart()
    {
        closeButton.SetButton(CloseCanvas);
        RemoveLostBox();
        OpenCanvas(false);
    }

    public void OpenCanvas(bool _open)
    {
        StartCoroutine(StaticOpenCanvas.OpenCanvas(canvasStructs, _open));
        SetPlayerPosition();
        SetLostBoxPosition();
    }

    void SetPlayerPosition()
    {
        Transform getPlayer = Game_Manager.current?.GetPlayer.transform;
        Vector3 front = getPlayer.TransformPoint(Vector3.forward * 5f);
        Vector2 offset = (new Vector2(getPlayer.position.x, getPlayer.position.z) - new Vector2(front.x, front.z)).normalized;
        player.anchoredPosition = new Vector2(getPlayer.position.x, getPlayer.position.z) * percent;
        player.rotation = Quaternion.LookRotation(offset);
    }

    public void SetLostBox(GameObject _lostBox)
    {
        setLostBox = _lostBox;
        lostBox.gameObject.SetActive(true);
    }

    void SetLostBoxPosition()
    {
        if (setLostBox == null)
            return;
        lostBox.anchoredPosition = new Vector2(setLostBox.transform.position.x, setLostBox.transform.position.z) * percent;
    }

    public void RemoveLostBox()
    {
        setLostBox = null;
        lostBox.gameObject.SetActive(false);
    }
}
