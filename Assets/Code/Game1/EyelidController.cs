using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyelidController : MonoBehaviour
{
    public static event System.Action OnAllEyesClosed;
    public EyelidMovement topEyelid;
    public EyelidMovement bottomEyelid;
    public EyelidController otherEyeController;

    private BoxCollider2D topCollider;
    private BoxCollider2D bottomCollider;
    private bool isLocked = false;
    public float speedIncreaseMultiplier = 1.5f;

    public bool BothEyelidsClosed => isLocked;

    void Start()
    {
        topCollider = topEyelid.GetComponent<BoxCollider2D>();
        bottomCollider = bottomEyelid.GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        if (isLocked) return;

        if (AreCollidersIntersecting())
        {
            LockEyelids();
            otherEyeController?.AccelerateOtherEye();
            CheckAllEyesClosed();
        }
    }

    bool AreCollidersIntersecting()
    {
        return topCollider.bounds.Intersects(bottomCollider.bounds);
    }

    void LockEyelids()
    {
        isLocked = true;
        topEyelid.isLocked = true;
        bottomEyelid.isLocked = true;
    }

    public void AccelerateOtherEye()
    {
        topEyelid.MultiplySpeed(speedIncreaseMultiplier);
        bottomEyelid.MultiplySpeed(speedIncreaseMultiplier);
    }

    void CheckAllEyesClosed()
    {
        if (otherEyeController.BothEyelidsClosed && this.BothEyelidsClosed)
        {
            OnAllEyesClosed?.Invoke();
        }
    }
}