using System.Collections;
using UnityEngine;

public class Trigger_Landing : Trigger_Setting
{
    public Sprite iconImage;
    Unit_Player player;
    Coroutine setLanding;

    public Trigger_Setting triggerSetting;
    public GameObject cameraPosition;

    [System.Serializable]
    public struct LandingSetting
    {
        public enum LandingType
        {
            LandingPoint,
            FishShop,// 생선 가게
            Event,
            Shipyard,// 조선소
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

        public Data_Shop shopData;
        public Data_Shop[] shipyardData;

        public Data_NPC shopNPC;
        public Data_NPC shipyardNPC;
    }
    public LandingStruct landingStruct;

    private void Start()
    {
        triggerSetting.deleTriggerAction = SetLandingAction;
        triggerSetting.GetIconSprite = iconImage;
    }

    void SetLandingAction()
    {
        player = Game_Manager.current.player;
        if (setLanding != null)
            StopCoroutine(setLanding);
        setLanding = StartCoroutine(SetLanding());
    }

    IEnumerator SetLanding()
    {
        cameraPosition.SetActive(true);

        Vector3 prevPosition = player.transform.position;
        Quaternion prevRotation = player.transform.rotation;
        float normalize = 0f;
        while (normalize < 1f)
        {
            normalize += Time.deltaTime * 0.5f;

            player.transform.position = Vector3.Lerp(prevPosition, triggerSetting.transform.position, normalize);
            player.transform.rotation = Quaternion.Lerp(prevRotation, triggerSetting.transform.rotation, normalize);
            yield return null;
        }
        SetLandingUI();
    }

    private void SetLandingUI()
    {
        Game_Manager.current.GetLanding.SetLanding(landingStruct);
        Game_Manager.current.GetLanding.outLanding = OutLanding;
        Game_Manager.current.mainUI.OpenCanvas(false);
    }

    void OutLanding()
    {
        // 카메라 포커스 제거
        cameraPosition.SetActive(false);
        Game_Manager.current.mainUI.OpenCanvas(true);
    }


}
