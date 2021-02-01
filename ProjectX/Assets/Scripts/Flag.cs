using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour
{
    [SerializeField]
    private float animationTime;
    public bool hasPassedLevel = false;

    private Animator anim;
    private float previousTime;

    private AudioSource audio;

    private QueueAudioClip musicManager;

    private void Start()
    {
        anim = GetComponent<Animator>();
        audio = GetComponent<AudioSource>();

        musicManager = GameObject.FindGameObjectWithTag("MusicManager").GetComponent<QueueAudioClip>();
    }
    private void Update()
    {
        if (hasPassedLevel)
        {
            if (Time.time - previousTime > animationTime)
            {
                anim.SetBool("HasPassed", false);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("I got triggered");
        audio.Play();

        musicManager.StopMusic();

        if (collision.gameObject.tag == "Player")
        {
            hasPassedLevel = true;
            anim.SetBool("HasPassed", true);
            previousTime = Time.time;
        }
    }
}
