using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Data_Dialog;

public class Dialog_Manager : MonoBehaviour, IPointerClickHandler
{
    public StaticOpenCanvas.CanvasStruct[] canvasStructs;
    Data_NPC dataNPC;
    Data_Dialog dataDialog;
    DialogStruct dialog;

    const float defaultTypingSpeed = 0.1f;
    const int defaultSize = 15;

    bool typing;
    public RawImage NPC_Image;
    public TMP_Text nameText, dialogText;
    public CanvasGroup selectCanvas;

    private float typingSpeed;
    Coroutine typingCoroutine, actionCoroutine;

    bool actionBool = false;

    public bool endDialog;
    public int currentDialog;

    public Dialog_SelectButton selectButton;
    public List<Dialog_SelectButton> dialogSelectButton = new List<Dialog_SelectButton>();
    private readonly Queue<Dialog_SelectButton> selectButtonQueue = new Queue<Dialog_SelectButton>();
    public RectTransform gridRect;

    RectTransform rectParent;

    public void SetStart()
    {
        rectParent = selectCanvas.GetComponent<RectTransform>();
        typingSpeed = defaultTypingSpeed;
        dialogText.fontSize = defaultSize;
        dialogText.color = Color.white;

        OpenCanvas(false);// 시작 닫기
    }

    public void DialogStart_NPC(Data_NPC _npc, int _dialogIndex)
    {
        int hour = Game_Manager.current.GetMainUI.timeUI.hour;
        Debug.LogWarning($"{_npc.npc_ID} : 현재 시간({hour}) 오픈 시간({_npc.openTime.x}~{_npc.openTime.y})");
        if (hour >= _npc.openTime.x && hour <= _npc.openTime.y)
        {
            // 오픈
            dataNPC = _npc;
            dataDialog = dataNPC.dataDialogs[_dialogIndex];
        }
        else
        {
            dataNPC = Singleton_Data.INSTANCE.Dict_NPC[String_NPC._player];
            dataDialog = dataNPC.dataDialogs[3];
        }
        nameText.text = dataNPC.npc_ID;
        NPC_Image.gameObject.SetActive(dataNPC.texture != null);
        NPC_Image.texture = dataNPC.texture;
        NPC_Image.SetNativeSize();

        Dialog_Npc(dataDialog);
        OpenCanvas(true);// 대화 시작
    }

    public void Dialog_Npc(Data_Dialog _dialog)
    {
        dataDialog = _dialog;

        // 기존 버튼 큐에 추가 및 제거
        for (int i = 0; i < dialogSelectButton.Count; i++)
        {
            dialogSelectButton[i].gameObject.SetActive(false);
            selectButtonQueue.Enqueue(dialogSelectButton[i]);
        }
        dialogSelectButton.Clear();

        currentDialog = 0;
        endDialog = false;
        selectCanvas.gameObject.SetActive(false);
        // 선택 버튼 생성
        for (int i = 0; i < dataDialog.selectStructs.Length; i++)
        {
            Dialog_SelectButton button = GetSelectButton();
            button.gameObject.SetActive(true);
            button.SetStart(dataDialog.selectStructs[i], SelectedButton);
            dialogSelectButton.Add(button);

            button.transform.SetAsLastSibling();// 순서 변경
        }
        DialogAction();
    }

    public void AddNPC(Data_NPC _npc, int _dialogIndex)
    {
        SelectStruct selectStruct = new SelectStruct
        {
            selectDialog = _npc.npc_ID,
            selectType = SelectStruct.SelectType.None,
            npcData = _npc,
            dialogIndex = _dialogIndex,
        };

        Dialog_SelectButton button = GetSelectButton();
        button.gameObject.SetActive(true);
        button.SetStart(selectStruct, SelectedButton);// 엔피씨 대화 추가
        dialogSelectButton.Add(button);

        button.transform.SetAsLastSibling();// 순서 변경
    }

