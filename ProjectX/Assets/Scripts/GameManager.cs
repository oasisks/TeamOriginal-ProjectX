using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject[] tetrisBlocks;

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
        spawnNext(); //TODO: move to update, spawn tetris block when the previous one gets deleted
    }

    private void Update()
    { 
        if (flag.hasPassedLevel)
        {
            // we show a UI congratulating/switch levels/etc.
        }
    }

    private void spawnNext() {
        // Random Index
        int i = Random.Range(0, tetrisBlocks.Length);

        // Spawn Group at current Position
        Instantiate(tetrisBlocks[i], transform.position, Quaternion.identity);
    }
}
