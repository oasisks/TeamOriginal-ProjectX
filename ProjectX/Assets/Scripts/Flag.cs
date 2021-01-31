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

    private void Start()
    {
        anim = GetComponent<Animator>();
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

        if (collision.gameObject.tag == "Player")
        {
            hasPassedLevel = true;
            anim.SetBool("HasPassed", true);
            previousTime = Time.time;
        }
    }
}
