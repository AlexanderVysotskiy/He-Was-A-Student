using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class WaterLevelController : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip liquidUpSound;
    [SerializeField] private AudioClip liquidDownSound;

    private float lastFillAmount;
    private bool wasIncreasing;

    [Header("Water References")]
    public Transform waterSurface;   
    public Transform waterMesh;      

    [Header("Position Settings")]
    public float minSurfaceY = 1.391f;
    public float maxSurfaceY = 2.28f;

    [Header("Scale Settings")]
    public float minMeshScaleY = 0.1f;
    public float maxMeshScaleY = 3.140625f;

    [Header("Control Settings")]
    public float sensitivity = 0.5f;
    [Range(0f, 1f)] public float initialFillAmount = 0.5f;

    public float CurrentFillAmount => currentFillAmount;
    public float CurrentSurfaceY => Mathf.Lerp(minSurfaceY, maxSurfaceY, currentFillAmount);
    public float CurrentScaleY => Mathf.Lerp(minMeshScaleY, maxMeshScaleY, currentFillAmount);

    private float currentFillAmount;
    private Vector3 initialMeshScale;
    private Vector2 dragStartPosition;
    private bool isDragging = false;

    void Start()
    {
        initialMeshScale = waterMesh.localScale;
        SetFillAmount(initialFillAmount);
        lastFillAmount = currentFillAmount;
    }

    public void SetFillAmount(float amount)
    {
        currentFillAmount = Mathf.Clamp01(amount);
        UpdateWaterVisuals();
    }

    void OnMouseDown()
    {
        isDragging = true;
        dragStartPosition = GetMouseWorldPosition();
    }

    void OnMouseDrag()
    {
        if (!isDragging) return;

        Vector2 currentMousePos = GetMouseWorldPosition();
        float mouseDeltaY = (currentMousePos.y - dragStartPosition.y) * sensitivity;

        float newFill = currentFillAmount + mouseDeltaY;
        bool isIncreasing = newFill > currentFillAmount;

       
        if (isIncreasing != wasIncreasing)
        {
            PlayMovementSound(isIncreasing);
            wasIncreasing = isIncreasing;
        }

        currentFillAmount = Mathf.Clamp01(newFill);
        UpdateWaterVisuals();
        dragStartPosition = currentMousePos;
    }

    void OnMouseUp()
    {
        isDragging = false;
        wasIncreasing = false;
    }

    void UpdateWaterVisuals()
    {
       
        float surfaceY = Mathf.Lerp(minSurfaceY, maxSurfaceY, currentFillAmount);
        waterSurface.localPosition = new Vector3(
            waterSurface.localPosition.x,
            surfaceY,
            waterSurface.localPosition.z
        );

       
        float scaleY = Mathf.Lerp(minMeshScaleY, maxMeshScaleY, currentFillAmount);
        waterMesh.localScale = new Vector3(
            initialMeshScale.x,
            scaleY,
            initialMeshScale.z
        );
    }

    Vector2 GetMouseWorldPosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    
    void OnValidate()
    {
        if (waterSurface != null && waterMesh != null)
        {
            initialMeshScale = waterMesh.localScale;
            SetFillAmount(initialFillAmount);
        }
    }
    private void PlayMovementSound(bool rising)
    {
        if (audioSource == null) return;

        AudioClip clip = rising ? liquidUpSound : liquidDownSound;
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}