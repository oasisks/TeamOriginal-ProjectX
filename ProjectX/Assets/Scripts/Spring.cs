using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] float upwardForce;
    private Animator anim;

    private AudioSource audiosrc;

    private void Start()
    {
        anim = GetComponent<Animator>();
        audiosrc = GetComponent<AudioSource>();
    }

    private void Update()
    {
        //Debug.Log(transform.rotation.eulerAngles);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player" || collision.collider.tag == "Enemy")
        {
            // play the animation
            anim.SetTrigger("Bounce");
            Rigidbody2D playerRB = collision.collider.GetComponent<Rigidbody2D>();
            Vector3 forceVector = new Vector3(Mathf.Cos((transform.eulerAngles.z + 90f) * Mathf.Deg2Rad), Mathf.Sin((transform.eulerAngles.z + 90f) * Mathf.Deg2Rad), 0f).normalized;
            Debug.Log(forceVector);
            playerRB.AddForce(forceVector * upwardForce, ForceMode2D.Impulse);

            audiosrc.Play();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player" || collision.collider.tag == "Enemy")
        {
            anim.SetTrigger("Bounce");
        }
    }
}
