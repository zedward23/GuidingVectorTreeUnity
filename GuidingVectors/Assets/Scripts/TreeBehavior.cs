using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeBehavior : MonoBehaviour
{
    public GameObject node;

    [SerializeField]
    public Vector3 bbMin;

    [SerializeField]
    public Vector3 bbMax;

    public float radius;

    public int global_ID_count;

    [SerializeField]
    public List<GameObject> createdNodes;

    public List<List<List<int>>> grid;

    public int sourceIdx;

    public int currNode;

    public Vector3Int totalCells;

    [SerializeField]
    private float angle;

    private List<int> visited = new List<int>();
    private List<int> unvisited = new List<int>();

    void DijkstrasStep(int currNode)
    {
        Vector3 currGuidingVec = this.createdNodes[currNode].gameObject.GetComponent<NodeBehavior>().GuidingVector;
        List<int> currNeighbors = this.createdNodes[currNode].gameObject.GetComponent<NodeBehavior>().OutGoingEdgesIDs;
        Vector3 src = this.createdNodes[currNode].gameObject.transform.position;

        for (int i = 0; i < currNeighbors.Count; i++)
        {
            if (!this.visited.Contains(currNeighbors[i]) && !this.unvisited.Contains(currNeighbors[i]))
            {
                this.unvisited.Add(currNeighbors[i]);
                createdNodes[currNeighbors[i]].gameObject.GetComponent<NodeBehavior>().ParentID = currNode;
            }

            createdNodes[currNeighbors[i]].gameObject.GetComponent<NodeBehavior>().lightUp();

            Vector3 newGuidingVec = Quaternion.Euler(0,0, angle) * currGuidingVec;
            Vector3 tgt = this.createdNodes[currNeighbors[i]].gameObject.transform.position;
            Vector3 dir = Vector3.Normalize(tgt - src);
            createdNodes[currNeighbors[i]].gameObject.GetComponent<NodeBehavior>().GuidingVector = newGuidingVec;
            Debug.Log("dir:"+dir);
            Debug.Log("guidingVec"+newGuidingVec);
            Debug.Log("weight" + (1 - Mathf.Abs(Vector3.Dot(dir, newGuidingVec))));
            float weight = Vector3.Distance(src, tgt) * (1 - Mathf.Abs(Vector3.Dot(dir, newGuidingVec)));

            if (weight + createdNodes[currNode].gameObject.GetComponent<NodeBehavior>().DistFromSource < createdNodes[currNeighbors[i]].gameObject.GetComponent<NodeBehavior>().DistFromSource)
            {
                createdNodes[currNeighbors[i]].gameObject.GetComponent<NodeBehavior>().DistFromSource = weight + createdNodes[currNode].gameObject.GetComponent<NodeBehavior>().DistFromSource;
            }
            
            Debug.Log("Neighbors: " + i + " is Node " + currNeighbors[i]);
        }

        if (!this.visited.Contains(currNode))
        {
            this.visited.Add(currNode);
            this.unvisited.Remove(currNode);
        }
    }
    
    void Dijkstras()
    {
        Debug.Log("before");
        Debug.Log("Unvisited: " + unvisited.Count);
        Debug.Log("Visited: " + visited.Count);
        Debug.Log("Curr Node: " + currNode);

        while (unvisited.Contains(currNode)) {
            DijkstrasStep(currNode);
            float minDist = float.MaxValue;
            for (int i = 0; i < unvisited.Count; i++)
            {
                if (createdNodes[unvisited[i]].gameObject.GetComponent<NodeBehavior>().DistFromSource < minDist)
                {
                    currNode = unvisited[i];
                    minDist = createdNodes[unvisited[i]].gameObject.GetComponent<NodeBehavior>().DistFromSource;
                }
            }
        } 
        Debug.Log("No more to explore");
        

        //Debug.Log("After");
        //Debug.Log("Unvisited: " + unvisited.Count);
        //Debug.Log("Visited: " + visited.Count);
        //Debug.Log("Curr Node: " + currNode);



    }

    // Start is called before the first frame update
    void Start()
    {
        global_ID_count = 0;

        Vector2 size = new Vector2(bbMax.x - bbMin.x, bbMax.y - bbMin.y);

        float cellWidth = this.radius / Mathf.Sqrt(2);

        totalCells = new Vector3Int((int)(size.x / cellWidth), (int)(size.y / cellWidth), 1);

        grid = new List<List<List<int>>>();

        List<List<int>> currX = new List<List<int>>();
        for (int i = 0; i <= totalCells.x; i++) 
        {
            List<int> currY = new List<int>();
            for (int j = 0; j <= totalCells.y; j++)
            {
                currY.Add(-1);
            }
            currX.Add(currY);
        }

        grid.Add(currX);

        List<Vector2> newPositions = PoissonScatterer.GeneratePoints(radius, size);
        
        for (int i = 0; i < newPositions.Count; i++)
        {
            SpawnNode();
            Vector2 pos = newPositions[i];
            createdNodes[createdNodes.Count - 1].transform.position = new Vector3(pos.x, pos.y, 0f);
            grid[0][(int)(pos.x / cellWidth)][(int)(pos.y / cellWidth)] = i;
            //Debug.Log(global_ID_count - 1);
        }
    }

    void SpawnNode()
    {
        createdNodes.Add(Instantiate(node));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            this.currNode = this.sourceIdx;
            createdNodes[currNode].gameObject.GetComponent<NodeBehavior>().GuidingVector = new Vector3(0, 1, 0);

            this.unvisited.Add(this.sourceIdx);
            Dijkstras();
        }
    }
}
