using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingGoomba : Enemy
{
    [SerializeField] bool flyHorizontal;
    [SerializeField] float speed; 
    [SerializeField] Transform raycastObject;
    [SerializeField] float sightDistance;

    private void Start()
    {
        direction.x = 1;
    }

    private void Update()
    {
        Fly();
    }

    private void Fly()
    {
        // the goomba needs to fly horizontally 
        if (flyHorizontal)
            Horizontal();
        // the goomba needs to fly vertically (don't know if this is necessary)
        if (!flyHorizontal)
            Vertical();
    }

    private void Horizontal()
    {
        // downward ray
        RaycastHit2D horizontalHit = Physics2D.Raycast(raycastObject.position, Vector2.right, sightDistance, ~LayerMask.NameToLayer("ground"));
        Debug.DrawLine(raycastObject.position, raycastObject.position + Vector3.right * sightDistance);

        //// downward ray
        //RaycastHit2D horizontalHit = Physics2D.Raycast(raycastTransform.position, Vector3.right, sightRange, ~LayerMask.NameToLayer("ground"));
        //Debug.DrawLine(raycastTransform.position, raycastTransform.position + Vector3.right * sightRange);

        if (horizontalHit.collider != null)
        {
            changeDirection();
            transform.localScale = scale;
        }

        //if (horizontalHit.collider != null)
        //    Debug.Log(horizontalHit.collider.name);
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void Vertical()
    {

    }
}
