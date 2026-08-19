using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] GameObject failUI;

    [Header("Core Actors")]
    [SerializeField] PlayerController player;
    [SerializeField] ObstacleSpawner spawner;
    [SerializeField] Target target;

    [Header("Transitions")]
    [SerializeField] CanvasGroup fadeScreen;
    [SerializeField] float fadeDuration = 0.5f;

    public static int NumOfCompletedLevels { get; private set; }
    /// <summary>
    /// Raised upon level completion. The int parameter represents the number of completed levels.
    /// </summary>
    public static event Action OnLevelReset;



    void OnEnable()
    {
        player.OnTargetReached += HandleVictory;
        player.OnLevelFailed += HandleDefeat;
    }

    void OnDisable()
    {
        player.OnTargetReached -= HandleVictory;
        player.OnLevelFailed -= HandleDefeat;
    }

    void HandleVictory()
    {
        NumOfCompletedLevels++;
        ResetLevel();
    }

    void HandleDefeat()
    {
        failUI.SetActive(true);
    }

    public void RestartGame()
    {
        NumOfCompletedLevels = 0;
        ResetLevel();
    }

    void ResetLevel()
    {
        player.Reset();
        target.Reset();
        spawner.Reset();
        OnLevelReset?.Invoke();
    }
}
