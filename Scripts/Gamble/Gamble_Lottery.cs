using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gamble_Lottery : MonoBehaviour
{
    public StaticOpenCanvas.CanvasStruct[] canvasStruct;
    public Canvas canvas;
    //public Data_Lottery dataLottery;
    public int testIndex;
    public Data_Lottery[] lotteries;
    public GameObject[] lotteryObjects;

    public LineRenderer lineRenderer;
    LineRenderer instLine;
    //public Transform target;
    //[SerializeField] Transform observer; // 이건 우리 단순화된 플레이어 캐릭터
    private float distanceToWorldPlane = 10f; // 이건 카메라랑 우리 캐릭터가 사는 평면 사이의 거리
    //[SerializeField] Camera playerCamera; // 이건 우리 카메라
    public Transform slotParent;
    public Gamble_Lotto_Slot answerSlot;
    Gamble_Lotto_Slot setEnterSlot;
    //Vector3 mouseWorldLocation; // 이건 플레이어 캐릭터가 바라봐야 할 곳

    private RenderTexture maskTexture;
    public Camera maskCamera;

    public RawImage maskImage;

    Vector3 point;

    public Custom_Button closeButton;
    private List<Vector3> positionsList = new List<Vector3>();
    private List<LineRenderer> lineList = new List<LineRenderer>();
    private Queue<LineRenderer> lineQueue = new Queue<LineRenderer>();
    public List<Gamble_Lotto_Slot> slotList = new List<Gamble_Lotto_Slot>();
    Queue<Gamble_Lotto_Slot> slotQueue = new Queue<Gamble_Lotto_Slot>();

    public TMPro.TMP_Text testText;
    public int sellPrice;
    public bool isAnswer, isEnd;
    public ParticleSystem answerEffect;

    void Start()
    {
        canvas.worldCamera = Camera_Manager.current.UICamera;
        closeButton.SetButton(CloseButton);
        maskTexture = new RenderTexture(Screen.width, Screen.height, 24);
        maskTexture.useMipMap = true;
        maskImage.texture = maskTexture;

        maskCamera.enabled = false;
        maskCamera.targetTexture = maskTexture;
        answerSlot.iconImage.material = Instantiate(answerSlot.iconImage.material);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
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
                    //slotQueue.Enqueue(setEnterSlot);
                    //slotList.Remove(setEnterSlot);
                    slotList.Add(setEnterSlot);
                }
                // 체크
                if (isAnswer == true && isEnd == false)// 정답이 공개 되어 있고 완료되지 않았으면
                {
                    if (CheckImage() == true)
                    {
                        isEnd = true;
                        testText.text = $"당첨!!!!!! : {sellPrice}";
                        Game_Manager.current.GetMainUI.MoveMoney(sellPrice);
                        Steam_StatsManager.current.CountLottery(sellPrice);// 당첨 카운트
                    }
                    else if (slotList.Count >= lotteries[testIndex].slotCount)
                    {
                        isEnd = true;
                        testText.text = "노당첨!!!!!!";
                    }
                }
            }
        }
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
        //foreach (var child in slotQueue)
        //{
        //    if (answerSlot.iconImage.sprite == child.iconImage.sprite)
        //    {
        //        answerSlot.iconImage.material.SetFloat("_FillAmount", 1f);
        //        child.iconImage.material.SetFloat("_FillAmount", 1f);
        //        answerEffect.transform.position = child.iconImage.transform.position;
        //        answerEffect.Play();
        //        return true;
        //    }
        //}
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
        testText.text = "";
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
        for (int i = 0; i < lotteryObjects.Length; i++)// 뒷배경
        {
            lotteryObjects[i].SetActive(i == testIndex);
        }
        Data_Lottery dataLottery = lotteries[testIndex];
        List<Data_Lottery.LottoSlot> lottoSlots = dataLottery.SetRandom(out Sprite _mainSprite, out int _sellPrice);// 미리 당첨 슬롯, 가격 세팅
        sellPrice = _sellPrice;// 당첨 가격
        answerSlot.SetSlot(_mainSprite, 0);
        answerSlot.deleEnterSlot = EnterSlot;
        answerSlot.iconImage.material.SetFloat("_FillAmount", 0f);

        for (int i = 0; i < lottoSlots.Count; i++)
        {
            Gamble_Lotto_Slot inst = TrySlot();
            inst.gameObject.SetActive(true);
            inst.SetSlot(lottoSlots[i].sprite, lottoSlots[i].reward);
            inst.deleEnterSlot = EnterSlot;
            inst.iconImage.material.SetFloat("_FillAmount", 0f);
            //slotList.Add(inst);
        }
        //Debug.LogWarning(slotList.Count);
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
            return slotQueue.Dequeue();
        Gamble_Lotto_Slot inst = Instantiate(answerSlot, slotParent);
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












    //public bool isOpen;

    //public GameObject target;
    //public CanvasGroup canvasGroup;
    public void OpenCanas()
    {
        Debug.LogWarning("ijijoijijijijj");
        ResetButton();
        //StartCoroutine(OpenCanvas());
        StartCoroutine(StaticOpenCanvas.OpenCanvas(canvasStruct, true));
    }
    void CloseButton()
    {
        StartCoroutine(StaticOpenCanvas.OpenCanvas(canvasStruct, false));
        //if (openCanvas != null)
        //    StopCoroutine(openCanvas);
        //openCanvas = StartCoroutine(CloseCanvas());
    }

    //IEnumerator OpenCanvas()
    //{
    //    canvasGroup.gameObject.SetActive(true);
    //    canvasGroup.alpha = 1f;
    //    Vector3 prevPoint = Input.mousePosition;
    //    float normalize = 0f;
    //    while (normalize < 1f)
    //    {
    //        normalize += Time.deltaTime * 10f;
    //        Vector3 actionPoint = Vector3.Lerp(prevPoint, target.transform.position, normalize);
    //        canvasGroup.transform.position = actionPoint;
    //        float rotate = Mathf.Lerp(45f, 0f, normalize);
    //        canvasGroup.transform.rotation = Quaternion.Euler(0f, 0f, rotate);
    //        float size = Mathf.Lerp(0f, 1f, normalize);
    //        canvasGroup.transform.localScale = Vector3.one * size;
    //        yield return null;
    //    }
    //}
    //Coroutine openCanvas;

    //IEnumerator CloseCanvas()
    //{
    //    Vector3 prevPoint = canvasGroup.transform.position;
    //    float normalize = 0f;
    //    while (normalize < 1f)
    //    {
    //        normalize += Time.deltaTime * 10f;
    //        Vector3 actionPoint = Vector3.Lerp(prevPoint, target.transform.position + Vector3.up * 500f, normalize);
    //        canvasGroup.transform.position = actionPoint;
    //        float alpha = Mathf.Lerp(0f, 1, normalize);
    //        canvasGroup.alpha = 1f - alpha;
    //        yield return null;
    //    }
    //    canvasGroup.gameObject.SetActive(false);
    //}
}