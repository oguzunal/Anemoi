﻿using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour {

    Mesh hexMesh;

    static List<Vector3> vertices = new List<Vector3>();
    static List<int> triangles = new List<int>();
    static List<Color> colors = new List<Color>();

    MeshCollider meshCollider;

    void Awake() {
        GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
        meshCollider = gameObject.AddComponent<MeshCollider>();
        hexMesh.name = "HexMesh";

        //vertices = new List<Vector3>();
        //triangles = new List<int>();
        //colors = new List<Color>();
    }

    public void Clear() {
        hexMesh.Clear();
        vertices.Clear();
        triangles.Clear();
        colors.Clear();
    }

    public void Apply() {
        hexMesh.SetVertices(vertices);
        hexMesh.SetTriangles(triangles, 0);
        hexMesh.SetColors(colors);
        hexMesh.RecalculateNormals();
        meshCollider.sharedMesh = hexMesh;
    }

    public void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3) {
        int vertexIndex = vertices.Count;

        vertices.Add(HexMetrics.Perturb(v1));
        vertices.Add(HexMetrics.Perturb(v2));
        vertices.Add(HexMetrics.Perturb(v3));

        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
    }

    public void AddTriangleUnperturbed(Vector3 v1, Vector3 v2, Vector3 v3) {
        int vertexIndex = vertices.Count;

        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);

        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
    }

    public void AddTriangleColor(Color color) {
        colors.Add(color);
        colors.Add(color);
        colors.Add(color);
    }

    public void AddTriangleColor(Color c1, Color c2, Color c3) {
        colors.Add(c1);
        colors.Add(c2);
        colors.Add(c3);
    }

    public void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4) {
        int vertexIndex = vertices.Count;

        vertices.Add(HexMetrics.Perturb(v1));
        vertices.Add(HexMetrics.Perturb(v2));
        vertices.Add(HexMetrics.Perturb(v3));
        vertices.Add(HexMetrics.Perturb(v4));

        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 3);
    }

    public void AddQuadColor(Color c1, Color c2, Color c3, Color c4) {
        colors.Add(c1);
        colors.Add(c2);
        colors.Add(c3);
        colors.Add(c4);
    }

    public void AddQuadColor(Color c1, Color c2) {
        colors.Add(c1);
        colors.Add(c1);
        colors.Add(c2);
        colors.Add(c2);
    }

    public void AddQuadColor(Color color) {
        colors.Add(color);
        colors.Add(color);
        colors.Add(color);
        colors.Add(color);
    }
}
