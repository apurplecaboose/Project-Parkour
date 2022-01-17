using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class DropPainter : MonoBehaviour {

    public bool genData = false;
    public MeshFilter target = null;
    public Mesh data = null;

    List<Vector3> points = new List<Vector3>();
    List<int> indices = new List<int>();

    private void OnRenderObject() {
        if (target == null) {
            target = this.GetComponent<MeshFilter>();
        }
        if (data == null) {
            data = new Mesh();
            data.name = "data";
            data.bounds = new Bounds(Vector3.zero, Vector3.one * 1000.0f);
        }

        if (genData) {
            indices.Clear();
            points.Clear();

            for (int i = 0; i < 100; i++) {
                indices.Add(i);
                points.Add(
                    new Vector3(
                        Random.Range(0.0f, 1.0f), 
                        Random.Range(0.0f, 1.0f), 
                        0.0f
                    )
                );
            }
            
            data.SetVertices(points);
            data.SetIndices(indices, MeshTopology.Points, 0);

            target.sharedMesh = data;
        }
    }

}
