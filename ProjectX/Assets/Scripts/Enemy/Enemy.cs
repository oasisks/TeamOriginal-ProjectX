using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public bool hasDied;
    public Vector3 direction = Vector3.zero;
    public Vector3 scale = new Vector3(1, 1, 1);

    public void changeDirection()
    {
        scale.x = -scale.x;
        direction *= -1;
    }
}
