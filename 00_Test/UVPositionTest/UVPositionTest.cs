using System.Collections;
using UnityEngine;
using static Camera_Manager;
using static UVPositionTest;

public class UVPositionTest : MonoBehaviour
{
    public enum MeshType
    {
        SharedMesh,
        Mesh,
    }
    public MeshType meshType;
    public GameObject targetObject;
    public Mesh mesh;
    public Vector3[] meshVertexs;
    public Color[] colors;
    Coroutine drawing, rotating;
    public Custom_Button clearButton;
    public Custom_Button drawButton;

    void Start()
    {
        clearButton.SetButton(ClearButton);
        drawButton.SetButton(DrawButton);
        SetMesh();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Audio_LoopFX();
            if (rotating != null)
                StopCoroutine(rotating);
            drawing = StartCoroutine(StartDrawing(true));
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Stop_LoopFX();
            if (drawing != null)
                StopCoroutine(drawing);
            CheckColor();
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (rotating != null)
                StopCoroutine(rotating);
            rotating = StartCoroutine(InputRotating(true));
        }
        else if (Input.GetMouseButtonUp(1))
        {
            if (rotating != null)
                StopCoroutine(rotating);
            rotating = StartCoroutine(InputRotating(false));
            SetMesh();
        }
    }
    public AudioClip reelClip;
    public AudioSource reelSound;
    public void Audio_LoopFX()
    {
        AudioSource audioSource = reelSound;
        audioSource.gameObject.SetActive(true);
        Debug.Log($"{audioSource.name}");
        audioSource.clip = reelClip;
        audioSource.loop = true;
        audioSource.pitch = Random.Range(0.7f, 1.3f);
        audioSource.Play();

        reelSound = audioSource;
    }

    public void Stop_LoopFX()
    {
        if (reelSound != null)
        {
            reelSound.Stop();
            reelSound.gameObject.SetActive(false);
        }
    }

    void ClearButton()
    {
        drawColor.a = 0f;
    }

    void DrawButton()
    {
        drawColor.a = 1f;
    }

    void SetMesh()
    {
        switch (meshType)
        {
            case MeshType.SharedMesh:
                mesh = targetObject.GetComponent<MeshFilter>().sharedMesh;
                break;
            case MeshType.Mesh:
                mesh = targetObject.GetComponent<MeshFilter>().mesh;
                break;
        }
        meshVertexs = new Vector3[mesh.vertices.Length];
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            Vector3 thePosition = targetObject.transform.TransformPoint(mesh.vertices[i]);
            meshVertexs[i] = thePosition;
        }

        if (mesh.colors.Length == 0)
            colors = new Color[mesh.vertices.Length];
        else
            colors = mesh.colors;
    }

    void CheckColor()
    {
        int compColor = 0;
        for (int i = 0; i < colors.Length; i++)
        {
            if (colors[i].a > 0.5f)
            {
                compColor++;
            }
        }
        complete = (compColor / (float)colors.Length) * 100f;
    }









    IEnumerator StartDrawing(bool _draw)
    {
        while (_draw == true)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform == targetObject.transform)
                {
                    DrawVertex(hit.point);
                    //HitShield(hit.point);
                    Debug.LogWarning("Hit " + hit.transform.name);
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    void DrawVertex(Vector3 _position)
    {
        gizmoPosition = _position;
        for (int i = 0; i < meshVertexs.Length; i++)
        {
            float distance = (_position - meshVertexs[i]).magnitude;
            if (distance < drawSize)
            {
                Color setColor = Color32.Lerp(colors[i], drawColor, (1f - (distance / drawSize)) * 0.3f);
                colors[i] = setColor;
            }
        }
        mesh.colors = colors;
    }







    public Color drawColor;
    public float drawSize = 0.1f;

    public Vector2 startPosition, currentPosition;
    public Vector3 direction;
    public float rotationSpeed = 5f;
    public float complete;
    public float smoothSpeed = 10f;
    Vector2 currentAngle;

    IEnumerator InputRotating(bool _input)
    {
        Vector2 prevPosition = GetInputScreen;
        Vector2 prevAngle = targetObject.transform.rotation.eulerAngles;
        while (_input == true)
        {
            yield return null;

            float currentX = prevPosition.x - GetInputScreen.x;
            float currentY = prevPosition.y - GetInputScreen.y;
            currentAngle = prevAngle + new Vector2(currentY, currentX) * 180f;
            targetObject.transform.rotation = Quaternion.Euler(currentAngle.x, currentAngle.y, 0f);
            Debug.Log("InputRotating");
        }
    }

    public Vector2 GetInputScreen
    {
        get
        {
            return Camera.main.ScreenToViewportPoint(Input.mousePosition);
        }
    }

    Vector3 gizmoPosition;
    //private void OnDrawGizmos()
    //{
    //    if (mesh == null || meshVertexs == null || mesh.colors == null)
    //        return;

    //    Gizmos.color = Color.white;
    //    Gizmos.DrawSphere(gizmoPosition, 0.1f);
    //    for (int i = 0; i < meshVertexs.Length; i++)
    //    {
    //        Gizmos.color = mesh.colors[i];
    //        Gizmos.DrawSphere(meshVertexs[i], 0.01f);
    //    }
    //}
}