using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game2Manager : MonoBehaviour
{
    
    public RotateMachineKnob knob;
    public ToggleSwitch[] switches;
    public WaterLevelController[] sliders;
    public MonitorController monitor;

    
    private float correctKnobAngle;
    private bool[] correctSwitchStates;
    private float[] correctSliderValues;
    private int correctMonitorValue;

   
    [Header("Randomization Ranges")]
    public float minKnobAngle = 0f;
    public float maxKnobAngle = 360f;
    public float[] allowedKnobAngles = { 0f, -60f, -135f, -215f, -290f };
    public float minSliderValue = 0f;
    public float maxSliderValue = 1f;
    public int minMonitorValue = 1;
    public int maxMonitorValue = 10;

    [Header("Slider 1 Settings")]
    public float slider1MinPos = -3.291f;
    public float slider1MaxPos = -2.803f;
    public float slider1MinScale = 0.112347f;
    public float slider1MaxScale = 1.797553f;

    [Header("Slider 2 Settings")]
    public float slider2MinPos = -3.208f;
    public float slider2MaxPos = -2.769f;
    public float slider2MinScale = 0.1607568f;
    public float slider2MaxScale = 1.665691f;


    [Header("Instruction Prefabs")]
    public Transform instructionKnob;
    public SpriteRenderer[] instructionSwitches;
    public Transform[] instructionSliderSurfaces;
    public Transform[] instructionSliderMeshes;
    public TextMesh instructionMonitorText;

    [Header("Instruction Switch Sprites")]
    public Sprite switchUpSprite;   
    public Sprite switchDownSprite;

    [Header("GO Button Settings")]
    public SpriteRenderer goButton; 
    public Color normalColor = Color.white;
    public Color pressedColor = Color.gray;
    private bool isButtonPressed = false;

    private bool _gameEnded = false;
    public string menuSceneName = "Menu";

    void OnEnable()
    {
        if (TimerManager.Instance != null)
        {
            TimerManager.Instance.OnTimerFinished += HandleTimerFinished;
        }
    }

    void OnDisable()
    {
        if (TimerManager.Instance != null)
        {
            TimerManager.Instance.OnTimerFinished -= HandleTimerFinished;
        }
    }

    private void Start()
    {
        
        _gameEnded = false;

        if (GameStateManager.Instance == null)
        {
            Debug.Log("GameStateManager not found, creating temporary instance.");
            GameObject gsmObj = new GameObject("GameStateManager");
            gsmObj.AddComponent<GameStateManager>();
        }

        correctSwitchStates = new bool[switches.Length];
        correctSliderValues = new float[sliders.Length];
        RandomizeCorrectValues(); 
        LogCorrectValues();
    }

    IEnumerator StartTimerWithCheck()
    {
        if (TimerManager.Instance == null)
        {
            GameObject timerObj = new GameObject("TimerManager");
            timerObj.AddComponent<TimerManager>();
        }

        yield return null; 
        TimerManager.Instance.StartTimer();
    }

    private void Update()
    {
        if (_gameEnded) return;
        HandleButtonInput();
        goButton.color = isButtonPressed ? pressedColor : normalColor;
    }

    void HandleTimerFinished()
    {
        if (_gameEnded) return;

        HandleGameOver();
    }

    void HandleGameWin()
    {
        _gameEnded = true;
        Debug.Log("Player Wins!");
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.RegisterGameResult(true, "Game2");
        }
        else
        {
            Debug.LogWarning("GameStateManager instance not found");
        }
        SceneManager.LoadScene(menuSceneName);
    }

    void HandleGameOver()
    {
        _gameEnded = true;
        Debug.Log("Game Over!");
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.RegisterGameResult(false, "Game2");
        }
        else
        {
            Debug.LogWarning("GameStateManager instance not found");
        }
        SceneManager.LoadScene(menuSceneName);
    }

    private void RandomizeCorrectValues()
    {
        correctKnobAngle = allowedKnobAngles[UnityEngine.Random.Range(0, allowedKnobAngles.Length)];

        for (int i = 0; i < switches.Length; i++)
        {
            correctSwitchStates[i] = UnityEngine.Random.value > 0.5f;
        }

        for (int i = 0; i < sliders.Length; i++)
        {
            correctSliderValues[i] = UnityEngine.Random.Range(minSliderValue, maxSliderValue);
        }

        correctMonitorValue = UnityEngine.Random.Range(minMonitorValue, maxMonitorValue + 1);
        ApplyValuesToInstructionParts();
    }

    private void LogCorrectValues()
    {
        string logMessage = "CORRECT SETTINGS\n";
        logMessage += $"Knob: {correctKnobAngle:0}°\n";

        for (int i = 0; i < switches.Length; i++)
        {
            logMessage += $"Switch {i + 1}: {(correctSwitchStates[i] ? "UP" : "DOWN")}\n";
        }

        for (int i = 0; i < sliders.Length; i++)
        {
            logMessage += $"Slider {i + 1}: {correctSliderValues[i]:0.00}\n";
        }

        logMessage += $"Monitor: {correctMonitorValue:00}\n";
        Debug.Log(logMessage);
        Debug.Log("Press SPACE to check your solution");
    }

    private void CheckPlayerInput()
    {
        if (_gameEnded) return;
        bool allCorrect = true;
        System.Text.StringBuilder feedbackMessage = new System.Text.StringBuilder();
        feedbackMessage.AppendLine("CHECKING SOLUTION");

        float knobAngle = knob.transform.eulerAngles.z;
        float normalizedPlayer = knobAngle % 360;
        if (normalizedPlayer < 0) normalizedPlayer += 360;
        float normalizedCorrect = correctKnobAngle % 360;
        if (normalizedCorrect < 0) normalizedCorrect += 360;
        bool knobCorrect = Mathf.Abs(Mathf.DeltaAngle(normalizedPlayer, normalizedCorrect)) <= 5f;

        feedbackMessage.AppendLine($"Knob: {knobAngle:0}° (Expected: {correctKnobAngle:0}°) - {(knobCorrect ? "CORRECT" : "WRONG")}");
        allCorrect &= knobCorrect;

        for (int i = 0; i < switches.Length; i++)
        {
            bool switchCorrect = switches[i].IsUp == correctSwitchStates[i];
            feedbackMessage.AppendLine($"Switch {i + 1}: {(switches[i].IsUp ? "UP" : "DOWN")} (Expected: {(correctSwitchStates[i] ? "UP" : "DOWN")}) - {(switchCorrect ? "CORRECT" : "WRONG")}");
            allCorrect &= switchCorrect;
        }

       
        for (int i = 0; i < sliders.Length; i++)
        {
            bool sliderCorrect = Approximately(sliders[i].CurrentFillAmount, correctSliderValues[i], 0.1f);
            feedbackMessage.AppendLine($"Slider {i + 1}: {sliders[i].CurrentFillAmount:0.00} (Expected: {correctSliderValues[i]:0.00}) - {(sliderCorrect ? "CORRECT" : "WRONG")}");
            allCorrect &= sliderCorrect;
        }

        bool monitorCorrect = monitor != null && monitor.CurrentValue == correctMonitorValue;
        feedbackMessage.AppendLine($"Monitor: {(monitor != null ? monitor.CurrentValue.ToString("00") : "N/A")} (Expected: {correctMonitorValue:00}) - {(monitorCorrect ? "CORRECT" : "WRONG")}");
        allCorrect &= monitorCorrect;

        feedbackMessage.AppendLine($"\n RESULT: {(allCorrect ? "SUCCESS!" : "TRY AGAIN")} ===");
        Debug.Log(feedbackMessage.ToString());

        if (allCorrect)
        {
            HandleGameWin();
        }
        else
        {
            HandleGameOver();
        }
    }

    private bool Approximately(float a, float b, float tolerance)
    {
        return Mathf.Abs(a - b) <= tolerance;
    }

    private void ApplyValuesToInstructionParts()
    {
        
        if (instructionKnob != null)
            instructionKnob.eulerAngles = new Vector3(0, 0, correctKnobAngle);

        
        for (int i = 0; i < instructionSwitches.Length; i++)
        {
            if (i < correctSwitchStates.Length && instructionSwitches[i] != null)
            {
                instructionSwitches[i].sprite = correctSwitchStates[i]
                    ? switchUpSprite   
                    : switchDownSprite; 
            }
        }

        for (int i = 0; i < instructionSliderSurfaces.Length; i++)
        {
            if (i >= correctSliderValues.Length || instructionSliderSurfaces[i] == null)
                continue;

            float minPos, maxPos;
            if (i == 0) 
            {
                minPos = slider1MinPos;
                maxPos = slider1MaxPos;
            }
            else
            {
                minPos = slider2MinPos;
                maxPos = slider2MaxPos;
            }

            float surfaceY = Mathf.Lerp(minPos, maxPos, correctSliderValues[i]);
            instructionSliderSurfaces[i].localPosition = new Vector3(
                instructionSliderSurfaces[i].localPosition.x,
                surfaceY,
                instructionSliderSurfaces[i].localPosition.z
            );
        }

        for (int i = 0; i < instructionSliderMeshes.Length; i++)
        {
            if (i >= correctSliderValues.Length || instructionSliderMeshes[i] == null)
                continue;

            float minScale, maxScale;
            if (i == 0) 
            {
                minScale = slider1MinScale;
                maxScale = slider1MaxScale;
            }
            else
            {
                minScale = slider2MinScale;
                maxScale = slider2MaxScale;
            }

            float scaleY = Mathf.Lerp(minScale, maxScale, correctSliderValues[i]);
            instructionSliderMeshes[i].localScale = new Vector3(
                instructionSliderMeshes[i].localScale.x,
                scaleY,
                instructionSliderMeshes[i].localScale.z
            );
        }

       
        if (instructionMonitorText != null)
            instructionMonitorText.text = correctMonitorValue.ToString("00");


    }
    private void OnSceneReloaded()
    {
        
        _gameEnded = false;
        isButtonPressed = false;

       
        correctSwitchStates = new bool[switches.Length];
        correctSliderValues = new float[sliders.Length];
        RandomizeCorrectValues();

    }
    void HandleButtonInput()
    {
        if (_gameEnded) return;
        
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == goButton.gameObject)
            {
                CheckPlayerInput();
                isButtonPressed = true;
            }
        }

        
        if (Input.GetMouseButtonUp(0))
        {
            isButtonPressed = false;
        }
    }
}