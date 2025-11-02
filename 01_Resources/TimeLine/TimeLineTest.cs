using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class TimeLineTest : MonoBehaviour
{
    public int currentIndex;
    public TMPro.TMP_Text tutorial_Text;
    public PlayableDirector playableDirector;
    public Custom_Button nextButton;

    [System.Serializable]
    public class TutorialStruct
    {
        public string id;
        public Data_NPC npc;
        public bool completed;
        [System.Serializable]
        public struct TutorialComment
        {
            public string comment;
            public int commentSize;
        }
        public TutorialComment[] tutorialComment;
    }
    public TutorialStruct tutorialStruct;

    private void Start()
    {

    }
    public bool acting;
    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if(acting == false)
            {
                acting = true;
                SetStart();
            }
        }
    }

    void SetStart()
    {
        Time.timeScale = 0.0f;
        nextButton.SetButton(NextButton);
        playableDirector.Play();
    }

    void NextButton()
    {
        playableDirector.Play();
    }

    public void Tutorial_Pause()
    {
        playableDirector.Pause();
        SetText(currentIndex);
        currentIndex++;
    }

    public void Tutorial_End()
    {
        Time.timeScale = 1.0f;
        currentIndex = 0;
        playableDirector.Stop();
    }

    void SetText(int _index)
    {
        tutorial_Text.fontSize = tutorialStruct.tutorialComment[_index].commentSize;
        tutorial_Text.text = tutorialStruct.tutorialComment[_index].comment;
    }
}
