using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;

    public int playerLives = 3;
    public int totalScore;
    public string lastGamePlayed; 
    public bool wasLastGameWon;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.Log("Duplicate GameStateManager destroyed");
            Destroy(gameObject);
        }
    }

    public void ResetGameState()
    {
        playerLives = 3;
        totalScore = 0;
        lastGamePlayed = "";
        wasLastGameWon = false;
    }

    public void RegisterGameResult(bool won, string gameName)
    {
        lastGamePlayed = gameName;

        if (won)
        {
            totalScore += 2;
            wasLastGameWon = true;
        }
        else
        {
            playerLives--;
            totalScore += 1;
            wasLastGameWon = false;
        }
    }
}