using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
class AudioManager : MonoBehaviour
{
    public AudioClip OnConnectionAdded = null;
    public AudioClip OnConnectionRemoved = null;
    public AudioClip OnGrabAttractor = null;
    public AudioClip OnReleaseAttractor = null;
    public AudioClip OnObstacleHit = null;
    public AudioClip OnObstacleFreed = null;

    private AudioSource[] musicLayers;

    public void Start()
    {
        var audioSource = GetComponent<AudioSource>();

        // setup music layers
        musicLayers = transform.Find("MusicLayers").GetComponentsInChildren<AudioSource>();
        for (int i = 0;i < musicLayers.Length;i++)
        {
            // play all layers, but only the first one unmuted
            musicLayers[i].volume = (i == 0) ? 1f : 0f;
            musicLayers[i].Play();
        }


        // setup connection change sfx
        var gm = transform.Find("/GameManager")?.GetComponent<GameManager>();
        if (gm != null)
        {
            gm.OnConnectionAdded += () =>
            {
                Debug.Log("Connection added");
                audioSource.PlayOneShot(OnConnectionAdded);
            };
            gm.OnConnectionRemoved += () =>
            {
                Debug.Log("Connection removed");
                audioSource.PlayOneShot(OnConnectionRemoved);
            };
            // change music tracks based on connected lines
            gm.OnNumConnectedChanged += (numConnected) =>
            {
                Debug.Log($"Number of Connections changed to {numConnected}");
                for (int i = 0; i < musicLayers.Length; i++)
                {
                    musicLayers[i].volume = (i < numConnected) ? 1f : 0f;
                };
            };

            gm.OnObstacleHit += () =>
            {
                Debug.Log("Obstacle hit");
                audioSource.PlayOneShot(OnObstacleHit);
            };

            gm.OnObstacleFreed += () =>
            {
                Debug.Log("Obstacle freed");
                audioSource.PlayOneShot(OnObstacleFreed);
            };
        }

        // setup attractor sfx
        var attractorGrabAudioSource = transform.Find("AttractorGrab").GetComponent<AudioSource>();
        var im = transform.Find("/GameManager")?.GetComponent<InputManager>();
        if (im != null)
        {
            im.OnGrabAttractor += (GameObject o) =>
            {
                Debug.Log("Attractor grabbed");
                attractorGrabAudioSource.PlayOneShot(OnGrabAttractor);

                attractorGrabAudioSource.Play();
            };
            im.OnReleaseAttractor += (GameObject o) =>
            {
                Debug.Log("Attractor released");
                attractorGrabAudioSource.Stop();
                attractorGrabAudioSource.PlayOneShot(OnReleaseAttractor);
            };
        }
    }
}