using System.Collections;
using TMPro;
using UnityEngine;
using static Data_Dialog;

public class Dialog_Test : MonoBehaviour
{
    public bool typing;
    public Data_Event eventData;
    TextStruct textStruct;
    public TMP_Text setText;

    float defaultTypingSpeed = 0.1f;
    float typingSpeed = 0;
    Vector2Int[] dialogVector;

    public Event_SelectButton selectButton;

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))// 스킵
        {
            if (typing == true)
                typing = false;
            else
                SetDialog(eventData.eventStruct);
        }
    }

    void SetDialog(TextStruct _textStruct)
    {
        StopAllCoroutines();

        textStruct = _textStruct;
        typingSpeed = defaultTypingSpeed;
        StartCoroutine(SetDialog());
    }

    IEnumerator SetDialog()
    {
        setText.text = SetReplace(textStruct);
        setText.ForceMeshUpdate(true);// 메쉬 재 생성 (리셋)
        setText.alpha = 0f;// 모든 글자 숨김
        yield return null;

        StartCoroutine(Typing(textStruct));
        StartCoroutine(TextAction(textStruct));
    }

    string SetReplace(TextStruct _textStruct)
    {
        string replace = Singleton_Data.INSTANCE.GetLanguage(_textStruct.contents);// 번역
        replace = replace.Replace("/n", "\n");// 띄어쓰기
        dialogVector = new Vector2Int[_textStruct.dialogTypes.Length];
        for (int i = 0; i < _textStruct.dialogTypes.Length; i++)
        {
            string temp = "{" + i + "}";
            string setReplace = Singleton_Data.INSTANCE.GetLanguage(_textStruct.dialogTypes[i].replaceText);// 번역
            int startIndex = replace.IndexOf(temp);
            dialogVector[i] = new Vector2Int(startIndex, startIndex + setReplace.Length);
            replace = replace.Replace(temp, setReplace);
        }
        // 색, 사이즈
        int lastIndex = dialogVector.Length - 1;
        for (int i = lastIndex; i >= 0; i--)
        {
            string textColor = P01_Utility.ColorToHex(_textStruct.dialogTypes[i].textColor);
            float size = _textStruct.dialogTypes[i].textSize;
            replace = replace.Insert(dialogVector[i].y, "</size></color>");// 끼워 넣기
            replace = replace.Insert(dialogVector[i].x, $"<color=#{textColor}><size={size}>");
        }
        return replace;
    }

    IEnumerator Typing(TextStruct _textStruct, string _voice = null)
    {
        typing = true;
        int subIndex = 0;
        TMP_TextInfo textInfo = setText.textInfo;
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
                setText.UpdateVertexData();
                yield return new WaitForSeconds(typingSpeed);
            }
        }
        setText.UpdateVertexData();
        typingSpeed = defaultTypingSpeed;// 기본 속도로 변경
        yield return new WaitForSeconds(typingSpeed);
        typing = false;
    }

    IEnumerator TextAction(TextStruct _textStruct)
    {
        bool actionBool = true;
        TMP_MeshInfo[] cachedMeshInfo = setText.textInfo.CopyMeshInfoVertexData();
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
                    var charInfo = setText.textInfo.characterInfo[c];
                    if (charInfo.isVisible == false)
                        continue;

                    int materialIndex = charInfo.materialReferenceIndex;
                    int vertexIndex = charInfo.vertexIndex;

                    // 원래 정점정보
                    Vector3[] sourceVertices = cachedMeshInfo[materialIndex].vertices;
                    // 현재 정점 정보를 얻고 덮어쓰기
                    Vector3[] destinationVertices = setText.textInfo.meshInfo[materialIndex].vertices;
                    SetActingText(_textStruct.dialogTypes[i], vertexIndex, sourceVertices, destinationVertices, c);
                }
            }
            yield return null;

            setText.UpdateVertexData();
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
}
