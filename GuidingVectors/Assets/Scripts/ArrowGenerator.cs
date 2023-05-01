using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowGenerator : MonoBehaviour
{
    Mesh mesh;

    [SerializeField]
    private float stemLength;

    public List<Vector3> verticesList;
    public List<int> trianglesList;

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        this.GetComponent<MeshFilter>().mesh = mesh;
        stemLength = .5f;
    }

    public void generateArrow(Vector3 src, Vector3 tgt, float scale)
    {

        Debug.Log("Ran");
        verticesList = new List<Vector3>();
        trianglesList = new List<int>();

        Vector3 dir = tgt - src;

        Vector3 norm = Vector3.Normalize(Vector3.Cross(dir, new Vector3(0, 0, 1)));

        verticesList.Add(stemLength * norm);
        verticesList.Add(dir/ scale);
        verticesList.Add(-stemLength * norm);

        trianglesList.Add(2);
        trianglesList.Add(1);
        trianglesList.Add(0);

        mesh.vertices = verticesList.ToArray();
        mesh.triangles = trianglesList.ToArray();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
