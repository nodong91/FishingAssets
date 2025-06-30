using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//[CreateAssetMenu(fileName = "Data_Animation", menuName = "Scriptable Objects/Data_Animation")]
public class Data_Animation : ScriptableObject
{

#if UNITY_EDITOR
    //public List<Object> resourceFolder = new List<Object>();

    //public void UpdateData()
    //{
    //    SetDataEditor();
    //}

    //public void SetDataEditor()
    //{
    //    if (resourceFolder.Count == 0)
    //    {
    //        Debug.LogError("애니메이션 폴더 필요");
    //        return;
    //    }

    //    List<AnimationClip> animationClips = new List<AnimationClip>();
    //    string[] paths = new string[resourceFolder.Count];

    //    for (int i = 0; i < resourceFolder.Count; i++)
    //    {
    //        paths[i] = AssetDatabase.GetAssetPath(resourceFolder[i]);
    //        Debug.LogWarning("File paths : " + paths[i]);
    //    }
    //    // 애니메이션 클립 추출
    //    var assets = AssetDatabase.FindAssets("t: AnimationClip", paths);

    //    for (int i = 0; i < assets.Length; i++)
    //    {
    //        var data = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(assets[i]), typeof(AnimationClip));
    //        animationClips.Add(data as AnimationClip);
    //        //Debug.LogWarning("assetPath : " + data);
    //    }
    //}

    public void SetAnimationClip(List<AnimationClip> _animationClips)
    {
        AnimationDatas = new List<AniClipClass>();
        for (int i = 0; i < (int)AnimationType.Death + 1; i++)
        {
            AniClipClass aniClipClass = new AniClipClass();
            animationType = (AnimationType)i;

            List<AnimationClip> aniList = new List<AnimationClip>();
            string str = animationType.ToString();

            for (int j = 0; j < _animationClips.Count; j++)
            {
                Debug.LogWarning("assetPath : " + _animationClips[j].name);
                if (_animationClips[j].name.Contains(str, System.StringComparison.OrdinalIgnoreCase))
                {
                    bool loopTime = false;
                    // 애니메이션 클립 이름으로 세팅
                    switch (animationType)
                    {
                        case AnimationType.Walk:
                        case AnimationType.Sprint:

                        case AnimationType.Idle:
                            loopTime = true;
                            aniClipClass.playType = AniClipClass.PlayType.Trigger;
                            break;

                        case AnimationType.Death:
                            aniClipClass.playType = AniClipClass.PlayType.Once;
                            break;

                        case AnimationType.Roll:
                        case AnimationType.Hit:
                        case AnimationType.KnockBack:
                            aniClipClass.playType = AniClipClass.PlayType.Single;
                            break;
                    }
                    // 애니메이션 루프 변경
                    SerializedProperty AniClip = new SerializedObject(_animationClips[j]).FindProperty("m_AnimationClipSettings.m_LoopTime");
                    AniClip.boolValue = loopTime;
                    AniClip.serializedObject.ApplyModifiedProperties();

                    // 애니메이션 스타일(스트링)이 포함되어 있는 경우 추가
                    aniList.Add(_animationClips[j]);
                }
            }
            aniClipClass.typeName = i + " : " + str + " (" + aniList.Count + ")";// 클래스 표기용
            aniClipClass.animationClips = aniList;
            AnimationDatas.Add(aniClipClass);
        }
    }
#endif
    // 필요한 기본 애니메이션 이름
    //"Default_Walk"
    //"Default_Run"
    //"Default_Idle"
    //"Default_Battle"
    //"Default_Action01"
    //"Default_Action02"

    [System.Serializable]
    public class AniClipClass
    {
        [HideInInspector]
        public string typeName;
        public enum PlayType
        {
            Trigger,
            Single,
            Once,
            Loop
        }
        public PlayType playType;
        public List<AnimationClip> animationClips;
    }
    public List<AniClipClass> AnimationDatas;

    public enum AnimationType
    {
        Idle,
        Walk,
        Sprint,
        Attack,
        Roll,
        Hit,
        KnockBack,
        Death// 마지막 타입
    }
    private AnimationType animationType;
}

public class AnimationClipOverrides : List<KeyValuePair<AnimationClip, AnimationClip>>
{
    public AnimationClipOverrides(int capacity) : base(capacity) { }

    public AnimationClip this[string name]
    {
        get { return this.Find(x => x.Key.name.Equals(name)).Value; }
        set
        {
            int index = this.FindIndex(x => x.Key.name.Equals(name));
            if (index != -1)
                this[index] = new KeyValuePair<AnimationClip, AnimationClip>(this[index].Key, value);
        }
    }
}