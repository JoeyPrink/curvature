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
    }
}
