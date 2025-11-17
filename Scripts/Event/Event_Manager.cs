using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Data_Dialog;
using static Data_Event;
using static Data_Quest;

public class Event_Manager : MonoBehaviour
{
    public StaticOpenCanvas.CanvasStruct[] canvasStructs;
    bool _open = false;
    public Event_SelectButton selectButton;
    Data_Event eventData;
    public TMP_Text eventName;
    public TMP_Text eventDescription;
    public CanvasGroup gridCanvas;
    bool typing = false;
    float typingSpeed = 0.1f;
    float defaultTypingSpeed = 0.1f;
    public List<string> eventKeys = new List<string>();

    Vector2Int[] dialogVector;

    private void Start()
    {
        for (int i = 0; i < canvasStructs.Length; i++)
        {
            canvasStructs[i].rect.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (typing == true)// 스킵
                typing = false;
        }
        //if (Input.GetMouseButtonUp(1))
        //{
        //    RemoveSelectButton();
        //    StartEvent();
        //}
    }

    void SetDictKey()
    {
        eventKeys.Clear();
        foreach (var child in Singleton_Data.INSTANCE.Dict_Event)
        {
            eventKeys.Add(child.Key);
        }
    }

    public void StartEvent()
    {
        if (eventKeys == null || eventKeys.Count == 0)
            SetDictKey();

        //string key = eventKeys[Random.Range(0, eventKeys.Count)];
        string key = "Data_Event_0001";
        Data_Event tempData = Singleton_Data.INSTANCE.Dict_Event[key];
        SetEvent(tempData);
        Debug.LogWarning(key);
    }

    public void SetEvent(Data_Event _eventData)
    {
        StopAllCoroutines();
        SetGridCanvas(0f);

        eventData = _eventData;
        if (eventData as Data_Event_Start)
        {
            // 선택 이벤트라면 이름 세팅
            Data_Event_Start tempData = eventData as Data_Event_Start;
            eventName.text = Singleton_Data.INSTANCE.GetLanguage(tempData.eventName);
        }
        typingSpeed = defaultTypingSpeed;
        StartCoroutine(SetDialog());
    }

    IEnumerator SetDialog()
    {
        eventDescription.text = SetReplace(eventData.eventStruct);
        eventDescription.ForceMeshUpdate(true);// 메쉬 재 생성 (리셋)
        eventDescription.alpha = 0f;// 모든 글자 숨김
        yield return null;

        if (_open == false)// 열려 있지 않으면 열기
        {
            _open = true;
            yield return StartCoroutine(StaticOpenCanvas.OpenCanvas(canvasStructs, true));
        }
        TextStruct textStruct = eventData.eventStruct;
        StartCoroutine(Typing(textStruct));
        StartCoroutine(TextAction(textStruct));
        SetSelectButton();// 선택 버튼 세팅
    }

    string SetReplace(TextStruct _textStruct)
    {
        string replace = Singleton_Data.INSTANCE.GetLanguage(_textStruct.contents);// 번역
        dialogVector = new Vector2Int[_textStruct.dialogTypes.Length];
        for (int i = 0; i < _textStruct.dialogTypes.Length; i++)
        {
            string temp = "{" + i + "}";
            string setReplace = Singleton_Data.INSTANCE.GetLanguage(_textStruct.dialogTypes[i].replaceText);// 번역
            int startIndex = replace.IndexOf(temp);
            dialogVector[i] = new Vector2Int(startIndex, startIndex + setReplace.Length);
            replace = replace.Replace(temp, setReplace);
            Debug.LogWarning($"{temp} {replace} (startReplace : {startIndex}) -1이면 바꿀 단어를 못찾아서 뒤에 에러터질꺼임");
        }

        // 색, 사이즈
        int lastIndex = dialogVector.Length - 1;
        for (int i = lastIndex; i >= 0; i--)
        {
            string textColor = _textStruct.dialogTypes[i].textColor;
            float size = _textStruct.dialogTypes[i].textSize;
            replace = replace.Insert(dialogVector[i].y, "</size></color>");// 끼워 넣기
            replace = replace.Insert(dialogVector[i].x, $"<color=#{textColor}><size={size}>");
        }
        return replace;
    }

