using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttractorRail : MonoBehaviour
{
    private Transform start;
    private Transform end;
    private Transform attractor;
    private bool isSetup = false;
    [SerializeField]
    private LineRenderer railRenderer;

    // Start is called before the first frame update
    void Start()
    {
        Setup();
    }

    private void Setup()
    {
        if (!isSetup)
        {
            start = transform.Find("Start");
            end = transform.Find("End");
            attractor = transform.Find("Attractor");
            isSetup = true;
            railRenderer.SetPositions(new Vector3[] {start.localPosition, end.localPosition});
        }
    }

    // Update is called once per frame
    void Update()
    {
        // constrain attractor to line segment
        FindDistanceToSegment(attractor.position, start.position, end.position, out var closestPointOnLine);

        attractor.position = closestPointOnLine;
    }

    public void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            Setup();
        }

        Color col = Gizmos.color;

        Gizmos.color = Color.gray;
        Gizmos.DrawLine(start.position, end.position);

        Gizmos.color = col;
    }


    // taken from http://csharphelper.com/blog/2016/09/find-the-shortest-distance-between-a-point-and-a-line-segment-in-c/
    // Calculate the distance between
    // point pt and the segment p1 --> p2.
    private double FindDistanceToSegment(
        Vector2 pt, Vector2 p1, Vector2 p2, out Vector2 closest)
    {
        float dx = p2.x - p1.x;
        float dy = p2.y - p1.y;
        if ((dx == 0) && (dy == 0))
        {
            // It's a point not a line segment.
            closest = p1;
            dx = pt.x - p1.x;
            dy = pt.y - p1.y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        // Calculate the t that minimizes the distance.
        float t = ((pt.x - p1.x) * dx + (pt.y - p1.y) * dy) /
            (dx * dx + dy * dy);

        // See if this represents one of the segment's
        // end points or a point in the middle.
        if (t < 0)
        {
            closest = new Vector2(p1.x, p1.y);
            dx = pt.x - p1.x;
            dy = pt.y - p1.y;
        }
        else if (t > 1)
        {
            closest = new Vector2(p2.x, p2.y);
            dx = pt.x - p2.x;
            dy = pt.y - p2.y;
        }
        else
        {
            closest = new Vector2(p1.x + t * dx, p1.y + t * dy);
            dx = pt.x - closest.x;
            dy = pt.y - closest.y;
        }

        return Math.Sqrt(dx * dx + dy * dy);
    }
}
