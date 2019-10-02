using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpringJoint2D))]
[RequireComponent(typeof(LineRenderer))]
public class Link : MonoBehaviour
{
    private Transform nodeA;
    private Transform nodeB;

    private LineRenderer lineRenderer;

    private void Start()
    {
        nodeA = transform.parent;
        nodeB = GetComponent<SpringJoint2D>().connectedBody.gameObject.transform;
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        var positions = new Vector3[]
        {
            nodeA.position,
            nodeB.position,
        };
        lineRenderer.SetPositions(positions);
    }
}
