using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static Data_Manager;

public class Static_JsonManager
{
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

    //======================================================================================
    // 폴더 찾기
    //======================================================================================
    static string keyCode = "SaveDataKey"; // 테스트용
    static void FindFolder(string folderName)
    {
        DirectoryInfo dirInfo = new DirectoryInfo(folderName);
        if (dirInfo.Exists == false)
        {
            // 없으면 만들기
            dirInfo.Create();
        }
    }

    public static void RemoveFile(string fileName)
    {
        File.Delete(Application.dataPath + Const_Save._save + fileName + ".json");
    }

    static string Encrypt(string _data)
    {
        _data = Static_AES.Program.Encrypt(_data, keyCode);
        return _data;
    }

    static string Decrypt(string _data)
    {
        _data = Static_AES.Program.Decrypt(_data, keyCode);
        return _data;
    }

    //======================================================================================
    // 튜토리얼 관련
    //======================================================================================
    public static void SaveTutorialData(string fileName, List<string> _data)
    {
        string filePath = Application.dataPath + Const_Save._saveDontDestroy;
        // 폴더 생성
        FindFolder(filePath);

        string toJson = JsonHelper.ToJson(_data, prettyPrint: true);
        toJson = Encrypt(toJson);  // 암호화 저장
        File.WriteAllText(filePath + fileName + ".json", toJson);
    }

    public static bool TryLoadTutorialData(string fileName, out List<string> _data)
    {
        string filePath = Application.dataPath + Const_Save._saveDontDestroy;
        string path = filePath + fileName + ".json";
        FileInfo fileInfo = new FileInfo(path);

        if (fileInfo.Exists == true)
        {
            string fromJson = File.ReadAllText(path);
            fromJson = Decrypt(fromJson);  // 복호화
            if (fromJson == null)// 복호화 실패
            {
                _data = default;
                return false;
            }
            _data = JsonHelper.FromJson<string>(fromJson);
            return true;
        }

        _data = default;
        return false;
    }

    public static void SaveEnableSkillData(string fileName, List<Vector2Int> _data)
    {
        string filePath = Application.dataPath + Const_Save._saveDontDestroy;
        // 폴더 생성
        FindFolder(filePath);

        string toJson = JsonHelper.ToJson(_data, prettyPrint: true);
        toJson = Encrypt(toJson);  // 암호화 저장
        File.WriteAllText(filePath + fileName + ".json", toJson);
    }

    public static bool TryLoadEnableSkillData(string fileName, out List<Vector2Int> _data)
    {
        string filePath = Application.dataPath + Const_Save._saveDontDestroy;
        string path = filePath + fileName + ".json";
        FileInfo fileInfo = new FileInfo(path);

        if (fileInfo.Exists == true)
        {
            string fromJson = File.ReadAllText(path);
            fromJson = Decrypt(fromJson);   // 복호화
            if (fromJson == null)// 복호화 실패
            {
                _data = default;
                return false;
            }
            _data = JsonHelper.FromJson<Vector2Int>(fromJson);
            return true;
        }

        _data = default;
        return false;
    }

    //======================================================================================
    // 옵션 데이터 관련
    //======================================================================================

    public static void SaveOptionData(string fileName, Data_Option _data)
    {
        string filePath = Application.dataPath + Const_Save._saveDontDestroy;
        // 폴더 생성
        FindFolder(filePath);

        string toJson = JsonUtility.ToJson(_data, prettyPrint: true);
        toJson = Encrypt(toJson);  // 암호화 저장
        File.WriteAllText(filePath + fileName + ".json", toJson);
    }

    public static bool TryLoadOptionData(string fileName, out Data_Option _data)
    {
        string filePath = Application.dataPath + Const_Save._saveDontDestroy;
        string path = filePath + fileName + ".json";
        FileInfo fileInfo = new FileInfo(path);

        if (fileInfo.Exists == true)
        {
            string fromJson = File.ReadAllText(path);
            fromJson = Decrypt(fromJson);  // 복호화
            if (fromJson == null)// 복호화 실패
            {
                _data = default;
                return false;
            }
            _data = JsonUtility.FromJson<Data_Option>(fromJson);
            return true;
        }

        _data = default;
        return false;
    }
    //======================================================================================
    // 중간 세이브 데이터 관련
    //======================================================================================

    public static void SaveCountinueData(string fileName, Data_Continue _data)
    {
        string filePath = Application.dataPath + Const_Save._save;
        // 폴더 생성
        FindFolder(filePath);

        string toJson = JsonUtility.ToJson(_data, prettyPrint: true);
        toJson = Encrypt(toJson);  // 암호화 저장
        File.WriteAllText(filePath + fileName + ".json", toJson);
    }

    public static bool TryLoadCountinueData(string fileName, out Data_Continue _data)
    {
        string filePath = Application.dataPath + Const_Save._save;
        string path = filePath + fileName + ".json";
        FileInfo fileInfo = new FileInfo(path);

        if (fileInfo.Exists == true)
        {
            string fromJson = File.ReadAllText(path);
            fromJson = Decrypt(fromJson);  // 복호화
            if (fromJson == null)// 복호화 실패
            {
                _data = default;
                return false;
            }
            _data = JsonUtility.FromJson<Data_Continue>(fromJson);
            return true;
        }

        _data = default;
        return false;
    }

    //======================================================================================
    // 중간 세이브 데이터 관련
    //======================================================================================

    public static void SaveQuestData(string fileName, UI_QuestManager.SetQuest _data)
    {
        string filePath = Application.dataPath + Const_Save._save;
        // 폴더 생성
        FindFolder(filePath);

        string toJson = JsonUtility.ToJson(_data, prettyPrint: true);
        toJson = Encrypt(toJson);  // 암호화 저장
        File.WriteAllText(filePath + fileName + ".json", toJson);
    }

