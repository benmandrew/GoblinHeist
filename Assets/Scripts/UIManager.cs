using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {

    public enum SubmitStatus {Success, NoName, NetworkFailure};

    private GameManager gm;
    private TopScores ts;

    private Text scoreText;
    //private Text livesText;

    private GameObject finishScreen;    
    private Text titleText;
    private InputField nameInput;
    private Text submitStatusText;
    private Text finishScoreText;

    private bool scoreSubmitted = false;

    private Color successColour;
    private Color failureColour;

    private GameObject tutorial;

    void Awake() {
        gm = GameObject.Find("Game Manager").GetComponent<GameManager>();
        ts = GameObject.Find("TopScores").GetComponent<TopScores>();
        finishScreen = GameObject.Find("FinishScreen");
        tutorial = GameObject.Find("Tutorial");
        titleText = GameObject.Find("GameOverTitle").GetComponent<Text>();
        scoreText = GameObject.Find("Score Text").GetComponent<Text>();
        //livesText = GameObject.Find("Lives Text").GetComponent<Text>();
        nameInput = GameObject.Find("InputField").GetComponent<InputField>();
        nameInput.onValidateInput +=
            delegate (string s, int i, char c) {
                if (s.Length >= 3) return '\0';
                c = char.ToUpper(c);
                return char.IsLetter(c) ? c : '\0';
            };
        submitStatusText = GameObject.Find("SubmitStatus").GetComponent<Text>();
        submitStatusText.gameObject.SetActive(false);
        finishScoreText = GameObject.Find("End Player Score").GetComponent<Text>();

        successColour = new Color(0.176f, 0.557f, 0.125f);
        failureColour = new Color(0.8f, 0.169f, 0.169f);
        finishScreen.SetActive(false);
        if (SceneManager.GetActiveScene().name != "Level1") {
            tutorial.SetActive(false);
        }
    }

    public void finishLevel() {
        tutorial.SetActive(false);
    }

    public IEnumerator enableGameOver() {
        float elapsedTime = 0.0f;
        while (elapsedTime < 0.5f) {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        finishScreen.SetActive(true);
        titleText.text = "GAME OVER!";
        enableFinish();
    }

    public IEnumerator enableWin() {
        float elapsedTime = 0.0f;
        while (elapsedTime < 0.5f) {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        finishScreen.SetActive(true);
        titleText.text = "YOU WIN!";
        enableFinish();
    }

    private void enableFinish() {
        finishScoreText.text = "YOUR " + scoreText.text;
        scoreText.gameObject.SetActive(false);
        //livesText.gameObject.SetActive(false);
        ts.getTopScores();
    }

    public void submitScore() {
        if (scoreSubmitted) {
            return;
        }
        string name = nameInput.text;
        if (name.Length == 0) {
            updateSubmitStatus(SubmitStatus.NoName);
            return;
        }
        StartCoroutine(gm.submitScore(name));
        updateSubmitStatus(SubmitStatus.Success);
    }

    public void updateScoreText(int value, bool doBlinkColor=false, Color newColor=new Color()) {
        scoreText.text = "SCORE: " + value;
        if (doBlinkColor) {
            StartCoroutine(blinkColor(newColor));
        } 
    }

    public IEnumerator blinkColor(Color newColor) {
        scoreText.color = newColor;
        yield return new WaitForSeconds(0.5f);
        scoreText.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }

    /*public void updateLivesText(int value) {
        livesText.text = "LIVES: " + value;
    }*/

    private void updateSubmitStatus(SubmitStatus status) {
        submitStatusText.gameObject.SetActive(true);
        switch (status) {
            case SubmitStatus.Success:
                submitStatusText.color = successColour;
                submitStatusText.text = "SUBMITTED";
                break;
            case SubmitStatus.NoName:
                submitStatusText.color = failureColour;
                submitStatusText.text = "ENTER NAME";
                break;
            case SubmitStatus.NetworkFailure:
                submitStatusText.color = failureColour;
                submitStatusText.text = "NETWORK\nERROR";
                break;
        }
    }

    public void goToMainMenu() {
        SceneManager.LoadScene("MainMenu");
    }
}
