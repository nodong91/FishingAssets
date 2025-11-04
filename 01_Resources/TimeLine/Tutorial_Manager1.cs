using UnityEngine;

public class Tutorial_Manager1 : MonoBehaviour
{
    public Tutorial_TimeLine[] timeLine;
    public int tutorialIndex;
    Tutorial_TimeLine currentTimeLine;

    public void StartTutorial()
    {
        GetTimeLine();
        currentTimeLine.SetStart();
    }

    public void GetTimeLine()
    {
        if (currentTimeLine != null)
            Destroy(currentTimeLine);
        currentTimeLine = Instantiate(timeLine[tutorialIndex], transform);
        currentTimeLine.delegate_Tutorial = TimelineEnd;
    }

    void TimelineEnd()
    {
        // 타임라인 끝

    }
}
