using System.Collections.Generic;
using UnityEngine;
using static Data_Manager;

public class Singleton_Data : MonoSingleton<Singleton_Data>
{
    //public Dictionary<string, Data_Manager.TranslateString> Dict_DialogString = new Dictionary<string, Data_Manager.TranslateString>();
    //public Dictionary<string, Data_Manager.TranslateString> Dict_TranslateString = new Dictionary<string, Data_Manager.TranslateString>();
    public Dictionary<string, AudioStruct> Dict_Audio = new Dictionary<string, AudioStruct>();
    public Dictionary<string, Sprite> Dict_Sprite = new Dictionary<string, Sprite>();
    //public Translation translation;
    public Dictionary<string, FishStruct> Dict_Fish = new Dictionary<string, FishStruct>();
    public Dictionary<string, UsedStruct> Dict_Used = new Dictionary<string, UsedStruct>();

    public Dictionary<string, LanguageStruct> Dict_Language = new Dictionary<string, LanguageStruct>();

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

    public void SetDictionary_Audio(List<AudioStruct> _data)
    {
        Dict_Audio = new Dictionary<string, AudioStruct>();
        for (int i = 0; i < _data.Count; i++)
        {
            string id = _data[i].id;
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

    public void SetDictionary_Sprite(List<Sprite> _data)
    {
        Dict_Sprite = new Dictionary<string, Sprite>();
        for (int i = 0; i < _data.Count; i++)
        {
            string id = _data[i].name;
            if (Dict_Sprite.ContainsKey(id) == true)
            {
                Debug.LogError($"{id}와 같은 이름이 존재 합니다.");
            }
            else
            {
                Dict_Sprite[id] = _data[i];
            }
        }
    }

    public void SetDictionary_Language(List<LanguageStruct> _data)
    {
        Dict_Language = new Dictionary<string, LanguageStruct>();
        for (int i = 0; i < _data.Count; i++)
        {
            string id = _data[i].id;
            if (Dict_Language.ContainsKey(id) == true)
            {
                Debug.LogError($"{id}와 같은 이름이 존재 합니다.");
            }
            else
            {
                Dict_Language[id] = _data[i];
            }
        }
    }

    public enum LanguageType
    {
        English,
        Korean,
        Japanese,
        Chinese,
        Count
    }
    public LanguageType languageType = LanguageType.English;

    public string GetLanguage(string _id)
    {
        if (Dict_Language.ContainsKey(_id) == true)
        {
            string text = string.Empty;
            switch (languageType)
            {
                case LanguageType.English:
                    text = Dict_Language[_id].english;
                    break;

                case LanguageType.Korean:
                    text = Dict_Language[_id].korean;
                    break;

                case LanguageType.Japanese:
                    text = Dict_Language[_id].japanese;
                    break;

                case LanguageType.Chinese:
                    text = Dict_Language[_id].chinese;
                    break;
            }
            if (string.IsNullOrEmpty(text) == false)
                return text;
        }
        return _id;
    }
}
