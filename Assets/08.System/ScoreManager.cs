using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public int totalScore;
    public int killScore;
    public int technicalScore;

    private static ScoreManager _instance;

    public static ScoreManager instance
    {
        get
        {
            return _instance;
        }
        set
        {
            _instance = value;
        }
    }

    private void Awake()
    {
        if (_instance == null)
            _instance = this;

        totalScore = 0;
    }

    public string DisplayScore()
    {
        return "SCORE: " + totalScore;
    }

    public void AddKillScore(int score)
    {
        killScore += score;
        totalScore += score;
    }

    public void AddTechnicalScore(int score)
    {
        technicalScore += score;
        totalScore += score;
    }

    public string SummaryScore()
    {
        return "FINAL SCORE"
            + "\n\nTechnical Score : " + technicalScore
            + "\nKilling Score : " + killScore
            + "\nTotal Score : " + totalScore;
    }
}