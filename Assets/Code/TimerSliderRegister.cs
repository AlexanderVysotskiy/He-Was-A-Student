using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerSliderRegister : MonoBehaviour
{
    void Awake()
    {
        if (TimerManager.Instance != null && GetComponent<Slider>() != null)
        {
            TimerManager.Instance.timerSlider = GetComponent<Slider>();
            TimerManager.Instance.ResetTimer();
            Debug.Log($"Registered slider in {gameObject.scene.name}", gameObject);
        }
    }



    void OnDestroy()
    {
        if (TimerManager.Instance != null)
        {
            TimerManager.Instance.timerSlider = null;
        }
    }
}