    public static bool TryLoadQuestData(string fileName, out UI_QuestManager.SetQuest _data)
    {
        string filePath = Application.dataPath + Const_Save._save;
        string path = filePath + fileName + ".json";
        FileInfo fileInfo = new FileInfo(path);

        if (fileInfo.Exists == true)
        {
            string fromJson = File.ReadAllText(path);
            fromJson = Decrypt(fromJson);  // 복호화
            if (fromJson == null)// 복호화 실패
            {
                _data = default;
                return false;
            }
            _data = JsonUtility.FromJson<UI_QuestManager.SetQuest>(fromJson);
            return true;
        }

        _data = default;
        return false;
    }

    //======================================================================================
    // 인벤토리 저장
    //======================================================================================

    [System.Serializable]
    public class InventoryData
    {
        public string name;
        public Vector2Int invenSize;
        public List<UI_Inventory_Base.SaveItemClass> saveItems;
    }

    public static void SaveInventory(string fileName, InventoryData _data)
    {
        //Debug.LogError($"저장 : {fileName} (아이템 수 : {_data.saveItems.Count})");
        if (String.IsNullOrEmpty(fileName))
            return;

        string filePath = Application.dataPath + Const_Save._save;
        // 폴더 생성
        FindFolder(filePath);

        string toJson = JsonUtility.ToJson(_data, prettyPrint: true);
        toJson = Encrypt(toJson);  // 암호화 저장
        File.WriteAllText(filePath + fileName + ".json", toJson);
    }

    public static bool TryLoadInventory(string fileName, out InventoryData _data)
    {
        string filePath = Application.dataPath + Const_Save._save;
        string path = filePath + fileName + ".json";
        FileInfo fileInfo = new FileInfo(path);

        if (fileInfo.Exists == true)
        {
            string fromJson = File.ReadAllText(path);
            fromJson = Decrypt(fromJson);  // 복호화
            if (fromJson == null)// 복호화 실패
            {
                _data = default;
                return false;
            }
            _data = JsonUtility.FromJson<InventoryData>(fromJson);
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
        string filePath = Application.dataPath + Const_Save._saveDontDestroy;
        // 폴더 없으면 생성
        FindFolder(filePath);

        string toJson = JsonHelper.ToJson(_data, prettyPrint: true);
        toJson = Encrypt(toJson);  // 암호화 저장
        File.WriteAllText(filePath + fileName + ".json", toJson);
    }

    public static bool TryLoadFishGuideData(string fileName, out List<FishGuide.SaveFishClass> _data)
    {
        string filePath = Application.dataPath + Const_Save._saveDontDestroy;
        string path = filePath + fileName + ".json";
        FileInfo fileInfo = new FileInfo(path);

        if (fileInfo.Exists == true)
        {
            string fromJson = File.ReadAllText(path);
            fromJson = Decrypt(fromJson);  // 복호화
            if (fromJson == null)// 복호화 실패
            {
                _data = default;
                return false;
            }
            _data = JsonHelper.FromJson<FishGuide.SaveFishClass>(fromJson);
            return true;
        }
        _data = default;
        return false;
    }

    //======================================================================================
    // 도감 저장
    //======================================================================================

    public static void SaveFishingNewsData(string fileName, UI_FishingNews.PriceStruct _data)
    {
        string filePath = Application.dataPath + Const_Save._save;
        // 폴더 없으면 생성
        FindFolder(filePath);

        string toJson = JsonUtility.ToJson(_data, prettyPrint: true);
        toJson = Encrypt(toJson);  // 암호화 저장
        File.WriteAllText(filePath + fileName + ".json", toJson);
    }

    public static bool TryLoadFishingNewsData(string fileName, out UI_FishingNews.PriceStruct _data)
    {
        string filePath = Application.dataPath + Const_Save._save;
        string path = filePath + fileName + ".json";
        FileInfo fileInfo = new FileInfo(path);

        if (fileInfo.Exists == true)
        {
            string fromJson = File.ReadAllText(path);
            fromJson = Decrypt(fromJson);  // 복호화
            if (fromJson == null)// 복호화 실패
            {
                _data = default;
                return false;
            }
            _data = JsonUtility.FromJson<UI_FishingNews.PriceStruct>(fromJson);
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

    //======================================================================================
    // 세이브 파일 전체 삭제
    //======================================================================================

    public static IEnumerator RemoveSaveFile()// 모든 세이브 파일 삭제
    {
        string path = Application.dataPath + Const_Save._save;
        FileDelete(path);
        yield return null;

        // 닫힐때 옵션이 저장이 되는데 창 데이터가 있어서 기존 내용이 저장됨
        Option_Manager.current.LoadOption();// 옵션 데이터 리셋
    }

    public static IEnumerator RemoveDontDestroyFile()// 모든 세이브 파일 삭제
    {
        string path = Application.dataPath + Const_Save._saveDontDestroy;
        FileDelete(path);
        yield return null;

        // 닫힐때 옵션이 저장이 되는데 창 데이터가 있어서 기존 내용이 저장됨
        Option_Manager.current.LoadOption();// 옵션 데이터 리셋
    }

    static void FileDelete(string _path)
    {
        FindFolder(_path);

        string[] allFiles = Directory.GetFiles(_path, "*", SearchOption.AllDirectories);
        foreach (string file in allFiles)
        {
            Debug.LogWarning("Delete File : " + file);// 파일 이름 찾아서 물고기 도감데이터 빼고 삭제
            File.Delete(file);
        }
        Directory.Delete(_path, true);
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
