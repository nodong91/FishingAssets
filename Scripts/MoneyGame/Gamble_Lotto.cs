using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Gamble_Lotto : MonoBehaviour
{
    public LineRenderer lineRenderer;
    LineRenderer instLine;
    //public Transform target;
    //[SerializeField] Transform observer; // 이건 우리 단순화된 플레이어 캐릭터
    private float distanceToWorldPlane = 10f; // 이건 카메라랑 우리 캐릭터가 사는 평면 사이의 거리
    //[SerializeField] Camera playerCamera; // 이건 우리 카메라
    public Transform slotParent;
    public Gamble_Lotto_Slot slot;
    Gamble_Lotto_Slot setEnterSlot;
    //Vector3 mouseWorldLocation; // 이건 플레이어 캐릭터가 바라봐야 할 곳

    private RenderTexture maskTexture;
    public Camera maskCamera;

    public RawImage maskImage;
    public CanvasGroup maskCanvasGroup;

    Vector3 point;

    //public Gamble_Lotto_Canvas gambleLottoCanvas;

    void Start()
    {
        resetButton.SetButton(ResetButton);
        maskTexture = new RenderTexture(Screen.width, Screen.height, 24);
        maskTexture.useMipMap = true;

        maskImage.texture = maskTexture;
        maskCamera.targetTexture = maskTexture;
        maskCamera.Render();
        SetLotto();
    }

    void Update()
    {
        //if (OnCard == true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                positionsList.Clear();
                instLine = TryLine();
                lineList.Add(instLine);

                point = SetWorldPoint();
                SetPoint(point);
            }
            else if (Input.GetMouseButton(0))
            {
                point = SetWorldPoint();
                float distance = (point - positionsList[^1]).magnitude;
                if (distance > 0.2f)// 일정거리가 멀어지면 포인트 기입
                {
                    if (enterSlot.Contains(setEnterSlot) == false)// 열었는지 확인
                    {
                        enterSlot.Add(setEnterSlot);
                        if (setEnterSlot.iconImage.sprite == slot.iconImage.sprite)
                        {
                            winner = true;
                            testText.text = "당첨!!!!!!";
                        }
                        else if (winner == false && enterSlot.Count == slotList.Count)
                        {
                            testText.text = "노당첨!!!!!!";
                        }
                    }
                    SetPoint(point);
                }
            }
        }
    }

    private void ResetButton()
    {
        winner = false;
        testText.text = "";
        SetLotto();
    }

    bool winner;
    public Custom_Button resetButton;
    public List<Vector3> positionsList = new List<Vector3>();
    public List<LineRenderer> lineList = new List<LineRenderer>();
    Queue<LineRenderer> lineQueue = new Queue<LineRenderer>();
    public List<Gamble_Lotto_Slot> enterSlot = new List<Gamble_Lotto_Slot>();
    List<Gamble_Lotto_Slot> slotList = new List<Gamble_Lotto_Slot>();
    Queue<Gamble_Lotto_Slot> slotQueue = new Queue<Gamble_Lotto_Slot>();

    public Data_Lotto data;
    public TMPro.TMP_Text testText;
    public int sellPrice;
    public TMPro.TMP_Text sellPriceText;

    public void SetLotto()
    {
        for (int i = 0; i < slotList.Count; i++)
        {
            slotQueue.Enqueue(slotList[i]);
        }
        for (int i = 0; i < lineList.Count; i++)
        {
            lineList[i].positionCount = 0;
            lineQueue.Enqueue(lineList[i]);
        }

        slotList.Clear();
        lineList.Clear();
        positionsList.Clear();
        enterSlot.Clear();

        SetRandom();
    }

    void SetRandom()
    {
        List<Data_Lotto.LottoSlot> lottoSlots = data.SetRandom(out Sprite _mainSprite, out int _sellPrice);
        sellPrice = _sellPrice;
        slot.SetSlot(_mainSprite, 0);
        for (int i = 0; i < lottoSlots.Count; i++)
        {
            Gamble_Lotto_Slot inst = TrySlot();
            inst.SetSlot(lottoSlots[i].sprite, lottoSlots[i].reward);
            inst.deleEnterSlot = EnterSlot;
            slotList.Add(inst);
        }
        sellPriceText.text = sellPrice.ToString();
    }

    void EnterSlot(Gamble_Lotto_Slot _slot)
    {
        if (slot != _slot)
            setEnterSlot = _slot;
    }

    Gamble_Lotto_Slot TrySlot()
    {
        if (slotQueue.Count > 0)
            return slotQueue.Dequeue();
        Gamble_Lotto_Slot inst = Instantiate(slot, slotParent);
        return inst;
    }

    LineRenderer TryLine()
    {
        if (lineQueue.Count > 0)
        {
            return lineQueue.Dequeue();
        }
        LineRenderer inst = Instantiate(lineRenderer, transform);
        return inst;
    }

    Vector3 SetWorldPoint()
    {
        Vector3 mousePoint = Input.mousePosition; // 이건 커서의 x, y 픽셀 위치를 가져오는데, z는 항상 0.0f임. 화면이 2D라서 깊이가 없거든
        mousePoint.z = distanceToWorldPlane; // 그래서 z를 우리 캐릭터가 사는 평면이랑 카메라 사이의 거리로 바꿔줌
        return maskCamera.ScreenToWorldPoint(mousePoint);

    }

    void SetPoint(Vector3 _point)
    {
        positionsList.Add(_point);
        Vector3[] positions = positionsList.ToArray();
        instLine.positionCount = positions.Length;
        instLine.SetPositions(positions);
    }
}