using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Obstacle : MonoBehaviour
{
    public float radius;

    [SerializeField]
    private MeshRenderer image;

    private void Start() {
        image.transform.localScale = Vector3.one * radius * 2;
    }
    
    private void OnDrawGizmos()
    {
        Color col = Gizmos.color;

        Gizmos.color = new Color(1,0,1);
        Gizmos.DrawWireSphere(transform.position, radius);

        Gizmos.color = col;
    }
}
