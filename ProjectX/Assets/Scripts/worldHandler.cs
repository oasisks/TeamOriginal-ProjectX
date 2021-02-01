using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class worldHandler : MonoBehaviour
{	
	private Grid m_Grid;
	public Tilemap SolidTiles;
	public Tilemap NotSolidTiles;

    [SerializeField] private TileBase groundTile;
	//public GridInformation GridInfo;
    
    // Start is called before the first frame update
    void Start()
    {
        m_Grid = GetComponent<Grid>();
		//GridInfo = GetComponent<GridInformation>();
		//m_Foreground = GetComponent<Tilemap>();
		//m_BackGround = GetComponent<Tilemap>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_Grid && Input.GetMouseButtonDown(0)) //TESTING
		{
			Vector3 world = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            setTile_GlobalPos(world, groundTile, true);
		}
    }

    public TileBase getTile_GlobalPos(Vector3 globalpos) {
        Vector3Int cellpos = m_Grid.WorldToCell(globalpos);
        cellpos.z = 0;
        TileBase tile = SolidTiles.GetTile(cellpos);
        if(tile != null) {
            return tile;
        }
        tile = NotSolidTiles.GetTile(cellpos);
        if(tile != null) {
            return tile;
        }
        return null;
    }

    public void setTile_GlobalPos(Vector3 globalpos, TileBase tile, bool Solid) {
        if(tile == null) {
            return;
        }
        Vector3Int cellpos = m_Grid.WorldToCell(globalpos);
        cellpos.z = 0;
        if(Solid) {
            //print("Tried to place at" + place);
            SolidTiles.SetTile(cellpos, tile);
        } else {
            NotSolidTiles.SetTile(cellpos, tile);
        }
    }
}
