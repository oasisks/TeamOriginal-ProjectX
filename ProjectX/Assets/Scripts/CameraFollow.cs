using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private GameObject player;

    private float previousHeight;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        previousHeight = transform.position.y;
    }

    private void Update()
    {
        if(player != null) { //TODO: do this properly
            Follow();
        }
    }

    private void Follow()
    {
        // if the player has surpassed the camera y position
        if (previousHeight < player.transform.position.y)
        {
            previousHeight = player.transform.position.y;
            transform.position = new Vector3(transform.position.x, previousHeight, transform.position.z);
        }
    }

}

