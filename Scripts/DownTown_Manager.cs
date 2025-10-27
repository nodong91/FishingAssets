using UnityEngine;

public class DownTown_Manager : MonoBehaviour
{
    public string[] dayEvent;
    public string[] nightEvent;
    int hour => Game_Manager.current.GetMainUI.timeUI.hour;

    void Start()
    {
     
    }

    public void DownTownEvent()
    {
        if (hour >= 5f && hour < 18f)
        {
       
        }
        else
        {

        }
        Debug.LogWarning("이벤트 시간 : "+ hour);
    }
}
