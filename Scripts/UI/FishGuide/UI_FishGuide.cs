using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FishGuide : MonoBehaviour
{
    public StaticOpenCanvas.CanvasStruct[] canvasStructs;
    [System.Serializable]
    public struct FishStructGuide
    {
        public string name;
        public CanvasGroup canvasGroup;
        public List<UI_FishCard> cards;
    }
    private FishStructGuide currentFishStruct;

    public List<Data_Manager.FishStruct> allFishStruct;// 일단 물고기 정보 대신

    public Transform parent;
    public CanvasGroup parentBase;
    public Vector2Int guideSize;
    int cardAmount;// 총 카드 개수
    GridLayoutGroup gridLayoutGroup;
    public UI_FishCard cardBase;
    public Custom_Button backButton;

    public Custom_Button leftButton;
    public Custom_Button rightButton;
    public TMPro.TMP_Text pageText;

    public int currentIndex;
    int maxIndex = 0;

    Queue<FishStructGuide> instQueue = new Queue<FishStructGuide>();
    public void CloseCanvas() => Game_Manager.current.GetMainUI?.CloseCanvas();
    public UI_FishInfo fishInfo;
    public Slider completeSlider;
    public TMPro.TMP_Text completeText;

    public void SetStart()
    {
        gridLayoutGroup = parentBase.GetComponent<GridLayoutGroup>();
        gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayoutGroup.constraintCount = guideSize.x;
        cardAmount = guideSize.x * guideSize.y;

        LoadFishGuide();

        foreach (var fish in Singleton_Data.INSTANCE.Dict_Fish)
        {
            allFishStruct.Add(fish.Value);
        }
        maxIndex = allFishStruct.Count / 12;

        leftButton.SetButton(delegate { NextButton(-1); });
        rightButton.SetButton(delegate { NextButton(1); });

        SetInstanceStruct();

        backButton.SetButton(CloseCanvas);
        OpenCanvas(false);
    }

    public void OpenCanvas(bool _open)
    {
        Camera_Manager.current.CameraFocusOut(_open);
        StartCoroutine(StaticOpenCanvas.OpenCanvas(canvasStructs, _open));
        if (_open == true)
        {
            currentIndex = 0;
            pageText.text = (currentIndex + 1).ToString();
            StartCoroutine(MoveChange(true));

            UI_FishCard tempCard = currentFishStruct.cards[0];
            SelectCard(tempCard);// 제일 앞 카드 정보 출력
        }
    }

    void NextButton(int _index)
    {
        currentIndex += _index;
        if (currentIndex > maxIndex)
        {
            currentIndex = 0;
        }
        else if (currentIndex < 0)
        {
            currentIndex = maxIndex;
        }
        pageText.text = (currentIndex + 1).ToString();
        bool outBool = _index > 0;
        StartCoroutine(MoveChange(outBool));
    }

    void SetInstanceStruct()
    {
        currentFishStruct = GetFishStruct();
        SetcurrentStructCheck();
    }

    // 도감에 있는지 확인
    void SetcurrentStructCheck()
    {
        int startIndex = currentIndex * cardAmount;// 시작 넘버
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
            CheckCard(tempCard, allFishStruct[index]);
        }
        SetCompleteSlider();// 슬라이더 표기
    }

    void CheckCard(UI_FishCard _card, Data_Manager.FishStruct _fish)
    {
        string id = _fish.itemStruct.id;
        bool onDict = dictFishClass.ContainsKey(id);// 도감을 못 만들었으면 까맣게
        SaveFishClass tempFish = onDict == true ? dictFishClass[id] : null;
        _card.SetCard(_fish, tempFish);
    }

    public void AddFishClass(string _id, float _size)
    {
        StatsManager.current.CatchFish();// 업적 체크
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
        SetcurrentStructCheck();// 도감 다시 체크
        SaveFishGuide();
    }

    IEnumerator MoveChange(bool _out)// 그리드 교체
    {
        FishStructGuide tempStruct = new FishStructGuide
        {
            name = currentFishStruct.name,
            canvasGroup = currentFishStruct.canvasGroup,
            cards = currentFishStruct.cards,
        };
        SetInstanceStruct();

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

    FishStructGuide GetFishStruct()
    {
        if (instQueue.Count > 0)
        {
            return instQueue.Dequeue();
        }

        CanvasGroup instCanvas = Instantiate(parentBase, parent);
        List<UI_FishCard> temp = new List<UI_FishCard>();
        for (int i = 0; i < cardAmount; i++)// 카드 생성
        {
            UI_FishCard inst = Instantiate(cardBase, instCanvas.transform);
            inst.deleSelectCard = SelectCard;
            temp.Add(inst);
        }

        FishStructGuide fishStruct = new FishStructGuide
        {
            name = "",
            canvasGroup = instCanvas,
            cards = temp,
        };
        return fishStruct;
    }

    void SelectCard(UI_FishCard _card)
    {
        fishInfo.SetStart(_card);
    }

    void SetCompleteSlider()// 슬라이트 표기
    {
        int currentCount = dictFishClass.Count;
        int allCount = allFishStruct.Count;
        completeSlider.value = (float)currentCount / allCount;
        float fontSize = completeText.fontSize - completeText.fontSize * 0.2f;
        completeText.text = $"{(int)(completeSlider.value * 10000f) * 0.01f}<size={fontSize}>%</size>";
    }

    //===========================================================================================================================
    // 저장 및 불러오기
    //===========================================================================================================================

    public Dictionary<string, SaveFishClass> dictFishClass = new Dictionary<string, SaveFishClass>();

    [System.Serializable]
    public class SaveFishClass
    {
        public string id;
        public int amount;// 잡은 마리 수
        public float minSize;// 잡은 최소 크기
        public float maxSize;// 잡은 최대 크기
    }
    public List<SaveFishClass> saveFishClass;

    void SaveFishGuide()// 생선 도감 저장
    {
        saveFishClass.Clear();
        foreach (var fish in dictFishClass)
        {
            saveFishClass.Add(fish.Value);
        }
        Static_JsonManager.SaveFishGuideData(String_Save._fishGuideData, saveFishClass);
    }

    void LoadFishGuide()
    {
        if (Static_JsonManager.TryLoadFishGuideData(String_Save._fishGuideData, out List<SaveFishClass> _data))
        {
            for (int i = 0; i < _data.Count; i++)
            {
                string key = _data[i].id;
                dictFishClass[key] = _data[i];
            }
        }
    }
}
