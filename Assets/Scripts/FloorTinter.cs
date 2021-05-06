using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FloorTinter : MonoBehaviour {
    private Tilemap tm;

    private Color tint;
    private Color untint;

    void Awake() {
        tm = GetComponent<Tilemap>();
        tint = new Color(1.0f, 0.6f, 0.6f, 1.0f);
        untint = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }

    public void tintTile(Vector3 pos) {
        Vector3Int ipos = Vector3Int.RoundToInt(pos);
        tm.SetTileFlags(ipos, TileFlags.None);
        tm.SetColor(ipos, tint);
    }

    public void untintTile(Vector3 pos) {
        Vector3Int ipos = Vector3Int.RoundToInt(pos);
        tm.SetColor(ipos, untint);
    }
}
