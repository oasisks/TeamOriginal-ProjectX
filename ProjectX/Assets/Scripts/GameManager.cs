using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject[] tetrisBlocks;

    public float score; // this will store the players score

    private Flag flag;
    private Transform spawner;
    private GameObject player;
    private GameObject main_cam;
    private CanvasManager canvas;
    private float offset;

    private void Awake()
    {
        spawner = transform.GetChild(0).GetComponent<Transform>();
        player = Instantiate(playerPrefab, spawner.transform.position, Quaternion.identity);
        canvas = GameObject.FindGameObjectWithTag("canvas").GetComponent<CanvasManager>();
    }

    private void Start()
    {
        flag = GameObject.FindGameObjectWithTag("Flag").GetComponent<Flag>();
        main_cam = GameObject.FindGameObjectWithTag("MainCamera");
        offset = main_cam.GetComponent<Camera>().orthographicSize;
        spawnNext(); //TODO: move to update, spawn tetris block when the previous one gets deleted
    }

    private void Update()
    { 
        if (flag.hasPassedLevel)
        {
            // we show a UI congratulating/switch levels/etc.
            // UI
            canvas.enableFinishedPanel();
            // switches the level
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        if (!playerIsAlive())
        {
            // TODO: A UI that shows that the player has died.
            // TODO: A Quit and restart button
            // restart the level
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        Debug.Log(score);

    }

    private bool playerIsAlive()
    {
        if (player == null)
        {
            return false;
        }
        return true;
    }

    public void spawnNext() {
        Vector3 spawnpos = main_cam.transform.position;
        spawnpos.y = Mathf.Round(spawnpos.y+offset-2);
        spawnpos.x = Mathf.Round(spawnpos.x);
        spawnpos.z = 0;
        // Random Index
        int i = Random.Range(0, tetrisBlocks.Length);

        // Spawn Group at current Position
        Instantiate(tetrisBlocks[i], spawnpos, Quaternion.identity);
        print("Spawned block");
    }
}
