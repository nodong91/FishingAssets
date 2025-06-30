using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static FishingGame;

public class FishingGame1 : MonoBehaviour
{
    public enum GameType
    {
        Type1,
        Type2,
        Type3,
        Type4,
        Type5,
    }
    public GameType gameType = GameType.Type1;
    public Toggle[] toggles;
    public GameObject[] gameObjects;
    public Slider timeSlider;

    int keyCode;
    public TMPro.TMP_Text nextText;
    public Image arrowImage, targetImage;
    Coroutine testest, subAction;
    public float type1Value;
    public float testSpeed;
    public float runningTime = 0f;

    [System.Serializable]
    public class Type2Struct
    {
        public Image m_Main;
        public TMPro.TMP_Text m_Text;
        public Image m_Image;
        public float m_time;
    }
    public Type2Struct type2Struct;
    public List<Type2Struct> type2StructList = new List<Type2Struct>();
    Queue<Type2Struct> type2Queue = new Queue<Type2Struct>();

    public Image test3Image;
    float type3FillAmount;
    public TMPro.TMP_Text type3Text;

    void Start()
    {
        for (int i = 0; i < toggles.Length; i++)
        {
            int index = i;
            toggles[i].onValueChanged.AddListener(delegate { SetGame(index); });
            gameObjects[i].SetActive(false);
        }
        toggles[0].isOn = true;
        timeSlider.onValueChanged.AddListener(TimeSpeed);
        SetMouse();
    }

    void TimeSpeed(float _value)
    {
        testSpeed = Mathf.Lerp(1f, 10f, _value);
    }

    void SetMouse()
    {
        Singleton_Controller.INSTANCE.key_W += Key_W;
        Singleton_Controller.INSTANCE.key_A += Key_A;
        Singleton_Controller.INSTANCE.key_S += Key_S;
        Singleton_Controller.INSTANCE.key_D += Key_D;
    }

    void SetGame(int _index)
    {
        if (toggles[_index].isOn == true)
        {
            gameType = (GameType)_index;
            StartGame();
        }
        gameObjects[_index].SetActive(toggles[_index].isOn);
        Debug.LogWarning("SetGame : " + gameType);
    }
    public Vector2 safetyScreen;
    void StartGame()
    {
        if (testest != null)
            StopCoroutine(testest);

        switch (gameType)
        {
            case GameType.Type1:
                Type1_Game();
                break;

            case GameType.Type2:
                float x = gameObjects[1].GetComponent<RectTransform>().rect.width;
                float y = gameObjects[1].GetComponent<RectTransform>().rect.height;
                safetyScreen = new Vector2(x, y) * 0.5f;
                Type2_Game();
                break;

            case GameType.Type3:
                Type3_Game();
                break;

            case GameType.Type4:
                Type4_Game();
                break;

            case GameType.Type5:
                Type5_Game();
                break;
        }
    }

    void Key_W(bool _input)
    {
        // 실패시 쉐이크
        if (_input == true)
            ActionKey(0);
    }

    void Key_A(bool _input)
    {
        if (_input == true)
            ActionKey(1);
    }

    void Key_S(bool _input)
    {
        if (_input == true)
            ActionKey(2);
    }

    void Key_D(bool _input)
    {
        if (_input == true)
            ActionKey(3);
    }

    void ActionKey(int _index)
    {
        Debug.LogWarning(_index == keyCode);
        switch (gameType)
        {
            case GameType.Type1:
                Type1_Action(_index == keyCode);
                break;

            case GameType.Type2:
                Type2_Action(_index == keyCode);
                break;

            case GameType.Type3:
                Type3_Action(_index == keyCode);
                break;

            case GameType.Type4:
                Type4_Action(_index == keyCode);
                break;

            case GameType.Type5:
                Type5_Action(_index == keyCode);
                break;
        }
    }

    //==================================================================================================================================
    // 1
    //==================================================================================================================================

    void Type1_Action(bool _active)
    {
        if (_active == false)
            return;

        float areaX = targetImage.rectTransform.anchoredPosition.x;
        float areaSize = targetImage.rectTransform.sizeDelta.x * 0.5f;
        float arrowX = arrowImage.rectTransform.anchoredPosition.x;
        if (areaX - areaSize < arrowX && areaX + areaSize > arrowX)
        {
            // 성공
            SetType1();
        }
        else
        {
            // 실패
            //return;
        }
    }

