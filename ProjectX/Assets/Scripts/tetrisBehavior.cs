using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class tetrisBehavior : MonoBehaviour
{
    public Tilemap tilemap;
    private List<Vector3> tileWorldLocations;
    //public GameObject World;
    worldHandler World;
    
    // time of the last fall, used to auto fall after 
    // time parametrized by `level`
    private float lastFall;

    // last key pressed time, to handle long press behavior
    private float lastKeyDown;
    private float timeKeyPressed;

    // Start is called before the first frame update
    void Start()
    {
        World = FindObjectOfType<worldHandler>();
        lastFall = Time.time;
        lastKeyDown = Time.time;
        timeKeyPressed = Time.time;

        tileWorldLocations = new List<Vector3>();

        foreach (var pos in tilemap.cellBounds.allPositionsWithin)
        {   
            Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
            /*Vector3 place = tilemap.CellToWorld(localPlace);
            if (tilemap.HasTile(localPlace))
            {
                tileWorldLocations.Add(place);
            }*/
        }

        //print(tileWorldLocations);

        foreach (var pos in tileWorldLocations) {
            print(pos);
        }

        if (isValidGridPos()) {
            //insertOnGrid();
        } else { 
            print("KILLED ON START");
        }
    }

    /*public void AlignCenter() {
        transform.position += transform.position - Utils.Center(gameObject);
    }*/

    public bool isValidGridPos() {
        foreach (var pos in tilemap.cellBounds.allPositionsWithin) {   
            Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
            Vector3 place = tilemap.CellToWorld(localPlace);
            localPlace.z = 0;
            place.z = 0;
            if (tilemap.HasTile(localPlace) && World.getTile_GlobalPos(place) != null) {
                return false;
            }
        }
        return true;
    }

    public void insertInWorld() {
        foreach (var pos in tilemap.cellBounds.allPositionsWithin) {   
            Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
            Vector3 place = tilemap.CellToWorld(localPlace);
            localPlace.z = 0;
            place.z = 0;
            if (tilemap.HasTile(localPlace))
            {
                World.setTile_GlobalPos(place, tilemap.GetTile(localPlace));
                //print(place);
            }
        }
        print("Done\n");
        Destroy(gameObject);
    }

    private void tryPosChange(Vector3Int v) {
        transform.position += v;
        /*
        // See if valid
        if (isValidGridPos()) {
            return; //updateGrid();
        } else {
            transform.position -= v;
        }*/
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
        //if (UIController.isPaused) {
        //    return; // don't do nothing
        //}
        if (getKey(KeyCode.LeftArrow)) {
            tryPosChange(new Vector3Int(-1, 0, 0));
        } else if (getKey(KeyCode.RightArrow)) {  // Move right
            tryPosChange(new Vector3Int(1, 0, 0));
        }/* else if (getKey(KeyCode.UpArrow)) { // Rotate
            transform.Rotate(0, 0, -90);

            // see if valid
            if (isValidGridPos()) {
                // it's valid. Update grid
                updateGrid();
            } else {
                // it's not valid. revert
                transform.Rotate(0, 0, 90);
            }
        }*/ else if (getKey(KeyCode.DownArrow)/* || (Time.time - lastFall) >= 1*/) {
            //fallGroup();
            tryPosChange(new Vector3Int(0, -1, 0));
        } else if (Input.GetKeyDown(KeyCode.Space)) {
            insertInWorld();
            
            /*while (enabled) { // fall until the bottom 
                fallGroup();
            }*/
        }
    }

    /* 
    // update the grid
    void updateGrid() {
        // Remove old children from grid
        for (int y = 0; y < Grid.h; ++y) {
            for (int x = 0; x < Grid.w; ++x) {
                if (Grid.grid[x,y] != null &&
                    Grid.grid[x,y].parent == transform) {
                    Grid.grid[x,y] = null;
                }
            } 
        }

        insertOnGrid();
    }
    
    void fallGroup() {
        // modify
        transform.position += new Vector3(0, -1, 0);

        if (isValidGridPos()){
            // It's valid. Update grid... again
            updateGrid();
        } else {
            // it's not valid. revert
            transform.position += new Vector3(0, 1, 0);

            // Clear filled horizontal lines
            Grid.deleteFullRows();


            FindObjectOfType<Spawner>().spawnNext();


            // Disable script
            enabled = false;
        }

        lastFall = Time.time;

    }

    */
}
