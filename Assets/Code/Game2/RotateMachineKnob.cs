using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using UnityEngine;

public class RotateMachineKnob : MonoBehaviour
{
    private Camera myCam;
    private Vector3 screenPos;
    private float angleOffset;
    private Collider2D col;
    private bool isRotating;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip rotateSound;

    private void Start()
    {
        myCam = Camera.main;
        col = GetComponent<Collider2D>();
    }

    private void Update()
    {
        Vector3 mousePos = myCam.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            if (col == Physics2D.OverlapPoint(mousePos))
            {
                StartRotation();
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (isRotating)
            {
                ContinueRotation(mousePos);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            StopRotation();
        }
    }

    private void StartRotation()
    {
        isRotating = true;
        screenPos = myCam.WorldToScreenPoint(transform.position);
        Vector3 vec3 = Input.mousePosition - screenPos;
        angleOffset = (Mathf.Atan2(transform.right.y, transform.right.x) - Mathf.Atan2(vec3.y, vec3.x)) * Mathf.Rad2Deg;

        if (audioSource != null && rotateSound != null)
        {
            audioSource.loop = true;
            audioSource.clip = rotateSound;
            audioSource.Play();
        }
    }

    private void ContinueRotation(Vector3 mousePos)
    {
        if (col == Physics2D.OverlapPoint(mousePos))
        {
            Vector3 vec3 = Input.mousePosition - screenPos;
            float angle = Mathf.Atan2(vec3.y, vec3.x) * Mathf.Rad2Deg;
            float newAngle = angle + angleOffset;

            newAngle = newAngle % 360;
            if (newAngle < 0) newAngle += 360;
            transform.eulerAngles = new Vector3(0, 0, newAngle);
        }
        else
        {
            StopRotation();
        }
    }

    private void StopRotation()
    {
        if (isRotating)
        {
            isRotating = false;
            if (audioSource != null)
            {
                audioSource.Stop();
            }
        }
    }
}