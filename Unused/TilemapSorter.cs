using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapSorter : MonoBehaviour {
    public int baseSortingOrder = 1000;
    
    [SerializeField]
    private int offset = 0;

    [SerializeField]
    private bool inanimateObject = true;

    void LateUpdate() {
        /*Tilemap tilemap = GetComponent<Tilemap>();

        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] allTiles = tilemap.GetTilesBlock(bounds);

        for (int x = 0; x < bounds.size.x; x++) {
            for (int y = 0; y < bounds.size.y; y++) {
                TileBase tile = allTiles[x + y * bounds.size.x];
                if (tile != null) {
                    tile.sortingOrder = (int)(baseSortingOrder - (5*transform.position.y - offset));;
                } else {
                }
            }
        }
        gameObject.GetComponent<SpriteRenderer>().sortingOrder = (int)(baseSortingOrder - (5*transform.position.y - offset));
        if(inanimateObject) {
            Destroy(this);
        }*/
    }
}
