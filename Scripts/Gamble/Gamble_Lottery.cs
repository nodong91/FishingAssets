using System.Collections.Generic;
using UnityEngine;

public class Gamble_Lottery : MonoBehaviour
{
    public StaticOpenCanvas.CanvasStruct[] canvasStruct;
    public Canvas canvas;
    public int currentIndex;
    //public Data_Lottery[] lotteries;
    //public GameObject[] lotteryObjects;
    public Gamble_Card[] lotteryCards;

    public LineRenderer lineRenderer;
    LineRenderer instLine;
    private float distanceToWorldPlane = 10f; // 이건 카메라랑 우리 캐릭터가 사는 평면 사이의 거리
    //public Transform slotParent;
    public Gamble_Lotto_Slot answerSlot;
    Gamble_Lotto_Slot setEnterSlot;

    private RenderTexture maskTexture;
    public Camera maskCamera;


    Vector3 point;

    //public Custom_Button closeButton;
    private List<Vector3> positionsList = new List<Vector3>();
    private List<LineRenderer> lineList = new List<LineRenderer>();
    private Queue<LineRenderer> lineQueue = new Queue<LineRenderer>();
    public List<Gamble_Lotto_Slot> slotList = new List<Gamble_Lotto_Slot>();
    Queue<Gamble_Lotto_Slot> slotQueue = new Queue<Gamble_Lotto_Slot>();

    //public TMPro.TMP_Text testText;
    public int sellPrice;
    public bool isAnswer, isEnd;
    public ParticleSystem answerEffect;

    public void SetStart()
    {
        canvas.worldCamera = Camera_Manager.current.UICamera;
        //closeButton.SetButton(delegate { OpenCanvas(false); });
        maskTexture = new RenderTexture(Screen.width, Screen.height, 24);
        maskTexture.useMipMap = true;

        maskCamera.enabled = false;
        maskCamera.targetTexture = maskTexture;
        answerSlot.iconImage.material = Instantiate(answerSlot.iconImage.material);
    }

