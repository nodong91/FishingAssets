using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
[RequireComponent(typeof(PlayableDirector))]
[RequireComponent(typeof(SignalReceiver))]
public class Tutorial_TimeLine : MonoBehaviour
{
    PlayableDirector playableDirector;
    public TMPro.TMP_Text tutorial_Text;
    public Custom_Button nextButton;
    public string id;
    public bool completed;
    public int currentIndex;

    [System.Serializable]
    public struct TutorialComment
    {
        public string comment;
        public int commentSize;
    }
    public TutorialComment[] tutorialComment;
    [Header(" [ 대화 정보 ]")]
    public Data_NPC npcData;
    public int dialogIndex;

    public delegate void Delegate_Tutorial();
    public Delegate_Tutorial delegate_Tutorial;

    public void SetStart()
    {
        //Time.timeScale = 0.0f;
        currentIndex = 0;
        nextButton.SetButton(NextButton);
        playableDirector = GetComponent<PlayableDirector>();
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

        delegate_Tutorial?.Invoke();
    }

    void SetText(int _index)
    {
        tutorial_Text.fontSize = tutorialComment[_index].commentSize;
        tutorial_Text.text = tutorialComment[_index].comment;
    }
}