    public void EventSelectButton(string _nameTag)
    {
        SelectStruct selectStruct = new SelectStruct
        {
            selectDialog = _nameTag,
            selectType = SelectStruct.SelectType.Event,
            npcData = null,
            dialogIndex = 0,
        };

        Dialog_SelectButton button = GetSelectButton();
        button.gameObject.SetActive(true);
        button.SetStart(selectStruct, SelectedButton);// 엔피씨 대화 추가
        dialogSelectButton.Add(button);

        button.transform.SetAsLastSibling();// 순서 변경
    }

    //======================================================================================================
    // 버튼 클릭
    //======================================================================================================

    void SelectedButton(SelectStruct _selectStruct)// 선택 버튼 클릭
    {
        StopAllCoroutines();// 기존 움직이는 폰트가 있다면 정지

        actionBool = false;
        if (_selectStruct.npcData != null)// 엔피씨데이터가 있으면 대화 시작
        {
            DialogStart_NPC(_selectStruct.npcData, _selectStruct.dialogIndex);
            return;
        }

        switch (_selectStruct.selectType)
        {
            case SelectStruct.SelectType.Out:
                // 섬 나가기
                Game_Manager.current.GetLanding.BackButton();
                break;
            case SelectStruct.SelectType.OpenShop:
                // 상점 열기
                Game_Manager.current.GetMainUI.OpenShop();// 상점창 열기
                Data_NPC data_NPC = Singleton_Data.INSTANCE.Dict_NPC[String_NPC._shop];
                Game_Manager.current.GetInventory.OpenShop(data_NPC);
                break;
            case SelectStruct.SelectType.OpenShipyard:
                // 조선소 열기
                Game_Manager.current.GetMainUI.OpenShop();// 조선소도 상점창
                data_NPC = Singleton_Data.INSTANCE.Dict_NPC[String_NPC._shipyard];
                Game_Manager.current.GetInventory.OpenShipyard(data_NPC);
                break;

            case SelectStruct.SelectType.OpenSmuggler:
                // 밀수꾼 상점 열기
                Game_Manager.current.GetMainUI.OpenShop();// 상점창
                data_NPC = Singleton_Data.INSTANCE.Dict_NPC[String_NPC._smuggler];
                Game_Manager.current.GetInventory.OpenShipyard(data_NPC);
                break;

            case SelectStruct.SelectType.Upgrade:
                if (Game_Manager.current.GetPlayer.FullHealth == false)
                {
                    data_NPC = Singleton_Data.INSTANCE.Dict_NPC[String_NPC._shipyard];
                    //DialogStart_NPC(data_NPC, 1);
                    Data_Dialog warnDialog = data_NPC.dataDialogs[1];
                    Dialog_Npc(warnDialog);
                    Debug.LogWarning("체력이 가득 차지 않았으면 스킬창 못열게");
                    return;
                }
                Game_Manager.current.GetInventory.CloseShop();
                Game_Manager.current.GetSkill.OpenCanvas(true);
                break;

            case SelectStruct.SelectType.Result:
                // 퀘스트 결과 아이템 받기
                Game_Manager.current.GetMainUI.OpenQuestResult();
                break;

            case SelectStruct.SelectType.Rest:
                Game_Manager.current.GetLanding.RestButton();
                break;

            case SelectStruct.SelectType.NoticeBoard:
                Game_Manager.current.GetLanding.BoardButton();
                break;

            case SelectStruct.SelectType.Tutorial:
                Game_Manager.current.GetTutorial.StartTutorial();
                break;

            case SelectStruct.SelectType.Event:
                Game_Manager.current.GetEvent.StartEvent();
                break;
        }
        OpenCanvas(false);
    }

    public void Tutorial_Upgrade()
    {
        Game_Manager.current.GetSkill.OpenCanvas(true);
        OpenCanvas(false);
    }

    Dialog_SelectButton GetSelectButton()
    {
        if (selectButtonQueue.Count > 0)
            return selectButtonQueue.Dequeue();
        Dialog_SelectButton inst = Instantiate(selectButton, gridRect);
        return inst;
    }

    void OpenCanvas(bool _open)
    {
        StartCoroutine(StaticOpenCanvas.OpenCanvas(canvasStructs, _open));
    }

