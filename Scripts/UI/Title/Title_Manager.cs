using System.Collections;
using System.IO;
using UnityEngine;

public class Title_Manager : MonoBehaviour
{
    public StaticOpenCanvas.CanvasStruct[] canvasStructs;

    public Custom_Button continueButton, newStartButton, creditButton, settingButton, exitButton;
    public Option_Manager optionManager;
    public Credit_Rolling creditRolling;
    private Credit_Rolling instCreditRolling;
    public string titleTheme = "Main Theme";
    public string soundName = "pop-39222";

    public RectTransform selectMask;
    Coroutine enterCoroutine;
    Vector2 originalSize;
    public UI_Popup newGamePopup;

    void Start()
    {
        continueButton.gameObject.SetActive(FindFolder());
        continueButton.SetButton(ContinueButton, ActionEnter, ActionExit);
        newStartButton.SetButton(NewStartButton, ActionEnter, ActionExit);
        creditButton.SetButton(CreditButton, ActionEnter, ActionExit);
        settingButton.SetButton(SettingButton, ActionEnter, ActionExit);
        exitButton.SetButton(ExitButton, ActionEnter, ActionExit);

        originalSize = selectMask.sizeDelta;
        ActionExit(null);

        StartCoroutine(SetManager());
        OnTitle();
    }

    public void OnTitle()
    {
        StartCoroutine(StaticOpenCanvas.OpenCanvas(canvasStructs, true));
    }

    bool FindFolder()
    {
        string filePath = Application.dataPath + "/Save/";
        return Directory.Exists(filePath);
    }

    void ActionEnter(Custom_Button _button)
    {
        Singleton_Audio.INSTANCE.Audio_FX(soundName);
        selectMask.gameObject.SetActive(true);
        selectMask.position = _button.transform.position;

        if (enterCoroutine != null)
            StopCoroutine(enterCoroutine);
        enterCoroutine = StartCoroutine(ActingEnter());
    }

    IEnumerator ActingEnter()
    {
        float normalize = 0f;
        while (normalize < 1f)
        {
            normalize += Time.deltaTime * 5f;
            selectMask.sizeDelta = Vector2.Lerp(new Vector2(0, originalSize.y), originalSize, normalize);
            yield return null;
        }
    }

    void ActionExit(Custom_Button _button)
    {
        selectMask.gameObject.SetActive(false);
    }

    void ContinueButton()
    {
        //StopAllCoroutines();
        StartGame();
    }

    void StartGame()
    {
        isMove = false;
        Option_Manager.current.OpenCanvas(false);
        LoadingManager.current.GoMain();
    }

    private UI_Popup instNewGamePopup;
    public UI_Popup GetUIPopup
    {
        get
        {
            if (instNewGamePopup == null)
            {
                instNewGamePopup = Instantiate(newGamePopup, transform);
                instNewGamePopup.SetStart();
            }
            return instNewGamePopup;
        }
    }

    void NewStartButton()
    {
        if (FindFolder() == true)
        {
            GetUIPopup.buttonAction = NewGamePopup;
            GetUIPopup.OpenCanvas(true);
        }
        else
        {
            StartGame();
        }
    }

    void NewGamePopup(bool _action)
    {
        Debug.Log("NewGamePopup : " + _action);
        if (_action == true)
        {
            StartCoroutine(RemoveSaveFile());
        }
    }

    IEnumerator RemoveSaveFile()
    {
        string path = Application.dataPath + "/Save/";
        string[] allFiles = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
        foreach (string file in allFiles)
        {
            File.Delete(file);
        }
        Directory.Delete(path, true);
        // 닫힐때 옵션이 저장이 되는데 창 데이터가 있어서 기존 내용이 저장됨
        Option_Manager.current.LoadOption();// 옵션 데이터 리셋
        yield return null;

        StartGame();
    }

    void CreditButton()
    {
        if (instCreditRolling == null)
            instCreditRolling = Instantiate(creditRolling);
        instCreditRolling.OpenCanvas(true);
    }

    void SettingButton()
    {
        Option_Manager.current.OpenCanvas(true);
        Option_Manager.current.deleCloseOption = OnTitle;
        StartCoroutine(StaticOpenCanvas.OpenCanvas(canvasStructs, false));
    }

    void ExitButton()
    {
        LoadingManager.current.GoExit();
    }

    [Header(" [ Ship ]")]
    public Unit_Player player;
    public Transform startPoint, endPoint;
    public float speed = 0.1f;
    public Material reflectionMaterial;
    bool isMove = false;
    IEnumerator SetManager()
    {
        Camera_Manager.current.SetCameraManager();
        Camera_Manager.current.SetOrbitalTitle();

        if (Option_Manager.current == null)
        {
            Instantiate(optionManager);
            Option_Manager.current.SetStart();
        }

        if (LoadingManager.current != null)
            LoadingManager.current.deleComplate = LoadingComplate;// 로딩 완료
        yield return null;

        Option_Manager.current.SetThemeMusic(titleTheme); // 테마 음악 설정
        isMove = true;
        while (isMove == true)// Loop the movement
        {
            GetPlayer.gameObject.SetActive(true);
            float normalize = 0f;
            while (normalize < 1f)
            {
                normalize += Time.deltaTime * speed;
                Vector3 startPosition = new Vector3(startPoint.position.x, GetPlayer.transform.position.y, startPoint.position.z);
                Vector3 endPosition = new Vector3(endPoint.position.x, GetPlayer.transform.position.y, endPoint.position.z);
                GetPlayer.transform.position = Vector3.Lerp(startPosition, endPosition, normalize);
                GetPlayer.transform.rotation = Quaternion.Slerp(GetPlayer.transform.rotation, Quaternion.LookRotation(endPosition - startPosition), Time.deltaTime * speed * 10f);
                yield return null;

                string shipPosition = "_ShipPosition";
                reflectionMaterial.SetVector(shipPosition, GetPlayer.transform.position);
            }
            GetPlayer.gameObject.SetActive(false);
            yield return new WaitForSeconds(1f);

            GetPlayer.transform.position = startPoint.position;
        }
    }

    void LoadingComplate()
    {
        Debug.Log("LoadingComplate");
    }

    private Unit_Player instPlayer;
    public Unit_Player GetPlayer
    {
        get
        {
            if (instPlayer == null)
            {
                instPlayer = Instantiate(player, transform);
            }
            return instPlayer;
        }
    }
}
