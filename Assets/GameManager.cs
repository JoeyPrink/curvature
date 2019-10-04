using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour 
{
    public event Action OnConnectionAdded = delegate { };
    public event Action OnConnectionRemoved = delegate { };
    public event Action<int> OnNumConnectedChanged = delegate { };

    private int numConnectedLines = 0;

    void Start()
    {
    }

    void Update()
    {
        List<Attractor> attractors;
        List<Polyline> lines;
        List<Obstacle> obstacles;
        attractors = FindObjectsOfType<Attractor>().ToList();
        lines = FindObjectsOfType<Polyline>().ToList();
        obstacles = FindObjectsOfType<Obstacle>().ToList();
        for (int i = 0; i < lines.Count; i++) {
            lines[i].Deform(attractors);
        }

        // calculate blocked lines
        var blockedLines = PolylineService.CalculatedBlocked(lines, obstacles);

        // update blocked and unblocked lines
        foreach (var l in lines.Except(blockedLines))
        {
            l.Blocked = false;

        }
        foreach (var bl in blockedLines)
        {
            bl.Blocked = true;
        }

        // calculate connections, exclude blocked lines
        var start = lines.FirstOrDefault(l => l.Type == PolylineType.Start);
        if (start == null)
            Debug.LogWarning("No starting PolyLine defined!");

        IEnumerable<Polyline> connected = new List<Polyline>();
        if (!blockedLines.Contains(start)) // consider special case when start line is already blocked as well
            connected = PolylineService.CalculateConnected(start, lines.Except(blockedLines)); // TODO: do not calculate each frame

        var newNumConnectedLines = connected.Count();
        if (numConnectedLines != newNumConnectedLines)
        {
            OnNumConnectedChanged(newNumConnectedLines);
            numConnectedLines = newNumConnectedLines;
        }

        // update connected and unconnected lines, send events
        foreach (var l in lines.Except(connected))
        {
            if (l.Connected)
                OnConnectionRemoved();
            l.Connected = false;
        }
        foreach(var c in connected)
        {
            if (!c.Connected)
                OnConnectionAdded();
            c.Connected = true;
        }

    }
}
