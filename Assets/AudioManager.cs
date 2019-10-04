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

    private AudioSource[] musicLayers;

    public void Start()
    {
        var audioSource = GetComponent<AudioSource>();

        // setup connection change sfx
        var gm = transform.Find("/GameManager").GetComponent<GameManager>();
        gm.OnConnectionAdded += () =>
        {
            Debug.Log("Connection added");
            audioSource.PlayOneShot(OnConnectionAdded, 0.1f);
        };
        gm.OnConnectionRemoved += () =>
        {
            Debug.Log("Connection removed");
            audioSource.PlayOneShot(OnConnectionRemoved, 0.1f);
        };

        // setup music layers
        musicLayers = transform.Find("MusicLayers").GetComponentsInChildren<AudioSource>();
        for (int i = 0;i < musicLayers.Length;i++)
        {
            // play all layers, but only the first one unmuted
            musicLayers[i].volume = (i == 0) ? 1f : 0f;
            musicLayers[i].Play();
        }

        // change music tracks based on connected lines
        gm.OnNumConnectedChanged += (numConnected) =>
        {
            Debug.Log($"Number of Connections changed to {numConnected}");
            for (int i = 0; i < musicLayers.Length; i++)
            {
                musicLayers[i].volume = (i < numConnected) ? 1f : 0f;
            };
        };

        // setup attractor sfx
        var attractorGrabAudioSource = transform.Find("AttractorGrab").GetComponent<AudioSource>();
        var im = transform.Find("/GameManager").GetComponent<InputManager>();
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