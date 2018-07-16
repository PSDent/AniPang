using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class User : MonoBehaviour {
    const int INCREASE_SCORE = 25;

    GameManager gameMgr;
    Text scoreText;
    int score = 0;
    int targetScore = 0;

    private void Awake()
    {
        scoreText = GameObject.Find("Canvas").transform.Find("ScoreBar")
    .Find("ScoreNum").GetComponent<Text>();
        gameMgr = GetComponent<GameManager>();
    }

    private void FixedUpdate()
    {
        if (score < targetScore)
            score += INCREASE_SCORE;
        else
            score = targetScore;

        scoreText.text = score.ToString();
    }

    public void AddTargetScore(int val)
    {
        Debug.Log("Target Value : " + val);
        targetScore += val;
    }
}
