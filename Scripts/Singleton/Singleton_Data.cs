using System.Collections.Generic;
using UnityEngine;
using static Data_Manager;

public class Singleton_Data : MonoSingleton<Singleton_Data>
{
    //public Dictionary<string, Data_Manager.TranslateString> Dict_DialogString = new Dictionary<string, Data_Manager.TranslateString>();
    //public Dictionary<string, Data_Manager.TranslateString> Dict_TranslateString = new Dictionary<string, Data_Manager.TranslateString>();
    public Dictionary<string, AudioClip> Dict_Audio = new Dictionary<string, AudioClip>();
    //public Dictionary<string, Skill_Set> Dict_SkillSet = new Dictionary<string, Skill_Set>();
    //public Translation translation;
    public Dictionary<string, FishStruct> Dict_Fish = new Dictionary<string, FishStruct>();
    public Dictionary<string, UsedStruct> Dict_Used = new Dictionary<string, UsedStruct>();


    //public void SetDictionary_DialogString(List<Data_Manager.TranslateString> _data)
    //{
    //    Dict_DialogString = SetTranslation(_data);
    //}

    //public void SetDictionary_TranslationString(List<Data_Manager.TranslateString> _data)
    //{
    //    Dict_TranslateString = SetTranslation(_data);
    //}

    //public string TryTranslation(int _type, string _id)
    //{
    //    Dictionary<string, Data_Manager.TranslateString> temp = default;
    //    switch (_type)
    //    {
    //        case 0:
    //            temp = Dict_DialogString;
    //            break;

    //        case 1:
    //            temp = Dict_TranslateString;
    //            break;
    //    }

    //    switch (translation)
    //    {
    //        case Translation.Korean:
    //            return temp[_id].KR;

    //        case Translation.English:
    //            return temp[_id].EN;

    //        case Translation.Japanese:
    //            return temp[_id].JP;

    //        case Translation.Chinese:
    //            return temp[_id].CN;
    //    }
    //    return null;
    //}

    //Dictionary<string, Data_Manager.TranslateString> SetTranslation(List<Data_Manager.TranslateString> _data)
    //{
    //    Dictionary<string, Data_Manager.TranslateString> Dict_Temp = new Dictionary<string, Data_Manager.TranslateString>();
    //    for (int i = 0; i < _data.Count; i++)
    //    {
    //        string id = _data[i].ID;
    //        if (Dict_Temp.ContainsKey(id) == true)
    //        {
    //            Debug.LogError($"{id}와 같은 이름이 존재 합니다.");
    //        }
    //        else
    //        {
    //            Dict_Temp[id] = _data[i];
    //        }
    //    }
    //    return Dict_Temp;
    //}

    public void SetDictionary_Used(List<UsedStruct> _data)
    {
        Dict_Used = new Dictionary<string, UsedStruct>();
        for (int i = 0; i < _data.Count; i++)
        {
            string id = _data[i].itemStruct.id;
            if (Dict_Used.ContainsKey(id) == true)
            {
                Debug.LogError($"{id}와 같은 이름이 존재 합니다.");
            }
            else
            {
                Dict_Used[id] = _data[i];
            }
        }
    }

    public void SetDictionary_Fish(List<FishStruct> _data)
    {
        Dict_Fish = new Dictionary<string, FishStruct>();
        for (int i = 0; i < _data.Count; i++)
        {
            string id = _data[i].itemStruct.id;
            if (Dict_Fish.ContainsKey(id) == true)
            {
                Debug.LogError($"{id}와 같은 이름이 존재 합니다.");
            }
            else
            {
                Dict_Fish[id] = _data[i];
            }
        }
    }

    public ItemStruct GetItemStruct(string _id)
    {
        if (_id.Contains("Fs"))
        {
            return Dict_Fish[_id].itemStruct;
        }
        else if (_id.Contains("Us"))
        {
            return Dict_Used[_id].itemStruct;
        }
        return default;
    }

    //public enum Translation
    //{
    //    Korean,
    //    English,
    //    Japanese,
    //    Chinese
    //}

    public void SetDictionary_Audio(List<AudioClip> _data)
    {
        Dict_Audio = new Dictionary<string, AudioClip>();
        for (int i = 0; i < _data.Count; i++)
        {
            string id = _data[i].name;
            if (Dict_Audio.ContainsKey(id) == true)
            {
                Debug.LogError($"{id}와 같은 이름이 존재 합니다.");
            }
            else
            {
                Dict_Audio[id] = _data[i];
            }
        }
    }
}