    void DialogAction()
    {
        if (typing == true)
        {
            StartTyping(false);// 스킵
        }
        else if (endDialog == false)
        {
            if (currentDialog >= dataDialog.dialogStructs.Length)
            {
                // 대화 끝
                EndDialog();// 스킵으로 끝
                return;
            }
            // 다음 대화 진행
            StopAllCoroutines();
            typingCoroutine = StartCoroutine(StartDialog());
        }
        else if (endDialog == true)
        {
            Debug.LogWarning("eijfhoiajso;idfjoaijeoifj;oisdfnbaousdfh");
        }
    }

    IEnumerator StartDialog()
    {
        dialog = dataDialog.dialogStructs[currentDialog];
        dialogText.text = TryDialogString();
        dialogText.ForceMeshUpdate(true);// 메쉬 재 생성 (리셋)
        dialogText.alpha = 0f;// 모든 글자 숨김
        yield return null;

        StartTyping(true);
        StartActing();

        currentDialog++;
    }

    string TryDialogString()
    {
        string temp = dialog.contents;
        int length = dialog.dialogTypes.Length - 1;
        for (int i = length; i >= 0; i--)
        {
            float size = dialog.dialogTypes[i].textSize;
            string textColor = dialog.dialogTypes[i].textColor;
            int x = dialog.dialogTypes[i].dialogIndex.x;
            int y = dialog.dialogTypes[i].dialogIndex.y;
            temp = temp.Insert(y, "</size></color>");
            temp = temp.Insert(x, $"<color=#{textColor}><size={size}>");
        }
        return temp;
    }

    void EndDialog()
    {
        endDialog = true;
        if (dialogSelectButton.Count > 0)
        {
            // 메뉴 출력
            for (int i = 0; i < dialogSelectButton.Count; i++)
            {
                dialogSelectButton[i].gameObject.SetActive(true);
            }
            typingCoroutine = StartCoroutine(SetSelectDialog());
        }
    }

    IEnumerator SetSelectDialog()// 선택지 버튼 열기
    {
        selectCanvas.gameObject.SetActive(true);
        float normalize = 0f;
        while (normalize < 1f)
        {
            normalize += Time.deltaTime * 10f;
            selectCanvas.alpha = normalize;
            rectParent.anchoredPosition = Vector3.Lerp(Vector3.up * 30f, Vector3.zero, normalize);
            yield return null;
        }
    }

    void OpenShop()
    {

    }

    void OpenShipyard()
    {

    }

    void OpenQuest()
    {

    }

    void StartActing()
    {
        if (actionCoroutine != null)
            StopCoroutine(actionCoroutine);
        actionCoroutine = StartCoroutine(TextAction(dialog));
    }

    IEnumerator TextAction(DialogStruct _dialogStruct)
    {
        actionBool = true;
        TMP_Text component = dialogText;
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

    //===================================================================================================
    // 글자 움직임   
    //===================================================================================================
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

    void StartTyping(bool _typing)
    {
        typing = _typing;
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(Typing(dialog));
    }

    IEnumerator Typing(DialogStruct _dialogStruct)
    {
        int subIndex = 0;
        TMP_TextInfo textInfo = dialogText.textInfo;
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
                if (dataNPC != null)
                    Singleton_Audio.INSTANCE.Audio_Dialog(dataNPC.voice);
                dialogText.UpdateVertexData();
                yield return new WaitForSeconds(typingSpeed);
            }
        }
        dialogText.UpdateVertexData();
        typingSpeed = defaultTypingSpeed;// 기본 속도로 변경
        yield return new WaitForSeconds(typingSpeed);

        typing = false;
        if (currentDialog >= dataDialog.dialogStructs.Length)
        {
            // 대화 끝
            EndDialog();// 자연스럽게 끝
        }
    }

    //IEnumerator WaitingNext()
    //{
    //    dialogIndex++;

    //    float normalize = 0f;
    //    while (normalize < 1f && typing == false)
    //    {
    //        normalize += Time.deltaTime / 3f;
    //        Debug.LogWarning("다음 문장 기다림");
    //        yield return null;
    //    }
    //}

    public void OnPointerClick(PointerEventData eventData)
    {
        DialogAction();// 대화 액션 실행
    }
}