using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject[] tetrisBlocks;

    public int score; // this will store the players score
    public TMP_Text scoreText;
    public TMP_Text gameoverText;
    public TMP_Text gameoverMsgText;

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

    public Image[] healthHearts;
    private int health;

    private bool started;


    private void Awake()
    {
        spawner = transform.GetChild(0).GetComponent<Transform>();
        player = Instantiate(playerPrefab, spawner.transform.position, Quaternion.identity);
        canvas = GameObject.FindGameObjectWithTag("canvas").GetComponent<CanvasManager>();
        score = 0;
        health = healthHearts.Length;
        started = true;
    }

    private void Start()
    {   
        started = false;
        current = null;
        hold = null;

        flag = GameObject.FindGameObjectWithTag("Flag").GetComponent<Flag>();
        main_cam = GameObject.FindGameObjectWithTag("MainCamera");
        offset = main_cam.GetComponent<Camera>().orthographicSize;
        getPositions();
        createNext();
        spawnNext();
    }

    private void Update()
    {
        if (flag.hasPassedLevel)
        {
            // we show a UI congratulating/switch levels/etc.
            // UI
            canvas.enableFinishedPanel();
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Static;
            // switches the level
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        if (started && !playerIsAlive())
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
        //scoreText.text = score.ToString("D4");
    }

    public void loseLife() {
        healthHearts[health-1].enabled = false;
        if(health > 1){
            health--;
        } else {
            endGame("You ran out of lives :(");
        }
    }

    public void endGame(string msg) {
        print(msg);
        //gameoverText.gameObject.SetActive(true);
        //gameoverMsgText.gameObject.SetActive(true);
        //gameoverMsgText.text = msg;
        Time.timeScale = 0;
    }
}
