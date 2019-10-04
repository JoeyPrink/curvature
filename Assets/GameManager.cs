﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour 
{
    public event Action OnConnectionAdded = delegate { };
    public event Action OnConnectionRemoved = delegate { };
    public event Action<int> OnNumConnectedChanged = delegate { };

    private int numConnectedLines = 0;

    private GameState gameState = GameState.Playing;

    private string[] levelNames =
    {
        "01-Level01RE",
        "02-Level02RE 1",
        "03-Level03RE"
    };

    public enum GameState
    {
        Playing,
        Won
    }

    void Update()
    {
        switch (gameState)
        {
            case GameState.Playing:
                List<Attractor> attractors;
                List<Polyline> lines;
                List<Obstacle> obstacles;
                attractors = FindObjectsOfType<Attractor>().ToList();
                lines = FindObjectsOfType<Polyline>().ToList();
                obstacles = FindObjectsOfType<Obstacle>().ToList();
                for (int i = 0; i < lines.Count; i++)
                {
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
                var end = lines.FirstOrDefault(l => l.Type == PolylineType.End);
                if (start == null)
                    Debug.LogWarning("No starting PolyLine defined!");
                if (end == null)
                    Debug.LogWarning("No end PolyLine defined!");

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
                foreach (var c in connected)
                {
                    if (!c.Connected)
                        OnConnectionAdded();
                    c.Connected = true;
                }

                // check win condition and spawn level complete overlay
                if (end != null && end.Connected)
                {
                    gameState = GameState.Won;

                    // force disable grab input
                    var im = transform.Find("/GameManager").GetComponent<InputManager>();
                    im.Disable();

                    // load and display level complete overlay
                    var p = Resources.Load<GameObject>("Prefabs/LevelCompleteOverlay");
                    var levelCompleteOverlay = GameObject.Instantiate(p);
                    levelCompleteOverlay.GetComponent<Canvas>().worldCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
                    levelCompleteOverlay.GetComponent<Canvas>().planeDistance = 5; // HACK: manually position ui plane between regular game objects and camera
                    var continueButton = levelCompleteOverlay.transform.FindDeepComponent<Button>("ContinueButton");
                    continueButton.onClick.AddListener(() => {
                        LoadNextLevel();
                    });
                }
                break;
            case GameState.Won:
                break;
        }
    }

    private void LoadNextLevel()
    {
        var currentLevelIndex = Array.IndexOf(levelNames, SceneManager.GetActiveScene().name);
        if (currentLevelIndex != -1)
        {
            var nextLevelIndex = currentLevelIndex + 1;
            if (nextLevelIndex >= levelNames.Length)
            {
                // TODO: last level, what to do?
                Debug.Log("This was the last level! Congratulations, I guess? ;)");
            }
            else
            {
                SceneManager.LoadScene(levelNames[nextLevelIndex]);
            }
        } else
        {
            Debug.LogWarning("Cannot load next scene: current scene not found in list of levels!");
        }
    }
}
