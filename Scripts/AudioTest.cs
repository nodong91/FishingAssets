using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(AudioTest))]
public class AudioTest_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        GUIStyle fontStyle = new GUIStyle(GUI.skin.button);
        fontStyle.fontSize = 15;
        fontStyle.normal.textColor = Color.yellow;

        AudioTest Inspector = target as AudioTest;
        if (GUILayout.Button("Data Parse", fontStyle, GUILayout.Height(30f)))
        {
            Inspector.UpdateData();
            EditorUtility.SetDirty(Inspector);
        }
        GUILayout.Space(10f);
        base.OnInspectorGUI();
    }
}
#endif

public class AudioTest : MonoBehaviour
{
    public AudioClip clip;
    public float endTime;
    public AudioClip trimmed;
    public void UpdateData()
    {
        trimmed = TrimSilence(clip, endTime);
    }


    AudioClip TrimSilence(AudioClip clip, float endTime)
    {
        float[] samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);

        int endIndex = (int)(endTime * clip.frequency) * clip.channels;
        int i;
        for (i = endIndex - 1; i >= 0; i--)
        {
            if (Mathf.Abs(samples[i]) > 0.1f)
            {
                break;
            }
        }
        float[] trimmedSamples = new float[i + 1];
        Array.Copy(samples, trimmedSamples, trimmedSamples.Length);

        AudioClip trimmedClip = AudioClip.Create("trimmedClip", trimmedSamples.Length / clip.channels, clip.channels, clip.frequency, false);
        trimmedClip.SetData(trimmedSamples, 0);
        return trimmedClip;
    }
}
