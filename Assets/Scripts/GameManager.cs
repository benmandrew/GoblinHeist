using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    [SerializeField]
    private int playerScore = 0;

    //[SerializeField]
    //private int playerLives = 5;

    [SerializeField]
    private GameObject player;

    public bool isResetting = false;
    public bool isPaused = false;

    [SerializeField]
    private TreasureManager tm;

    // [SerializeField]
    public Leaderboard leaderboard;

    [SerializeField]
    private int currentLevel = 1;
    [SerializeField]
    private int numLevels = 1;

    [SerializeField]
    private bool reset = false;

    private GameObject entrance;
    private GameObject exit;

    private WallCollider wc;
    private UIManager ui;

    private List<Enemy> enemies;

    private float timeToScoreDecrement = 1.0f / 4.0f; // points deducted per second
    private float elapsedTime = 0.0f;

    private AudioManger am;

    public Color dark;
    public Color transparent;
    private SpriteRenderer dc;

    public Color deathColor;
    public Color addScoreColor;

    private void Awake()
    {
        isPaused = true;
        dc = GameObject.Find("Dark Cover").GetComponent<SpriteRenderer>();
        dc.color = dark;
        StartCoroutine(startLevel());
    }

    private IEnumerator startLevel() {
        Color inc = (transparent - dark) / 64;
        yield return new WaitForSeconds(0.2f);
        while (dc.color != transparent)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            dc.color += inc;
        }
        isPaused = false;
    }

    void Start() {
        am = gameObject.GetComponent<AudioManger>();
        /*if (!PlayerPrefs.HasKey("Lives"))
        {
            playerLives = 3;
        }
        else
        {
            playerLives = PlayerPrefs.GetInt("Lives");
        }*/
        if (!PlayerPrefs.HasKey("Score"))
        {
            playerScore = 0;
        }
        else
        {
            playerScore = PlayerPrefs.GetInt("Score");
        }
        player = GameObject.FindGameObjectWithTag("Player");
        tm = GameObject.Find("Game Manager").GetComponent<TreasureManager>();
        leaderboard = GameObject.Find("Leaderboard").GetComponent<Leaderboard>();
        entrance = GameObject.Find("Stairs Up");
        player.transform.position = entrance.transform.position + new Vector3(1, -0.5f, 0);
        exit = GameObject.Find("Stairs Down");

        wc = GameObject.Find("Walls").GetComponent<WallCollider>();
        ui = GameObject.Find("Canvas").GetComponent<UIManager>();
        ui.updateScoreText(playerScore);
        //ui.updateLivesText(playerLives);

        // if (PlayerPrefs.HasKey("Score")) {
        //     playerScore = PlayerPrefs.GetInt("Score");
        // } else {
        //     PlayerPrefs.SetInt("Score", 0);
        // }

        if (PlayerPrefs.HasKey("CurrLevel")) {
            currentLevel = PlayerPrefs.GetInt("CurrLevel");
        }

        getEnemies();
    }

    private void getEnemies() {
        enemies = new List<Enemy>();
        GameObject enemyContainer = GameObject.Find("Enemies");
        foreach(Transform child in enemyContainer.transform) {
            enemies.Add(child.gameObject.GetComponent<Enemy>());
        }
    }

    private void Update() {
        if (reset) {
            reset = false;
            PlayerPrefs.SetInt("Score", 0);
            PlayerPrefs.SetInt("CurrLevel", 1);
            playerScore = PlayerPrefs.GetInt("Score");
            currentLevel = PlayerPrefs.GetInt("CurrLevel");
        }

        if (playerScore > 0 && !isPaused) {
            if (elapsedTime > timeToScoreDecrement) {
                playerScore--;
                elapsedTime = 0;
                ui.updateScoreText(playerScore);
            }
            elapsedTime += Time.deltaTime;
        }
    }

    public void triggeredUpdate(Vector3 newPlayerPos) {
        am.Play("move");
        GameObject treasure = tm.playerOnTreasure(newPlayerPos);
        if (treasure != null) {
            am.Play("treasure");
            if (treasure.tag == "Treasure") {
                // Add the score from getting the treasure
                addScore(tm.getTreasureScore());
                // Destroy the game object
                treasure.SetActive(false);
            } else if (treasure.tag == "Key") {
                // We found the key!                
                // hasKey = true;
                // Debug.Log("Has key!");
                // StartCoroutine(leaderboard.submitScore("test", 100));
                treasure.SetActive(false);
            }
        }
        if (newPlayerPos == exit.transform.position) {
            ui.finishLevel();
            levelComplete();
        }
        bool playerSeen = false;
        foreach (Enemy enemy in enemies) {
            enemy.applyAction();
            enemy.untintTiles();
        }
        foreach (Enemy enemy in enemies) {
            if (enemy.canSeePlayer(newPlayerPos)) {
                playerSeen = true;
            }
        }
        if (playerSeen) loseLife();
    }

    private void loseLife() {
        am.Play("seen");
        if (playerScore <= 100) {
            playerScore = 0;
        }
        else {
            playerScore -= 100;
        }
        ui.updateScoreText(playerScore, true, deathColor);
        //playerLives--;
        /*if (playerLives <= 0) {
            loseGame();
        }*/
        //else {
        //ui.updateLivesText(playerLives);
        StartCoroutine(resetLevel());
        //}
    }

    private void loseGame() {
        isPaused = true;
        StartCoroutine(ui.enableGameOver());
    }

    private IEnumerator resetLevel() {
        isResetting = true;
        float elapsedTime = 0.0f;
        while (elapsedTime < 0.75f) {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        player.GetComponent<PlayerMovement>().playerReset();
        
        foreach (Enemy enemy in enemies) {
            enemy.reset();
        }
        tm.enableAll();
        isResetting = false;
    }

    public bool spaceWillBeFree(Vector3 newPlayerPos) {
        if (wc.isColliding(newPlayerPos)) return false;
        foreach (Enemy enemy in enemies) {
            if (newPlayerPos == enemy.getNextPosition() && !enemy.nextAction()) {
                return false;
            }
            if (newPlayerPos == enemy.getNextPosition()) {
                enemy.setWait();
            }
        }
        return true;
    }

    public void addScore(int score) {
        playerScore += score;
        ui.updateScoreText(playerScore, true, addScoreColor);
    }

    public void setScore(int score) {
        playerScore = score;
        ui.updateScoreText(playerScore);
    }

    public int getScore() {
        return playerScore;
    }

    public void levelComplete() 
    {
        isPaused = true;
        PlayerPrefs.SetInt("Score", playerScore);
        PlayerPrefs.SetInt("CurrLevel", currentLevel + 1);
        //PlayerPrefs.SetInt("Lives", playerLives);
        // Load scene of currentLevel + 1 (or the new CurrLevel in PlayerPrefs)
        if (currentLevel + 1 <= numLevels)
        {
            StartCoroutine(nextLevel());
            
        }
        else {
            StartCoroutine(ui.enableWin());
        }
    }

    public IEnumerator nextLevel() {
        Color inc = (dark - transparent)/64;
        while (dc.color != dark) {
            yield return new WaitForSeconds(Time.deltaTime);
            dc.color += inc;
        }
        am.Play("stairs");
        yield return new WaitForSeconds(1.5f);
        string levelName = "Level" + (currentLevel + 1);
        SceneManager.LoadScene(levelName);
        // Start without key
        // hasKey = false;
    }

    public IEnumerator submitScore(string name) {
        yield return StartCoroutine(leaderboard.submitScore(name, playerScore));
    }
}
