using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reflection_Manager : MonoBehaviour
{

    Camera reflectionCamera;
    Camera mainCamera;

    public GameObject reflectionPlane;

    const string TextureName = "_RenderTexture";
    Material reflectionMaterial;
    public Material GetMaterial { get { return reflectionMaterial; } }
    RenderTexture reflectionTexture;

    void Start()
    {
        GameObject reflectionCameraGo = new GameObject("ReflectionCamera");
        reflectionCamera = reflectionCameraGo.AddComponent<Camera>();
        reflectionCamera.enabled = false;

        mainCamera = Camera.main;

        reflectionMaterial = reflectionPlane.GetComponent<Renderer>().material;
        reflectionTexture = new RenderTexture(Screen.width, Screen.height, 24);
        reflectionTexture.useMipMap = true;
    }

    void Update()
    {
        OnPostRender();
    }

    private void OnPostRender()
    {
        bool fog = RenderSettings.fog;
        RenderReflection();
    }

    // 판떼기 위치에 따라 카메라 위치 바뀌게
    void RenderReflection()
    {
        reflectionCamera.CopyFrom(mainCamera);
        reflectionCamera.cullingMask = reflectionCamera.cullingMask & ~(1 << LayerMask.NameToLayer("Water"));// Water 빼기

        Vector3 cameraDirectionWorldSpace = mainCamera.transform.forward;
        Vector3 cameraUpWorldSpace = mainCamera.transform.up;
        Vector3 cameraPositionWorldSpace = mainCamera.transform.position - reflectionPlane.transform.position;

        // 플로어 위치
        Vector3 cameraDirectionPlaneSpace = reflectionPlane.transform.InverseTransformDirection(cameraDirectionWorldSpace);
        Vector3 cameraUpPlaneSpace = reflectionPlane.transform.InverseTransformDirection(cameraUpWorldSpace);
        Vector3 cameraPositionPlaneSpace = reflectionPlane.transform.InverseTransformDirection(cameraPositionWorldSpace);

        // 미러 Vector
        cameraDirectionPlaneSpace.y *= -1.0f;
        cameraUpPlaneSpace.y *= -1.0f;
        cameraPositionPlaneSpace.y *= -1.0f;

        // back to World Space
        cameraDirectionWorldSpace = reflectionPlane.transform.TransformDirection(cameraDirectionPlaneSpace);
        cameraUpWorldSpace = reflectionPlane.transform.TransformDirection(cameraUpPlaneSpace);
        cameraPositionWorldSpace = reflectionPlane.transform.TransformDirection(cameraPositionPlaneSpace) + reflectionPlane.transform.position;

        // 카메라 포지션
        reflectionCamera.transform.position = cameraPositionWorldSpace;
        reflectionCamera.transform.LookAt(cameraDirectionWorldSpace + cameraPositionWorldSpace, cameraUpWorldSpace);

        reflectionCamera.targetTexture = reflectionTexture;
        reflectionCamera.Render();

        DrawQuad();
    }

    void DrawQuad()
    {
        GL.PushMatrix();

        reflectionMaterial.SetPass(0);
        reflectionMaterial.SetTexture(TextureName, reflectionTexture);

        GL.LoadOrtho();
        GL.Begin(GL.QUADS);

        GL.TexCoord2(1.0f, 0.0f);
        GL.Vertex3(0.0f, 0.0f, 0.0f);

        GL.TexCoord2(1.0f, 1.0f);
        GL.Vertex3(0.0f, 1.0f, 0.0f);

        GL.TexCoord2(0.0f, 1.0f);
        GL.Vertex3(1.0f, 1.0f, 0.0f);

        GL.TexCoord2(0f, 0f);
        GL.Vertex3(1.0f, 0.0f, 0.0f);

        GL.End();
        GL.PopMatrix();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (reflectionCamera != null)
            Gizmos.DrawSphere(reflectionCamera.transform.position, 1f);
        if (mainCamera != null)
            Gizmos.DrawSphere(mainCamera.transform.position, 1f);
    }
}