    IEnumerator Typing(TextStruct _textStruct, string _voice = null)
    {
        OpenSelectGroup(false);

        typing = true;
        int subIndex = 0;
        TMP_TextInfo textInfo = eventDescription.textInfo;
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (_textStruct.dialogTypes.Length > 0)
            {
                float speed = _textStruct.dialogTypes[subIndex].typingSpeed;
                if (i == dialogVector[subIndex].x)
                {
                    if (speed > 0)// 타이핑 스피드가 0 이상이라면..
                        typingSpeed = speed;
                }
                else if (i == dialogVector[subIndex].y)
                {
                    if (subIndex + 1 < _textStruct.dialogTypes.Length)
                        subIndex++;
                    typingSpeed = defaultTypingSpeed;// 기본 속도
                }
            }

            var charInfo = textInfo.characterInfo[i];
            if (charInfo.isVisible == false)
                continue;

            int materialIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;
            Color32[] vertexColors = textInfo.meshInfo[materialIndex].colors32;
            for (int j = 0; j < 4; j++)
            {
                int index = vertexIndex + j;
                vertexColors[index].a = (byte)255;// 활성화
            }

            if (typing == true)
            {
                if (_voice != null)// 타이핑 소리가 있다면 출력
                    Singleton_Audio.INSTANCE.Audio_Dialog(_voice);
                eventDescription.UpdateVertexData();
                yield return new WaitForSeconds(typingSpeed);
            }
        }
        eventDescription.UpdateVertexData();
        typingSpeed = defaultTypingSpeed;// 기본 속도로 변경
        yield return new WaitForSeconds(typingSpeed);

