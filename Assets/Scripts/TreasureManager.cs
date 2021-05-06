using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureManager : MonoBehaviour
{

    [SerializeField]
    private List<GameObject> treasures;
    private GameObject key;

    [SerializeField]
    private int treasureScore = 100;

    [SerializeField]
    private float delta = 0.2f;

    // Start is called before the first frame update
    void Awake() {
        GameObject[] ts = GameObject.FindGameObjectsWithTag("Treasure");
        foreach (GameObject t in ts) {
            treasures.Add(t);
        }
        key = GameObject.FindGameObjectWithTag("Key");
        StartCoroutine(treasureFlashCooldown());
    }

    public GameObject playerOnTreasure(Vector3 newPlayerPos) {
        foreach (GameObject t in treasures) {
            if (newPlayerPos.x > t.transform.localPosition.x - delta &&
                newPlayerPos.x < t.transform.localPosition.x + delta &&
                newPlayerPos.y > t.transform.localPosition.y - delta &&
                newPlayerPos.y < t.transform.localPosition.y + delta) {
                // Remove t from treasures
                treasures.Remove(t);
                return t;
            }
        }
        if (key != null) {
            if (newPlayerPos.x > key.transform.localPosition.x - delta &&
                newPlayerPos.x < key.transform.localPosition.x + delta &&
                newPlayerPos.y > key.transform.localPosition.y - delta &&
                newPlayerPos.y < key.transform.localPosition.y + delta) {
                return key;
            }
        }
        return null;
    }

    public void enableAll() {
        foreach (GameObject t in treasures) {
            t.SetActive(true);
        }
    }

    public int getTreasureScore() {
        return treasureScore;
    }

    private IEnumerator treasureFlashCooldown() {
        while (true) {
            yield return new WaitForSeconds(0.5f);
            doTreasureFlash();
        }
    }

    private void doTreasureFlash() {
        foreach (GameObject t in treasures) {
            t.GetComponentInChildren<TreasureSpriteManager>().changeSprite();
        }
    }
}
