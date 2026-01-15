using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(Instancer))]
public class Instancer_Custom : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Space(10f);

        GUIStyle fontStyle = new GUIStyle(GUI.skin.button);
        fontStyle.fontSize = 15;
        fontStyle.normal.textColor = Color.yellow;

        Instancer Inspector = target as Instancer;
        if (GUILayout.Button("Set Batch", fontStyle, GUILayout.Height(30f)))
        {
            Inspector.SetBatch();
            EditorUtility.SetDirty(Inspector);
        }
    }
}
#endif

public class Instancer : MonoBehaviour
{
    public LayerMask layerMask;
    public List<BATCH> setBatch = new List<BATCH>();
    public bool gpuInstancing;
    public List<GameObject> MissingMaterials = new List<GameObject>();

    [System.Serializable]
    public struct BATCH
    {
        [HideInInspector] public string name;
        public Mesh mesh;
        public Material[] mat;
        public Matrix4x4[] matrix;

        public BATCH(Mesh _mesh, Material[] _mat, Matrix4x4[] _matrix)
        {
            name = _mesh.name;
            mesh = _mesh;
            mat = _mat;
            matrix = _matrix;
        }
    }

    public void SetBatch()
    {
        // 매터리얼 - Enable GPU Instancing 활성화
        MeshFilter[] tempFilter = gameObject.GetComponentsInChildren<MeshFilter>();
        Dictionary<string, List<MeshFilter>> meshFilter = new Dictionary<string, List<MeshFilter>>();
        for (int i = 0; i < tempFilter.Length; i++)
        {
            Renderer addRenderer = tempFilter[i].GetComponent<Renderer>();
            Material[] addMaterials = addRenderer.sharedMaterials;
            for (int j = 0; j < addMaterials.Length; j++)
            {
                string dictKey = tempFilter[i].sharedMesh.name + addMaterials[j].name;
                if (meshFilter.ContainsKey(dictKey) == false)
                {
                    meshFilter[dictKey] = new List<MeshFilter>();
                }
                meshFilter[dictKey].Add(tempFilter[i]);
            }
        }
        Debug.LogWarning(meshFilter.Count);
        FilterListing(meshFilter);
    }

    void FilterListing(Dictionary<string, List<MeshFilter>> _meshFilter)
    {
        List<List<MeshFilter>> newMeshFilter = new List<List<MeshFilter>>();
        foreach (var child in _meshFilter)
        {
            int dictCount = 0;
            List<MeshFilter> temp = new List<MeshFilter>();
            int count = 0;
            for (int i = 0; i < child.Value.Count; i++)
            {
                if (count < 1023)
                {
                    count++;
                    temp.Add(child.Value[i]);
                }
                else
                {
                    dictCount++;
                    count = 0;
                    newMeshFilter.Add(temp);
                    temp = new List<MeshFilter>();
                }
            }
            newMeshFilter.Add(temp);
        }
        SetMatrix(newMeshFilter);
    }

    void SetMatrix(List<List<MeshFilter>> newMeshFilter)
    {
        setBatch = new List<BATCH>();
        for (int i = 0; i < newMeshFilter.Count; i++)
        {
            List<MeshFilter> newMesh = newMeshFilter[i];
            int sliceValue = newMesh.Count;
            var matrices = new Matrix4x4[sliceValue];
            int matriceCount = 0;
            for (int j = 0; j < sliceValue; j++)
            {
                matrices[matriceCount % sliceValue] = FindMatrix(newMesh[j]);
                matriceCount++;

                if (matriceCount % sliceValue == 0)
                {
                    Mesh addMesh = newMesh[j].sharedMesh;
                    Renderer addRenderer = newMesh[j].GetComponent<Renderer>();
                    Material[] addMaterials = addRenderer.sharedMaterials;
                    for (int m = 0; m < addMaterials.Length; m++)
                    {
                        if (addMaterials[m] != null)
                        {
                            addMaterials[m].enableInstancing = gpuInstancing;
                            Debug.LogWarning(addMaterials[m].name);
                        }
                        else
                        {
                            MissingMaterials.Add(newMesh[j].gameObject);
                        }
                    }
                    BATCH addBatch = new BATCH(addMesh, addMaterials, matrices);
                    setBatch.Add(addBatch);
                }
            }
        }
    }

    Matrix4x4 FindMatrix(MeshFilter addMesh)
    {
        //Transform parent = addMesh.transform.parent;
        Vector3 position = addMesh.transform.position;  // 오브젝트 위치
        Quaternion rotate = addMesh.transform.rotation;// 오브젝트 회전
        Vector3 scale = addMesh.transform.localScale;// 오브젝트 스케일
        //while (parent != null)
        //{
        //    scale = new Vector3(
        //        scale.x * parent.localScale.x,
        //        scale.y * parent.localScale.y,
        //        scale.z * parent.localScale.z
        //        );
        //    parent = parent.parent;
        //}
        return Matrix4x4.TRS(position, rotate, scale);
    }

    //public void SetHight()
    //{
    //    if (setHights != null)
    //        for (int i = 0; i < setHights.Length; i++)
    //        {
    //            MeshFilter[] newMeshFilter = setHights[i].GetComponentsInChildren<MeshFilter>();
    //            for (int j = 0; j < newMeshFilter.Length; j++)
    //            {
    //                if (Physics.Raycast(newMeshFilter[j].transform.position + Vector3.up * 1000f, Vector3.down, out RaycastHit hit, Mathf.Infinity, layerMask))
    //                {
    //                    newMeshFilter[j].transform.position = hit.point;
    //                }
    //            }
    //        }

    //    GameObject parent = new GameObject("[ Parent ]");
    //    int setCount = 100;
    //    for (int x = 0; x < setCount; x++)
    //    {
    //        for (int y = 0; y < setCount; y++)
    //        {
    //            Vector3 pos = new Vector3(x - (setCount - 1) * 0.5f, 0, y - (setCount - 1) * 0.5f) * 10f;
    //            GameObject test = Instantiate(temp, pos, Quaternion.identity, parent.transform);
    //            test.transform.localScale = Vector3.one;
    //        }
    //    }
    //    //parent.transform.position = new Vector3(setCount * 10f, 0f, setCount * 10f);
    //}

    //======================================================================================================================

    //void Start()
    //{
    //    Renderer[] hideRenderer = gameObject.GetComponentsInChildren<Renderer>();
    //    for (int i = 0; i < hideRenderer.Length; i++)
    //    {
    //        hideRenderer[i].enabled = false;
    //    }
    //    //CloneMaterial();
    //}
    //public List<Material> cloneMaterial = new List<Material>();
    //public Material cloneMat;
    //void CloneMaterial()
    //{
    //    cloneMat = Instantiate(setBatch[0].mat[0]);
    //    cloneMat.SetInt("_InOut", 1);
    //    //cloneMaterial = new List<Material>();
    //    //foreach (var batch in setBatch)
    //    //{
    //    //    for (int i = 0; i < batch.mat.Length; i++)
    //    //    {
    //    //        Material mat = Instantiate(batch.mat[i]);
    //    //        mat.SetFloat("_InOut", 1f);
    //    //        cloneMaterial.Add(mat);
    //    //    }
    //    //}
    //}

    //private void Update()
    //{
    //    UpdateBatch();
    //}

    public void UpdateBatch()
    {
        for (int i = 0; i < setBatch.Count; i++)
        {
            BATCH batch = setBatch[i];
            for (int j = 0; j < batch.mat.Length; j++)
            {
                Graphics.DrawMeshInstanced(batch.mesh, j, batch.mat[j], batch.matrix, batch.matrix.Length);
            }
        }
    }
}