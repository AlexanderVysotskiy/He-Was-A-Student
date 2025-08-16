using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;
    public AudioClip MainMenu;
    public AudioClip ClickButton;
    void Start()
    {
        musicSource.clip = MainMenu;
        musicSource.Play();
    }

    
    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }
    
    public void ExitButton()
    {
        PlaySFX(ClickButton);
        Application.Quit();
    }

    
    public void StartGame()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.ResetGameState();
        }
        PlaySFX(ClickButton);
        SceneManager.LoadScene("Game1");
    }
}
