using UnityEngine;

public class Option_Language : MonoBehaviour
{
    const string _control = "etc_1006";
    const string _graphic = "etc_1007";
    const string _audio = "etc_1008";
    const string _setting = "etc_1009";
    const string _shake = "etc_1010";
    const string _language = "etc_1011";

    const string _fullScreen = "etc_1012";
    const string _qualityLevel = "etc_1013";
    const string _resolution = "etc_1014";
    const string _frame = "etc_1015";

    const string _music = "etc_1016";
    const string _master = "etc_1017";
    const string _bgm = "etc_1018";
    const string _fx = "etc_1019";
    const string _enviroment = "etc_1020";

    const string _title = "etc_1021";
    const string _saveNexit = "etc_1022";

    [Header(" [ 토글 ]")]
    public TMPro.TMP_Text controlText;
    public TMPro.TMP_Text graphicText;
    public TMPro.TMP_Text audioText;
    public TMPro.TMP_Text settingText;

    [Header(" [ 컨트롤 ]")]
    public TMPro.TMP_Text shakeText;
    public TMPro.TMP_Text languageText;

    [Header(" [ 그래픽 ]")]
    public TMPro.TMP_Text fullScreenText;
    public TMPro.TMP_Text qualityLevelText;
    public TMPro.TMP_Text resolutionText;
    public TMPro.TMP_Text frameText;

    [Header(" [ 오디오 ]")]
    public TMPro.TMP_Text musicText;
    public TMPro.TMP_Text masterText;
    public TMPro.TMP_Text bgmText;
    public TMPro.TMP_Text fxText;
    public TMPro.TMP_Text enviromentText;

    [Header(" [ 세팅 ]")]
    public TMPro.TMP_Text titleText;
    public TMPro.TMP_Text saveNexitText;

    public void SetStart()
    {
        controlText.text = Singleton_Data.INSTANCE.GetLanguage(_control);
        graphicText.text = Singleton_Data.INSTANCE.GetLanguage(_graphic);
        audioText.text = Singleton_Data.INSTANCE.GetLanguage(_audio);
        settingText.text = Singleton_Data.INSTANCE.GetLanguage(_setting);

        shakeText.text = Singleton_Data.INSTANCE.GetLanguage(_shake);
        languageText.text = Singleton_Data.INSTANCE.GetLanguage(_language);

        fullScreenText.text = Singleton_Data.INSTANCE.GetLanguage(_fullScreen);
        qualityLevelText.text = Singleton_Data.INSTANCE.GetLanguage(_qualityLevel);
        resolutionText.text = Singleton_Data.INSTANCE.GetLanguage(_resolution);
        frameText.text = Singleton_Data.INSTANCE.GetLanguage(_frame);

        musicText.text = Singleton_Data.INSTANCE.GetLanguage(_music);
        masterText.text = Singleton_Data.INSTANCE.GetLanguage(_master);
        bgmText.text = Singleton_Data.INSTANCE.GetLanguage(_bgm);
        fxText.text = Singleton_Data.INSTANCE.GetLanguage(_fx);
        enviromentText.text = Singleton_Data.INSTANCE.GetLanguage(_enviroment);

        titleText.text = Singleton_Data.INSTANCE.GetLanguage(_title);
        saveNexitText.text = Singleton_Data.INSTANCE.GetLanguage(_saveNexit);
    }
}
