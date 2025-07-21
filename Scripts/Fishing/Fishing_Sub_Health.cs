using System.Collections;
using UnityEngine;

public class Fishing_Sub_Health : Fishing_Sub
{
    public TMPro.TMP_Text type4Text;
    Coroutine playing;
    public string originString;

    //==================================================================================================================================
    //
    //==================================================================================================================================

    public override void SetStart()
    {
        base.SetStart();
    }

    public override void StartGame()
    {
        base.StartGame();
        SetKeyCode();
        if (playing != null)
            StopCoroutine(playing);
        playing = StartCoroutine(Playing());
    }

    public override void InputMouseLeft(bool _input)
    {
        bool active = originString[0].ToString() == "L";
        FillAmount(active);
    }

    public override void InputMouseRight(bool _input)
    {
        bool active = originString[0].ToString() == "R";
        FillAmount(active);
    }

    void FillAmount(bool _active)
    {
        EndGame();
        if (_active)
        {
            if (originString.Length > 0)
            {
                string newString = originString.Substring(1) + GetKeyCode();
                originString = newString;
                type4Text.text = originString;
                //keyCode = KeyCodeToIndex(newString[0].ToString());

                if (AddAmount(0.2f) == true)
                {
                    EndGame();
                }
            }
        }
        else if (fillAmount > 0f)
        {
            AddAmount(-0.2f);
        }
    }

    void SetKeyCode()
    {
        originString = "";
        for (int i = 0; i < 5; i++)
        {
            originString += GetKeyCode();
        }
        type4Text.text = originString;
    }

    string GetKeyCode()
    {
        keyCode = Random.Range(0, 2);
        string temp = (keyCode == 0) ? "L" : "R";
        return temp;
    }

    IEnumerator Playing()
    {
        fillAmount = 0f;
        bool setStart = true;
        while (setStart == true)
        {
            if (fillAmount > 0f)
            {
                fillAmount -= Time.deltaTime * 0.1f;
            }
            gageImage.material.SetFloat("_FillAmount", fillAmount);
            yield return null;
        }
    }

}
