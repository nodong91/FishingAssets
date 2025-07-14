using System.Collections.Generic;
using UnityEngine;

public class Singleton_Data : MonoSingleton<Singleton_Data>
{
    public Dictionary<string, Data_Manager.DialogStruct> Dict_Dialog = new Dictionary<string, Data_Manager.DialogStruct>();
    public Dictionary<string, Data_Manager.TranslateString> Dict_DialogString = new Dictionary<string, Data_Manager.TranslateString>();
    public Dictionary<string, Data_Manager.TranslateString> Dict_TranslateString = new Dictionary<string, Data_Manager.TranslateString>();
    public Dictionary<string, AudioClip> Dict_Audio = new Dictionary<string, AudioClip>();
    //public Dictionary<string, Skill_Set> Dict_SkillSet = new Dictionary<string, Skill_Set>();
    public Translation translation;
    public Dictionary<string, Data_Manager.FishStruct> Dict_Fish = new Dictionary<string, Data_Manager.FishStruct>();
    public Dictionary<string, Data_Manager.PartsStruct> Dict_Parts = new Dictionary<string, Data_Manager.PartsStruct>();
    public Dictionary<string, Data_Manager.EquipStruct> Dict_Equip = new Dictionary<string, Data_Manager.EquipStruct>();
    public Dictionary<string, Data_Manager.ItemStruct> Dict_Item = new Dictionary<string, Data_Manager.ItemStruct>();

    public void SetDictionary_Dialog(List<Data_Manager.DialogStruct> _data)
    {
        Dict_Dialog = new Dictionary<string, Data_Manager.DialogStruct>();
        for (int i = 0; i < _data.Count; i++)
        {
            string id = _data[i].ID;
            if (Dict_Dialog.ContainsKey(id) == true)
            {
                Debug.LogError($"{id}와 같은 이름이 존재 합니다.");
            }
            else
            {
                Dict_Dialog[id] = _data[i];
            }
        }
    }

    public void SetDictionary_DialogString(List<Data_Manager.TranslateString> _data)
    {
        Dict_DialogString = SetTranslation(_data);
    }

    public void SetDictionary_TranslationString(List<Data_Manager.TranslateString> _data)
    {
        Dict_TranslateString = SetTranslation(_data);
    }

    public string TryTranslation(int _type, string _id)
    {
        Dictionary<string, Data_Manager.TranslateString> temp = default;
        switch (_type)
        {
            case 0:
                temp = Dict_DialogString;
                break;

            case 1:
                temp = Dict_TranslateString;
                break;
        }

        switch (translation)
        {
            case Translation.Korean:
                return temp[_id].KR;

            case Translation.English:
                return temp[_id].EN;

            case Translation.Japanese:
                return temp[_id].JP;

            case Translation.Chinese:
                return temp[_id].CN;
        }
        return null;
    }

    Dictionary<string, Data_Manager.TranslateString> SetTranslation(List<Data_Manager.TranslateString> _data)
    {
        Dictionary<string, Data_Manager.TranslateString> Dict_Temp = new Dictionary<string, Data_Manager.TranslateString>();
        for (int i = 0; i < _data.Count; i++)
        {
            string id = _data[i].ID;
            if (Dict_Temp.ContainsKey(id) == true)
            {
                Debug.LogError($"{id}와 같은 이름이 존재 합니다.");
            }
            else
            {
                Dict_Temp[id] = _data[i];
            }
        }
        return Dict_Temp;
    }

    public void SetDictionary_Equip(List<Data_Manager.EquipStruct> _data)
    {
        Dict_Equip = new Dictionary<string, Data_Manager.EquipStruct>();
        for (int i = 0; i < _data.Count; i++)
        {
            string id = _data[i].itemStruct.id;
            if (Dict_Equip.ContainsKey(id) == true)
            {
                Debug.LogError($"{id}와 같은 이름이 존재 합니다.");
            }
            else
            {
                Dict_Equip[id] = _data[i];
            }
        }
    }

    public void SetDictionary_Fish(List<Data_Manager.FishStruct> _data)
    {
        Dict_Fish = new Dictionary<string, Data_Manager.FishStruct>();
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

    public void SetDictionary_Parts(List<Data_Manager.PartsStruct> _data)
    {
        Dict_Parts = new Dictionary<string, Data_Manager.PartsStruct>();
        for (int i = 0; i < _data.Count; i++)
        {
            string id = _data[i].id;
            if (Dict_Parts.ContainsKey(id) == true)
            {
                Debug.LogError($"{id}와 같은 이름이 존재 합니다.");
            }
            else
            {
                Dict_Parts[id] = _data[i];
            }
        }
    }

    public void SetDictionary_Item(List<Data_Manager.ItemStruct> _data)
    {
        Dict_Item = new Dictionary<string, Data_Manager.ItemStruct>();
        for (int i = 0; i < _data.Count; i++)
        {
            string id = _data[i].id;
            if (Dict_Item.ContainsKey(id) == true)
            {
                Debug.LogError($"{id}와 같은 이름이 존재 합니다.");
            }
            else
            {
                Dict_Item[id] = _data[i];
            }
        }
    }

    public enum Translation
    {
        Korean,
        English,
        Japanese,
        Chinese
    }

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
