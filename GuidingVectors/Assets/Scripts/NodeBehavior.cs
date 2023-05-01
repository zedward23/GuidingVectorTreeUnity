using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeBehavior : MonoBehaviour
{
    [SerializeField]
    private int ID;

    public bool isEndPt;

    [SerializeField]
    public int ParentID;

    [SerializeField]
    public float DistFromSource;

    [SerializeField]
    private List<Vector3> OutGoingEdges;

    [SerializeField]
    public List<int> OutGoingEdgesIDs;

    [SerializeField]
    public Vector3 GuidingVector;
    private TreeBehavior tree;

    private GameObject boundary;

    [SerializeField]
    private List<Material> materials;

    [SerializeField]
    private Vector3Int voxelID;

    public int getID()
    {
        return this.ID;
    }

    // Start is called before the first frame update
    void Start()
    {
        this.isEndPt = false;
        this.ParentID = -1;
        this.DistFromSource = float.MaxValue;
        
        tree = GameObject.FindGameObjectWithTag("Tree").GetComponent<TreeBehavior>();
        this.ID = tree.global_ID_count;
        tree.global_ID_count += 1;

        boundary = gameObject.transform.GetChild(0).gameObject;
        boundary.transform.localScale = new Vector3(2 * tree.radius / gameObject.transform.localScale.x,
                                                    2 * tree.radius / gameObject.transform.localScale.y,
                                                    2 * tree.radius / gameObject.transform.localScale.z);
        gameObject.GetComponent<SphereCollider>().radius = 2 * tree.radius / gameObject.transform.localScale.x;


        float cellWidth = tree.radius / Mathf.Sqrt(2);
        voxelID = new Vector3Int((int)(gameObject.transform.position.x / cellWidth), (int)(gameObject.transform.position.y / cellWidth), 0);
        gameObject.transform.name = "Node (" + voxelID.x + "," + voxelID.y + ")" + " ID:" + this.ID;
    }

    (float, int) checkSector(int x_0, int y_0, float minDist)
    {
        if ((x_0 < tree.totalCells.x && x_0 >= 0) && (y_0 < tree.totalCells.y && y_0 >= 0))
        {
            if (tree.grid[0][x_0][y_0] == -1)
            {
                return (-1f, -1);
            }
            if (Vector3.Distance(tree.createdNodes[tree.grid[0][x_0][y_0]].transform.position, gameObject.transform.position) < minDist)
            {
                float dist = Vector3.Distance(tree.createdNodes[tree.grid[0][x_0][y_0]].transform.position, gameObject.transform.position);
                return (dist, tree.grid[0][x_0][y_0]);
            }
            else
            {
                return (minDist, -1);
            }
        }
        else
        {
            return (-1f, -1);
        }

    }

    void hideNeighbors()
    {
        for (int i = 0; i < this.OutGoingEdgesIDs.Count; i++)
        {
            gameObject.transform.GetChild(i + 1).gameObject.SetActive(!gameObject.transform.GetChild(i + 1).gameObject.activeSelf);

        }
    }
    
    void showNeighbors()
    {
        Vector3 src = gameObject.transform.position;
        for (int i = 0; i < this.OutGoingEdgesIDs.Count; i++)
        {
            //tree.createdNodes[OutGoingEdgesIDs[i]].gameObject.GetComponent<Renderer>().material = materials[2];
            Vector3 tgt = tree.createdNodes[OutGoingEdgesIDs[i]].gameObject.transform.position;
            gameObject.transform.GetChild(i + 1).gameObject.GetComponent<ArrowGenerator>().generateArrow(src, tgt, gameObject.transform.localScale.x);

        }

    }

    void tracePath()
    {
        Debug.Log(gameObject.transform.name + " is Tracing");

        if (this.isEndPt)
        {
            int parentId = this.ParentID;
            while (parentId != -1)
            {
                tree.createdNodes[parentId].gameObject.GetComponent<NodeBehavior>().isEndPt = true;
                parentId = tree.createdNodes[parentId].gameObject.GetComponent<NodeBehavior>().ParentID;
            }
        }
    }

    void showParent()
    {
        Vector3 src = gameObject.transform.position;
        if (this.ParentID != -1)
        {
            
            Vector3 tgt = tree.createdNodes[this.ParentID].gameObject.transform.position;

            Debug.Log("tgt "+tgt);
            Debug.Log("src " + src);

            gameObject.transform.GetChild(5).gameObject.SetActive(true);
            gameObject.transform.GetChild(5).gameObject.GetComponent<ArrowGenerator>().generateArrow(src, tgt, gameObject.transform.localScale.x);
        }
        if (this.ID == tree.sourceIdx)
        {
            gameObject.GetComponent<Renderer>().material = materials[1];
        }

    }

    void showPaths()
    {
        if (!this.isEndPt)
        {
            gameObject.transform.GetChild(5).gameObject.SetActive(false);
        }

    }


    void findNeighbors()
    {
        Debug.Log(gameObject.transform.name);
        int x = this.voxelID.x;
        int y = this.voxelID.y;
        int x_0;
        int y_0;

        //++ Sector
        {

            float minDistpp = float.MaxValue;
            float minDistpm = float.MaxValue;
            float minDistmp = float.MaxValue;
            float minDistmm = float.MaxValue;
            int neighborIdxpp = -1;
            int neighborIdxpm = -1;
            int neighborIdxmp = -1;
            int neighborIdxmm = -1;
            for (int xdelt = -3; xdelt <= 3; xdelt++)
            {
                for (int ydelt = -2; ydelt <= 2; ydelt++)
                {
                    x_0 = x + xdelt;
                    y_0 = y + ydelt;

                    (float, int) output;

                    if (xdelt == 0 && ydelt == 0)
                    {

                    }
                    else if (xdelt <= 0 && ydelt <= 0)
                    {
                        output = checkSector(x_0, y_0, minDistmm);
                        if (output.Item1 != -1)
                        {
                            if (output.Item1 < minDistmm)
                            {
                                minDistmm = output.Item1;
                                neighborIdxmm = output.Item2;
                            }
                        }
                    }
                    else if (xdelt > 0 && ydelt <= 0)
                    {
                        output = checkSector(x_0, y_0, minDistpm);
                        if (output.Item1 != -1)
                        {
                            if (output.Item1 < minDistpm)
                            {
                                minDistpm = output.Item1;
                                neighborIdxpm = output.Item2;
                            }
                        }
                    }
                    else if (xdelt <= 0 && ydelt > 0)
                    {
                        output = checkSector(x_0, y_0, minDistmp);
                        if (output.Item1 != -1)
                        {
                            if (output.Item1 < minDistmp)
                            {
                                minDistmp = output.Item1;
                                neighborIdxmp = output.Item2;
                            }
                        }
                    }
                    else if (xdelt > 0 && ydelt > 0)
                    {
                        output = checkSector(x_0, y_0, minDistpp);
                        if (output.Item1 != -1)
                        {
                            if (output.Item1 < minDistpp)
                            {
                                minDistpp = output.Item1;
                                neighborIdxpp = output.Item2;
                            }
                        }
                    }
                }
            }

            if (neighborIdxmm != -1)
            {
                this.OutGoingEdgesIDs.Add(neighborIdxmm);
            }

            if (neighborIdxpp != -1)
            {
                this.OutGoingEdgesIDs.Add(neighborIdxpp);
            }

            if (neighborIdxpm != -1)
            {
                this.OutGoingEdgesIDs.Add(neighborIdxpm);
            }

            if (neighborIdxmp != -1)
            {
                this.OutGoingEdgesIDs.Add(neighborIdxmp);
            }
        }
    }


    public void lightUp()
    {
        if (this.ParentID == -1)
        {
            gameObject.GetComponent<Renderer>().material = materials[3];
        } else
        {
            gameObject.GetComponent<Renderer>().material = materials[0];
        }
        
    }

    void initGraph()
    {
        this.findNeighbors();
        showNeighbors();
        hideNeighbors();
    }

    // Update is called once per frame
    void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                boundary.SetActive(!boundary.activeSelf);
            }
    
            
            if (Input.GetKeyDown(KeyCode.Return))
            {
                initGraph();
            }

            if (Input.GetKeyDown(KeyCode.P)){
                showParent();
            }

            if (Input.GetKeyDown(KeyCode.Q)){
                showPaths();
            }
            
            if (Input.GetKeyDown(KeyCode.H)){
                hideNeighbors();
                //lightUp();
            }

            if (Input.GetKeyDown(KeyCode.T)){
                tracePath();
            }

            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.name.Contains("Node"))
                    {
                        if (hit.transform.name.Contains("ID:" + this.ID))
                        {
                            if (Input.GetKey(KeyCode.LeftShift))
                            {
                                this.isEndPt = true;
                                Debug.Log(gameObject.transform.name + " is now an end point");
                            }else
                            {
                                gameObject.GetComponent<Renderer>().material = materials[1];
                                this.DistFromSource = 0;
                            }
                            Debug.Log("Hit Node " + this.ID);
                            tree.sourceIdx = this.ID;
                            
                        
                        }
                        else
                        {
                            gameObject.GetComponent<Renderer>().material = materials[0];
                            this.DistFromSource = float.MaxValue;
                        }
    
    
                    }
                }
            }
        }
    }