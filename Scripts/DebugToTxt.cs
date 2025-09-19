using System.Collections;
using System.IO;
using UnityEngine;
public class DebugToTxt : MonoBehaviour
{
    [SerializeField]
    private bool isLog = true;

    [SerializeField]
    private bool isWarning = true;

    [SerializeField]
    private bool isError = true;

    private string fileName;
    private string filePath;
    public TMPro.TMP_Text m_Text;

    bool open = false;
    public Custom_Button onButton;
    public GameObject canvas;
    public string saveLog;
    string viewLog;

    public void Awake()
    {
        DontDestroyOnLoad(this);

        // 오늘 날짜로 파일의 이름을 결정
        fileName = System.DateTime.Now.ToString("yyyy-MM-dd");
        //Application.logMessageReceived += LogToTxt;
    }

    private void Start()
    {
        onButton.SetButton(OpenCanvas);
        // 생성될 파일의 경로설정
#if UNITY_EDITOR
        filePath = Application.dataPath + "/Save/";

#elif UNITY_ANDROID
        filePath = Application.persistentDataPath;

#endif  
    }

    private void OpenCanvas()
    {
        open = !open;
        canvas.SetActive(open);
    }


    private void OnEnable()
    {
        Application.logMessageReceived += LogToTxt;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= LogToTxt;
    }

    public void LogToTxt(string _logString, string _stackTrace, LogType _type)
    {
        string tempString = _logString;
        if (_type == LogType.Log)
        {
            if (!isLog)
                return;
        }
        else if (_type == LogType.Warning)
        {
            if (!isWarning)
                return;
            tempString = $"<color=#FFFF00>{_logString}</color>";
        }
        else if (_type == LogType.Error)
        {
            if (!isError)
                return;
            tempString = $"<color=#FF0000>{_logString}</color>";
        }
        saveLog += $"> {_logString} \n";
        viewLog += $"> {tempString} \n";
        m_Text.text = viewLog;
    }

    private void OnApplicationQuit()
    {
        // 로그 메세지를 텍스트 파일에 저장
        // 파일이 없다면 새롭게 생성
        FindFolder();
        using (StreamWriter sw = new StreamWriter(Path.Combine(filePath, fileName), true))
        {
            sw.WriteLine($"[{System.DateTime.Now}] {saveLog} \n");
        }
    }

    void FindFolder()
    {
        DirectoryInfo dirInfo = new DirectoryInfo(filePath);
        if (dirInfo.Exists == false)
        {
            // 없으면 만들기
            dirInfo.Create();
        }
    }
}