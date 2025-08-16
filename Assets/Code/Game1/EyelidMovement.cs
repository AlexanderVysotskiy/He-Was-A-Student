using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyelidMovement : MonoBehaviour
{
    public float minSpeed = 1f;
    public float maxSpeed = 3f;
    public bool isTopEyelid = false;
    public float dragSensitivity = 0.01f;
    public float minScale = 0.5f;
    public float maxScale = 5f;
    public bool isLocked = false;
    public float speed;

    
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip dragSound;

    private bool isDragging = false;
    private Vector3 initialScale;
    private Vector3 initialMousePosition;

    void Start()
    {
        speed = Random.Range(minSpeed, maxSpeed);
    }

    void Update()
    {
        if (isLocked) return;

        if (!isDragging)
        {
            float newScaleY = transform.localScale.y + speed * Time.deltaTime;
            newScaleY = Mathf.Clamp(newScaleY, minScale, maxScale);
            transform.localScale = new Vector3(transform.localScale.x, newScaleY, transform.localScale.z);
        }
    }

    void OnMouseDown()
    {
        if (isLocked) return;
        isDragging = true;
        initialScale = transform.localScale;
        initialMousePosition = Input.mousePosition;

        if (audioSource != null && dragSound != null)
        {
            audioSource.clip = dragSound;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    void OnMouseDrag()
    {
        if (isLocked) return;
        Vector3 currentMousePosition = Input.mousePosition;
        float deltaY = currentMousePosition.y - initialMousePosition.y;

        float scaleDelta = isTopEyelid ? -deltaY * dragSensitivity : deltaY * dragSensitivity;
        float newScaleY = Mathf.Clamp(initialScale.y + scaleDelta, minScale, maxScale);

        transform.localScale = new Vector3(transform.localScale.x, newScaleY, transform.localScale.z);
    }

    void OnMouseUp()
    {
        isDragging = false;
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    public void MultiplySpeed(float multiplier)
    {
        speed *= multiplier;
    }
}