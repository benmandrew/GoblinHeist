using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    private bool isMoving = false;
    private Vector3 origin, target;
    private static float timeToMove = 0.2f;
    private static float timePerStep = timeToMove / 3.0f;

    private GameManager gm;

    public Sprite right1;
    public Sprite right2;
    public Sprite rightIdle;
    public Sprite left1;
    public Sprite left2;
    public Sprite leftIdle;
    public Sprite up1;
    public Sprite up2;
    public Sprite upIdle;
    public Sprite down1;
    public Sprite down2;
    public Sprite downIdle;

    private SpriteRenderer sr;
    private Sprite idle;
    private Sprite walk1;
    private Sprite walk2;

    private enum Direction { Up, Down, Left, Right };
    private Direction dir;

    void Start() {
        gm = GameObject.Find("Game Manager").GetComponent<GameManager>();
        sr = this.GetComponentInChildren<SpriteRenderer>();
    }

    void Update() {
        Vector3 direction = new Vector3();
        if (!gm.isPaused && !isMoving && !gm.isResetting) {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
                direction = Vector3.up;
                dir = Direction.Up;
                setSprites();
            }
            else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
                direction = Vector3.down;
                dir = Direction.Down;
                setSprites();
            }
            else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)){
                direction = Vector3.left;
                dir = Direction.Left;
                setSprites();
            }
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
                direction = Vector3.right;
                dir = Direction.Right;
                setSprites();
            }
            else {
                return;
            }
        }
        Vector3 newPlayerPos = transform.position + direction;
        if (!isMoving && gm.spaceWillBeFree(newPlayerPos) && !gm.isResetting && !gm.isPaused) {
            StartCoroutine(movePlayer(direction));
            gm.triggeredUpdate(newPlayerPos);
        }
    }

    public void playerReset() {
        this.transform.position = GameObject.Find("Stairs Up").transform.position + new Vector3(1, -0.5f, 0);
        dir = Direction.Right;
        setSprites();
    }

    private IEnumerator movePlayer(Vector3 direction) {
        isMoving = true;
        float elapsedTime = 0.0f;
        float nextStepTime = 0.0f;
        origin = transform.position;
        target = origin + direction;
        while (elapsedTime < timeToMove) {
            if (elapsedTime > nextStepTime) {
                nextStepTime += timePerStep;
                transform.position = Vector3.Lerp(
                    origin, target, nextStepTime / timeToMove);
            }
            elapsedTime += Time.deltaTime;

            // Janky movement code
            bool alternate1 = false;
            bool alternate2 = false;
            alternate1 = !alternate1;
            if (alternate1)
            {
                alternate2 = !alternate2;
                if (alternate2)
                {
                    sr.sprite = walk1;
                }
                else
                {
                    sr.sprite = walk2;
                }
            }
            yield return null;
        }
        transform.position = target;
        sr.sprite = idle;
        while (elapsedTime < timeToMove + 0.05) {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        isMoving = false;
    }

    private void setSprites()
    {
        switch (dir)
        {
            case Direction.Right:
                idle = rightIdle;
                walk1 = right1;
                walk2 = right2;
                break;
            case Direction.Up:
                idle = upIdle;
                walk1 = up1;
                walk2 = up2;
                break;
            case Direction.Left:
                idle = leftIdle;
                walk1 = left1;
                walk2 = left2;
                break;
            case Direction.Down:
                idle = downIdle;
                walk1 = down1;
                walk2 = down2;
                break;
        }
        sr.sprite = idle;
    }
}
