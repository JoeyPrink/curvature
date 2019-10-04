using System;
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

    public static string[] LevelNames =
    {
        "Level01",
        "Level02",
        "Level03",
        "Level04",
        "Level05",
        "Level06"
    };

    public enum GameState
    {
        Playing,
        Won
    };

    private int fpsCount = 0;
    private float fpsRunningTotal = 0;
    private float lastFps = 0;

    private float endConnectedCounter = 0;

    void OnGUI() {
//        GUI.Label(new Rect(100,100,200,100), $"FPS: {lastFps}");
    }

    void Update()
    {
        switch (gameState)
        {
            case GameState.Playing:
                    UpdateWhilePlaying();
                break;
            case GameState.Won:
                break;
	}

        fpsRunningTotal += Time.deltaTime;
        fpsCount++;
        if (fpsCount == 10) {
            lastFps = 1 / (fpsRunningTotal / (float) fpsCount);
            fpsCount = 0;
            fpsRunningTotal = 0;
        }
    }

    private void UpdateWhilePlaying()
    {
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
        if (end != null && end.Connected) {
            endConnectedCounter += Time.deltaTime;
            if (endConnectedCounter > 1) {
                gameState = GameState.Won;

                // force disable grab input
                var im = transform.Find("/GameManager").GetComponent<InputManager>();
                im.Disable();

                // load and display level complete overlay
                var p = Resources.Load<GameObject>("Prefabs/LevelCompleteOverlay");
                var levelCompleteOverlay = GameObject.Instantiate(p, GameObject.Find("IngameUI").transform);
                var continueButton = levelCompleteOverlay.transform.FindDeepComponent<Button>("ContinueButton");
                continueButton.onClick.AddListener(() => {
                    LoadNextLevel();
                });
            }
        }
        else {
            endConnectedCounter = 0;
        }
        
    }

    public static void LoadLevel(string name)
    {
        SceneManager.LoadScene(name);
    }

    private static void LoadNextLevel()
    {
        var currentLevelIndex = Array.IndexOf(LevelNames, SceneManager.GetActiveScene().name);
        if (currentLevelIndex != -1)
        {
            var nextLevelIndex = currentLevelIndex + 1;
            if (nextLevelIndex >= LevelNames.Length)
            {
                // last level, show win screen
                SceneManager.LoadScene("WinScreen");
            }
            else
            {
                LoadLevel(LevelNames[nextLevelIndex]);
            }
        } else
        {
            Debug.LogWarning("Cannot load next scene: current scene not found in list of levels!");
        }
    }
}