    void SetType1()
    {
        keyCode = Random.Range(0, 4);
        string key = TryKeyCode(keyCode);
        nextText.text = key;

        float randomPosition = Random.Range(-type1Value, type1Value);
        targetImage.rectTransform.anchoredPosition = new Vector3(randomPosition, 0f);

        float randomScale = Random.Range(10f, 30f);
        targetImage.rectTransform.sizeDelta = new Vector2(randomScale, targetImage.rectTransform.sizeDelta.y);
    }
    void Type1_Game()
    {
        SetType1();
        testest = StartCoroutine(Type1_ing());
    }

    IEnumerator Type1_ing()
    {
        runningTime = 0f;
        while (true)
        {
            runningTime += testSpeed * Time.deltaTime;
            float mathfSin = (Mathf.Sin(runningTime) + 1f) / 2f;
            float value = Mathf.Lerp(-type1Value, type1Value, mathfSin);
            arrowImage.rectTransform.anchoredPosition = new Vector2(value, 0f);
            //Debug.LogWarning(mathfSin);
            yield return null;
        }
    }












    //==================================================================================================================================
    // 2
    //==================================================================================================================================

    void Type2_Action(bool _active)
    {
        if (_active == false)
            return;

        float timing = type2Struct.m_time + runningTime % 1f;
        Debug.LogWarning(timing);
        if (timing == 0.45f)
        {

        }
    }

    void SetType2()
    {

    }
    void Type2_Game()
    {
        testest = StartCoroutine(Type2_Instancing());
    }

    IEnumerator Type2_Instancing()
    {
        runningTime = 0f;
        for (int i = 0; i < 6; i++)
        {
            keyCode = Random.Range(0, 4);
            string key = TryKeyCode(keyCode);
            float x = Random.Range(-safetyScreen.x, safetyScreen.x);
            float y = Random.Range(-safetyScreen.y, safetyScreen.y);
            Vector2 randomPoint = new Vector2(x, y);

            Type2Struct inst = GetInstanceRing();
            inst.m_Main.rectTransform.anchoredPosition = randomPoint;
            inst.m_Main.gameObject.SetActive(true);
            inst.m_Text.text = key;
            type2StructList.Add(inst);

            if (subAction != null)
                StopCoroutine(subAction);
            subAction = StartCoroutine(Type2_ing());

            yield return new WaitForSeconds(0.7f);
        }
    }

    IEnumerator Type2_ing()
    {
        while (type2StructList.Count > 0)
        {
            runningTime += testSpeed * Time.deltaTime * 0.5f;
            for (int i = 0; i < type2StructList.Count; i++)
            {
                float typeTime = runningTime - type2StructList[i].m_time;
                type2StructList[i].m_Image.material.SetFloat("_Amount", typeTime);
                if (typeTime > 1f)
                {
                    type2StructList[i].m_Main.gameObject.SetActive(false);
                    type2Queue.Enqueue(type2StructList[i]);
                    type2StructList.Remove(type2StructList[i]);

                    //if (type2 != null)
                    //    StopCoroutine(type2);
                    //type2 = StartCoroutine(Type2_ing());
                }
            }
            yield return null;
        }
    }

    Type2Struct GetInstanceRing()
    {
        if (type2Queue.Count > 0)
        {
            Type2Struct que = type2Queue.Dequeue();
            que.m_time = runningTime;
            return que;
        }
        //mainImage
        Image instMain = Instantiate(type2Struct.m_Main, gameObjects[1].transform);
        TMPro.TMP_Text instText = instMain.transform.GetChild(1).GetComponent<TMPro.TMP_Text>();
        Image instImage = instMain.transform.GetChild(0).GetComponent<Image>();
        instImage.material = Instantiate(instImage.material);
        Type2Struct inst = new Type2Struct
        {
            m_Main = instMain,
            m_Text = instText,
            m_Image = instImage,
            m_time = runningTime,
        };
        return inst;
    }

















    //==================================================================================================================================
    // 3
    //==================================================================================================================================

