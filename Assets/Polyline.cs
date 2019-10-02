using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEditor;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;


public struct Vertex {
    public Vector3 restPos;
    public Vector3 currentPos;

    public Vertex(Vector3 pos) {
        this.restPos = this.currentPos = pos;
    }
}


public class Polyline : MonoBehaviour {

//    public List<Transform> vertTransforms = new List<Transform>();
    private List<Vertex> verts = new List<Vertex>();

    [SerializeField]
    private int subdivisions = 0;

    void Start() {
        Create();
    }

    void Update()
    {
        
    }

    public void Deform(List<Attractor> attractors) {
        for (int i = 0; i < verts.Count; i++) {
            Vertex vert = verts[i];
            vert.currentPos = vert.restPos;
            foreach (Attractor attractor in attractors) {
                Vector3 dir = attractor.GetAttractDir(vert.restPos);
                vert.currentPos += dir;
            }

            verts[i] = vert;
        }
    }

    private void Create() {
        verts.Clear();
        Vector3 start, end;
        end = transform.GetChild(0).position;
        
        for (int i = 1; i < transform.childCount; i++) {
            start = end;
            end = transform.GetChild(i).position;
            verts.Add(new Vertex(start));
            for (int j = 1; j <= subdivisions; j++) {
                Vector3 cPos = start+(end - start) * j / (float) subdivisions;
                verts.Add(new Vertex(cPos));
            }
        }
        verts.Add(new Vertex(end));
    }
    

    private void OnDrawGizmos() {
        if (!Application.isPlaying) {
            Create();
        }
        
        Color col = Gizmos.color;
        Gizmos.color = Color.gray;
        for (int i = 1; i < verts.Count; i++) {
            Vector3 segmentStart = verts[i-1].restPos;
            Vector3 segmentEnd = verts[i].restPos;
            Gizmos.DrawLine(segmentStart, segmentEnd);
        }    
        Gizmos.color = Color.cyan;

        for (int i = 1; i < verts.Count; i++) {
            Vector3 segmentStart = verts[i-1].currentPos;
            Vector3 segmentEnd = verts[i].currentPos;
            Gizmos.DrawLine(segmentStart, segmentEnd);
        }    

        Gizmos.color = col;
    }
}
