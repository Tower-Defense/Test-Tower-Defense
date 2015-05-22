using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class HighScores : MonoBehaviour
{
    public Text[] Dates;
    public Text[] Scores;


    public void GetScores()
    {
        var scores = new Dictionary<string, int>();
        int scoreCounter = PlayerPrefs.GetInt("ScoreCounter", 0);

        for (int i = 0; i < scoreCounter; i++)
        {
            string date = PlayerPrefs.GetString("ScoreDate" + i, "");
            int score = PlayerPrefs.GetInt("Score" + i, 0);

            scores.Add(date, score);
        }

        int j = 0;

        foreach(var score in scores.OrderByDescending(x=>x.Value))
        {
            if (j >= Dates.Length || j >= Scores.Length)
                break;
            Dates[j].text = score.Key;
            Scores[j].text = score.Value.ToString();

            j++;

        }
    }


}
