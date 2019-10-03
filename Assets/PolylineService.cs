using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

static class PolylineService
{
    public static IEnumerable<Polyline> CalculateConnected(Polyline start, IEnumerable<Polyline> polylines)
    {
        var queue = new Queue<Polyline>();
        queue.Enqueue(start);

        var connected = new List<Polyline>();
        connected.Add(start);

        var yetToCheck = new List<Polyline>(polylines);
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
    // TODO: brute force implementation, slow, consider improving
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

    // TODO: has a problem when start and end points are the same
    // taken from https://gamedev.stackexchange.com/questions/26004/how-to-detect-2d-line-on-line-collision
    private static bool IsIntersecting(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
    {
        // HACK: work around bug that makes lines with equal start and end point report as intersecting with each other
        if (a == b) return false;
        if (c == d) return false;

        float denominator = ((b.x - a.x) * (d.y - c.y)) - ((b.y - a.y) * (d.x - c.x));
        float numerator1 = ((a.y - c.y) * (d.x - c.x)) - ((a.x - c.x) * (d.y - c.y));
        float numerator2 = ((a.y - c.y) * (b.x - a.x)) - ((a.x - c.x) * (b.y - a.y));

        // Detect coincident lines (has a problem, read https://gamedev.stackexchange.com/questions/26004/how-to-detect-2d-line-on-line-collision)
        if (denominator == 0) return numerator1 == 0 && numerator2 == 0;

        float r = numerator1 / denominator;
        float s = numerator2 / denominator;

        return (r >= 0 && r <= 1) && (s >= 0 && s <= 1);
    }
}