        typing = false;
        OpenSelectGroup(true);
    }

    IEnumerator TextAction(TextStruct _textStruct)
    {
        bool actionBool = true;
        TMP_MeshInfo[] cachedMeshInfo = eventDescription.textInfo.CopyMeshInfoVertexData();
        while (actionBool == true)
        {
            for (int i = 0; i < _textStruct.dialogTypes.Length; i++)
            {
                // x - 시작 포지션
                // y - 끝 포지션
                // z - 액션 타입
                if (_textStruct.dialogTypes[i].actionType == ActionType.None)// 액션 타입이 None이 아니면
                    continue;

                int x = dialogVector[i].x;
                int y = dialogVector[i].y;
                for (int c = x; c < y; c++)
                {
                    var charInfo = eventDescription.textInfo.characterInfo[c];
                    if (charInfo.isVisible == false)
                        continue;

                    int materialIndex = charInfo.materialReferenceIndex;
                    int vertexIndex = charInfo.vertexIndex;

                    // 원래 정점정보
                    Vector3[] sourceVertices = cachedMeshInfo[materialIndex].vertices;
                    // 현재 정점 정보를 얻고 덮어쓰기
                    Vector3[] destinationVertices = eventDescription.textInfo.meshInfo[materialIndex].vertices;
                    SetActingText(_textStruct.dialogTypes[i], vertexIndex, sourceVertices, destinationVertices, c);
                }
            }
            yield return null;

            eventDescription.UpdateVertexData();
            Debug.LogWarning("TextActing");
        }
    }

    void SetActingText(TextStruct.DialogType type, int vertexIndex, Vector3[] sourceVertices, Vector3[] destinationVertices, int _index)
    {
        switch (type.actionType)
        {
            case ActionType.None:

                break;

            case ActionType.Move:
                TryAimationMove(type, vertexIndex, sourceVertices, destinationVertices, _index);
                break;

            case ActionType.Wave:
                TryAimationWave(type, vertexIndex, sourceVertices, destinationVertices, _index);
                break;

            case ActionType.Jitter:
                TryAimationJitter(type, vertexIndex, sourceVertices, destinationVertices, _index);
                break;
        }
    }

    void TryAimationWave(TextStruct.DialogType type, int vertexIndex, Vector3[] sourceVertices, Vector3[] destinationVertices, int _index)
    {
        for (int v = 0; v < 4; v++)
        {
            int index = vertexIndex + v;
            float actionRange = type.actionInterval;
            float animTime = (Time.time * type.actionSpeed) + (type.actionInterval * _index);
            float x = Mathf.Sin(animTime + sourceVertices[index].y * type.actionInterval) * type.actionAngle.x;
            float y = Mathf.Cos(animTime + sourceVertices[index].x * type.actionInterval) * type.actionAngle.y;
            destinationVertices[index] = sourceVertices[index] + new Vector3(x, y, 0f);
        }
    }

    void TryAimationMove(TextStruct.DialogType type, int vertexIndex, Vector3[] sourceVertices, Vector3[] destinationVertices, int _index)
    {
        for (int v = 0; v < 4; v++)
        {
            int index = vertexIndex + v;
            float animTime = Time.time * type.actionSpeed + (type.actionInterval * _index);
            float x = Mathf.Sin(animTime) * type.actionAngle.x;
            float y = Mathf.Cos(animTime) * type.actionAngle.y;
            destinationVertices[index] = sourceVertices[index] + new Vector3(x, y, 0f);
        }
    }

    void TryAimationJitter(TextStruct.DialogType type, int vertexIndex, Vector3[] sourceVertices, Vector3[] destinationVertices, int _index)
    {
        for (int v = 0; v < 4; v++)
        {
            int index = vertexIndex + v;
            float x = Random.Range(-type.actionAngle.x, type.actionAngle.x);
            float y = Random.Range(-type.actionAngle.y, type.actionAngle.y);
            destinationVertices[index] = sourceVertices[index] + new Vector3(x, y, 0f);
        }
    }






    //=======================================================================================================
    // 셀렉트 버튼 세팅
    //=======================================================================================================

    // 클릭 선택시 아이디로 이벤트 결과 출력
    // 결과는 Data_Event방식으로 똑같이 출력하고 선택지는 나가기만
    // 이후 받는 아이템이랑 이런건 보상창 연결
    // 패널티 (돈을 잃는다. 등)는 바로 결과 출력

    public List<Event_SelectButton> dialogSelectButton = new List<Event_SelectButton>();
    private readonly Queue<Event_SelectButton> selectButtonQueue = new Queue<Event_SelectButton>();

    void OpenSelectGroup(bool _open)
    {
        if (_open)
        {
            StartCoroutine(OpenCanvas());
        }
        else
        {
            gridCanvas.gameObject.SetActive(false);
        }
    }

    IEnumerator OpenCanvas()
    {
        gridCanvas.gameObject.SetActive(true);
        float normalize = 0f;
        while (normalize < 1f)
        {
            normalize += Time.deltaTime * 15f;
            float alpha = normalize;
            SetGridCanvas(alpha);
            gridCanvas.transform.localPosition = Vector3.up * (1f - alpha) * 50f;
            yield return null;
        }
    }

    void SetGridCanvas(float _alpha)
    {
        gridCanvas.alpha = _alpha;
        gridCanvas.interactable = _alpha > 0;
        gridCanvas.blocksRaycasts = _alpha > 0;
    }

    void SetSelectButton()
    {
        if (eventData as Data_Event)
        {
            Data_Event tempData = eventData as Data_Event;
            for (int i = 0; i < eventData.eventSelect.Length; i++)
            {
                EventSelect selectStruct = new EventSelect
                {
                    selectDialog = eventData.eventSelect[i].selectDialog,
                    selectEvent = eventData.eventSelect[i].selectEvent,
                };
                SetSelectButton(selectStruct);
            }
        }
    }

    void SetSelectButton(EventSelect _selectStruct)// 버튼 세팅
    {
        Event_SelectButton button = GetSelectButton();
        button.gameObject.SetActive(true);
        button.SetStart(_selectStruct, SelectedButton);// 엔피씨 대화 추가
        button.transform.SetAsLastSibling();// 순서 변경
        dialogSelectButton.Add(button);
    }

    Event_SelectButton GetSelectButton()
    {
        if (selectButtonQueue.Count > 0)
            return selectButtonQueue.Dequeue();
        Event_SelectButton inst = Instantiate(selectButton, gridCanvas.transform);
        return inst;
    }

    void RemoveSelectButton()
    {
        for (int i = 0; i < dialogSelectButton.Count; i++)
        {
            selectButtonQueue.Enqueue(dialogSelectButton[i]);
            dialogSelectButton[i].gameObject.SetActive(false);
        }
        dialogSelectButton.Clear();
    }

    //=======================================================================================================
    // 셀렉트 버튼 액션
    //=======================================================================================================

    void SelectedButton(EventSelect _eventSelect)
    {
        StopAllCoroutines();// 기존 움직이는 폰트가 있다면 정지
        RemoveSelectButton();

        // 다음 대화 체크
        Data_Event tempEvent = _eventSelect.GetEventData();
        if (tempEvent == null)// 다음 대화가 없다면
        {
            _open = false;
            // 기존 대화 보상이 있는지 확인
            if (eventData as Data_Event_Result)// 보상 이벤트라면
            {
                Data_Event_Result tempData = eventData as Data_Event_Result;
                Game_Manager.current.GetInventory.SetResult(tempData.itemRewards);// 대화 이벤트 보상
                Game_Manager.current.GetMainUI.dele_CloseButton = CloseButton;
                Debug.LogWarning("이벤트 보상 - 인벤토리 열기");
            }
            else
            {
                Game_Manager.current.GetLanding.OpenLandingUI();// 섬 유아이 열기
                Debug.LogWarning("보상 대화가 아님");
            }
            StartCoroutine(StaticOpenCanvas.OpenCanvas(canvasStructs, false));// 이벤트 창 닫기
        }
        else
        {
            SetEvent(tempEvent);
        }
    }

    void CloseButton()
    {
        Game_Manager.current.OutOfControll(false);
        Game_Manager.current.GetInventory.CloseResult();//보상 닫기
        Game_Manager.current.GetMainUI.OpenCanvas(true);
    }
}
