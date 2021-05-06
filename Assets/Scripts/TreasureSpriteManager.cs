using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureSpriteManager : MonoBehaviour
{

    public Sprite sprite1;
    public Sprite sprite2;
    private SpriteRenderer sr;

    void Awake() {
        sr = gameObject.GetComponentInChildren<SpriteRenderer>();
        sr.sprite = sprite1;
    }

    public void changeSprite() {
        if (sr.sprite == sprite1) {
            sr.sprite = sprite2;
        }
        else {
            sr.sprite = sprite1;
        }
    }
}
