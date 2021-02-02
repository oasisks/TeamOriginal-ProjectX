using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goomba : Enemy
{
    [Header("Properties")]
    [SerializeField] float distance = 5f;
    [SerializeField] float speed = 2f;

    private Transform raycastTransform;
    private Vector3 direction = Vector3.zero;
    private Vector3 scale = new Vector3(1, 1, 1);
    private Animator animator;

    private void Start()
    {
        raycastTransform = transform.GetChild(0).GetComponent<Transform>();
        animator = GetComponent<Animator>();
        Debug.Log(raycastTransform.name);
        direction.x = 1;
    }

    private void Update()
    {
        GoombaMovement();
    }

    private void GoombaMovement()
    {
        // downward ray
        RaycastHit2D verticalHit = Physics2D.Raycast(raycastTransform.transform.position, -Vector2.up, distance, ~LayerMask.NameToLayer("ground"));
        Debug.DrawLine(raycastTransform.transform.position, raycastTransform.transform.position - Vector3.up * distance);
        Debug.Log(verticalHit.collider == null);

        if (verticalHit.collider == null)
        {
            scale.x = -scale.x;
            transform.localScale = scale;
            direction *= -1;
        }
        transform.Translate(direction * speed * Time.deltaTime);
    }
}
