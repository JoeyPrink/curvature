﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public struct Vertex {
    public Vector3 restPos;
    public Vector3 currentPos;

    public Vertex(Vector3 pos) {
        this.restPos = this.currentPos = pos;
    }
}

public enum PolylineType
{
    Start,
    End,
    Intermediate
}


public class Polyline : MonoBehaviour {

    private readonly List<Vertex> verts = new List<Vertex>();

    public bool Connected = false;

    public bool Blocked = false;

    public PolylineType Type = PolylineType.Intermediate;

    [SerializeField]
    private int subdivisions = 0;

    [SerializeField]
    private float lineWidth = 0.2f;

    
    public List<Vertex> Verts => verts;

    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private Mesh mesh;
    private LineRenderer lineRenderer;

    private int numVerts;
    private int numTris;
    
    [Header("Drawing order")]
    [SerializeField]
    private float lineZ = -0.5f;
    [SerializeField]
    private float areaZ = -0.0f;

    

    void Start() {
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
        mesh = new Mesh();
        Create();
        numVerts = verts.Count * 2;
        numTris = (verts.Count * 2 - 2)*3;
        mesh.vertices = new Vector3[numVerts];
        mesh.triangles = new int[numTris];
        mesh.uv = new Vector2[numVerts];
        mesh.colors = new Color[numVerts];
        mesh.normals = new Vector3[numVerts];
        mesh.MarkDynamic();
        meshFilter.mesh = mesh;

        lineRenderer = GetComponent<LineRenderer>();
        
    }

    void Update() {
        RebuildMesh();
    }

    private void RebuildMesh() {
        Vector3[] v = mesh.vertices;
        int[] t = mesh.triangles;
        Vector2[] uv = mesh.uv;
        Color[] col = mesh.colors;
        Vector3[] n = mesh.normals;

        for (int i = 0; i < verts.Count; i++) {
            v[i * 2] = transform.InverseTransformPoint(verts[i].restPos + Vector3.forward*areaZ);
            v[i * 2+1] = transform.InverseTransformPoint(verts[i].currentPos + Vector3.forward*areaZ);
            float currU = i / (float) (verts.Count - 1);
            uv[i*2] = new Vector2(currU, 0);
            uv[i*2+1] = new Vector2(currU, 1);
            n[i*2] = new Vector3(0, 0, -1);
            n[i*2+1] = new Vector3(0, 0, -1);
            col[i*2] = Color.gray;
            col[i*2+1] = Color.white;
        }

        int triIndex = 0;
        for (int i = 0; i < verts.Count - 1; i++) {
            int startIndex = i * 2;
            t[triIndex++] = startIndex;
            t[triIndex++] = startIndex+1;
            t[triIndex++] = startIndex+2;
            t[triIndex++] = startIndex+1;
            t[triIndex++] = startIndex+3;
            t[triIndex++] = startIndex+2;
        }

        mesh.vertices = v;
        mesh.triangles = t;
        mesh.uv = uv;
        mesh.colors = col;
        mesh.normals = n;
        
        Vector3[] positions = new Vector3[verts.Count];
        for (int i = 0; i < verts.Count; i++) {
            positions[i] = verts[i].currentPos + Vector3.forward*lineZ;
        }

        lineRenderer.positionCount = verts.Count;
        lineRenderer.SetPositions(positions);
        lineRenderer.widthMultiplier = lineWidth;
    }
    

    private int numIterations = 20;
    public void Deform(List<Attractor> attractors) {
        for (int i = 1; i < verts.Count - 1; i++) {
            Vertex vert = verts[i];
            vert.currentPos = vert.restPos;
            
            for (int j=0; j<numIterations; j++) {
                Vector3 currentPos = vert.currentPos;
                foreach (Attractor attractor in attractors) {
                    Vector3 dir = attractor.GetAttractDir(vert.currentPos, vert.restPos);

                    dir *= 1f / numIterations;
                    
                    currentPos += dir;
                }

                vert.currentPos = currentPos;
            }

            verts[i] = vert;
        }
    }

    private void Create() {
        verts.Clear();

        Transform start, end;
        end = transform.GetChild(0);
        for (int i = 1; i < transform.childCount; i++) {
            start = end;
            end = transform.GetChild(i);

            Vector3 p1 = start.position;
            Vector3 p2 = end.position;
            Vector3 h1, h2; // bezier handles
            if (start.childCount > 0) {
                h1 = start.GetChild(start.childCount-1).position; // out
            }
            else {
                h1 = start.position;
            }
            if (end.childCount > 0) {
                h2 = end.GetChild(0).position; // in
            }
            else {
                h2 = end.position;
            }

            for (int j = 0; j < subdivisions + 1; j++) {
                float t = j / (float) subdivisions;
                Vector3 pos = Mathf.Pow(1 - t, 3) * p1
                              + 3 * Mathf.Pow(1 - t, 2) * t * h1
                              + 3 * (1 - t) * Mathf.Pow(t, 2) * h2
                              + Mathf.Pow(t, 3) * p2;
                              
                verts.Add(new Vertex(pos));
            }
        }
    }
    

    private void OnDrawGizmos() {
        if (!Application.isPlaying) {
            Create();
        }

        DrawBezierHandles();
        
        Color col = Gizmos.color;

        var baseColor = Color.cyan;
        if (Blocked)
            baseColor = Color.Lerp(new Color(1, 0, 1), Color.black, 0.5f);
        else if (Connected)
            baseColor = Color.green;

        for (int i = 1; i < verts.Count; i++) {
            Vector3 segmentStart, segmentEnd;
            Gizmos.color = Color.black;
            Gizmos.DrawLine(verts[i-1].restPos, verts[i-1].currentPos);

            
            Gizmos.color = Color.gray;
            segmentStart = verts[i-1].restPos;
            segmentEnd = verts[i].restPos;
            Gizmos.DrawLine(segmentStart, segmentEnd);

            Gizmos.color = baseColor;
            segmentStart = verts[i-1].currentPos;
            segmentEnd = verts[i].currentPos;
            Gizmos.DrawLine(segmentStart, segmentEnd);
        }

        if (Type == PolylineType.Start)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawCube(verts[0].currentPos, new Vector3(0.3f, 0.3f, 0.3f));
        } else if (Type == PolylineType.End)
        {
            Gizmos.color = baseColor;
            Gizmos.DrawCube(verts[0].currentPos, new Vector3(0.3f, 0.3f, 0.3f));
        }

        Gizmos.color = col;
    }

    private void DrawBezierHandles() {
        Color col = Gizmos.color;

        // end, start / out, in
        Color[] handleColors = new Color[] {new Color(0.3f, 0.5f, 0.6f), new Color(0.4f, 0.8f, 1f)};
        foreach (Transform child in transform) {
            Vector3 centerPos = child.position;
            if (child.childCount != 0) {
                for (int i = 0; i < child.childCount; i++) {
                    Gizmos.color = handleColors[Mathf.Min(handleColors.Length - 1, i)];
                    Gizmos.DrawLine(centerPos, child.GetChild(i).position);
                }
            }
        }
        
        Gizmos.color = col;
    }
}
