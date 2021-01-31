using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class worldHandler : MonoBehaviour
{
    public TileBase Ground;
	public TileBase Spike;
	
	public Grid m_Grid;
	public Tilemap GroundTiles;
	public Tilemap SpikeTiles;
	//public GridInformation GridInfo;
    
    // Start is called before the first frame update
    void Start()
    {
        //m_Grid = GetComponent<Grid>();
		//GridInfo = GetComponent<GridInformation>();
		//m_Foreground = GetComponent<Tilemap>();
		//m_BackGround = GetComponent<Tilemap>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_Grid && Input.GetMouseButtonDown(0))
		{
			Vector3 world = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			/*Vector3Int gridPos = m_Grid.WorldToCell(world);
            gridPos.z = 0;
            print(gridPos);
            SpikeTiles.SetTile(gridPos, Spike);*/
            setTile_GlobalPos(world, Ground);
		}
    }

    public TileBase getTile_GlobalPos(Vector3 position) {
        Vector3Int place = m_Grid.WorldToCell(position);
        place.z = 0;
        TileBase tile = GroundTiles.GetTile(place);
        if(tile != null) {
            return tile;
        }
        tile = SpikeTiles.GetTile(place);
        if(tile != null) {
            return tile;
        }
        return null;
    }

    public void setTile_GlobalPos(Vector3 position, TileBase tile) {
        if(tile == null) {
            return;
        }
        Vector3Int place = m_Grid.WorldToCell(position);
        place.z = 0;
        if(tile == Ground) {
            //print("Tried to place at" + place);
            GroundTiles.SetTile(place, Ground);
        } else if(tile == Spike) {
            SpikeTiles.SetTile(place, tile);
        }
    }
}
