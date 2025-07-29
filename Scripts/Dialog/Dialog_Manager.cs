using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using static UI_Main;
using UnityEngine.UI;
using static Trigger_Landing;

public class Dialog_Manager : MonoBehaviour, IPointerClickHandler
{
    public CanvasStruct[] canvasStructs;
    public string FXSound;
    const float defaultTypingSpeed = 0.1f;
    const int defaultSize = 15;

    bool typing;
    public RawImage NPC_Image;
    public TMP_Text dialogText;
    public Data_DialogType actionType;

    private float typingSpeed;
    Coroutine typingCoroutine, actionCoroutine;

    int dialogIndex;
    float interval;
    bool actionBool = false;

    int currentDialog;
    public Data_Manager.DialogStruct[] dialogStructs;
    [System.Serializable]
    public struct SelectButton
    {
        public TMP_Text text;
        public Button button;
    }
    public SelectButton[] selectButton;
    Data_Manager.DialogStruct dialog;

    private void Start()
    {
        SetDialogManager();
    }

    public void SetDialogManager()
    {
        typingSpeed = defaultTypingSpeed;
        dialogText.fontSize = defaultSize;
        dialogText.color = Color.white;

        OpenCanvas(false);
    }

    public void DialogStart()
    {
        currentDialog = 0;
        endDialog = false;
        for (int i = 0; i < selectButton.Length; i++)
        {
            int index = i;
            selectButton[i].button.onClick.AddListener(delegate { InputButton(index); });
            selectButton[i].button.gameObject.SetActive(false);
        }
        DialogAction();
        OpenCanvas(true);
    }

    public void OpenCanvas(bool _open)
    {
        StartCoroutine(OpenCanvasMoving(canvasStructs, _open));
    }

    void DialogAction()
    {
        if (typing == true)
        {
            StartTyping(false);// 스킵
        }
        else if(endDialog == false)
        {
            StopAllCoroutines();
            typingCoroutine = StartCoroutine(StartDialog());
        }
    }
    bool endDialog;
    //const string inID = "{";
    //const string outID = "}";
    //public string setID;
    //public List<Vector3Int> actionList = new List<Vector3Int>();
    //public List<float> speedList = new List<float>();

    IEnumerator StartDialog()
    {
        if (currentDialog >= dialogStructs.Length)
        {
            // 대화 끝
            endDialog = true;
            EndDialog();
            yield break;
        }
        //bool id = Singleton_Data.INSTANCE.Dict_Dialog.ContainsKey(setID);
        //dialogText.text = id ? TryDialog(setID) : $"<size=25>{setID}</size> : 아이디가 없습니다!!";
        dialog = dialogStructs[currentDialog];
        dialogText.text = TryDialogString();
        dialogText.ForceMeshUpdate(true);// 메쉬 재 생성 (리셋)
        dialogText.alpha = 0f;
        yield return null;

        //PreSetHide();// 글자 숨김
        //yield return new WaitForSeconds(defaultTypingSpeed);
        //yield return new WaitForFixedUpdate();
        //dialogText.alpha = 1f;
        StartTyping(true);
        StartActing();

        currentDialog++;
    }

    string TryDialogString()
    {
        string temp = dialog.dialogString;
        int length = dialog.dialogTypes.Length - 1;
        for (int i = length; i >= 0; i--)
        {
            float size = dialog.dialogTypes[i].size;
            string textColor = dialog.dialogTypes[i].color;
            int x = dialog.dialogTypes[i].dialogIndex.x;
            int y = dialog.dialogTypes[i].dialogIndex.y;
            temp = temp.Insert(y, "</size></color>");
            temp = temp.Insert(x, $"<color=#{textColor}><size={size}>");
        }
        return temp;
    }

    void EndDialog()
    {
        // 메뉴 출력
        for (int i = 0; i < selectButton.Length; i++)
        {
            selectButton[i].text.text = i.ToString();
            selectButton[i].button.gameObject.SetActive(true);
        }
    }

