using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class tetrisBehavior : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] private Tilemap SolidTiles;
    [SerializeField] private Tilemap NotSolidTiles;
    [SerializeField] private float secondsUntilFall;

    [Header("Score Related")]
    [SerializeField] int scoreIncrement;

    [Header("Misc")]
    [SerializeField] public bool rotatable;
    //public bool canActivate = false;

    public bool active;

    private List<Vector3> tileWorldLocations;
    private Transform rot_center;

    private Grid m_Grid;
    private worldHandler World;
    private GameManager gm;
    private CanvasManager cm;

    // time of the last fall, used to auto fall
    private float lastFall;

    // last key pressed time, to handle long press behavior
    private float lastKeyDown;
    private float timeKeyPressed;

    // Start is called before the first frame update
    void Start()
    {
        //this.GetComponent<Renderer>().material.color.a = 0.5f;
        m_Grid = GetComponent<Grid>();
        World = FindObjectOfType<worldHandler>(); //TODO: Find by tag?
        cm = FindObjectOfType<CanvasManager>();
        gm = FindObjectOfType<GameManager>(); //TODO: Find by tag?
        rot_center = transform.GetChild(0).GetComponent<Transform>();
        lastFall = Time.time;
        lastKeyDown = Time.time;
        timeKeyPressed = Time.time;

        tileWorldLocations = new List<Vector3>();

        foreach (var pos in tileWorldLocations) {
            print(pos);
        }

        if (!isValidGridPos()) {
            gm.endGame("You built too high :(");
        }
    }

    /*public void AlignCenter() {
        transform.position += transform.position - Utils.Center(gameObject);
    }*/

    private Vector3 getWorldCoord(Vector3Int pos) { //Hacky hack
        if(transform.eulerAngles.z == 90) {
            pos.y += 1;
        } else if(transform.eulerAngles.z == 180) {
            pos.x += 1;
            pos.y += 1;
        } else if(transform.eulerAngles.z == 270) {
            pos.x += 1;
        }

        return m_Grid.CellToWorld(pos);
    }

    public bool isValidGridPos() {
        foreach (var pos in SolidTiles.cellBounds.allPositionsWithin) {   //TODO: concatenate these?
            Vector3Int localPlace = new Vector3Int(pos.x, pos.y, 0);
            Vector3 place = getWorldCoord(localPlace);
            if (SolidTiles.HasTile(localPlace) && World.getTile_GlobalPos(place) != null) {
                return false;
            }
        }

        foreach (var pos in NotSolidTiles.cellBounds.allPositionsWithin) {
            Vector3Int localPlace = new Vector3Int(pos.x, pos.y, 0);
            Vector3 place = getWorldCoord(localPlace);
            if (NotSolidTiles.HasTile(localPlace) && World.getTile_GlobalPos(place) != null) {
                return false;
            }
        }
        return true;
    }

    public void insertInWorld() {
        foreach (var pos in SolidTiles.cellBounds.allPositionsWithin) {   //TODO: concatenate these?
            Vector3Int localPlace = new Vector3Int(pos.x, pos.y, 0);
            Vector3 place = getWorldCoord(localPlace);
            TileBase tile = SolidTiles.GetTile(localPlace);
            if (tile != null)
            {
                World.setTile_GlobalPos(place, tile, true);
            }
        }
        foreach (var pos in NotSolidTiles.cellBounds.allPositionsWithin) {
            Vector3Int localPlace = new Vector3Int(pos.x, pos.y, 0);
            Vector3 place = getWorldCoord(localPlace);
            TileBase tile = NotSolidTiles.GetTile(localPlace);
            if (tile != null)
            {
                World.setTile_GlobalPos(place, tile, false);
            }
        }

        foreach(Transform child in this.transform) { //.GetComponentsInChildren<Transform>(checkInactive)) {
            if(child.gameObject.CompareTag("entityWithinTetrisBlock") == true) {
                // we don't want to rotate the enemy
                if (child.gameObject.layer == LayerMask.NameToLayer("enemy")) {
                    GameObject e = Instantiate(child.gameObject, child.position, Quaternion.identity);
                    try { // TODO: this is a BAD way of doing this
                        e.GetComponent<Goomba>().Activate(); 
                    } catch (NullReferenceException ex) {

                    }
                    try { // TODO: this is a BAD way of doing this
                        e.GetComponent<FlyingGoomba>().Activate(); 
                    } catch (NullReferenceException ex) {

                    }
                } else {
                    Instantiate(child.gameObject, child.position, child.rotation);
                }
            }
            //GameObject obj = child.gameObject;
        }

        print("Done\n");
        //canActivate = true;
        gm.increaseScore(scoreIncrement);

        // we should only spawn next when the player has not finished the level yet
        if (!gm.flag.hasPassedLevel)
            gm.spawnNext();

        Destroy(gameObject);
    }

    private void tryPosChange(Vector3Int v) {
        transform.position += v;

        // See if valid
        if (isValidGridPos()) {
            return; //updateGrid();
        } else {
            transform.position -= v;
        }
        return;
    }

    // getKey if is pressed now on longer pressed by 0.5 seconds | if that true apply the key each 0.05f while is pressed
    bool getKey(KeyCode key) {
        bool keyDown = Input.GetKeyDown(key);
        bool pressed = Input.GetKey(key) && Time.time - lastKeyDown > 0.5f && Time.time - timeKeyPressed > 0.05f;

        if (keyDown) {
            lastKeyDown = Time.time;
        }
        if (pressed) {
            timeKeyPressed = Time.time;
        }

        return keyDown || pressed;
    }

    // Update is called once per frame
    void Update()
    {
        if (!cm.pauseMenuOn && active) {
            if (getKey(KeyCode.LeftArrow)) {
                tryPosChange(new Vector3Int(-1, 0, 0));
            } else if (getKey(KeyCode.RightArrow)) {  // Move right
                tryPosChange(new Vector3Int(1, 0, 0));
            } else if (rotatable && getKey(KeyCode.UpArrow)) { // Rotate
                //transform.Rotate(0, 0, 90);
                transform.RotateAround(rot_center.position, Vector3.forward, 90);
                if (!isValidGridPos()) {
                    // it's not valid. revert
                    //transform.Rotate(0, 0, -90);
                    transform.RotateAround(rot_center.position, Vector3.forward, -90);
                }
            } else if ((getKey(KeyCode.DownArrow) || (Time.time - lastFall) >= secondsUntilFall) && !gm.flag.hasPassedLevel) {
                lastFall = Time.time;
                transform.position += new Vector3(0, -1, 0);
                if (!isValidGridPos()){
                    transform.position += new Vector3(0, 1, 0);
                    insertInWorld();
                }
            } else if (Input.GetKeyDown(KeyCode.K)) { //Freeze
                insertInWorld();

                /*while (enabled) { // fall until the bottom
                    fallGroup();
                }*/
            } else if (Input.GetKeyDown(KeyCode.J)) { //Hold
                active = false;
                gm.hold_transfer();
            }
        }
    }
}
