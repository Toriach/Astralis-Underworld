using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter),typeof(MeshRenderer))]
public class MeshGenTest : MonoBehaviour
{
    Mesh mesh;
    public Material material;
    public Vector3[] vertices;
    public int[] triangles;

    private void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer>().sharedMaterial = material;

        CreateShape();
        UpdateMesh();
    }

    private void CreateShape()
    {
        vertices = new Vector3[10];

        vertices[0] = new Vector3(0,0,0);
        vertices[1] = new Vector3(0,0,1);
        vertices[2] = new Vector3(1,0,0);
        vertices[3] = new Vector3(1,0,1);
        vertices[4] = new Vector3(1,-1,1);
        vertices[5] = new Vector3(1,-1,0);
        vertices[6] = new Vector3(0,-1,0);
        vertices[7] = new Vector3(0,-1,1);

        triangles = new int[30];
        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;

        triangles[3] = 1;
        triangles[4] = 3;
        triangles[5] = 2;

        triangles[6] = 3;
        triangles[7] = 4;
        triangles[8] = 2;

        triangles[9] = 4;
        triangles[10] = 5;
        triangles[11] = 2;

        triangles[12] = 0;
        triangles[13] = 2;
        triangles[14] = 6;

        triangles[15] = 6;
        triangles[16] = 2;
        triangles[17] = 5;

        triangles[18] = 6;
        triangles[19] = 1;
        triangles[20] = 0;

        triangles[21] = 6;
        triangles[22] = 7;
        triangles[23] = 1;

        triangles[24] = 1;
        triangles[25] = 7;
        triangles[26] = 3;

        triangles[27] = 3;
        triangles[28] = 7;
        triangles[29] = 4;
    }

    private void UpdateMesh()
    {
        mesh.Clear();
        
        mesh.vertices = vertices;
        mesh.triangles = triangles; 

        mesh.RecalculateNormals();
    }

    private void Update()
    {
        UpdateMesh();
    }
}
