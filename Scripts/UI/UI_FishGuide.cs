using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FishGuide : MonoBehaviour
{
    [System.Serializable]
    public struct FishStruct
    {
        public string name;
        public CanvasGroup canvasGroup;
        public List<UI_FishCard> cards;
    }
    public List<FishStruct> fishStructList;
    private FishStruct currentFishStruct;

    public List<Data_Manager.FishStruct> allFishStruct;// 일단 물고기 정보 대신

    public Transform parent;
    public CanvasGroup parentBase;
    public UI_FishCard cardBase;
    UI_FishCard selectCard;
    public Button closeButton;

    public Toggle[] toggles;
    public int currentIndex;
    Queue<FishStruct> instQueue = new Queue<FishStruct>();

    public void SetStart()
    {
        LoadFishGuide();

        foreach (var fish in Singleton_Data.INSTANCE.Dict_Fish)
        {
            Data_Manager.FishStruct temp = fish.Value;
            allFishStruct.Add(temp);
            //AddFishClass(temp.id, 3.5f);
        }
        closeButton.onClick.AddListener(delegate { OpenCanvas(false); });
        OpenCanvas(false);
        for (int i = 0; i < toggles.Length; i++)
        {
            int index = i;
            toggles[index].onValueChanged.AddListener(delegate { SetToggle(index); });
        }
        toggles[0].isOn = true;
        SetCurrentFishStruct();
    }

    public void OpenCanvas(bool _open)
    {
        gameObject.SetActive(_open);
    }

    void SetToggle(int _index)
    {
        if (toggles[_index].isOn == true && currentIndex != _index)
        {
            bool outBool = currentIndex > _index;
            currentIndex = _index;
            StartCoroutine(MoveChange(outBool));
        }
    }

    void SetCurrentFishStruct()
    {
        currentFishStruct = GetFishStruct();
        int startIndex = currentIndex * 21;
        for (int i = 0; i < currentFishStruct.cards.Count; i++)
        {
            UI_FishCard tempCard = currentFishStruct.cards[i];
            int index = i + startIndex;
            if (index >= allFishStruct.Count)
            {
                tempCard.CardDisplay(false);
                continue;
            }
            tempCard.CardDisplay(true);
            // 도감에 있는지 확인
            SetCard(tempCard, allFishStruct[index]);
        }
    }

    void SetCard(UI_FishCard _card, Data_Manager.FishStruct _fish)
    {
        string id = _fish.itemStruct.id;
        bool onDict = dictFishClass.ContainsKey(id);// 도감을 못 만들었으면 까맣게
        SaveFishClass tempFish = onDict == true ? dictFishClass[id] : null;
        _card.SetCard(_fish, tempFish);
    }

    IEnumerator MoveChange(bool _out)
    {
        FishStruct tempStruct = new FishStruct
        {
            name = currentFishStruct.name,
            canvasGroup = currentFishStruct.canvasGroup,
            cards = currentFishStruct.cards,
        };
        SetCurrentFishStruct();

        Vector3 outPos = _out == false ? Vector3.left : Vector3.right;
        float outLength = 300f;
        float normalize = 0f;
        while (normalize < 1f)
        {
            normalize += Time.deltaTime * 10f;
            if (tempStruct.canvasGroup != null)
            {
                float alpha = 1f - normalize;
                tempStruct.canvasGroup.transform.localPosition = Vector3.Lerp(outPos * outLength, Vector3.zero, alpha);
                SetCanvas(tempStruct.canvasGroup, alpha);
            }
            currentFishStruct.canvasGroup.transform.localPosition = Vector3.Lerp(outPos * -outLength, Vector3.zero, normalize);
            SetCanvas(currentFishStruct.canvasGroup, normalize);
            yield return null;
        }

        if (tempStruct.canvasGroup != null)
        {
            instQueue.Enqueue(tempStruct);
        }
    }

    void SetCanvas(CanvasGroup _canvasGroup, float _alpha)
    {
        bool alpha = (_alpha > 0f);
        _canvasGroup.alpha = _alpha;
        _canvasGroup.interactable = alpha;
        _canvasGroup.blocksRaycasts = alpha;
    }

    FishStruct GetFishStruct()
    {
        if (instQueue.Count > 0)
        {
            return instQueue.Dequeue();
        }
        CanvasGroup instCanvas = Instantiate(parentBase, parent);
        List<UI_FishCard> temp = new List<UI_FishCard>();
        for (int i = 0; i < 21; i++)// 카드 생성
        {
            UI_FishCard inst = Instantiate(cardBase, instCanvas.transform);
            inst.deleSelectCard = SelectCard;
            temp.Add(inst);
        }

        FishStruct fishStruct = new FishStruct
        {
            name = "",
            canvasGroup = instCanvas,
            cards = temp,
        };
        return fishStruct;
    }

    void SelectCard(UI_FishCard _card)
    {
        if (selectCard == null)
        {
            selectCard = Instantiate(cardBase, cardBase.transform.parent);
            CanvasGroup canvasGroup = selectCard.AddComponent<CanvasGroup>();
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            selectCard.transform.localScale = Vector3.one * 1.3f;
        }

        selectCard.gameObject.SetActive(_card != null);
        if (_card != null)
        {
            selectCard.transform.position = _card.transform.position;
            SetCard(selectCard, _card.fishStruct);
        }
    }

    //===========================================================================================================================
    // 저장 및 불러오기
    //===========================================================================================================================

    public Dictionary<string, SaveFishClass> dictFishClass = new Dictionary<string, SaveFishClass>();
    public void AddFishClass(string _id, float _size)
    {
        if (dictFishClass.ContainsKey(_id) == true)
        {
            SaveFishClass fish = dictFishClass[_id];
            fish.amount++;
            if (_size < fish.minSize)
            {
                fish.minSize = _size;
            }
            if (_size > fish.maxSize)
            {
                fish.maxSize = _size;
            }
        }
        else
        {
            SaveFishClass newClass = new SaveFishClass
            {
                id = _id,
                amount = 1,
                minSize = _size,
                maxSize = _size,
            };
            dictFishClass[_id] = newClass;
        }
        SaveFishGuide();
    }

    [System.Serializable]
    public class SaveFishClass
    {
        public string id;
        public int amount;// 잡은 마리 수
        public float minSize;// 잡은 최소 크기
        public float maxSize;// 잡은 최대 크기
    }
    public List<SaveFishClass> saveFishClass;

    void SaveFishGuide()
    {
        saveFishClass.Clear();
        foreach (var fish in dictFishClass)
        {
            saveFishClass.Add(fish.Value);
        }
        Static_JsonManager.SaveFishGuideData("FishGuideData", saveFishClass);
    }

    void LoadFishGuide()
    {
        if (Static_JsonManager.TryLoadFishGuideData("FishGuideData", out List<SaveFishClass> _data))
        {
            for (int i = 0; i < _data.Count; i++)
            {
                string key = _data[i].id;
                dictFishClass[key] = _data[i];
            }
        }
    }
}
