using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attractor : MonoBehaviour {
    public float minRange = 2;
    public float maxRange = 4;
    public float strength = 1;

    public bool repulsor = false;

    public Vector3 GetAttractDir(Vector3 initialPos) {
        Vector3 result = transform.position - initialPos;
        float dist = result.magnitude;
        float force = 1 - Mathf.Pow(Mathf.Clamp01((dist - minRange) / (maxRange - minRange)), 2);
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
