using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour {
    
    void Start()
    {
    }

    void Update()
    {
        List<Attractor> attractors;
        List<Polyline> lines;
        attractors = FindObjectsOfType<Attractor>().ToList();
        lines = FindObjectsOfType<Polyline>().ToList();
        for (int i = 0; i < lines.Count; i++) {
            lines[i].Deform(attractors);
        }

        // calculate connections
        var start = lines.FirstOrDefault(l => l.Type == PolylineType.Start);
        if (start == null)
            Debug.LogWarning("No starting PolyLine defined!");
        var connected = PolylineService.CalculateConnected(start, lines); // TODO: do not calculate each frame

        // mark connected lines
        foreach (var l in lines) l.Connected = false;
        foreach(var c in connected)
        {
            c.Connected = true;
        }
    }
}
