using UnityEngine;
using System.Collections.Generic;

public class InstancedMeshRenderer : MonoBehaviour
{
    // 메쉬와 재질 데이터를 가져올 씬 내의 원본 오브젝트 참조
    public GameObject sourceObject;
    private Mesh meshToDraw;
    public Material instancedMaterial;
    private List<Matrix4x4> matrices;

    public void SetStart()
    {
        //
        MeshFilter meshFilter = sourceObject.GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            meshToDraw = meshFilter.sharedMesh;
        }
        else
        {
            Debug.LogError("원본 오브젝트에 MeshFilter 컴포넌트가 존재해야 합니다");
            enabled = false;
            return;
        }

        // 변환 행렬 리스트를 초기화합니다
        matrices = new List<Matrix4x4>();

        // 각 인스턴스가 배치될 무작위 위치와 회전 및 크기 정보를 생성합니다


        Vector3 halfSize = new Vector3(gridSize.x, 0f, gridSize.y);
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3 position = (new Vector3(x, 0f, y) * 10f) - (halfSize * 5f);
                Quaternion rotation = Quaternion.Euler(0f, 0f, 0f);
                Vector3 scale = Vector3.one;
                // 위치와 회전 및 크기를 조합하여 변환 행렬을 생성하고 리스트에 추가합니다
                matrices.Add(Matrix4x4.TRS(position, rotation, scale));
            }
        }

        //// 인스턴스만 화면에 그리기 위해 원본 오브젝트의 렌더러를 비활성화합니다
        //sourceObject.GetComponent<Renderer>().enabled = false;

        //SetBatch();
        //Test();
    }

    void Update()
    {
        if (meshToDraw == null)
            return;
        // 준비된 모든 인스턴스를 단 한 번의 호출로 화면에 렌더링합니다
        Graphics.DrawMeshInstanced(
            meshToDraw,          // 그려낼 메쉬 데이터
            0,                   // 서브메쉬 인덱스
            instancedMaterial,   // GPU 인스턴싱 옵션이 활성화된 재질
            matrices             // 인스턴스별 변환 행렬 리스트
        );
        //for (int i = 0; i < batchDatas.Count; i++)
        //{
        //    Graphics.DrawMeshInstanced(
        //        batchDatas[i].meshToDraw,
        //        0,
        //        batchDatas[i].instancedMaterial,
        //        batchDatas[i].modelMatrix
        //    );
        //}
    }
    [System.Serializable]
    public struct BatchData
    {
        public Mesh meshToDraw;
        public Material instancedMaterial;
        public List<Matrix4x4> modelMatrix;

        public void AddMatrix(Matrix4x4 matrix)
        {
            if (modelMatrix == null)
            {
                modelMatrix = new List<Matrix4x4>();
            }
            modelMatrix.Add(matrix);
        }
    }
    public List<BatchData> batchDatas = new List<BatchData>();
    public Vector2Int gridSize = new Vector2Int(100, 100);
    public float gridScale = 2.0f;

    void SetBatch()
    {
        Dictionary<string, BatchData> batchDict = new Dictionary<string, BatchData>();
        // 원본 오브젝트의 MeshFilter 컴포넌트에서 메쉬 데이터를 추출합니다
        MeshFilter[] meshFilters = gameObject.GetComponentsInChildren<MeshFilter>();
        foreach (var mf in meshFilters)
        {
            if (mf.TryGetComponent<Renderer>(out var renderer))
            {
                string dictID = mf.sharedMesh.name + renderer.sharedMaterial.name;
                if (batchDict.ContainsKey(dictID))
                {
                    BatchData existingData = batchDict[dictID];
                    existingData.AddMatrix(mf.transform.localToWorldMatrix);
                    batchDict[dictID] = existingData;
                    continue;
                }

                BatchData data = new()
                {
                    meshToDraw = mf.sharedMesh,
                    instancedMaterial = renderer.sharedMaterial
                };
                data.AddMatrix(mf.transform.localToWorldMatrix);
                batchDict[dictID] = data;
            }
        }

        batchDatas.Clear();
        foreach (var batch in batchDict)
        {
            batchDatas.Add(batch.Value);
        }
        //parent.SetActive(false);
    }
    BatchData batchData;
    //GameObject parent;
    //[ContextMenu("asdfasdfasdf")]
    void Test()
    {
        //if (parent != null)
        //{
        //    DestroyImmediate(parent);
        //}
        //parent = new GameObject("GridParent");
        //parent.transform.SetParent(transform);

        MeshFilter meshFilter = sourceObject.GetComponent<MeshFilter>();
        Vector3 halfSize = new Vector3(gridSize.x, 0f, gridSize.y);
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3 position = (new Vector3(x, 0f, y) * 10f) - (halfSize * 5f);
                Quaternion rotation = Quaternion.Euler(0f, 0f, 0f);
                Vector3 scale = Vector3.one;
                Matrix4x4 matrix = Matrix4x4.TRS(position, rotation, scale);

                BatchData data = new()
                {
                    meshToDraw = meshFilter.sharedMesh,
                    instancedMaterial = instancedMaterial
                };
                data.AddMatrix(matrix);
            }
        }
    }
}