using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goomba : Enemy
{
    [Header("Properties")]
    [SerializeField] float horizontalDistance = 5f;
    [SerializeField] float speed = 2f;
    [SerializeField] float hitDistance = 0.5f;
    [SerializeField] tetrisBehavior tetris;
    [SerializeField] float dyingAnimTime = 0.25f;
    [SerializeField] Transform killableObject;

    private float previousTime;

    private Transform raycastTransform;
    private Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private bool canMove;

    private AudioSource audiosrc;

    private void Awake()
    {
        raycastTransform = transform.GetChild(0).GetComponent<Transform>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        direction.x = 1;

        audiosrc = GetComponent<AudioSource>();
    }

    private void Update()
    {
        /*if (tetris.canActivate) 
        {
            Activate();
            canMove = true;
            tetris.canActivate = false; //removed to support more than one goomba per tetris block
        }*/
        if (canMove)
            GoombaMovement();

        KillGoomba();
    }

    private void GoombaMovement()
    {
        // enable gravity
        rb.gravityScale = 1f;

        // downward ray
        RaycastHit2D verticalHit = Physics2D.Raycast(raycastTransform.position, -Vector2.up, horizontalDistance, ~LayerMask.NameToLayer("ground"));
        Debug.DrawLine(raycastTransform.position, raycastTransform.position - Vector3.up * horizontalDistance);

        //// downward ray
        //RaycastHit2D horizontalHit = Physics2D.Raycast(raycastTransform.position, Vector3.right, sightRange, ~LayerMask.NameToLayer("ground"));
        //Debug.DrawLine(raycastTransform.position, raycastTransform.position + Vector3.right * sightRange);

        if (verticalHit.collider == null)
        {
            changeDirection();
            transform.localScale = scale;
        }

        //if (horizontalHit.collider != null)
        //    Debug.Log(horizontalHit.collider.name);
        transform.Translate(direction * speed * Time.deltaTime);
    }

    public void Activate()
    {
        // add a rigidbody component
        rb = gameObject.AddComponent<Rigidbody2D>();
        // freeze the z-axis
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void KillGoomba()
    {
        // the goomba gets killed when a player hits it from above
        Vector3 topCenter = new Vector3(killableObject.position.x, killableObject.position.y + spriteRenderer.size.y / 2 + 0.02f, 0);
        RaycastHit2D upwardHit = Physics2D.Raycast(topCenter, Vector3.up * hitDistance, hitDistance);
        Debug.DrawLine(topCenter, topCenter + Vector3.up * hitDistance);


        if (upwardHit.collider != null)
        {
            if (upwardHit.collider.tag == "Player")
            {
                if (!hasDied) audiosrc.Play();

                // play the animation
                animator.SetBool("hasDied", true);
                previousTime = Time.time;
                hasDied = true;
            }
        }

        if (animator.GetBool("hasDied"))
        {
            if (Time.time - previousTime > dyingAnimTime)
            {
                Destroy(gameObject);
            }
        }
    }
}
