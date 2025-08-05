using UnityEngine;
using UnityEngine.UI;

public class Title_Manager : MonoBehaviour
{
    public Button startButton;

    void Start()
    {
        startButton.onClick.AddListener(StartButton);
    }

    void StartButton()
    {
        SceneLoader.OnSceneLoaded("Fishing", UnityEngine.SceneManagement.LoadSceneMode.Single);
        SceneLoader.OnSceneLoaded("SampleScene", UnityEngine.SceneManagement.LoadSceneMode.Additive);
    }
}
