using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attractor : MonoBehaviour {
    public float range = 3;
    public float strength = 1;

    public Vector3 GetAttractDir(Vector3 restPos) {
        Vector3 result = transform.position-restPos;
        float dist = result.magnitude;
        result /= dist;
        float force = Mathf.Pow(1 - Mathf.Clamp01(dist / range), 2) * strength;
        force = Mathf.Clamp01(force);
        result *= force;
        return result;
    }

    private void OnDrawGizmos() {
        Color col = Gizmos.color;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
        Gizmos.color = col;
    }
}
