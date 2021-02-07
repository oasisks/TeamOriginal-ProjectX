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

    private AudioSource audiosrc;

    private QueueAudioClip musicManager;

    public GameObject confetti;

    private void Start()
    {
        anim = GetComponent<Animator>();
        audiosrc = GetComponent<AudioSource>();

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
        if (collision.gameObject.tag == "Player")
        {
            // music
            audiosrc.Play();
            musicManager.StopMusic();
            Instantiate(confetti, Vector3.zero, Quaternion.identity);

            // animation
            anim.SetBool("HasPassed", true);
            previousTime = Time.time;

            // level passed
            hasPassedLevel = true;
        }
    }
}
