using UnityEngine;
using static UI_Main;

public class Map_Minimap : MonoBehaviour
{

    public RectTransform player;

    //void Update()
    //{
    //    if (Game_Manager.current?.GetPlayer != null)
    //    {
    //        SizeRemap();
    //    }
    //}

    public void SetStart()
    {
        closeButton.SetButton(SetButton);
    }

    void SetButton()
    {
        Game_Manager.current.GetMainUI.dele_CloseButton();
    }

    public void OpenCanvas(bool _open)
    {
        StartCoroutine(StaticOpenCanvas.OpenCanvas(canvasStructs, _open));
        SizeRemap();
    }

    public StaticOpenCanvas.CanvasStruct[] canvasStructs;
    public float percent = 1f;

    void SizeRemap()
    {
        Transform getPlayer = Game_Manager.current?.GetPlayer.transform;
        Vector3 front = getPlayer.TransformPoint(Vector3.forward * 5f);
        player.anchoredPosition = new Vector2(getPlayer.position.x, getPlayer.position.z) * percent;
        Vector2 offset = (new Vector2(getPlayer.position.x, getPlayer.position.z) - new Vector2(front.x, front.z)).normalized;
        player.rotation = Quaternion.LookRotation(offset);

    }

    public Custom_Button closeButton;
}
