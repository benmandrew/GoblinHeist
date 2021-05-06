using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
 
public class MainMenu : MonoBehaviour {

    private GameObject topScoresScreen;
    private GameObject startButton;
    private GameObject topScoresButton;

    private TopScores topScores;

    public void Start() {
        topScoresScreen = GameObject.Find("TopScores");
        startButton = GameObject.Find("StartButton");
        topScoresButton = GameObject.Find("TopScoresButton");
        topScoresScreen.SetActive(false);
    }

    public void showTopScores() {
        topScoresScreen.SetActive(true);
        startButton.SetActive(false);
        topScoresButton.SetActive(false);

        topScores = topScoresScreen.GetComponent<TopScores>();
        topScores.getTopScores();
    }

    public void hideTopScores() {
        topScoresScreen.SetActive(false);
        startButton.SetActive(true);
        topScoresButton.SetActive(true);
    }

    public void StartGame() {
        PlayerPrefs.SetInt("CurrLevel", 1);
        PlayerPrefs.SetInt("Score", 500);
        //PlayerPrefs.SetInt("Lives", 5);
        if (!PlayerPrefs.HasKey("HighScore")) {
            PlayerPrefs.SetInt("HighScore", 0);
        }
        SceneManager.LoadScene("Level1");
    }
}