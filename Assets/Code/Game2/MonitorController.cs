using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonitorController : MonoBehaviour
{
    public TextMesh monitorText;
    public SpriteRenderer leftArrow;
    public SpriteRenderer rightArrow;

    public Color normalColor = Color.white;
    public Color pressedColor = Color.gray;
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip buttonPressSound;
    
    private int currentValue = 1;
    public int CurrentValue => currentValue; 

    private bool isLeftPressed = false;
    private bool isRightPressed = false;

    void Update()
    {
        HandleArrowInput();

        leftArrow.color = isLeftPressed ? pressedColor : normalColor;
        rightArrow.color = isRightPressed ? pressedColor : normalColor;
    }

    void HandleArrowInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit.collider != null)
            {
                if (hit.collider.gameObject == leftArrow.gameObject)
                {
                    DecreaseValue();
                    isLeftPressed = true;
                }
                else if (hit.collider.gameObject == rightArrow.gameObject)
                {
                    IncreaseValue();
                    isRightPressed = true;
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isLeftPressed = false;
            isRightPressed = false;
        }
    }

    void DecreaseValue()
    {
        currentValue--;
        if (currentValue < 1) currentValue = 10;
        UpdateDisplay();
        PlayButtonSound();
    }

    void IncreaseValue()
    {
        currentValue++;
        if (currentValue > 10) currentValue = 1;
        UpdateDisplay();
        PlayButtonSound();
    }

    void UpdateDisplay()
    {
        if (monitorText != null)
        {
            monitorText.text = currentValue.ToString("00");
        }
    }

    void Start()
    {
        if (leftArrow.GetComponent<Collider2D>() == null)
            leftArrow.gameObject.AddComponent<BoxCollider2D>();

        if (rightArrow.GetComponent<Collider2D>() == null)
            rightArrow.gameObject.AddComponent<BoxCollider2D>();

        UpdateDisplay();
    }

    private void PlayButtonSound()
    {
        if (audioSource != null && buttonPressSound != null)
        {
            audioSource.PlayOneShot(buttonPressSound);
        }
    }
}
