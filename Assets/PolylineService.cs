using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

static class PolylineService
{
    public static IEnumerable<Polyline> CalculatedBlocked(List<Polyline> lines, IEnumerable<Obstacle> obstacles)
    {
        var blocked = new List<Polyline>();

        foreach(var line in lines)
        {
            foreach (var obstacle in obstacles)
                if (IsIntersecting(line, obstacle))
                    blocked.Add(line);
        }
        return blocked;
    }

    public static IEnumerable<Polyline> CalculateConnected(Polyline start, IEnumerable<Polyline> lines)
    {
        var queue = new Queue<Polyline>();
        queue.Enqueue(start);

        var connected = new List<Polyline>();
        connected.Add(start);

        var yetToCheck = new List<Polyline>(lines);
        yetToCheck.Remove(start);

        while(queue.Count > 0)
        {
            var current = queue.Dequeue();

            for (int i = 0;i < yetToCheck.Count;i++)
            {
                var toCheck = yetToCheck[i];
                if (IsIntersecting(current, toCheck))
                {
                    connected.Add(toCheck);
                    yetToCheck.RemoveAt(i);

                    queue.Enqueue(toCheck);

                    i--; // because we removed an element from the collection we're iterating
                }
            }
        }

        return connected;
    }

    // polyline 2 polyline intersetion test
    // TODO: brute force implementation, potentially slow, consider improving
    public static bool IsIntersecting(Polyline a, Polyline b)
    {
        var vertsA = a.Verts;
        var vertsB = b.Verts;
        for (var i = 0;i < vertsA.Count - 1;i++)
        {
            for (var j = 0; j < vertsB.Count - 1; j++)
            {

                if (IsIntersecting(vertsA[i].currentPos, vertsA[i + 1].currentPos, vertsB[j].currentPos, vertsB[j + 1].currentPos))
                    return true;
            }
        }
        return false;
    }

    // polyline 2 polyline intersection test
    public static bool IsIntersecting(Polyline p, Obstacle o)
    {
        var verts = p.Verts;
        for (var i = 0; i < verts.Count - 1; i++)
        {
            if (IsIntersecting(verts[i].currentPos, verts[i + 1].currentPos, o.transform.position, o.radius))
            {
                return true;
            }
        }
        return false;
    }

    // line segment 2 line segment intersection test
    // taken from https://gamedev.stackexchange.com/questions/26004/how-to-detect-2d-line-on-line-collision
    private static bool IsIntersecting(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
    {
        float denominator = ((b.x - a.x) * (d.y - c.y)) - ((b.y - a.y) * (d.x - c.x));
        if (denominator < 0.000001f) {
            return false;
        }
        
        
        float numerator1 = ((a.y - c.y) * (d.x - c.x)) - ((a.x - c.x) * (d.y - c.y));
        float numerator2 = ((a.y - c.y) * (b.x - a.x)) - ((a.x - c.x) * (b.y - a.y));

        // Detect coincident lines (has a problem, read https://gamedev.stackexchange.com/questions/26004/how-to-detect-2d-line-on-line-collision)
        if (denominator == 0) return numerator1 == 0 && numerator2 == 0;

        float r = numerator1 / denominator;
        float s = numerator2 / denominator;

        return (r >= 0 && r <= 1) && (s >= 0 && s <= 1);
    }

    // line segment 2 circle intersection test
    private static bool IsIntersecting(Vector2 aa, Vector2 bb, Vector2 circleCenter, float radius)
    {
        // TODO: refactor
        return lineCircle(aa.x, aa.y, bb.x, bb.y, circleCenter.x, circleCenter.y, radius);
    }

    // line segment 2 circle intersection test
    // taken from http://www.jeffreythompson.org/collision-detection/line-circle.php
    private static bool lineCircle(float x1, float y1, float x2, float y2, float cx, float cy, float r)
    {

        // is either end INSIDE the circle?
        // if so, return true immediately
        bool inside1 = pointCircle(x1, y1, cx, cy, r);
        bool inside2 = pointCircle(x2, y2, cx, cy, r);
        if (inside1 || inside2) return true;

        // get length of the line
        float distX = x1 - x2;
        float distY = y1 - y2;
        float len = Mathf.Sqrt((distX * distX) + (distY * distY));

        // get dot product of the line and circle
        float dot = (((cx - x1) * (x2 - x1)) + ((cy - y1) * (y2 - y1))) / Mathf.Pow(len, 2);

        // find the closest point on the line
        float closestX = x1 + (dot * (x2 - x1));
        float closestY = y1 + (dot * (y2 - y1));

        // is this point actually on the line segment?
        // if so keep going, but if not, return false
        bool onSegment = linePoint(x1, y1, x2, y2, closestX, closestY);
        if (!onSegment) return false;

        // optionally, draw a circle at the closest
        // point on the line
        //fill(255, 0, 0);
        //noStroke();
        //ellipse(closestX, closestY, 20, 20);

        // get distance to closest point
        distX = closestX - cx;
        distY = closestY - cy;
        float distance = Mathf.Sqrt((distX * distX) + (distY * distY));

        if (distance <= r)
        {
            return true;
        }
        return false;
    }


    // POINT/CIRCLE
    private static bool pointCircle(float px, float py, float cx, float cy, float r)
    {

        // get distance between the point and circle's center
        // using the Pythagorean Theorem
        float distX = px - cx;
        float distY = py - cy;
        float distance = Mathf.Sqrt((distX * distX) + (distY * distY));

        // if the distance is less than the circle's
        // radius the point is inside!
        if (distance <= r)
        {
            return true;
        }
        return false;
    }


    // LINE/POINT
    private static bool linePoint(float x1, float y1, float x2, float y2, float px, float py)
    {

        // get distance from the point to the two ends of the line
        float d1 = Vector2.Distance(new Vector2(px, py), new Vector2(x1, y1));
        float d2 = Vector2.Distance(new Vector2(px, py), new Vector2(x2, y2));

        // get the length of the line
        float lineLen = Vector2.Distance(new Vector2(x1, y1), new Vector2(x2, y2));

        // since floats are so minutely accurate, add
        // a little buffer zone that will give collision
        float buffer = 0.1f;    // higher # = less accurate

        // if the two distances are equal to the line's
        // length, the point is on the line!
        // note we use the buffer here to give a range,
        // rather than one #
        if (d1 + d2 >= lineLen - buffer && d1 + d2 <= lineLen + buffer)
        {
            return true;
        }
        return false;
    }
}