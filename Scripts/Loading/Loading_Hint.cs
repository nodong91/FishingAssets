using System.Collections.Generic;
using UnityEngine;

public class Loading_Hint : MonoBehaviour
{
    private List<string> hintStrings = new List<string>();
    public TMPro.TMP_Text hintText;

    public void SetStart()
    {
        hintStrings.Clear();
        foreach (var child in Singleton_Data.INSTANCE.Dict_Language)
        {
            if (child.Key.Contains("ht_"))// »˘∆Æ∏∏ √ﬂ√‚
            {
                hintStrings.Add(child.Key);
            }
        }
    }

    public void SetHint()
    {
        if (Option_Manager.current == null)
        {
            hintText.text = "";
            return;
        }
        string key = hintStrings[Random.Range(0, hintStrings.Count)];
        hintText.text = Singleton_Data.INSTANCE.GetLanguage(key);
    }
}