    void InputButton(int _index)
    {
        switch (_index)
        {
            case 0:
                OpenShop();
                break;
            case 1:
                OutDialog();
                break;
        }
    }

    void OpenShop()
    {
        LandingStruct getLandingData = Game_Manager.current.landingUI.GetLandingData;
        Game_Manager.current.inventory.OpenShop(getLandingData);
    }

    void OutDialog()
    {
        Game_Manager.current.inventory.CloseShop();
        OpenCanvas(false);
    }

    //string TryDialog(string _string)
    //{
    //    Data_Manager.DialogStruct mainDialog = Singleton_Data.INSTANCE.Dict_Dialog[_string];
    //    string mainString = mainDialog.ID;
    //    actionBool = false;
    //    List<string> ids = new List<string>();
    //    string[] start = mainString.Split(inID);// id추출
    //    for (int i = 0; i < start.Length; i++)
    //    {
    //        int endIndex = start[i].IndexOf(outID);
    //        if (endIndex > -1)
    //        {
    //            string result = start[i].Substring(0, endIndex);
    //            ids.Add(result);
    //        }
    //    }
    //    actionList.Clear();
    //    speedList.Clear();
    //    string setIndex = mainString;// 글자 개수 뽑을 때 사용
    //    setIndex = setIndex.Replace(lineEnd, " ");// 줄넘김 제거(\n 일경우 제거 안됨)
    //    string setText = mainString;// 실제 출력 시 사용
    //    setText = setText.Replace(lineEnd, "\n");// 줄넘김 변경

    //    for (int i = 0; i < ids.Count; i++)
    //    {
    //        string setting = inID + ids[i] + outID;
    //        Data_Manager.DialogStruct temp = Singleton_Data.INSTANCE.Dict_Dialog[ids[i]];
    //        string tempText = mainDialog.ID;
    //        if (temp.textStyle != Data_DialogType.TextStyle.None)
    //            actionBool = true;
    //        int startPoint = setIndex.IndexOf(setting);// 시작 위치
    //        int endPoint = startPoint + tempText.Length;
    //        Vector3Int actionVector = new Vector3Int(startPoint, endPoint, (int)(temp.textStyle - 1));
    //        actionList.Add(actionVector);

    //        float speed = temp.speed;
    //        speedList.Add(speed);

    //        setIndex = setIndex.Replace(setting, tempText);
    //        string richString = SetRichText(tempText, temp.size, temp.color, temp.bold);
    //        setText = setText.Replace(setting, richString);

    //    }
    //    Debug.LogWarning($"{setText}");
    //    int mainSize = mainDialog.size > 0 ? mainDialog.size : defaultSize;
    //    string mainColor = mainDialog.color.Length > 0 ? mainDialog.color : defaultColor;
    //    bool mainBold = mainDialog.bold;
    //    setText = SetRichText(setText, mainSize, mainColor, mainBold);
    //    return setText;
    //}

    //string SetRichText(string _text, int _size, string _color, bool _bold)
    //{
    //    string temp = _size > 0 ? $"<size={_size}>{_text}</size>" : _text;
    //    temp = _color.Length > 0 ? $"<color=#{_color}>{temp}</color>" : temp;
    //    temp = _bold == true ? $"<b>{temp}</b>" : temp;

    //    return temp;
    //}

    // 미리 세팅해놓고 숨기기
    //void PreSetHide()
    //{
    //    TMP_TextInfo textInfo = dialogText.textInfo;
    //    for (int c = 0; c < textInfo.characterCount; c++)
    //    {
    //        var charInfo = textInfo.characterInfo[c];
    //        if (!charInfo.isVisible)
    //            continue;

