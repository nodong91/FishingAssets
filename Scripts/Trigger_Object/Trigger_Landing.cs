using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger_Landing : Trigger_Setting
{
    public int themeMusic;
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
        triggerSetting.deleTriggerAction = SetLandingAction;// 섬 입장
        triggerSetting.GetIconSprite = iconImage;
    }

    void SetLandingAction()
    {
        player = Game_Manager.current.player;
        CheckQuest();

        if (setLanding != null)
            StopCoroutine(setLanding);
        setLanding = StartCoroutine(SetLanding());
    }

    IEnumerator SetLanding()
    {
        Option_Manager.current.SetThemeMusic(themeMusic);

        cameraPosition.SetActive(true);
        SetLandingUI();

        Vector3 prevPosition = player.transform.position;
        Quaternion prevRotation = player.transform.rotation;

        Vector3 targetPosition = new Vector3(triggerSetting.transform.position.x, player.transform.position.y, triggerSetting.transform.position.z);
        float normalize = 0f;
        while (normalize < 1f)
        {
            normalize += Time.deltaTime * 0.5f;

            player.transform.position = Vector3.Lerp(prevPosition, targetPosition, normalize);
            player.transform.rotation = Quaternion.Lerp(prevRotation, triggerSetting.transform.rotation, normalize);
            yield return null;
        }
    }

    public void CheckQuest()
    {
        // NPC 누가 퀘스트를 가지고 있는지 확인
        // 대화에 표시를 해야하기 때문
        //List<Data_Quest> myQuestList = Game_Manager.current.GetNews.myQuestList;

    }

    private void SetLandingUI()
    {
        Game_Manager.current.GetLanding.SetLanding(landingStruct);
        Game_Manager.current.GetLanding.outLanding = OutLanding;
        Game_Manager.current.GetMainUI.OpenCanvas(false);
    }

    void OutLanding()
    {
        // 카메라 포커스 제거
        cameraPosition.SetActive(false);
        Game_Manager.current.GetMainUI.OpenCanvas(true);
        Game_Manager.current.SetThemeMusic();
    }
}
