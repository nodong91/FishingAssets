using System.Collections.Generic;
using UnityEngine;

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
        public UI_Time.WeatherType weatherType;

        public float energy;
        public float money;
        public List<Vector2Int> destroySlot;
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
    //public Static_JsonManager.InventoryData loadingTestData;
    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Z))
    //    {
    //        Static_JsonManager.InventoryData testData = new Static_JsonManager.InventoryData();
    //        testData.name = "jijoiajoisdjfoahsdk";
    //        List<UI_Inventory_Base.SaveItemClass> testClass1111 = new List<UI_Inventory_Base.SaveItemClass>();
    //        for (int i = 0; i < 3; i++)
    //        {
    //            UI_Inventory_Base.SaveItemClass itemClass = new UI_Inventory_Base.SaveItemClass();
    //            itemClass.id = "ejijfijosajd " + i;
    //            testClass1111.Add(itemClass);
    //        }
    //        testData.inventoryClass = testClass1111;
    //        Static_JsonManager.SaveTest("jijoiajoisdjfoahsdk", testData);
    //    }

    //    if (Input.GetKeyDown(KeyCode.X))
    //    {
    //        if (Static_JsonManager.TryLoadTest("jijoiajoisdjfoahsdk", out Static_JsonManager.InventoryData testData))
    //        {
    //            loadingTestData = testData;
    //        }
    //    }
    //}

    public void SetContinue()
    {
        setSaveContinue = new SetSaveContinue
        {
            playerPosition = Game_Manager.current.player.transform.position,
            playerRotation = Game_Manager.current.player.transform.rotation,
            playerScale = Game_Manager.current.player.transform.localScale,

            timeSpeed = Game_Manager.current.GetTimeUI.timeSpeed,
            minute = Game_Manager.current.GetTimeUI.minute,
            hour = Game_Manager.current.GetTimeUI.hour,
            day = Game_Manager.current.GetTimeUI.day,
            weatherType = Game_Manager.current.GetTimeUI.weatherType,

            energy = Game_Manager.current.GetInventory.TryEnergy,
            money = Game_Manager.current.GetInventory.TryMoney,

            destroySlot = Game_Manager.current.GetInventory.TryDestroySlot,
        };
        SaveContinue();
    }

    public void GetContinue()
    {
        LoadContinue();
        if (setSaveContinue == null)
            return;
        // 위치
        Game_Manager.current.player.transform.position = setSaveContinue.playerPosition;
        Game_Manager.current.player.transform.rotation = setSaveContinue.playerRotation;
        Game_Manager.current.player.transform.localScale = setSaveContinue.playerScale;

        float timeSpeed = setSaveContinue.timeSpeed;
        float minute = setSaveContinue.minute;
        int hour = setSaveContinue.hour;
        int day = setSaveContinue.day;
        Game_Manager.current.GetTimeUI.SetStart(timeSpeed, minute, hour, day);// 시간

        Game_Manager.current.GetInventory.TryMoney = setSaveContinue.money;// 돈
        Game_Manager.current.GetInventory.TryDestroySlot = setSaveContinue.destroySlot;// 부서진 슬롯
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
        else
        {
            setSaveContinue = new SetSaveContinue
            {
                playerPosition = Vector3.zero,
                playerRotation = Quaternion.identity,
                playerScale = Vector3.one,

                timeSpeed = 10f,
                minute = 30f,
                hour = 7,
                day = 0,
                weatherType = UI_Time.WeatherType.Sun,

                energy = 0f,
                money = 0f,
            };
        }
    }

}
