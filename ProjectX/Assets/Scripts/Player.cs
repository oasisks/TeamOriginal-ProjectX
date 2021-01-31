using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float playerSpeed;
    [SerializeField]
    private float jumpForce;
    [SerializeField]
    private float smoothTime;
    [SerializeField]
    private float health; // we may change this to hearts (i.e. a player has 3 hearts and if all three hearts are gone then you die)
    [SerializeField]
    private float invincibleTime;


    private Rigidbody2D rb;
    private bool isGrounded = true;
    private bool invincible = false;
    private Vector2 jumpForceVector;
    private Vector3 scaleVector = new Vector3(1, 1, 1);
    private Vector3 referencedVelocity = Vector3.zero;
    private float previousTime; 
    private Animator animator;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        jumpForceVector = new Vector2(0, jumpForce);
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        PlayerMovement();
        Death();
        ResetInvincibleAnim();
    }

    private void PlayerMovement()
    {
        // basic movement
        Vector3 velocity = Vector2.zero;
        if(Input.GetKey(KeyCode.A)) {
            velocity.x = -1;
        } else if(Input.GetKey(KeyCode.D)) {
            velocity.x = 1;
        }

        velocity = velocity.normalized * playerSpeed;
        rb.velocity = Vector3.SmoothDamp(rb.velocity, velocity, ref referencedVelocity, smoothTime);

        // flip the duck towards the direction the duck is moving
        if (velocity.x < 0)
        {
            scaleVector.x = -1;
        }
        else if (velocity.x > 0)
        {
            scaleVector.x = 1;
        }
        transform.localScale = scaleVector;

        // jumping 
        if (Input.GetKeyDown(KeyCode.W) && isGrounded)
        {
            rb.AddForce(jumpForceVector);
            isGrounded = false;
        }
    }

    private void Death()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void ResetInvincibleAnim()
    {
        if (Time.time - previousTime > invincibleTime)
        {
            animator.SetBool("HitHarmfulObjects", false);
            invincible = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // ground collision for jumping
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("ground"))
        {
            isGrounded = true;
        }

        // harmful objects 
        if (collision.collider.tag == "harmfulObjects" && !invincible)
        {
            // minus however the damage is
            // this will vary base on the object type (i.e. lava, enemies)
            // this is assuming we are not using a discrete health system
            health -= 1;
            animator.SetBool("HitHarmfulObjects", true);
            invincible = true;
            previousTime = Time.time;
            Debug.Log("I hit harmful object");

        }
    }
}
