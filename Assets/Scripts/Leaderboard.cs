using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Leaderboard : MonoBehaviour {
    private string leaderboardHost = "http://vm.mxbi.net:6969/";

    public IEnumerator submitScore(string name, int score) {
        string url = leaderboardHost + "/submit/" + score.ToString() + "/" + name;
        UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        // ui.updateSubmitStatus(SubmitStatus.);
    }

    public IEnumerator getLeaderboard(Text namesObj, Text scoresObj, Text datesObj) {
        string url = leaderboardHost + "/top10";

        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SendWebRequest();

        while (!request.isDone) yield return null;

        string res = request.downloadHandler.text;

        // Dictionary<string, int> top10 = new Dictionary<string, int>();
        List<string[]> top10 = new List<string[]>();

        foreach (string line in res.Split('\n')) {
            if (line.Length > 0) {
                string[] user = line.Split(':');
                top10.Add(user);
            }
        } 

        string names = "";
        string scores = "";
        string whens = "";
        foreach (string[] tuple in top10) {
            names += tuple[0] + "\n";
            scores += tuple[1] + "\n";
            whens += tuple[2] + "\n";
        }
        namesObj.text = names;
        scoresObj.text = scores;
        datesObj.text = whens;
    }
}
