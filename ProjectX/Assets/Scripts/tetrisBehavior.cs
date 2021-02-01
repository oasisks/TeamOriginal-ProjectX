using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class tetrisBehavior : MonoBehaviour
{
    [SerializeField] private Tilemap SolidTiles;
    [SerializeField] private Tilemap NotSolidTiles;
    [SerializeField] private float secondsUntilFall;

    private List<Vector3> tileWorldLocations;
    private Transform rot_center;

    private Grid m_Grid;
    worldHandler World;
    GameManager gm;
    
    // time of the last fall, used to auto fall
    private float lastFall;

    // last key pressed time, to handle long press behavior
    private float lastKeyDown;
    private float timeKeyPressed;

    // Start is called before the first frame update
    void Start()
    {
        m_Grid = transform.GetComponent<Grid>();
        World = FindObjectOfType<worldHandler>(); //TODO: Find by tag?
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
            print("KILLED ON START"); //TODO: make this game over
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
                Instantiate(child.gameObject, child.position, child.rotation);
            }
            //GameObject obj = child.gameObject;
        }

        print("Done\n");
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
        //if (UIController.isPaused) {
        //    return; // don't do nothing
        //}
        if (getKey(KeyCode.LeftArrow)) {
            tryPosChange(new Vector3Int(-1, 0, 0));
        } else if (getKey(KeyCode.RightArrow)) {  // Move right
            tryPosChange(new Vector3Int(1, 0, 0));
        } else if (getKey(KeyCode.UpArrow)) { // Rotate
            //transform.Rotate(0, 0, 90);
            transform.RotateAround(rot_center.position, Vector3.forward, 90);

            // see if valid
            if (isValidGridPos()) {
                // it's valid. Update grid
            } else {
                // it's not valid. revert
                //transform.Rotate(0, 0, -90);
                transform.RotateAround(rot_center.position, Vector3.forward, -90);
            }
        } else if (getKey(KeyCode.DownArrow) || (Time.time - lastFall) >= secondsUntilFall) {
            lastFall = Time.time;
            //fallGroup();
            transform.position += new Vector3(0, -1, 0);
            if (!isValidGridPos()){
                transform.position += new Vector3(0, 1, 0);
                insertInWorld();
            }
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