    void Update()// 복권 긁기
    {
        if (Input.GetMouseButtonDown(0))
        {
            Singleton_Audio.INSTANCE.Audio_LoopFX(Const_Audio._lottery);
            positionsList.Clear();
            instLine = TryLine();
            instLine.gameObject.SetActive(true);
            lineList.Add(instLine);

            point = SetWorldPoint();
            SetPoint(point);
        }
        else if (Input.GetMouseButton(0))
        {
            point = SetWorldPoint();
            float distance = (point - positionsList[^1]).magnitude;
            if (distance > 0.001f)
            {
                SetPoint(point);// 일정거리가 멀어지면 포인트 기입
                if (setEnterSlot == answerSlot)// 정답슬롯 열렸는지 확인
                {
                    isAnswer = true;
                }
                else if (slotList.Contains(setEnterSlot) == false)// 열었는지 확인
                {
                    slotList.Add(setEnterSlot);
                }
                // 체크
                if (isAnswer == true && isEnd == false)// 정답이 공개 되어 있고 완료되지 않았으면
                {
                    if (CheckImage() == true)
                    {
                        isEnd = true;
                        //testText.text = $"당첨!!!!!! : {sellPrice}";
                        Game_Manager.current.GetMainUI.MoveMoney(sellPrice);
                        Steam_StatsManager.current.CountLottery(sellPrice);// 당첨 카운트
                    }
                    else if (slotList.Count >= lotteryCards[currentIndex].lottery.slotCount)
                    {
                        isEnd = true;
                        //testText.text = "노당첨!!!!!!";
                    }
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Singleton_Audio.INSTANCE.Stop_LoopFX();
        }
    }

    public void SetLottery(int _index)// 복권 세팅
    {
        currentIndex = _index;// 복권 종류
        Game_Manager.current.GetInventory.GetBackButton.SetButton(delegate { OpenCanvas(false); });
        OpenCanvas(true);
    }

    void OpenCanvas(bool _open)
    {
        if (_open == true)
            ResetButton();
        else
            Game_Manager.current.GetInventory.SetBackButton();// 닫으면 인벤토리 닫기 버튼으로 변경
        //StartCoroutine(OpenCanvas());
        StartCoroutine(StaticOpenCanvas.OpenCanvas(canvasStruct, _open));
    }

    bool CheckImage()
    {
        for (int i = 0; i < slotList.Count; i++)
        {
            if (answerSlot.iconImage.sprite == slotList[i].iconImage.sprite)
            {
                answerSlot.iconImage.material.SetFloat("_FillAmount", 1f);
                slotList[i].iconImage.material.SetFloat("_FillAmount", 1f);
                answerEffect.transform.position = slotList[i].iconImage.transform.position;
                answerEffect.Play();
                return true;
            }
        }
        return false;
    }

    private void FixedUpdate()
    {
        maskCamera.Render();
    }

    private void ResetButton()
    {
        isEnd = false;
        isAnswer = false;
        //testText.text = "";
        SetLotto();
    }

    public void SetLotto()
    {
        // 기존 슬롯, 라인 초기화
        for (int i = 0; i < slotList.Count; i++)
        {
            slotList[i].iconImage.color = Color.white;
            slotQueue.Enqueue(slotList[i]);
            slotList[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < lineList.Count; i++)
        {
            lineList[i].positionCount = 0;
            lineQueue.Enqueue(lineList[i]);
            lineList[i].gameObject.SetActive(false);
        }

        slotList.Clear();
        lineList.Clear();
        positionsList.Clear();

        SetRandom();
    }

    void SetRandom()
    {
        for (int i = 0; i < lotteryCards.Length; i++)// 뒷배경
        {
            lotteryCards[i].gameObject.SetActive(i == currentIndex);
        }
        lotteryCards[currentIndex].maskImage.texture = maskTexture;// 마스크 랜더투텍스처 세팅

        Data_Lottery dataLottery = lotteryCards[currentIndex].lottery;
        List<Data_Lottery.LottoSlot> lottoSlots = dataLottery.SetRandom(out Sprite _mainSprite, out int _sellPrice);// 미리 당첨 슬롯, 가격 세팅
        sellPrice = _sellPrice;// 당첨 가격
        answerSlot.SetSlot(_mainSprite, 0);
        answerSlot.deleEnterSlot = EnterSlot;
        answerSlot.iconImage.material.SetFloat("_FillAmount", 0f);

        Transform parent = lotteryCards[currentIndex].answerSlotPosition.transform;
        answerSlot.transform.SetParent(parent);
        answerSlot.transform.position = parent.position;

        parent = lotteryCards[currentIndex].gridParent.transform;
        for (int i = 0; i < lottoSlots.Count; i++)
        {
            Gamble_Lotto_Slot inst = TrySlot();
            inst.gameObject.SetActive(true);
            inst.transform.SetParent(parent);
            inst.transform.localScale = Vector3.one;

            inst.SetSlot(lottoSlots[i].sprite, lottoSlots[i].reward);
            inst.deleEnterSlot = EnterSlot;
            inst.iconImage.material.SetFloat("_FillAmount", 0f);
            inst.transform.SetAsLastSibling();// 순서 변경
        }
    }

    void EnterSlot(Gamble_Lotto_Slot _slot)
    {
        Debug.LogWarning(_slot);
        if (_slot != null)
            setEnterSlot = _slot;
    }

    Gamble_Lotto_Slot TrySlot()
    {
        if (slotQueue.Count > 0)
        {
            Gamble_Lotto_Slot slot = slotQueue.Dequeue();
            return slot;
        }
        Gamble_Lotto_Slot inst = Instantiate(answerSlot, canvas.transform);
        inst.iconImage.material = Instantiate(inst.iconImage.material);
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