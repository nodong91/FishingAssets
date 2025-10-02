using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;

namespace P01_Tool
{
    public class ScreenShotEditor : EditorWindow
    {
        //private Vector2 scrollPosition;
        private Vector2 screenSize;
        GUIStyle titleFont = new GUIStyle();
        [MenuItem("Tools/ScreenShot")]
        static void OpenChangePrefab()
        {
            ScreenShotEditor window = EditorWindow.GetWindow<ScreenShotEditor>("ScreenShot");
            window.minSize = new Vector2(300f, window.minSize.y);

            window.Show();
        }

        void OnGUI()
        {
            titleFont.fontSize = 20;
            titleFont.normal.textColor = Color.gray;

            GUILayout.Space(10f);
            GUILayout.Label(" 스크린샷 찍기 ", titleFont);
            GUILayout.Space(10f);
            
            EditorGUILayout.BeginHorizontal();
            screenSize = EditorGUILayout.Vector2Field("Screen Size:", screenSize);
            if (GUILayout.Button("Capture Screen"))
            {
                if (Selection.activeTransform == null || Selection.activeTransform.GetComponent<Camera>() == null)
                {
                    EditorUtility.DisplayDialog("Select Camera", "You must select a Camera first!", "OK");
                    return;
                }
                Camera selectCamera = Selection.activeTransform.GetComponent<Camera>();
                SaveScreenShot(selectCamera);

                AssetDatabase.Refresh();
            }
            EditorGUILayout.EndHorizontal();
        }

        void SaveScreenShot(Camera camera)
        {
            int resWidth = (int)screenSize.x;
            int resHeight = (int)screenSize.y;
            string path = EditorUtility.SaveFilePanelInProject("Save png", "New ScreenShot", "png",
                                            "Please enter a file name to save the texture to");
            RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
            camera.targetTexture = rt;
            Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
            //TextureImporter textureImporter = screenShot as TextureImporter;

            Rect rec = new Rect(0, 0, screenShot.width, screenShot.height);
            camera.Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
            screenShot.Apply();

            byte[] bytes = screenShot.EncodeToPNG();
            if (bytes != null)
            {
                File.WriteAllBytes(path, bytes);
            }

            //--Clean up--
            RenderTexture.active = null;
            camera.targetTexture = null;
        }
    }
}
#endif