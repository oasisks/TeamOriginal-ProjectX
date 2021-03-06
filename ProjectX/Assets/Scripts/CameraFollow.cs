using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Vector3 offset;
    private GameObject player;

    private float previousHeight;
    private Camera cam;

    private GameManager gm;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        previousHeight = offset.y + transform.position.y;
        cam = GetComponent<Camera>();
        gm = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        if(player != null) { //TODO: do this properly
            Follow();
        }
        KillPlayer();
    }

    private void Follow()
    {
        // if the player has surpassed the camera y position
        if (previousHeight < player.transform.position.y)
        {
            float heightDifference = player.transform.position.y - previousHeight;
            previousHeight = player.transform.position.y;
            transform.position = new Vector3(transform.position.x, transform.position.y + heightDifference, transform.position.z);
        }
    }
    
    private void KillPlayer()
    {
        float minimumYPos = transform.position.y - cam.orthographicSize;
        if (player != null && player.transform.position.y < minimumYPos)
        {
            Destroy(player);
            gm.playerDiedFromHeight = true;
        }
    }

}

