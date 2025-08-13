using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
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
    public TMP_Text dialogText;
    public CanvasGroup selectCanvas;

    private float typingSpeed;
    Coroutine typingCoroutine, actionCoroutine;

    bool actionBool = false;

    public bool endDialog;
    public int currentDialog;

    public Dialog_SelectButton selectButton;
    public List<Dialog_SelectButton> dialogSelectButton = new List<Dialog_SelectButton>();
    private readonly Queue<Dialog_SelectButton> selectButtonQueue = new Queue<Dialog_SelectButton>();

    RectTransform rectParent;

    public void SetStart()
    {
        rectParent = selectCanvas.GetComponent<RectTransform>();
        typingSpeed = defaultTypingSpeed;
        dialogText.fontSize = defaultSize;
        dialogText.color = Color.white;

        OpenCanvas(false);// 시작 닫기
    }

    public void DialogStart(Data_NPC _npc)
    {
        dataNPC = _npc;
        dataDialog = _npc.dataDialogs[0];
        NPC_Image.texture = _npc.texture;
        NPC_Image.SetNativeSize();

        DialogStart(_npc.dataDialogs[0]);
        // 퀘스트가 있는지 확인
        List<Data_Quest> checkQuests = Option_Manager.current.GetQuestManager.CheckNPC(_npc.npc_ID);
        Debug.LogError($"{_npc.npc_ID} : {checkQuests.Count}");
        // 퀘스트가 있다면 선택지 버튼 생성
        for (int i = 0; i < checkQuests.Count; i++)
        {
            Dialog_SelectButton button = SetSelectButton(checkQuests[i].selectStruct);
            button.transform.SetAsFirstSibling();// 순서 변경
            button.questData = checkQuests[i];
        }
        OpenCanvas(true);// 대화 시작
    }

    public void DialogStart(Data_Dialog _dialog)
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
            Dialog_SelectButton button = SetSelectButton(dataDialog.selectStructs[i]);
            button.transform.SetAsLastSibling();// 순서 변경
        }
        DialogAction();
    }

    Dialog_SelectButton SetSelectButton(SelectStruct _selectStruct)
    {
        Dialog_SelectButton button = GetSelectButton();
        button.gameObject.SetActive(true);
        button.SetStart(_selectStruct, InputButton);
        dialogSelectButton.Add(button);
        return button;
    }

    void InputButton(SelectStruct.SelectType _selectType)
    {
        Debug.LogWarning($"선택지 버튼 : {_selectType}");

    }

    Dialog_SelectButton GetSelectButton()
    {
        if (selectButtonQueue.Count > 0)
            return selectButtonQueue.Dequeue();
        Dialog_SelectButton inst = Instantiate(selectButton, rectParent);
        return inst;
    }

    public void OpenCanvas(bool _open)
    {
        StaticOpenCanvas.deleEndOpen = EndOpenCanvas;
        StartCoroutine(StaticOpenCanvas.OpenCanvas(canvasStructs, _open));
    }

    void EndOpenCanvas()
    {

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
                EndDialog();
                return;
            }
            // 다음 대화 진행
            StopAllCoroutines();
            typingCoroutine = StartCoroutine(StartDialog());
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
        // 메뉴 출력
        for (int i = 0; i < dialogSelectButton.Count; i++)
        {
            dialogSelectButton[i].gameObject.SetActive(true);
        }
        typingCoroutine = StartCoroutine(SetSelectDialog());
    }

    IEnumerator SetSelectDialog()
    {
        selectCanvas.gameObject.SetActive(true);
        float normalize = 0f;
        while (normalize < 1f)
        {
            normalize += Time.deltaTime * 10f;
            selectCanvas.alpha = normalize;
            rectParent.anchoredPosition = Vector3.Lerp(Vector3.down * 30f, Vector3.zero, normalize);
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

    public void OutDialog()
    {
        StopAllCoroutines();
        OpenCanvas(false);// 대화 완료
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
            Debug.LogWarning("TextAction");
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
            EndDialog();
        }
        else
        {
            //StartCoroutine(WaitingNext());
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
        DialogAction();
    }
}