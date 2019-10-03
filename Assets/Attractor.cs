using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attractor : MonoBehaviour {
    public float minRange = 2;
    public float maxRange = 4;
    public float strength = 1;

    public bool repulsor = true;

    public Vector3 GetAttractDir(Vector3 restPos) {
        if (repulsor)
        {
            Vector3 result = transform.position - restPos;
            float dist = result.magnitude;
            float force = 1 - Mathf.Pow(Mathf.Clamp01((dist - minRange) / (maxRange - minRange)), 2);
            force *= -strength;
            result.Normalize();
            result *= force;
            result = result.normalized * (Mathf.Log(result.magnitude + 1));
            return result;
        }
        else
        {
            Vector3 result = transform.position - restPos;
            float dist = result.magnitude;
            float force = 1 - Mathf.Clamp01(Mathf.Pow((dist - minRange) / (maxRange - minRange), 2));
            force *= strength;
            result *= force;
                
            result = result.normalized * (Mathf.Log(result.magnitude + 1));
            return result;
        }
    }

    private void OnDrawGizmos() {
        Color col = Gizmos.color;
        //Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(transform.position, minRange);
        //Gizmos.color = Color.yellow;
        //Gizmos.DrawWireSphere(transform.position, maxRange);
        Gizmos.color = col;
    }
}
