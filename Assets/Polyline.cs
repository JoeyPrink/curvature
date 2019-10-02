﻿using System;
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

    /*
    [SerializeField]
    private Transform critter;
    private float critterPos = 0.5f;
    private float critterDir = 1;
    [SerializeField]
    private float critterSpeed = 1;
    */
    

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

    public void Deform(List<Attractor> attractors) {
        for (int i = 1; i < verts.Count - 1; i++) {
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
        for (int i = 1; i < verts.Count; i++) {
            Vector3 segmentStart, segmentEnd;
            Gizmos.color = Color.black;
            Gizmos.DrawLine(verts[i-1].restPos, verts[i-1].currentPos);

            
            Gizmos.color = Color.gray;
            segmentStart = verts[i-1].restPos;
            segmentEnd = verts[i].restPos;
            Gizmos.DrawLine(segmentStart, segmentEnd);

            Gizmos.color = Color.cyan;
            segmentStart = verts[i-1].currentPos;
            segmentEnd = verts[i].currentPos;
            Gizmos.DrawLine(segmentStart, segmentEnd);
        }    

        Gizmos.color = col;
    }
}
