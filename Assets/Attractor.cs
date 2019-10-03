﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attractor : MonoBehaviour {
    public float minRange = 2;
    public float maxRange = 4;
    public float strength = 1;

    public AnimationCurve forceCurve;

    public bool repulsor = false;

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

    private void OnDrawGizmos() {
        Color col = Gizmos.color;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, minRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxRange);
        Gizmos.color = col;
    }
}
