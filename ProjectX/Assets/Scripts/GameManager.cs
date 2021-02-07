using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Necesities")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject[] tetrisBlocks;

    [Header("UI Things")]
    public int score; // this will store the players score
    public RawImage[] healthHearts;

    [HideInInspector]
    public Flag flag;
    private Transform spawner;
    private GameObject player;
    private GameObject main_cam;
    private CanvasManager canvas;
    private float offset;
    private tetrisBehavior current;
    private tetrisBehavior next;
    private tetrisBehavior hold;
    private Vector3 spawnpos;
    private Vector3 holdpos;
    private Vector3 nextpos;
    private Player playerScript;
    public bool playerKilled = false;
    public bool playerDiedFromHeight = false;



    private void Awake()
    {
        spawner = transform.GetChild(0).GetComponent<Transform>();
        player = Instantiate(playerPrefab, spawner.transform.position, Quaternion.identity);
        playerScript = player.GetComponent<Player>();
        canvas = GameObject.FindGameObjectWithTag("canvas").GetComponent<CanvasManager>();
        score = 0;
    }

    private void Start()
    {  
        current = null;
        hold = null;

        flag = GameObject.FindGameObjectWithTag("Flag").GetComponent<Flag>();
        main_cam = GameObject.FindGameObjectWithTag("MainCamera");
        offset = main_cam.GetComponent<Camera>().orthographicSize;
        getPositions();
        createNext();
        spawnNext();
        // we need to ensure that the game's time scale is always set to one when the game has started
        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (flag.hasPassedLevel)
        {
            // we need to disable Audios from player
            playerScript.StopAllAudio();
            canvas.enableFinishedPanel();

            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Static;
            // switches the level
            if (Input.anyKeyDown)
            {
                // we are going to move on to the next scene 
                //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                Debug.Log("I pressed a key");
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        if (playerDiedFromHeight)
        {
            endGame("You are out of bounds :(");
        }
        if (!playerKilled && !playerIsAlive())  // if player is not killed from insufficient life
        {
            // TODO: A UI that shows that the player has died.
            // TODO: A Quit and restart button
            // restart the level
            endGame("You crushed your duck :(");
        }

    }

    private bool playerIsAlive()
    {   
        if (player == null)
        {
            return false;
        }
        return true;
    }

    private void getPositions() {
        Vector3 campos = main_cam.transform.position;
        spawnpos = new Vector3(Mathf.Round(campos.x-3), Mathf.Round(campos.y+offset-2), 0);
        nextpos = new Vector3(Mathf.Round(campos.x+10), Mathf.Round(campos.y+5), 0);
        holdpos = new Vector3(Mathf.Round(campos.x+10), Mathf.Round(campos.y-2), 0);
    }

    private void createNext() {
        // Random Index
        int i = Random.Range(0, tetrisBlocks.Length);
        // Spawn Group at current Position
        next = Instantiate(tetrisBlocks[i], nextpos, Quaternion.identity).GetComponent<tetrisBehavior>();
        next.active = false;
        print("Spawned block");
    }

    public void spawnNext() {
        getPositions();
        current = next;
        current.transform.position = spawnpos;
        current.active = true;
        createNext(); //starts at nextpos
    }

    public void hold_transfer() {
        getPositions();
        current.active = false;
        if(hold != null) {
            hold.active = true;
            hold.transform.position = current.transform.position;
            current.transform.position = holdpos;
            tetrisBehavior temp = hold;
            hold = current;
            current = temp;
        } else {
            current.transform.position = holdpos;
            hold = current;
            spawnNext();
        }
    }

    public void increaseScore(int amount) {
        score += amount;
        canvas.UpdateScore(score);
    }

    public void loseLife(int health) {
        // update the health UI
        canvas.DisableHealth(health);
        if (health == 0)
            endGame("You ran out of lives :(");
    }

    public void endGame(string msg) {
        print(msg);
        canvas.PlayGameoverUI(msg);
        Time.timeScale = 0;
    }
}
