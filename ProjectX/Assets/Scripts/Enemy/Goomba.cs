using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goomba : Enemy
{
    [SerializeField]
    private float distance = 5f;
    [SerializeField]
    private float speed = 2f;

    private Transform raycastTransform;
    private Vector3 direction = Vector3.zero;
    private Vector3 scale = new Vector3(1, 1, 1);

    private void Start()
    {
        raycastTransform = transform.GetChild(0).GetComponent<Transform>();
        Debug.Log(raycastTransform.name);
        direction.x = 1;
    }

    private void Update()
    {
        GoombaMovement();
    }

    private void GoombaMovement()
    {
        RaycastHit2D hit = Physics2D.Raycast(raycastTransform.transform.position, -Vector2.up, distance);
        Debug.DrawLine(raycastTransform.transform.position, raycastTransform.transform.position - Vector3.up * 5);
        if (hit.collider == null)
        {
            scale.x = -scale.x;
            transform.localScale = scale;
            direction *= -1;
        }
        transform.Translate(direction * speed * Time.deltaTime);
    }
}