    //        int materialIndex = charInfo.materialReferenceIndex;
    //        Color32[] vertexColors = textInfo.meshInfo[materialIndex].colors32;
    //        int vertexIndex = charInfo.vertexIndex;
    //        for (int i = 0; i < 4; i++)
    //        {
    //            int index = vertexIndex + i;
    //            vertexColors[index].a = 0;// 투명화
    //        }
    //    }

    //    // 메쉬 업데이트
    //    for (int i = 0; i < textInfo.materialCount; i++)
    //    {
    //        if (textInfo.meshInfo[i].mesh == null) { continue; }
    //        textInfo.meshInfo[i].mesh.colors32 = textInfo.meshInfo[i].colors32;
    //        textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;   // 변경
    //        dialogText.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
    //    }
    //}

    void StartActing()
    {
        if (actionCoroutine != null)
            StopCoroutine(actionCoroutine);
        actionCoroutine = StartCoroutine(TextAction(dialog));
    }

    IEnumerator TextAction(Data_Manager.DialogStruct _dialogStruct)
    {
        actionBool = true;
        TMP_Text component = dialogText;
        TMP_MeshInfo[] cachedMeshInfo = component.textInfo.CopyMeshInfoVertexData();
        while (actionBool == true)
        {
            yield return new WaitForSeconds(interval);
            for (int i = 0; i < _dialogStruct.dialogTypes.Length; i++)
            {
                // x - 시작 포지션
                // y - 끝 포지션
                // z - 액션 타입
                if (_dialogStruct.dialogTypes[i].dialogAnimation != Data_DialogType.DialogAnimation.None)// 액션 타입이 None이 아니면
                {
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
                        //Color32[] vertexColors = component.textInfo.meshInfo[materialIndex].colors32;
                        //Data_DialogType.ActionType type = dialogType.actionType[_actionText[i].z];
                        //SetActionType(type, vertexIndex, sourceVertices, destinationVertices, c);
                        TryAimationWave(actionType, vertexIndex, sourceVertices, destinationVertices, c);
                    }
                }
            }
            component.UpdateVertexData();
            Debug.LogWarning("TextAction");
        }
    }

    void TryAnimationCurve(Data_DialogType type, int vertexIndex, Vector3[] sourceVertices, Vector3[] destinationVertices, int _index)
    {
        AnimationCurve curve = type.curve;
        float curveTime = (Time.time * type.speed) + (type.interval * _index);
        float curveValue = curve.Evaluate(curveTime);
        for (int v = 0; v < 4; v++)
        {
            int index = vertexIndex + v;
            float x = curveValue * type.angle.x;
            float y = curveValue * type.angle.y;
            destinationVertices[index] = sourceVertices[index] + new Vector3(x, y, 0f);
        }
    }

    void TryAimationWave(Data_DialogType type, int vertexIndex, Vector3[] sourceVertices, Vector3[] destinationVertices, int _index)
    {
        AnimationCurve curve = type.curve;
        for (int v = 0; v < 4; v++)
        {
            int index = vertexIndex + v;
            float curveTime = (Time.time * type.speed) + (type.interval * _index);
            float curveValue = curve.Evaluate(curveTime);

            float x = curveValue * type.angle.x;
            float y = curveValue * type.angle.y;
            //float animTime = Time.time * type.speed;
            //float actionRange = 5f * 0.01f;
            //float x = Mathf.Sin(curveTime + sourceVertices[index].x * actionRange) * type.angle.x;
            //float y = Mathf.Cos(curveTime + sourceVertices[index].y * actionRange) * type.angle.y;
            destinationVertices[index] = sourceVertices[index] + new Vector3(y, x, 0f);
        }
    }

    //void SetActionType(Data_DialogType.ActionType type, int vertexIndex, Vector3[] sourceVertices, Vector3[] destinationVertices, int _index)
    //{
    //    switch (type.type)
    //    {
    //        case Data_Manager.DialogInfoamtion.TextType.None:

    //            break;

    //        case Data_Manager.DialogInfoamtion.TextType.Move:
    //            Vector3 offset = Wobble(Time.time * type.speed + _index, type.angle, type.range);
    //            for (int v = 0; v < 4; v++)
    //            {
    //                int index = vertexIndex + v;
    //                destinationVertices[index] = sourceVertices[index] + offset;
    //            }
    //            break;

    //        case Data_Manager.DialogInfoamtion.TextType.MoveAll:
    //            offset = Wobble(Time.time * type.speed, type.angle, type.range);
    //            for (int v = 0; v < 4; v++)
    //            {
    //                int index = vertexIndex + v;
    //                destinationVertices[index] = sourceVertices[index] + offset;
    //            }
    //            break;

    //        case Data_Manager.DialogInfoamtion.TextType.Wave:
    //            for (int v = 0; v < 4; v++)
    //            {
    //                int index = vertexIndex + v;
    //                float actionRange = type.range * 0.01f;
    //                float animTime = Time.time * type.speed;
    //                float x = Mathf.Sin(animTime + sourceVertices[index].x * actionRange) * type.angle.x;
    //                float y = Mathf.Cos(animTime + sourceVertices[index].y * actionRange) * type.angle.y;
    //                destinationVertices[index] = sourceVertices[index] + new Vector3(y, x, 0f);
    //            }
    //            break;

    //        case Data_Manager.DialogInfoamtion.TextType.Squash:
    //            for (int v = 0; v < 4; v++)
    //            {
    //                int index = vertexIndex + v;
    //                float actionRange = type.range * 0.01f;
    //                float animTime = Time.time * type.speed + _index;
    //                float x = Mathf.Sin(animTime + sourceVertices[index].x * actionRange) * type.angle.x;
    //                float y = Mathf.Cos(animTime + sourceVertices[index].y * actionRange) * type.angle.y;
    //                destinationVertices[index] = sourceVertices[index] + new Vector3(x, y, 0f);
    //            }
    //            break;

    //        case Data_Manager.DialogInfoamtion.TextType.Jitter:
    //            for (int v = 0; v < 4; v++)
    //            {
    //                int index = vertexIndex + v;
    //                for (int j = 0; j < 2; j++)
    //                {
    //                    float randomIndex = Random.Range(-type.range, type.range);
    //                    destinationVertices[index][j] = sourceVertices[index][j] + randomIndex;
    //                }
    //            }
    //            break;

    //        case Data_Manager.DialogInfoamtion.TextType.Test:

    //            break;
    //    }
    //}

    //Vector2 Wobble(float _time, Vector2 _angle, float _length)
    //{
    //    return new Vector2(Mathf.Sin(_time * _angle.x) * _angle.x, Mathf.Cos(_time * _angle.y) * _angle.y) * _length;
    //}

    void StartTyping(bool _typing)
    {
        typing = _typing;
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(Typing(dialog));
    }

    IEnumerator Typing(Data_Manager.DialogStruct _dialogStruct)
    {
        int subIndex = 0;
        TMP_TextInfo textInfo = dialogText.textInfo;
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (_dialogStruct.dialogTypes.Length > 0)
            {
                float speed = _dialogStruct.dialogTypes[subIndex].Speed;
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
                //Singleton_Audio.INSTANCE.Audio_SetFX(FXSound);
                dialogText.UpdateVertexData();
                yield return new WaitForSeconds(typingSpeed);
            }
        }
        dialogText.UpdateVertexData();
        typing = false;
        typingSpeed = defaultTypingSpeed;// 기본 속도

        StartCoroutine(WaitingNext());
    }


    IEnumerator WaitingNext()
    {
        dialogIndex++;

        float normalize = 0f;
        while (normalize < 1f && typing == false)
        {
            normalize += Time.deltaTime / 3f;
            yield return null;
        }
    }










    public void OnPointerClick(PointerEventData eventData)
    {
        DialogAction();
    }
}
