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

public enum PolylineType
{
    Start,
    End,
    Intermediate
}


public class Polyline : MonoBehaviour {

    private readonly List<Vertex> verts = new List<Vertex>();

    public bool Connected = false;

    public PolylineType Type = PolylineType.Intermediate;

    [SerializeField]
    private int subdivisions = 0;

    /*
    [SerializeField]
    private Transform critter;
    private float critterPos = 0.5f;
    private float critterDir = 1;
    [SerializeField]
    private float critterSpeed = 1;
    */

    public List<Vertex> Verts => verts;
    

    void Start() {
        Create();
    }

    void Update() {
        /*
        critterPos += critterDir*Time.deltaTime;
        critterPos = Mathf.Clamp01(critterPos);
        if (critterPos == 0 || critterPos == 1) {
            critterDir *= -1;
        }

        float totalDist = 0;
        for (int i = 1; i < verts.Count; i++) {
            float cDist = (verts[i].currentPos - verts[i - 1].currentPos).magnitude;
            totalDist += cDist;
        }

        float targetDist = totalDist * critterPos;
        totalDist = 0;
        for (int i = 1; i < verts.Count; i++) {
            float cDist = (verts[i].currentPos - verts[i - 1].currentPos).magnitude;
            if (totalDist + cDist >= targetDist) {
                float t = (targetDist - totalDist)/cDist;
                Vector3 result = t * (verts[i].currentPos - verts[i - 1].currentPos) + verts[i - 1].currentPos;
                critter.position = result;
                break;
            }
            totalDist += cDist;
        }
        */
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
        /*
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
        */

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
        for (int i = 1; i < verts.Count; i++) {
            Vector3 segmentStart, segmentEnd;
            Gizmos.color = Color.black;
            Gizmos.DrawLine(verts[i-1].restPos, verts[i-1].currentPos);

            
            Gizmos.color = Color.gray;
            segmentStart = verts[i-1].restPos;
            segmentEnd = verts[i].restPos;
            Gizmos.DrawLine(segmentStart, segmentEnd);

            Gizmos.color = (Connected) ? Color.green : Color.cyan;
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
            Gizmos.color = (Connected) ? Color.green : Color.cyan;
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
