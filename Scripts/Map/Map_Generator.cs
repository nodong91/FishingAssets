using System.Collections.Generic;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(Map_Generator))]
public class Editor_Map_Generator : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Space(10f);

        GUIStyle fontStyle = new GUIStyle(GUI.skin.button);
        fontStyle.fontSize = 15;
        fontStyle.normal.textColor = Color.yellow;

        Map_Generator Inspector = target as Map_Generator;
        if (GUILayout.Button("UpdateData", fontStyle, GUILayout.Height(30f)))
        {
            Inspector.UpdateData();
            EditorUtility.SetDirty(Inspector);
        }
    }
}
#endif

//[ExecuteInEditMode]
public class Map_Generator : MonoBehaviour
{
    [Header("[ Map Grid ]")]
    public float nodeSize = 2f;
    public Vector2Int worldGrid;
    private Vector2 worldSize;
    public GameObject hideObject;

    [System.Serializable]
    public class Node
    {
        [HideInInspector]
        public string nodeName;
        public Data_Manager.AreaType areaType;
        public Vector3 worldPosition;
        public GameObject onObject;// 유닛이 아니라 다른게 올라올 수도??
        public string neighbours;

        public Vector2Int grid;
        public Vector2Int cost;
        public Vector2Int parentNode;

        public Node(Vector3 _worldPos, Vector2Int _grid)
        {
            worldPosition = _worldPos;
            grid = _grid;
        }

        public void SetNodeType(Data_Manager.AreaType _areaType)
        {
            areaType = _areaType;
            nodeName = $"{areaType} : {grid}, {cost}";
        }

        public void UnitOnNode(GameObject _onObject)
        {
            onObject = _onObject;
        }

        public int fCost
        {
            get
            {
                return cost.x + cost.y;
            }
        }
    }
    public Node[,] nodeMap;
    private List<Node> allNodes;
    public int fishCount = 15;
    public int boxCount = 15;

    public GameObject[] safetyArea;

    public Trigger_Fish triggerFish;
    public Trigger_RandomBox triggerRandomBox;

    public static Map_Generator current;

    public void UpdateData()
    {
        SetNodeGrid();
    }

    private void Awake()
    {
        current = this;
    }

    private void Start()
    {
        SetStart();
        hideObject.SetActive(false);
    }

    public void SetStart()
    {
        SetNodeGrid();
        for (int area = 0; area < (int)(Data_Manager.AreaType.Abyssal + 1); area++)
        {
            Data_Manager.AreaType areaType = (Data_Manager.AreaType)area;
            if (areaType != Data_Manager.AreaType.None)// 생성 불가 위치
            {
                for (int i = 0; i < fishCount; i++)// 낚시터 세팅
                {
                    //Data_Manager.AreaType areaType = Data_Manager.AreaType.Shallow;
                    Node node = GetTypeNode(areaType);// 임시 연안 노드 랜덤으로 가져오기
                    Trigger_Fish inst = Instantiate(triggerFish, transform);
                    inst.SetAreaType(areaType);
                    inst.transform.position = node.worldPosition;
                    inst.transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
                }

                for (int i = 0; i < boxCount; i++)// 박스 세팅
                {
                    //Data_Manager.AreaType areaType = Data_Manager.AreaType.Shallow;
                    Node node = GetTypeNode(areaType);// 임시 연안 노드 랜덤으로 가져오기
                    Trigger_RandomBox inst = Instantiate(triggerRandomBox, transform);
                    inst.SetAreaType(areaType);
                    inst.transform.position = node.worldPosition;
                    inst.transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
                }
            }
        }
    }

