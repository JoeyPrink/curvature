﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attractor : MonoBehaviour {
    public float minRange = 2;
    public float maxRange = 4;
    public float strength = 1;

    public AnimationCurve forceCurve;

    public bool repulsor = false;

    public MeshRenderer preview;

    void Start() {
        var im = transform.Find("/GameManager").GetComponent<InputManager>();
        im.OnGrabAttractor += OnGrab;
        im.OnReleaseAttractor += OnRelease;
    }

    private void OnGrab(GameObject go) {
        if (go == gameObject) {
            preview.material.SetFloat("_On", 1);
        }
    }
    private void OnRelease(GameObject go) {
        preview.material.SetFloat("_On", 0);
    }
    
    public Vector3 GetAttractDir(Vector3 currentPos, Vector3 restPos) {
        Vector3 result = transform.position - currentPos;
        float dist = (transform.position - restPos).magnitude;
        float force = 1 - Mathf.Clamp01((dist - minRange) / (maxRange - minRange));

        force = forceCurve.Evaluate(force);

        force *= strength;
        if (repulsor) {
            force *= -1;
        }

        result.Normalize();
        result *= force;
        return result;
    }

    private void Update() {
        preview.material.SetFloat("_MinRange", minRange);
        preview.material.SetFloat("_MaxRange", maxRange);
        preview.material.SetFloat("_Direction", (repulsor) ? 1 : -1);
    }

    private void OnDrawGizmos() {
        Color col = Gizmos.color;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, minRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxRange);
        Gizmos.color = col;
    }
}
