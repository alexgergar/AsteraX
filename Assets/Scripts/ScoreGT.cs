using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreGT : MonoBehaviour
{
    public Text scoreText;

    // Use this for initialization
    void Start()
    {
        if (scoreText == null)
        {
            scoreText = GetComponent<Text>();
        }
        scoreText.text = "0";
        EventBroker.UpdateScore += UpdateTheScore;
    }

    private void OnDisable()
    {
        EventBroker.UpdateScore -= UpdateTheScore;
    }

    private void UpdateTheScore(int score)
    {
        scoreText.text = score.ToString();
    }

}
