using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goomba : Enemy
{
    [Header("Properties")]
    [SerializeField] float horizontalDistance = 5f;
    [SerializeField] float speed = 2f;
    [SerializeField] tetrisBehavior tetris;
    private Transform raycastTransform;
    private Vector3 direction = Vector3.zero;
    private Vector3 scale = new Vector3(1, 1, 1);
    private Animator animator;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        raycastTransform = transform.GetChild(0).GetComponent<Transform>();
        animator = GetComponent<Animator>();
        direction.x = 1;
    }

    private void Update()
    {
        if (tetris.canActivate)
            GoombaMovement();
        else
            Deactivate();
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
            scale.x = -scale.x;
            transform.localScale = scale;
            direction *= -1;
        }

        //if (horizontalHit.collider != null)
        //    Debug.Log(horizontalHit.collider.name);
        transform.Translate(direction * speed * Time.deltaTime);
    }
    private void Deactivate()
    {
        // disable gravity
        rb.gravityScale = 0f;

        // Maybe some other stuff (subject to change)
    }
}
