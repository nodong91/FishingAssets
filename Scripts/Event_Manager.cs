using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
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
    List<string> eventKeys = new List<string>();

    private void Start()
    {
        for (int i = 0; i < canvasStructs.Length; i++)
        {
            canvasStructs[i].rect.gameObject.SetActive(false);
        }
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
        if(eventKeys == null || eventKeys.Count == 0)
        {
            SetDictKey();
        }
        string key = eventKeys[Random.Range(0, eventKeys.Count)];
        Data_Event tempData = Singleton_Data.INSTANCE.Dict_Event[key];
        SetEvent(tempData);
        Debug.LogWarning(key);
    }

    public void SetEvent(Data_Event _evnet)
    {
        eventData = _evnet;
        if (eventData as Data_Event_Select)
        {
            Data_Event_Select tempData = eventData as Data_Event_Select;
            eventName.text = tempData.eventName;
        }
        StartCoroutine(OpenEvent());
    }

    IEnumerator OpenEvent()
    {
        eventDescription.text = TryDialogString(eventData.eventDescription);
        eventDescription.ForceMeshUpdate(true);// 메쉬 재 생성 (리셋)
        eventDescription.alpha = 0f;// 모든 글자 숨김
        yield return null;

        if (_open == false)// 열려 있지 않으면 열기
        {
            _open = true;
            yield return StartCoroutine(StaticOpenCanvas.OpenCanvas(canvasStructs, true));
        }

        StartCoroutine(Typing(eventData.eventDescription));
        StartCoroutine(TextAction(eventData.eventDescription));
        SetSelectButton();// 선택 버튼 세팅
    }

    string TryDialogString(DialogStruct _dialogStruct)
    {
        string temp = _dialogStruct.contents;
        int length = _dialogStruct.dialogTypes.Length - 1;
        for (int i = length; i >= 0; i--)
        {
            float size = _dialogStruct.dialogTypes[i].textSize;
            string textColor = _dialogStruct.dialogTypes[i].textColor;
            int x = _dialogStruct.dialogTypes[i].dialogIndex.x;
            int y = _dialogStruct.dialogTypes[i].dialogIndex.y;
            temp = temp.Insert(y, "</size></color>");
            temp = temp.Insert(x, $"<color=#{textColor}><size={size}>");
        }
        return temp;
    }

    //=======================================================================================================
    // 폰트 액션
    //=======================================================================================================

    IEnumerator Typing(DialogStruct _dialogStruct, string _voice = null)
    {
        SetGridCanvas(0f);
        typing = true;
        int subIndex = 0;
        TMP_TextInfo textInfo = eventDescription.textInfo;
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (_dialogStruct.dialogTypes.Length > 0)
            {
                float speed = _dialogStruct.dialogTypes[subIndex].typingSpeed;
                if (i == _dialogStruct.dialogTypes[subIndex].dialogIndex.x)
                {
                    if (speed > 0)// 타이핑 스피드가 0 이상이라면..
                        typingSpeed = speed;
                }
                else if (i == _dialogStruct.dialogTypes[subIndex].dialogIndex.y)
                {
                    if (subIndex + 1 < _dialogStruct.dialogTypes.Length)
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
        SetGridCanvas(1f);
    }

    void SetGridCanvas(float _alpha)
    {
        gridCanvas.alpha = _alpha;
        gridCanvas.interactable = _alpha > 0;
        gridCanvas.blocksRaycasts = _alpha > 0;
    }


    IEnumerator TextAction(DialogStruct _dialogStruct)
    {
        bool actionBool = true;
        TMP_Text component = eventDescription;
        TMP_MeshInfo[] cachedMeshInfo = component.textInfo.CopyMeshInfoVertexData();
        while (actionBool == true)
        {
            for (int i = 0; i < _dialogStruct.dialogTypes.Length; i++)
            {
                // x - 시작 포지션
                // y - 끝 포지션
                // z - 액션 타입
                if (_dialogStruct.dialogTypes[i].actionType == ActionType.None)// 액션 타입이 None이 아니면
                    continue;

                int x = _dialogStruct.dialogTypes[i].dialogIndex.x;
                int y = _dialogStruct.dialogTypes[i].dialogIndex.y;
                for (int c = x; c < y; c++)
                {
                    var charInfo = component.textInfo.characterInfo[c];
                    if (charInfo.isVisible == false)
                        continue;

                    int materialIndex = charInfo.materialReferenceIndex;
                    int vertexIndex = charInfo.vertexIndex;

                    // 원래 정점정보
                    Vector3[] sourceVertices = cachedMeshInfo[materialIndex].vertices;
                    // 현재 정점 정보를 얻고 덮어쓰기
                    Vector3[] destinationVertices = component.textInfo.meshInfo[materialIndex].vertices;
                    SetActingText(_dialogStruct.dialogTypes[i], vertexIndex, sourceVertices, destinationVertices, c);
                }
            }
            yield return null;

            component.UpdateVertexData();
            Debug.LogWarning("TextActing");
        }
    }

    void SetActingText(DialogStruct.DialogType type, int vertexIndex, Vector3[] sourceVertices, Vector3[] destinationVertices, int _index)
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

    void TryAimationWave(DialogStruct.DialogType type, int vertexIndex, Vector3[] sourceVertices, Vector3[] destinationVertices, int _index)
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

    void TryAimationMove(DialogStruct.DialogType type, int vertexIndex, Vector3[] sourceVertices, Vector3[] destinationVertices, int _index)
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

    void TryAimationJitter(DialogStruct.DialogType type, int vertexIndex, Vector3[] sourceVertices, Vector3[] destinationVertices, int _index)
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

    void SetSelectButton()
    {
        if (eventData as Data_Event)
        {
            Data_Event tempData = eventData as Data_Event;
            for (int i = 0; i < tempData.eventSelect.Length; i++)
            {
                EventSelect selectStruct = new EventSelect
                {
                    selectDialog = tempData.eventSelect[i].selectDialog,
                    selectEvent = tempData.eventSelect[i].selectEvent,
                };
                SetSelectButton(selectStruct);
            }
        }
    }

    void SetSelectButton(EventSelect _selectStruct)
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
            if (eventData as Data_Event_Result)
            {
                Data_Event_Result tempData = eventData as Data_Event_Result;
                ResultStruct resultStruct = new ResultStruct
                {
                    inventorySize = new Vector2Int(5, 5),
                    money = 0,
                    itemID = tempData.itemRewards
                };
                Game_Manager.current.GetInventory.SetResult(resultStruct);
                Game_Manager.current.GetMainUI.OpenResult();
                Debug.LogWarning("이벤트 보상 - 인벤토리 열기");
            }
            else
            {
                Game_Manager.current.GetLanding.OpenLandingUI();
                Debug.LogWarning("보상 대화가 아님");
            }
            StartCoroutine(StaticOpenCanvas.OpenCanvas(canvasStructs, false));// 이벤트 창 닫기
        }
        else
        {
            SetEvent(tempEvent);
        }
    }
}
