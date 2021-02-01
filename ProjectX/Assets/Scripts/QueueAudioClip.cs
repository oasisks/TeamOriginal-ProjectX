using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class QueueAudioClip: MonoBehaviour
{
    public AudioSource[] audioSources;
    private int i;
    public bool changeClip;
    private bool didChangeClip;

    void Start() {
        i = 0;
    }

    void FixedUpdate() {
        if(changeClip) {
            audioSources[i].loop = false;
            didChangeClip = false;
        }

        if (!audioSources[i].isPlaying && changeClip && !didChangeClip) {
            if (i < audioSources.Length) i++;
            audioSources[i].Play();
            Debug.Log("Music siwtched to to stage " + i);
            didChangeClip = true;
            changeClip = false;
        }
    }

    void StopMusic() {
        audioSources[i].Stop();
    }
}
