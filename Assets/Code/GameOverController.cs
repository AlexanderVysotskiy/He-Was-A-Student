using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource; 
    public AudioClip game1WinSound;
    public AudioClip game1LoseSound;
    public AudioClip game2WinSound;
    public AudioClip game2LoseSound;
    public AudioClip buttonClickSound;

    [Header("UI")]
    public Image[] heartIcons;
    public Text scoreText;
    public Button nextGameButton; 
    public Button mainMenuButton; 
    public Text resultText;
    public Text finalScoreText;

    [Header("Text")]
    public string winMessage = "Victory!";
    public string game1LoseMessage = "Game 1 Failed!";
    public string game2LoseMessage = "Game 2 Failed!";

    void Start()
    {
        
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        PlayOutcomeSound();
        SetupButtonSFX();

        if (GameStateManager.Instance == null)
        {
            Debug.LogError("GameStateManager not found!");
            return;
        }

        bool gameOver = GameStateManager.Instance.playerLives <= 0;

        UpdateHeartsDisplay();
        scoreText.text = $"SCORE: {GameStateManager.Instance.totalScore}";

        if (gameOver)
        {
            ShowFinalGameOver();
        }
        else
        {
            ShowRegularResult(GameStateManager.Instance.wasLastGameWon);
        }
    }

    void UpdateHeartsDisplay()
    {
        for (int i = 0; i < heartIcons.Length; i++)
        {
            heartIcons[i].gameObject.SetActive(i < GameStateManager.Instance.playerLives);
        }
    }

    void ShowFinalGameOver()
    {
        resultText.gameObject.SetActive(false);
        finalScoreText.gameObject.SetActive(true);
        nextGameButton.gameObject.SetActive(false); 

        finalScoreText.text = $"FINAL SCORE: {GameStateManager.Instance.totalScore}";
    }

    void ShowRegularResult(bool isGameOver)
    {
        finalScoreText.gameObject.SetActive(false);
        nextGameButton.gameObject.SetActive(true);

        if (isGameOver)
        {
            resultText.text = winMessage;
        }
        else
        {
            resultText.text = GameStateManager.Instance.lastGamePlayed == "Game1"
                ? game1LoseMessage
                : game2LoseMessage;
        }
    }

    public void LoadNextGame()
    {
        
        if (TimerManager.Instance != null)
        {
            Destroy(TimerManager.Instance.gameObject);
        }
       

       
        SceneManager.LoadScene(
            GameStateManager.Instance.lastGamePlayed == "Game1" ? "Game2" : "Game1",
            LoadSceneMode.Single
        );
    }

    private IEnumerator LoadSceneFresh(string sceneName)
    {
       
        Scene currentScene = SceneManager.GetActiveScene();
        yield return SceneManager.UnloadSceneAsync(currentScene);

        
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));

        
        foreach (GameObject root in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            root.BroadcastMessage("OnSceneReloaded", SendMessageOptions.DontRequireReceiver);
        }
    }

    public void ReturnToMainMenu()
    {
        if (GameStateManager.Instance != null)
        {
            Destroy(GameStateManager.Instance.gameObject);
        }
        if (TimerManager.Instance != null)
        {
            Destroy(TimerManager.Instance.gameObject);
        }
        SceneManager.LoadScene("MainMenu");
    }

    void PlayOutcomeSound()
    {
        if (GameStateManager.Instance == null || audioSource == null) return;

        AudioClip clip = null;

        if (GameStateManager.Instance.lastGamePlayed == "Game1")
        {
            clip = GameStateManager.Instance.wasLastGameWon ? game1WinSound : game1LoseSound;
        }
        else if (GameStateManager.Instance.lastGamePlayed == "Game2")
        {
            clip = GameStateManager.Instance.wasLastGameWon ? game2WinSound : game2LoseSound;
        }

        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
        
    }

    void SetupButtonSFX()
    {
        if (buttonClickSound == null || audioSource == null) return;

        Button[] buttons = GetComponentsInChildren<Button>(true);
        foreach (Button btn in buttons)
        {
            btn.onClick.AddListener(() => {
                if (buttonClickSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(buttonClickSound);
                }
            });
        }
    }
}