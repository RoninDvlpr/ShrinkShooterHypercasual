using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] CanvasGroup scoreCanavsGroup;
    [SerializeField] TextMeshProUGUI scoreText;

    void OnEnable()
    {
        GameManager.OnLevelReset += UpdateScore;
    }

    void OnDisable()
    {
        GameManager.OnLevelReset -= UpdateScore;    
    }

    void UpdateScore()
    {
        if (GameManager.NumOfCompletedLevels == 0)
            scoreCanavsGroup.alpha = 0;
        else
        {
            scoreText.text = "Completed zones: " + GameManager.NumOfCompletedLevels.ToString();
            scoreCanavsGroup.alpha = 1;
        }
    }
}
