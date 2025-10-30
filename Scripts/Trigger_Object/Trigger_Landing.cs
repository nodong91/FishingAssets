using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class Trigger_Landing : MonoBehaviour
{
    public Sprite iconImage;
    Unit_Player player;
    Coroutine setLanding;

    public Trigger_Setting landingPoint;
    public CinemachineVolumeSettings cameraPosition, focusShip;

    [System.Serializable]
    public struct LandingSetting
    {
        public enum LandingType
        {
            LandingPoint,
            FishShop,// 생선 가게
            DownTown,
            Shipyard,// 조선소
            Board,// 퀘스트 보드
            Count
        }
        public LandingType landingType;
        public GameObject landingPoint;
    }

    [System.Serializable]
    public struct LandingStruct
    {
        public string landingID;
        public LandingSetting[] landingSetting;

        public Data_NPC player;
        public Data_NPC shopNPC;
        public Data_NPC shipyardNPC;
        public Data_NPC smugglerNPC;
    }
    public LandingStruct landingStruct;

    private void Start()
    {
        landingPoint.deleTriggerAction = SetLandingAction;// 섬 입장
        landingPoint.GetIconSprite = iconImage;// 트리거 아이콘 설정
        Game_Manager.current.currentLand = this;
    }

    public void SetLandingAction()
    {
        player = Game_Manager.current.GetPlayer;

        if (setLanding != null)
            StopCoroutine(setLanding);
        setLanding = StartCoroutine(SetLanding());
    }

    IEnumerator SetLanding()
    {
        cameraPosition.gameObject.SetActive(true);
        SetLandingUI();

        Vector3 prevPosition = player.transform.position;
        Quaternion prevRotation = player.transform.rotation;

        Vector3 targetPosition = new Vector3(landingPoint.transform.position.x, player.transform.position.y, landingPoint.transform.position.z);
        float normalize = 0f;
        while (normalize < 1f)
        {
            normalize += Time.deltaTime * 0.5f;

            player.transform.position = Vector3.Lerp(prevPosition, targetPosition, normalize);
            player.transform.rotation = Quaternion.Lerp(prevRotation, landingPoint.transform.rotation, normalize);
            yield return null;
        }
    }

    private void SetLandingUI()
    {
        Game_Manager.current.GetLanding.SetLanding(landingStruct);
        Game_Manager.current.GetLanding.outLanding = OutLanding;
        Game_Manager.current.GetMainUI.OpenCanvas(false);
    }

    public void CameraOutFouce(bool _on)
    {
        cameraPosition.FocusOffset = _on ? 0f : 10f;
    }

    void OutLanding()
    {
        // 카메라 포커스 제거
        cameraPosition.gameObject.SetActive(false);
        Game_Manager.current.GetMainUI.OpenCanvas(true);
        Game_Manager.current.SetThemeMusic();
    }
}
