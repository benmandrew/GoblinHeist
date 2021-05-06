using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopScores : MonoBehaviour {
    private Text nameContent;
    private Text scoreContent;
    private Text whenContent;

    private Leaderboard leaderboard;

    void initialise() {
        nameContent = GameObject.Find("End Other Names").GetComponent<Text>();
        scoreContent = GameObject.Find("End Other Scores").GetComponent<Text>();
        whenContent = GameObject.Find("End Other Dates").GetComponent<Text>();
        leaderboard = GameObject.Find("Leaderboard").GetComponent<Leaderboard>();
    }

    public void getTopScores() {
        initialise();
        StartCoroutine(leaderboard.getLeaderboard(nameContent, scoreContent, whenContent));
    }
}
