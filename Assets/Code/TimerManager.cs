using System.Collections;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TimerManager : MonoBehaviour
{
    public static TimerManager Instance { get; private set; }

    public delegate void TimerAction();
    public event TimerAction OnTimerStarted;
    public event TimerAction OnTimerFinished;

    [Header("Timer Settings")]
    public float gameDuration = 30f;
    public Slider timerSlider;

    private float _timeRemaining;
    private bool _timerRunning;
    private bool _isInitialized;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
            InitializeTimer();
            _isInitialized = true;
        }
        else
        {
            Destroy(gameObject);
        }
    }
        public void InitializeTimer()
        {
            _timeRemaining = gameDuration;
            if (timerSlider != null) timerSlider.value = 1f;
            _timerRunning = false;
        }

        void OnDestroy()
        {
            if (Instance == this)
            {
                SceneManager.sceneLoaded -= OnSceneLoaded;
            }
        }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu" || scene.name == "Menu") return;
        
        StartCoroutine(InitializeSceneTimer());
    }


    private void FindTimerSlider()
        {
            timerSlider = GameObject.FindGameObjectWithTag("TimerSlider")?.GetComponent<Slider>();

            if (timerSlider != null)
            {
                timerSlider.value = 1f;
                Debug.Log("TimerSlider found");
            }
            else
            {
                Debug.LogError("TimerSlider not found");
            }
        }

        void Update()
        {
            if (!_isInitialized || !_timerRunning || timerSlider == null) return;

            _timeRemaining -= Time.deltaTime;
            timerSlider.value = _timeRemaining / gameDuration;

            if (_timeRemaining <= 0)
        {
            Debug.Log("Timer finished! Invoking event...");
            StopTimer();
                OnTimerFinished?.Invoke();
            }
        }

    public void ResetTimer()
    {
        _timeRemaining = gameDuration;
        _timerRunning = false;

        if (timerSlider != null)
        {
            timerSlider.value = 1f;
            Debug.Log("Timer reset with slider");
        }
        else
        {
            Debug.LogWarning("ResetTimer called with null slider");
        }
    }
    private IEnumerator InitializeSceneTimer()
    {
        yield return new WaitForEndOfFrame();
        FindTimerSlider();
        ResetTimer();

        
        StopTimer();
        _timeRemaining = gameDuration;

        if (SceneManager.GetActiveScene().name.StartsWith("Game"))
        {
            StartTimer();
        }
    }

    public void StopTimer()
    {
        _timerRunning = false;
    }


    public void StartTimer()
    {
        if (timerSlider == null)
        {
            Debug.LogWarning("StartTimer called with null slider!");
            FindTimerSlider();
        }

        _timerRunning = true;
        _timeRemaining = gameDuration;

        if (timerSlider != null)
        {
            timerSlider.value = 1f;
        }

        OnTimerStarted?.Invoke();
    }
    public bool IsRunning => _timerRunning;
}