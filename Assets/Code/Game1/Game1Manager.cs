using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game1Manager : MonoBehaviour
{
    [Header("Eyelid Controllers")]
    public EyelidController leftEyeController;
    public EyelidController rightEyeController;

    private bool _gameEnded;

    public string menuSceneName = "Menu";

    void OnEnable()
    {
        if (TimerManager.Instance != null)
        {
            TimerManager.Instance.OnTimerFinished += HandleTimerFinished;
        }
        EyelidController.OnAllEyesClosed += HandleGameOver;
    }

    void OnDisable()
    {
        if (TimerManager.Instance != null)
        {
            TimerManager.Instance.OnTimerFinished -= HandleTimerFinished;
        }
        EyelidController.OnAllEyesClosed -= HandleGameOver;
    }

    private void OnSceneReloaded()
    {
        _gameEnded = false;
    }

    
    void Start()
    {
       
    }


    void HandleTimerFinished()
    {
        Debug.Log("TimerFinished event received!");

        if (leftEyeController == null || rightEyeController == null)
        {
            Debug.LogError("Eyelid controllers are not assigned!");
            HandleGameOver(); 
            return;
        }

        if (!leftEyeController.BothEyelidsClosed || !rightEyeController.BothEyelidsClosed)
        {
            Debug.Log("we won???");
            HandleGameWin();
        }
        else
        {
            HandleGameOver();
        }
    }

    void HandleGameWin()
    {
        _gameEnded = true;
        Debug.Log("Player Wins!");

      
        GameStateManager.Instance.RegisterGameResult(true, "Game1");
        SceneManager.LoadScene(menuSceneName);
    }

    void HandleGameOver()
    {
        _gameEnded = true;
        Debug.Log("Game Over!");

        GameStateManager.Instance.RegisterGameResult(false, "Game1");
        SceneManager.LoadScene(menuSceneName);
    }
}