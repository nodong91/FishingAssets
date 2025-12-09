using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_Manager : MonoBehaviour
{
    //=========================================================================================================
    // 튜토리얼 저장
    //=========================================================================================================

    public List<string> completedTutorial;
    public Sprite_Animation fishingTutorial;

    public static Tutorial_Manager current;

    public void Awake()
    {
        current = this;
        SetStart();
    }
    public void SetStart()
    {
        LoadTutorial();
        fishingTutorial.OpenCanvas(false);
    }

    public bool IsTutorialCompleted(string _id)// 완료 인지 확인
    {
        Debug.Log($"Tutorial 완료 : {_id} {completedTutorial.Count}");
        return completedTutorial.Contains(_id);
    }

    public void CompletedTutorial(string _id)// 완료 저장
    {
        if (completedTutorial == null)
            completedTutorial = new List<string>();
        completedTutorial.Add(_id);
        SaveTutorial();
    }

    public void FishingTutorial()
    {
        fishingTutorial.OpenCanvas(true);
    }

    void SaveTutorial()
    {
        Static_JsonManager.SaveTutorialData(Const_Tutorial._tutorialKey, completedTutorial);
    }

    public void LoadTutorial()
    {
        if (Static_JsonManager.TryLoadTutorialData(Const_Tutorial._tutorialKey, out List<string> _completedTutorial))
        {
            completedTutorial = _completedTutorial;
        }
        else
        {
            completedTutorial = new List<string>();
        }
    }
}