    void Type3_Action(bool _active)
    {
        if (_active == true)
        {
            if (type3FillAmount < 1f)
                type3FillAmount += 0.05f;
        }
        else
        {
            if (type3FillAmount > 0f)
                type3FillAmount -= 0.05f;
        }
    }

    void SetType3()
    {
        keyCode = Random.Range(0, 4);
        string key = TryKeyCode(keyCode);
        type3Text.text = key;
    }

    void Type3_Game()
    {
        SetType3();
        testest = StartCoroutine(Type3_ing());
    }

    IEnumerator Type3_ing()
    {
        type3FillAmount = 0f;
        runningTime = 0f;
        float randomTime = 0f;
        bool setStart = true;
        while (setStart == true)
        {
            runningTime += testSpeed * Time.deltaTime;
            if (type3FillAmount > 1f)
            {
                setStart = false;
            }
            else if (type3FillAmount > 0f)
            {
                type3FillAmount -= Time.deltaTime * 0.1f;
            }
            test3Image.material.SetFloat("_FillAmount", type3FillAmount);
            if (runningTime > randomTime)
            {
                randomTime += Random.Range(1f, 3f);
                SetType3();
            }
            yield return null;
        }
    }

    string TryKeyCode(int _index)
    {
        string key = "";
        switch (_index)
        {
            case 0:
                key = "W";
                break;

            case 1:
                key = "A";
                break;

            case 2:
                key = "S";
                break;

            case 3:
                key = "D";
                break;
        }
        return key;
    }
    //==================================================================================================================================
    // 4
    //==================================================================================================================================
    public Image type4Image;
    public TMPro.TMP_Text type4Text;
    int KeyCodeToIndex(string _string)
    {
        switch (_string)
        {
            case "W":
                return 0;
            case "A":
                return 1;
            case "S":
                return 2;
            case "D":
                return 3;
        }
        return default;
    }

    void Type4_Action(bool _active)
    {
        if (_active)
        {
            string originString = type4Text.text;
            if (originString.Length > 0)
            {
                int randomIndex = Random.Range(0, 4);
                string newString = originString.Substring(1) + TryKeyCode(randomIndex);
                type4Text.text = newString;
                keyCode = KeyCodeToIndex(newString[0].ToString());

                if (runningTime < 1f)
                    runningTime += 0.2f;
            }
        }
        else
        {
            runningTime -= 0.2f;
        }
    }

    void SetType4()
    {
        type4Image.material = Instantiate(type4Image.material);
        string actionText = "";
        for (int i = 0; i < 5; i++)
        {
            int randomIndex = Random.Range(0, 4);
            actionText += TryKeyCode(randomIndex);
        }
        type4Text.text = actionText;
        keyCode = KeyCodeToIndex(actionText[0].ToString());
    }

    void Type4_Game()
    {
        SetType4();
        testest = StartCoroutine(Type4_ing());
    }

    IEnumerator Type4_ing()
    {
        runningTime = 0f;
        bool setStart = true;
        while (setStart == true)
        {
            if (runningTime > 0f)
            {
                runningTime -= Time.deltaTime * 0.1f;
            }
            type4Image.material.SetFloat("_FillAmount", runningTime);
            yield return null;
        }
    }

    //==================================================================================================================================
    // 4
    //==================================================================================================================================
    public float runningSpeed = 1f;
    public RectTransform type5Base;
    public Image type5Image;
    public float type5Angle;
    void Type5_Action(bool _active)
    {
        dddd = true;
    }

    void Type5_Game()
    {
        dddd = false;
        testest = StartCoroutine(Type5_ing());
    }
    bool dddd;
    IEnumerator Type5_ing()
    {
        runningTime = 0f;
        float rotatespeed = runningSpeed;
        //float randomStop = runningSpeed * Random.Range(0.1f, 1f);
        bool setStart = true;
        while (setStart == true)
        {
            runningTime += Time.deltaTime * rotatespeed;
            type5Base.rotation = Quaternion.Euler(0, 0, runningTime);
            type5Angle = runningTime % 360f;
            yield return null;

            if (dddd == true)
            {
                if (rotatespeed > 0)
                {
                    //rotatespeed -= Time.deltaTime * randomStop;
                    rotatespeed -= Time.deltaTime * runningSpeed*10f;
                }
                else
                    setStart = false;
            }
        }
    }
}