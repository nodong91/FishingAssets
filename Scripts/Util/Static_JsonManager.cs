using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Static_JsonManager
{
    //======================================================================================
    // 폴더 찾기
    //======================================================================================

    static void FindFolder(string folderName)
    {
        DirectoryInfo dirInfo = new DirectoryInfo(folderName);
        if (dirInfo.Exists == false)
        {
            // 없으면 만들기
            dirInfo.Create();
        }
    }
    ////======================================================================================
    //// 채용 배치 캐릭터
    ////======================================================================================
    //public static void SaveRecruitmentStatus(string fileName, List<Data_RecruitmentStatus> recruitmentStatus)
    //{
    //    string filePath = Application.dataPath + "/Save/";
    //    // 폴더 없으면 생성
    //    FindFolder(filePath);

    //    string toJson = JsonHelper.ToJson(recruitmentStatus, prettyPrint: true);
    //    File.WriteAllText(filePath + fileName + ".json", toJson);
    //}
    //// 불러오기
    //public static bool TryLoadRecruitmentStatus(string fileName, out List<Data_RecruitmentStatus> recruitmentStatus)
    //{
    //    string filePath = Application.dataPath + "/Save/";
    //    string path = filePath + fileName + ".json";
    //    FileInfo fileInfo = new FileInfo(path);

    //    if (fileInfo.Exists == true)
    //    {
    //        string fromJson = File.ReadAllText(path);
    //        recruitmentStatus = JsonHelper.FromJson<Data_RecruitmentStatus>(fromJson);
    //        return true;
    //    }
    //    recruitmentStatus = default;
    //    return false;
    //}
    //======================================================================================
    // 캐릭터 정보 관련
    //======================================================================================

    //// 단일 저장
    //public static void SaveCustomizeData(string fileName, List<CharInfo> charInfos)
    //{
    //    string filePath = Application.dataPath + "/Save/";
    //    // 폴더 없으면 생성
    //    FindFolder(filePath);

    //    string toJson = JsonHelper.ToJson(charInfos, prettyPrint: true);
    //    //toJson = Static_AES.Program.Encrypt(toJson, "StatusData");          // 암호화 저장
    //    File.WriteAllText(filePath + fileName + ".json", toJson);
    //}

    //// 불러오기
    //public static bool TryLoadCustomizeData(string fileName, out List<CharInfo> charInfos)
    //{
    //    string filePath = Application.dataPath + "/Save/";
    //    string path = filePath + fileName + ".json";
    //    FileInfo fileInfo = new FileInfo(path);

    //    if (fileInfo.Exists == true)
    //    {
    //        string fromJson = File.ReadAllText(path);
    //        //fromJson = Static_AES.Program.Decrypt(fromJson, "StatusData");      // 복화
    //        charInfos = JsonHelper.FromJson<CharInfo>(fromJson);
    //        return true;
    //    }

    //    charInfos = new List<CharInfo>();
    //    return false;
    //}

    public static void RemoveFile(string fileName)
    {
        File.Delete(Application.dataPath + "/Save/" + fileName + ".json");
    }

    ////======================================================================================
    //// 카드 정보 관련
    ////======================================================================================
    //// 저장
    //public static void SaveCardData(string fileName, List<int> jsonCards)
    //{
    //    string filePath = Application.dataPath + "/Save/Save_Card/";
    //    // 폴더 없으면 생성
    //    FindFolder(filePath);

    //    string toJson = JsonHelper.ToJson(jsonCards, prettyPrint: true);
    //    File.WriteAllText(filePath + fileName + ".json", toJson);
    //}

    //// 불러오기
    //public static bool TryLoadCardData(string fileName, out List<int> jsonCards)
    //{
    //    string filePath = Application.dataPath + "/Save/Save_Card/";
    //    string path = filePath + fileName + ".json";
    //    FileInfo fileInfo = new FileInfo(path);

    //    if (fileInfo.Exists == true)
    //    {
    //        string fromJson = File.ReadAllText(path);
    //        jsonCards = JsonHelper.FromJson<int>(fromJson);
    //        return true;
    //    }
    //    jsonCards = default;
    //    return false;
    //}

    ////======================================================================================
    //// 옵션 데이터 관련
    ////======================================================================================

    //public static void SaveOptionData(string fileName, Data_Option option)
    //{
    //    string filePath = Application.dataPath + "/Save/";
    //    // 폴더 생성
    //    FindFolder(filePath);

    //    string toJson = JsonUtility.ToJson(option, prettyPrint: true);
    //    //toJson = Static_AES.Program.Encrypt(toJson, "SaveOptionData");          // 암호화 저장
    //    File.WriteAllText(filePath + fileName + ".json", toJson);
    //}

    //public static bool TryLoadOptionData(string fileName, out Data_Option option)
    //{
    //    string filePath = Application.dataPath + "/Save/";
    //    string path = filePath + fileName + ".json";
    //    FileInfo fileInfo = new FileInfo(path);

    //    if (fileInfo.Exists == true)
    //    {
    //        string fromJson = File.ReadAllText(path);
    //        //fromJson = Static_AES.Program.Decrypt(fromJson, "StatusData");      // 복화
    //        option = JsonUtility.FromJson<Data_Option>(fromJson);
    //        return true;
    //    }

    //    option = default;
    //    return false;
    //}
    ////======================================================================================
    //// 중간 세이브 데이터 관련
    ////======================================================================================

    //public static void SaveCountinueData(string fileName, Data_Countinue countinue)
    //{
    //    string filePath = Application.dataPath + "/Save/";
    //    // 폴더 생성
    //    FindFolder(filePath);

    //    string toJson = JsonUtility.ToJson(countinue, prettyPrint: true);
    //    //toJson = Static_AES.Program.Encrypt(toJson, "SaveOptionData");          // 암호화 저장
    //    File.WriteAllText(filePath + fileName + ".json", toJson);
    //}

    //public static bool TryLoadCountinueData(string fileName, out Data_Countinue countinue)
    //{
    //    string filePath = Application.dataPath + "/Save/";
    //    string path = filePath + fileName + ".json";
    //    FileInfo fileInfo = new FileInfo(path);

    //    if (fileInfo.Exists == true)
    //    {
    //        string fromJson = File.ReadAllText(path);
    //        //fromJson = Static_AES.Program.Decrypt(fromJson, "SaveOptionData");      // 복화
    //        countinue = JsonUtility.FromJson<Data_Countinue>(fromJson);
    //        return true;
    //    }

    //    countinue = default;
    //    return false;
    //}
    //======================================================================================
    // 인벤토리 저장
    //======================================================================================

    public static void SaveInventoryData(string fileName, List<UI_Inventory.SaveItemClass> _data)
    {
        string filePath = Application.dataPath + "/Save/";
        // 폴더 없으면 생성
        FindFolder(filePath);

        string toJson = JsonHelper.ToJson(_data, prettyPrint: true);
        File.WriteAllText(filePath + fileName + ".json", toJson);
    }
    // 불러오기
    public static bool TryLoadInventoryData(string fileName, out List<UI_Inventory.SaveItemClass> _data)
    {
        string filePath = Application.dataPath + "/Save/";
        string path = filePath + fileName + ".json";
        FileInfo fileInfo = new FileInfo(path);

        if (fileInfo.Exists == true)
        {
            string fromJson = File.ReadAllText(path);
            _data = JsonHelper.FromJson<UI_Inventory.SaveItemClass>(fromJson);
            return true;
        }
        _data = default;
        return false;
    }

    //======================================================================================
    // 도감 저장
    //======================================================================================

    public static void SaveFishGuideData(string fileName, List<FishGuide.SaveFishClass> _data)
    {
        string filePath = Application.dataPath + "/Save/";
        // 폴더 없으면 생성
        FindFolder(filePath);

        string toJson = JsonHelper.ToJson(_data, prettyPrint: true);
        File.WriteAllText(filePath + fileName + ".json", toJson);
    }

    public static bool TryLoadFishGuideData(string fileName, out List<FishGuide.SaveFishClass> _data)
    {
        string filePath = Application.dataPath + "/Save/";
        string path = filePath + fileName + ".json";
        FileInfo fileInfo = new FileInfo(path);

        if (fileInfo.Exists == true)
        {
            string fromJson = File.ReadAllText(path);
            _data = JsonHelper.FromJson<FishGuide.SaveFishClass>(fromJson);
            return true;
        }
        _data = default;
        return false;
    }

    //======================================================================================
    // Json 리스트 저장용 헬퍼
    //======================================================================================

    public static class JsonHelper
    {
        public static List<T> FromJson<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.Items;
        }

        public static string ToJson<T>(List<T> array, bool prettyPrint)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper, prettyPrint);
        }
    }

    public class Wrapper<T>
    {
        public List<T> Items;
    }
}

//======================================================================================
// 직렬화 클래스
//======================================================================================

//[System.Serializable]
//public struct CharInfo
//{
//    public int ID;
//    public bool Sex;
//    public string FullName;
//    public int Rank;// 레벨과 같은 개념

//    public string HairColor;
//    public string SkinColor;
//    public List<int> Armors;
//    public int Weapon;

//    public List<int> Cards;
//    public int inventoryAmount;
//    public InventoryItem[] inventoryItems;
//}

//[System.Serializable]
//public class Data_Countinue
//{
//    public List<CharInfo> TeamInfo;
//    // 맵 정보
//    public string MapDataName;
//    public int MapSeed;
//    public int FloorLevel;
//    public List<int> ClearedBlocks;
//    // 캐릭터 위치 정보
//    public Vector3 PlayerPosition;
//    public Quaternion PlayerRotation;
//    // 캐릭터 정보
//    public int CharNumber;
//    public int Hungry;
//    //public CharacterStatus Status;
//    // 인벤토리 정보
//    //public List<ItemSlot> Items;
//}

//[System.Serializable]
//public class Data_RecruitmentStatus
//{
//    public int gridX;
//    public int gridY;
//    public int charID;
//}
