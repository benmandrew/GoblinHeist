using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WallCollider : MonoBehaviour {
    Tilemap tm;
    HashSet<Vector3Int> wallPositions;

    void Awake() {
        tm = GetComponent<Tilemap>();
        wallPositions = new HashSet<Vector3Int>();
        foreach (Vector3Int pos in tm.cellBounds.allPositionsWithin) {
            if (!tm.HasTile(pos)) continue;
            wallPositions.Add(pos);
        }
    }

    public bool isColliding(Vector3 pos) {
        Vector3Int ipos = Vector3Int.RoundToInt(pos);
        return wallPositions.Contains(ipos);
    }
}
