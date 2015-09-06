using UnityEngine;
using System.Collections;


public class ReadNavMesh : MonoBehaviour {

	public Mesh mesh;
	public Graph graph;

	// Use this for initialization
	void Start () {
		this.graph = new Graph(mesh);
	}
	
	// Update is called once per frameaa
	void Update () {
        this.graph.print();
	}

}

public class Graph{
	private bool[][] connections;
	private float[][] weight;
	private int length;
	private Mesh mesh;
    public float max;


	/*
	 * Constructor
	 * Receive a mesh and create the matrix to save connections and weights
	 * 
	 * As default, if there is no connections, the weights value is setted to -1 and connections to FALSE
	 */
	public Graph(Mesh mesh){
        this.max = 0;
		this.mesh = mesh;
		this.length = mesh.vertices.Length;
		this.connections = new bool[this.length][];
		this.weight = new float[this.length][];

		for(int i=0; i<this.length; ++i){
			this.connections[i] = new bool[this.length];
			this.weight[i] = new float[this.length];
		}
		clean ();

		addConnections ();
	}

	private void clean(){
		for (int i=0; i<this.length; i++)
		for (int j=0; j<this.length; j++) {
				connections [i] [j] = false;
				weight[i][j] = -1;
			}
	}

	/*
	 * Reads the mesh's triangles, each 3 mean the vertices, so, they have connections among them
	 */
	public void addConnections(){
        int[] triangle = mesh.triangles;
		for (int i=0; i<triangle.Length; i+=3) {
            
            
            Debug.DrawLine(mesh.vertices[(int)triangle[i + 1]], mesh.vertices[(int)triangle[i+2]], Color.red);
            Debug.DrawLine(mesh.vertices[(int)triangle[i]], mesh.vertices[(int)triangle[i+2]], Color.red);
            
            addConnection((int)triangle[i], (int)triangle[i+1]);
			addConnection((int)triangle[i+1], (int)triangle[i+2]);
			addConnection((int)triangle[i], (int)triangle[i+2]);
		}

	}

	private void addConnection(int v1Index, int v2Index){
		connections[v1Index][v2Index] = true;
		connections[v2Index][v1Index] = true;

		float x, y, z;
		Vector3 v1 = (Vector3) mesh.vertices[v1Index];
		Vector3 v2 = (Vector3) mesh.vertices[v2Index];
		x = System.Math.Abs(v1.x - v2.x);
		y = System.Math.Abs(v1.y - v2.y);
		z = System.Math.Abs(v1.z - v2.z);

		float res = (float) System.Math.Sqrt((System.Math.Pow(x,2) + System.Math.Pow(y,2) + System.Math.Pow(z,2)));

		weight[v1Index][v2Index] = res;
		weight[v2Index][v1Index] = res;

        if (res > max)
            max = res;
        
	}

    public void print()    {
        for (int i = 0; i < mesh.triangles.Length; i += 3)
        {

            Debug.DrawLine(mesh.vertices[(int)mesh.triangles[i]], mesh.vertices[(int)mesh.triangles[i + 1]], Color.Lerp(Color.red, Color.black, weight[(int)mesh.triangles[i]][(int)mesh.triangles[i+1]] / max));
            Debug.DrawLine(mesh.vertices[(int)mesh.triangles[i+1]], mesh.vertices[(int)mesh.triangles[i + 2]], Color.Lerp(Color.red, Color.black, weight[(int)mesh.triangles[i+1]][(int)mesh.triangles[i+2]] / max));
            Debug.DrawLine(mesh.vertices[(int)mesh.triangles[i]], mesh.vertices[(int)mesh.triangles[i + 2]], Color.Lerp(Color.red, Color.black, weight[(int)mesh.triangles[i]][(int)mesh.triangles[i+2]] / max));
        }
    }
}
