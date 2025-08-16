using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleSwitch : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip switchSound;

    [SerializeField] private Sprite upSprite;
    [SerializeField] private Sprite downSprite;

    private SpriteRenderer spriteRenderer;
    private bool isUp = true;

    public bool IsUp => isUp;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = isUp ? upSprite : downSprite;
    }

    private void OnMouseDown()
    {
        ToggleSprite();
    }

    private void ToggleSprite()
    {
        isUp = !isUp;
        spriteRenderer.sprite = isUp ? upSprite : downSprite;
        if (audioSource != null && switchSound != null)
        {
            audioSource.PlayOneShot(switchSound);
        }
    }
}