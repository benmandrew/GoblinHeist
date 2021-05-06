using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Enemy : MonoBehaviour {
    
    private enum Action {MoveForward, TurnLeft, TurnRight, OneEighty, Wait};
    private enum Direction {Up, Down, Left, Right};

    [SerializeField]
    private List<Action> actionSequence;
    private List<Action> sequenceCopy;
    private int nActions;
    private int actionIdx;
    [SerializeField]
    private int viewDistance;
    [SerializeField]
    private Direction direction;

    private Vector3 origin, target;
    private static float timeToMove = 0.2f;
    private static float timePerStep = timeToMove / 3.0f;

    [SerializeField]
    private WallCollider wc; 
    private FloorTinter ft;
    private GameManager gm;
    private List<Vector3> tintedTiles;

    private Vector3 discretePos;
    private Vector3 startPos;
    private Direction startDir;

    private SpriteRenderer sr;
    private EnemySpriteController sc;
    private Sprite idle;
    private Sprite walk1;
    private Sprite walk2;

    void Awake() {
        wc = GameObject.Find("Walls").GetComponent<WallCollider>();
        ft = GameObject.Find("Ground").GetComponent<FloorTinter>();
        gm = GameObject.Find("Game Manager").GetComponent<GameManager>();
        sr = this.GetComponentInChildren<SpriteRenderer>();
        sc = gm.GetComponent<EnemySpriteController>();
        sequenceCopy = new List<Action>();
        for (int i = 0; i < actionSequence.Count; i++)
        {
            sequenceCopy.Add(actionSequence[i]);
        }
    }

    void Start() {
        nActions = actionSequence.Count;
        actionIdx = 0;
        tintedTiles = new List<Vector3>();

        discretePos = transform.position;
        startPos = transform.position;
        startDir = direction;

        setSprites();

        canSeePlayer(Vector3.zero);
    }

    public void applyAction() {
        switch (actionSequence[actionIdx]) {
            case Action.MoveForward:
                StartCoroutine(moveForward());
                break;
            case Action.TurnLeft:
                turnLeft();
                break;
            case Action.TurnRight:
                turnRight();
                break;
            case Action.OneEighty:
                reverse();
                break;
            case Action.Wait:
                break;
        }
        actionIdx = (actionIdx + 1) % nActions;
    }

    public bool nextAction() {
        return actionSequence[actionIdx] == Action.MoveForward;
    }

    public void setWait() {
        actionSequence[actionIdx] = Action.Wait;
    }

    public Vector3 getNextPosition() {
        if (actionSequence[actionIdx] == Action.MoveForward) {
            return transform.position + getDirection();
        }
        return transform.position;
    }

    private void turnLeft() {
        switch (direction) {
            case Direction.Up: direction = Direction.Left;
                setSprites(); break;
            case Direction.Left: direction = Direction.Down;
                setSprites(); break;
            case Direction.Down: direction = Direction.Right;
                setSprites(); break;
            case Direction.Right: direction = Direction.Up;
                setSprites(); break;
        }
    }

    private void turnRight() {
        switch (direction) {
            case Direction.Up: direction = Direction.Right;
                setSprites(); break;
            case Direction.Left: direction = Direction.Up;
                setSprites(); break;
            case Direction.Down: direction = Direction.Left;
                setSprites(); break;
            case Direction.Right: direction = Direction.Down;
                setSprites(); break;
        }
    }

    private void reverse() {
        switch (direction) {
            case Direction.Up: direction = Direction.Down;
                setSprites(); break;
            case Direction.Left: direction = Direction.Right;
                setSprites(); break;
            case Direction.Down: direction = Direction.Up;
                setSprites(); break;
            case Direction.Right: direction = Direction.Left;
                setSprites(); break;
        }
    }

    private void setSprites() {
        switch (direction) {
            case Direction.Right:
                idle = sc.rightIdle;
                walk1 = sc.right1;
                walk2 = sc.right2;
                break;
            case Direction.Up:
                idle = sc.upIdle;
                walk1 = sc.up1;
                walk2 = sc.up2;
                break;
            case Direction.Left:
                idle = sc.leftIdle;
                walk1 = sc.left1;
                walk2 = sc.left2;
                break;
            case Direction.Down:
                idle = sc.downIdle;
                walk1 = sc.down1;
                walk2 = sc.down2;
                break;
        }
        sr.sprite = idle;
    }

    private Vector3 getDirection() {
        switch (direction) {
            case Direction.Up: return Vector3.up;
            case Direction.Left: return Vector3.left;
            case Direction.Down: return Vector3.down;
            case Direction.Right: return Vector3.right;
        }
        return Vector3.zero;
    }

    private IEnumerator moveForward() {
        float elapsedTime = 0.0f;
        float nextStepTime = 0.0f;
        origin = transform.position;
        target = origin + getDirection();
        discretePos = target;
        bool alternate1 = false;
        bool alternate2 = false;
        while (elapsedTime < timeToMove) {
            if (elapsedTime > nextStepTime) {
                nextStepTime += timePerStep;
                transform.position = Vector3.Lerp(
                    origin, target, nextStepTime / timeToMove);
            }
            elapsedTime += Time.deltaTime;

            // Janky movement code
            alternate1 = !alternate1;
            if (alternate1) {
                alternate2 = !alternate2;
                if (alternate2) {
                    sr.sprite = walk1;
                }
                else {
                    sr.sprite = walk2;
                }
            }
            yield return null;
        }
        transform.position = target;
        sr.sprite = idle;
    }

    public void untintTiles() {
        foreach (Vector3 tilePos in tintedTiles) {
            ft.untintTile(tilePos);
        }
        tintedTiles.Clear();
    }

    // Also tints visible tiles
    public bool canSeePlayer(Vector3 playerPos) {
        bool playerVisible = false;
        Vector3 dir = getDirection();
        for (int i = 0; i <= viewDistance; i++) {
            Vector3 pos = discretePos + dir * i;
            if (wc.isColliding(pos)) break;
            ft.tintTile(pos);
            tintedTiles.Add(pos);
            if (playerPos == pos) playerVisible = true;
        }
        return playerVisible;
    }

    public void reset() {
        transform.position = startPos;
        discretePos = startPos;
        direction = startDir;
        actionIdx = 0;
        setSprites();
        untintTiles();
        canSeePlayer(Vector3.zero);
        for (int i = 0; i < actionSequence.Count; i++)
        {
            actionSequence[i] = sequenceCopy[i];
        }
    }
}
