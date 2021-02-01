using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject playerPrefab;


    private Flag flag;
    private Transform spawner;
    private GameObject player;

    private void Awake()
    {
        spawner = transform.GetChild(0).GetComponent<Transform>();
    }

    private void Start()
    {
        flag = GameObject.FindGameObjectWithTag("Flag").GetComponent<Flag>();
        player = Instantiate(playerPrefab, spawner.transform.position, Quaternion.identity);
    }
    private void Update()
    { 
        if (flag.hasPassedLevel)
        {
            // we show a UI congratulating/switch levels/etc.
        }
    }
}
