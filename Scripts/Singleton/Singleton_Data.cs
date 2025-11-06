using System.Collections.Generic;
using UnityEngine;
using static Data_Manager;

public class Singleton_Data : MonoSingleton<Singleton_Data>
{
    public Dictionary<string, AudioStruct> Dict_Audio = new Dictionary<string, AudioStruct>();
    public Dictionary<string, Sprite> Dict_Sprite = new Dictionary<string, Sprite>();
    public Dictionary<string, FishStruct> Dict_Fish = new Dictionary<string, FishStruct>();
    public Dictionary<string, UsedStruct> Dict_Used = new Dictionary<string, UsedStruct>();
    public Dictionary<string, SkillStruct> Dict_Skill = new Dictionary<string, SkillStruct>();
    public Dictionary<string, QuestStruct> Dict_Quest = new Dictionary<string, QuestStruct>();
    public Dictionary<string, Data_NPC> Dict_NPC = new Dictionary<string, Data_NPC>();
    public Dictionary<string, Data_Ship> Dict_Ship = new Dictionary<string, Data_Ship>();
    public Dictionary<string, Data_Event_Select> Dict_Event = new Dictionary<string, Data_Event_Select>();
    public Dictionary<string, LanguageStruct> Dict_Language = new Dictionary<string, LanguageStruct>();

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
        if (_id.Contains("fs"))
        {
            return Dict_Fish[_id].itemStruct;
        }
        else if (_id.Contains("us"))
        {
            return Dict_Used[_id].itemStruct;
        }
        return default;
    }

    public void SetSkillStruct(List<SkillStruct> _data)
    {
        Dict_Skill = new Dictionary<string, SkillStruct>();
        for (int i = 0; i < _data.Count; i++)
        {
            string id = _data[i].id;
            if (Dict_Skill.ContainsKey(id) == true)
            {
                Debug.LogError($"{id}와 같은 이름이 존재 합니다.");
            }
            else
            {
                Dict_Skill[id] = _data[i];
            }
        }
    }

    public void SetQuestStruct(List<QuestStruct> _data)
    {
        Dict_Quest = new Dictionary<string, QuestStruct>();
        for (int i = 0; i < _data.Count; i++)
        {
            string id = _data[i].id;
            if (Dict_Quest.ContainsKey(id) == true)
            {
                Debug.LogError($"{id}와 같은 이름이 존재 합니다.");
            }
            else
            {
                Dict_Quest[id] = _data[i];
            }
        }
    }

    public void SetEvnetStruct(List<Data_Event_Select> _data)
    {
        Dict_Event = new Dictionary<string, Data_Event_Select>();
        for (int i = 0; i < _data.Count; i++)
        {
            string id = _data[i].name;
            if (Dict_Event.ContainsKey(id) == true)
            {
                Debug.LogError($"({id})와 같은 이름이 존재 합니다.");
            }
            else
            {
                Dict_Event[id] = _data[i];
            }
        }
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

    public void SetDictionary_NPC(List<Data_NPC> _data)
    {
        Dict_NPC = new Dictionary<string, Data_NPC>();
        for (int i = 0; i < _data.Count; i++)
        {
            string id = _data[i].npc_ID;
            if (Dict_NPC.ContainsKey(id) == true)
            {
                Debug.LogError($"{id}와 같은 이름이 존재 합니다.");
            }
            else
            {
                Dict_NPC[id] = _data[i];
            }
        }
    }

    public void SetDictionary_ShipData(List<Data_Ship> _data)
    {
        Dict_Ship = new Dictionary<string, Data_Ship>();
        for (int i = 0; i < _data.Count; i++)
        {
            string id = _data[i].id;
            if (Dict_Ship.ContainsKey(id) == true)
            {
                Debug.LogError($"{id}와 같은 이름이 존재 합니다.");
            }
            else
            {
                Dict_Ship[id] = _data[i];
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
        //Debug.LogWarning($"번역 : {languageType}");
        if (_id == null)
            return "";

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
