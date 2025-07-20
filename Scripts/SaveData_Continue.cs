using System.Collections.Generic;
using UnityEngine;
using static SaveData_Continue;

public class SaveData_Continue : MonoBehaviour
{
    public class SetSaveContinue
    {
        public Vector3 playerPosition;
        public Quaternion playerRotation;
        public Vector3 playerScale;

        public float timeSpeed;
        public float minute;
        public int hour;
        public int day;

        public float money;
    }
    public SetSaveContinue setSaveContinue;
    public string saveData = "SaveContinue";

    public static SaveData_Continue current;

    private void Awake()
    {
        current = this;
    }

    //===========================================================================================================================
    // 저장 및 불러오기
    //===========================================================================================================================
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            SetContinue();
            Game_Manager.current.cameraManager.InputShake();
        }
    }

    public void SetContinue()
    {
        setSaveContinue = new SetSaveContinue
        {
            playerPosition = Game_Manager.current.player.transform.position,
            playerRotation = Game_Manager.current.player.transform.rotation,
            playerScale = Game_Manager.current.player.transform.localScale,

            timeSpeed = Game_Manager.current.timeUI.timeSpeed,
            minute = Game_Manager.current.timeUI.minute,
            hour = Game_Manager.current.timeUI.hour,
            day = Game_Manager.current.timeUI.day,

            money = Game_Manager.current.inventory.TryMoney,
        };
        SaveContinue();
    }

    public void GetContinue()
    {
        LoadContinue();
        if (setSaveContinue == null)
            return;

        Game_Manager.current.player.transform.position = setSaveContinue.playerPosition;
        Game_Manager.current.player.transform.rotation = setSaveContinue.playerRotation;
        Game_Manager.current.player.transform.localScale = setSaveContinue.playerScale;

        float timeSpeed = setSaveContinue.timeSpeed;
        float minute = setSaveContinue.minute;
        int hour = setSaveContinue.hour;
        int day = setSaveContinue.day;
        Game_Manager.current.timeUI.SetStart(timeSpeed, minute, hour, day);

        Game_Manager.current.inventory.TryMoney = setSaveContinue.money;
    }

    void SaveContinue()
    {
        Static_JsonManager.SaveCountinueData(saveData, setSaveContinue);
    }

    void LoadContinue()
    {
        if (Static_JsonManager.TryLoadCountinueData(saveData, out SetSaveContinue _data))
        {
            setSaveContinue = _data;
        }
    }

}
