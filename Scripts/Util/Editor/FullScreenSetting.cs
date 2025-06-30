using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using System.Reflection;
//using static UnityEditor.Rendering.CameraUI;
//using UnityEngine.UI.Extensions;

namespace P01.Editor
{
    public class FullScreenSetting : EditorWindow
    {
        [MenuItem("Graphics Tool/03. FullScreenSetting")]
        public static void ShowWindow()
        {
            FullScreenSetting window = GetWindow<FullScreenSetting>("FullScreenSetting");
            window.minSize = new Vector2(500f, 200f);
            window.Show();
        }

        public Material fullscreenMaterial;
        public FullScreenPassRendererFeature fullScreenRendererFeature;

        bool tutorialToggle;

        void OnGUI()
        {
            GUIStyle guiText = new()
            {
                fontSize = 13,
                normal = { textColor = Color.yellow },
                alignment = TextAnchor.MiddleLeft
            };

            GUIStyle buttonText = new(GUI.skin.button)
            {
                fontSize = 13,
                normal = { textColor = Color.yellow },
                alignment = TextAnchor.MiddleCenter
            };
            float height = 20f;
            float width = position.width * 0.5f;
            EditorGUILayout.BeginVertical();
            if (GUILayout.Button("사용법", buttonText, GUILayout.Height(20f)))
            {
                tutorialToggle = !tutorialToggle;
            }
            ShowTutorial(guiText);
            EditorGUILayout.EndVertical();

            buttonText.fontSize = 15;
            buttonText.fontStyle = FontStyle.Bold;
            GUI.color = Color.yellow;
            GUI.color = Color.white;
            EditorGUILayout.BeginHorizontal("box");
            EditorGUILayout.BeginVertical();
            featureIndex = EditorGUILayout.IntField("Feature Index", featureIndex);
            injectionPoint = (FullScreenPassRendererFeature.InjectionPoint)EditorGUILayout.EnumPopup("랜더 순서", injectionPoint, GUILayout.Height(height));
            fullscreenMaterial = EditorGUILayout.ObjectField("풀스크린 매터리얼", fullscreenMaterial, typeof(Material), true, GUILayout.Height(height)) as Material;
            EditorGUILayout.ObjectField("랜더 파이프라인", fullScreenRendererFeature, typeof(FullScreenPassRendererFeature), true, GUILayout.Height(height));
            EditorGUILayout.EndVertical();

            if (GUILayout.Button("SetFeature", buttonText, GUILayout.Height(height * 4f)))
            {
                SetRendererFeature();
            }
            GUILayout.EndHorizontal();
        }

        void ShowTutorial(GUIStyle _guiText)
        {
            if (tutorialToggle == true)
            {
                _guiText.normal.textColor = Color.gray;
                GUILayout.Space(10f);
                GUILayout.Label(" 1. 랜더 순서 결정", _guiText);
                GUILayout.Label("  - Before Rendering Transparents : 알파 출력 전에 적용", _guiText);
                GUILayout.Label("  - Before Rendering Post Processing : PP출력 전 적용", _guiText);
                GUILayout.Label("  - After Rendering Post Processing : PP출력 후 적용", _guiText);
                GUILayout.Label(" 2. 풀스크린 매터리얼에 적용할 매터리얼 추가", _guiText);
                GUILayout.Label(" 3. SetFeature 클릭", _guiText);

                GUILayout.Space(10f);
                _guiText.normal.textColor = Color.yellow;
                GUILayout.Label(" ※ Material이 없을 시 FullScreen Off", _guiText);
            }
        }

        ScriptableRendererData scriptableRendererData;
        FullScreenPassRendererFeature.InjectionPoint injectionPoint =
            FullScreenPassRendererFeature.InjectionPoint.AfterRenderingPostProcessing;
        int featureIndex = 0;
        void SetRendererFeature()
        {
            var pipeline = ((UniversalRenderPipelineAsset)GraphicsSettings.defaultRenderPipeline);
            if (pipeline == null)
                return;
            var propertyInfo = pipeline.GetType().GetField("m_RendererDataList", BindingFlags.Instance | BindingFlags.NonPublic);
             scriptableRendererData = ((ScriptableRendererData[])propertyInfo.GetValue(pipeline))[0];
            Debug.LogWarning($"scriptableRendererData : {scriptableRendererData.rendererFeatures.Count}");

            fullScreenRendererFeature = (FullScreenPassRendererFeature)scriptableRendererData.rendererFeatures[featureIndex];
            //renderObject = (RenderObjects)scriptableRendererData.rendererFeatures[12];

            if (fullScreenRendererFeature != null)
            {
                fullScreenRendererFeature.SetActive(fullscreenMaterial != null);
                fullScreenRendererFeature.passMaterial = fullscreenMaterial;
                fullScreenRendererFeature.injectionPoint = injectionPoint;
            }

#if UNITY_EDITOR
            scriptableRendererData.SetDirty();
#endif
        }
    }
}
#endif