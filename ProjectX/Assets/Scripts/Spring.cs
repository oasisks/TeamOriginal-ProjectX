using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : MonoBehaviour
{
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player")
            anim.SetBool("playerHasCollided", true);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        anim.SetBool("playerHasCollided", false);
    }
}
