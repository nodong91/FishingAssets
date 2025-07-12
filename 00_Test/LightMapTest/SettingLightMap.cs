using System.Collections.Generic;
using UnityEngine;
using System.Collections;

//#if UNITY_EDITOR
//using UnityEditor;

//[CustomEditor(typeof(LightMapTest))]
//public class LightMapTest_Editor : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        GUIStyle fontStyle = new GUIStyle(GUI.skin.button);
//        fontStyle.fontSize = 15;
//        fontStyle.normal.textColor = Color.yellow;

//        LightMapTest Inspector = target as LightMapTest;
//        if (GUILayout.Button("Save", fontStyle, GUILayout.Height(30f)))
//        {
//            Inspector.UpdateData();
//            EditorUtility.SetDirty(Inspector);
//        }
//        GUILayout.Space(10f);
//        base.OnInspectorGUI();
//    }
//}
//#endif
public class SettingLightMap : MonoBehaviour
{
    public LightmapData[] lightmaps;
    [System.Serializable]
    public struct LightMapMesh
    {
        public MeshRenderer lightMeshRenderer;
        //public float scaleInLightmap;
        public int lightmapIndex;
        public Vector4 lightmapScaleOffset;
        public int realtimeLightmapIndex;
        public Vector4 realtimeLightmapScaleOffset;
        public readonly void SetLightmap(int _currentIndex)
        {
            //lightMeshRenderer.scaleInLightmap = scaleInLightmap;
            lightMeshRenderer.lightmapIndex = _currentIndex + lightmapIndex;
            lightMeshRenderer.lightmapScaleOffset = lightmapScaleOffset;
            lightMeshRenderer.realtimeLightmapIndex = realtimeLightmapIndex;
            lightMeshRenderer.realtimeLightmapScaleOffset = realtimeLightmapScaleOffset;
        }
    }
    public List<LightMapMesh> lightMapMeshes = new List<LightMapMesh>();
    [System.Serializable]
    public struct LightmapTextures
    {
        public Texture2D lightmapColor;
        public Texture2D lightmapDir;
        public Texture2D shadowMask;
    }
    public List<LightmapTextures> lightmapTextures = new List<LightmapTextures>();
    [ContextMenu("Save Lightmap")]
    public void UpdateData()
    {
        SetLightMap();
    }

    void SetLightMap()
    {
        lightmaps = LightmapSettings.lightmaps;

        lightMapMeshes = new List<LightMapMesh>();
        MeshRenderer[] meshRenderer = gameObject.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < meshRenderer.Length; i++)
        {
            LightMapMesh tempLightMap = new LightMapMesh
            {
                lightMeshRenderer = meshRenderer[i],
                //scaleInLightmap = meshRenderer[i].scaleInLightmap,
                lightmapIndex = meshRenderer[i].lightmapIndex,
                lightmapScaleOffset = meshRenderer[i].lightmapScaleOffset,
                realtimeLightmapIndex = meshRenderer[i].realtimeLightmapIndex,
                realtimeLightmapScaleOffset = meshRenderer[i].realtimeLightmapScaleOffset,
            };
            lightMapMeshes.Add(tempLightMap);
        }
        lightmapTextures = new List<LightmapTextures>();

        for (int i = 0; i < lightmaps.Length; i++)
        {
            LightmapTextures lightmapData = new LightmapTextures
            {
                lightmapColor = lightmaps[i].lightmapColor,
                lightmapDir = lightmaps[i].lightmapDir,
                shadowMask = lightmaps[i].shadowMask,
            };
            lightmapTextures.Add(lightmapData);
        }
    }
    public int currentCount;
    private void Start()
    {
        StartCoroutine(SettingLigntmap());
        //Setting();
    }

    public void Setting()
    {
        LightmapData[] currentLightmap = LightmapSettings.lightmaps;
        currentCount = currentLightmap.Length;
        LightmapData[] tempLightmaps = new LightmapData[lightmapTextures.Count + currentCount];
        for (int i = 0; i < tempLightmaps.Length; i++)
        {
            //Debug.LogWarning($"{currentCount} > {i}");
            if (currentLightmap.Length > i)
            {
                tempLightmaps[i] = currentLightmap[i];
            }
            else
            {
                int index = i - currentLightmap.Length;
                LightmapData lightmapData = new LightmapData
                {
                    lightmapColor = lightmapTextures[index].lightmapColor,
                    lightmapDir = lightmapTextures[index].lightmapDir,
                    shadowMask = lightmapTextures[index].shadowMask,
                };
                tempLightmaps[i] = lightmapData;
            }
        }
        LightmapSettings.lightmaps = tempLightmaps;

        for (int i = 0; i < lightMapMeshes.Count; i++)
        {
            lightMapMeshes[i].SetLightmap(currentCount);
        }
    }

    IEnumerator SettingLigntmap()
    {
        lightmaps = new LightmapData[lightmapTextures.Count];
        for (int i = 0; i < lightmaps.Length; i++)
        {
            LightmapData lightmapData = new LightmapData
            {
                lightmapColor = lightmapTextures[i].lightmapColor,
                lightmapDir = lightmapTextures[i].lightmapDir,
                shadowMask = lightmapTextures[i].shadowMask,
            };
            lightmaps[i] = lightmapData;
            yield return null;
        }
        LightmapSettings.lightmaps = lightmaps;

        for (int i = 0; i < lightMapMeshes.Count; i++)
        {
            lightMapMeshes[i].SetLightmap(currentCount);
            yield return null;
        }
    }
}