    public void SetNodeGrid()
    {
        worldSize = (Vector2)worldGrid * nodeSize;

        nodeMap = new Node[worldGrid.x, worldGrid.y];
        Vector3 worldBottomLeft = transform.position - Vector3.right * worldSize.x / 2 - Vector3.forward * worldSize.y / 2;
        if (allNodes == null)
            allNodes = new List<Node>();
        allNodes.Clear();

        for (int x = 0; x < worldGrid.x; x++)
        {
            for (int y = 0; y < worldGrid.y; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeSize + nodeSize * 0.5f) + Vector3.forward * (y * nodeSize + nodeSize * 0.5f);
                Vector2Int grid = new Vector2Int(x, y);

                nodeMap[x, y] = new Node(worldPoint, grid);
                Data_Manager.AreaType areaType = SetNodeType(nodeMap[x, y]);// 노드 타입 세팅
                nodeMap[x, y].SetNodeType(areaType);
                allNodes.Add(nodeMap[x, y]);
            }
        }
    }

    Data_Manager.AreaType SetNodeType(Node _node)
    {
        LayerMask layerMask = ~(1 << LayerMask.NameToLayer("Water"));
        Vector3 worldPoint = _node.worldPosition + Vector3.up * 10f;
        if (Physics.Raycast(worldPoint, Vector3.down, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            //Debug.DrawRay(worldPoint, Vector3.down * hit.distance, Color.yellow, 1f);
            Debug.Log($"{hit.transform.name} : " + hit.distance);
            if (hit.distance < 10f)
            {
                return Data_Manager.AreaType.None;
            }
            else if (hit.distance < 11f)
            {
                shallowNodes.Add(_node);
                return Data_Manager.AreaType.Shallow;
            }
            else if (hit.distance < 15f)
            {
                coastalNodes.Add(_node);
                return Data_Manager.AreaType.Coastal;
            }
            else if (hit.distance < 18f)
            {
                oceanicNodes.Add(_node);
                return Data_Manager.AreaType.Oceanic;
            }
        }
        abyssalNodes.Add(_node);
        return Data_Manager.AreaType.Abyssal;
    }

    List<Node> shallowNodes = new List<Node>();
    List<Node> coastalNodes = new List<Node>();
    List<Node> oceanicNodes = new List<Node>();
    List<Node> abyssalNodes = new List<Node>();

    Queue<Node> shallowQueue = new Queue<Node>();
    Queue<Node> coastalQueue = new Queue<Node>();
    Queue<Node> oceanicQueue = new Queue<Node>();
    Queue<Node> abyssalQueue = new Queue<Node>();

    public Node GetTypeNode(Data_Manager.AreaType _areaType)
    {
        switch (_areaType)
        {
            case Data_Manager.AreaType.Shallow:
                if (shallowQueue.Count == 0)
                {
                    shallowQueue = P01_Utility.ShuffleQueue(shallowNodes, 0);
                }
                return shallowQueue.Dequeue();

            case Data_Manager.AreaType.Coastal:
                if (coastalQueue.Count == 0)
                {
                    coastalQueue = P01_Utility.ShuffleQueue(coastalNodes, 0);
                }
                return coastalQueue.Dequeue();

            case Data_Manager.AreaType.Oceanic:
                if (oceanicQueue.Count == 0)
                {
                    oceanicQueue = P01_Utility.ShuffleQueue(oceanicNodes, 0);
                }
                return oceanicQueue.Dequeue();

            case Data_Manager.AreaType.Abyssal:
                if (abyssalQueue.Count == 0)
                {
                    abyssalQueue = P01_Utility.ShuffleQueue(abyssalNodes, 0);
                }
                return abyssalQueue.Dequeue();
        }
        return null;
    }

    [Tooltip("8방향 이동가능")]
    public bool diagonal;

    // 근처 타일 리스팅
    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.grid.x + x;
                int checkY = node.grid.y + y;

                if (checkX >= 0 && checkX < worldGrid.x && checkY >= 0 && checkY < worldGrid.y)
                {
                    if (diagonal || (x == 0 || y == 0))    // 대각선 움직임 (8방향)
                    {
                        neighbours.Add(nodeMap[checkX, checkY]);
                    }
                }
            }
        }
        return neighbours;
    }

    public Node GetNodeFromPosition(Vector3 worldPosition)// 월드포지션으로 노드 찾기
    {
        float percentX = (worldPosition.x + worldSize.x * 0.5f) / worldSize.x;
        float percentY = (worldPosition.z + worldSize.y * 0.5f) / worldSize.y;

        int x = Mathf.Clamp((int)(worldGrid.x * percentX), 0, worldGrid.x - 1);
        int y = Mathf.Clamp((int)(worldGrid.y * percentY), 0, worldGrid.y - 1);
        return nodeMap[x, y];
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, new Vector3(worldSize.x, 1, worldSize.y));
        Handles.color = Color.blue;
        //UnityEditor.Handles.DrawWireDisc(safetyArea.transform.position, Vector3.up, safetyRadius);
        if (nodeMap != null)
        {
            foreach (Node n in nodeMap)
            {
                switch (n.areaType)
                {
                    case Data_Manager.AreaType.None:
                        Gizmos.color = Color.white;
                        break;
                    case Data_Manager.AreaType.Shallow:
                        Gizmos.color = Color.red;
                        break;
                    case Data_Manager.AreaType.Coastal:
                        Gizmos.color = Color.green;
                        break;
                    case Data_Manager.AreaType.Oceanic:
                        Gizmos.color = Color.blue;
                        break;
                    case Data_Manager.AreaType.Abyssal:
                        Gizmos.color = Color.black;
                        break;
                }
                Gizmos.DrawSphere(n.worldPosition, 0.3f);
                //GUIStyle fontStyle = new()
                //{
                //    fontSize = 20,
                //    normal = { textColor = Color.yellow },
                //    alignment = TextAnchor.MiddleCenter,
                //    fontStyle = FontStyle.Bold,
                //};
                //UnityEditor.Handles.Label(n.worldPosition, $"{n.grid.x}/{n.grid.y}", fontStyle);
            }
        }
    }
#endif
}
