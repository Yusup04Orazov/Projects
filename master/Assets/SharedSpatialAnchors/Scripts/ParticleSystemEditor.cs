using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemEditor : MonoBehaviour
{
    private float xval, limit, backLimit;
    public float scaledVal;
    private Vector3 startPosition, prevPosition, curPosition;
    public ParticleSystem particleSystem; // Reference to the particle system
    private float startSpeed = 100f; // Default start speed value
    private Vector2 sliderRange = new Vector2(-1f, 1f); // Range of slider values

    void Start()
    {
        limit = -0.0939f;
        backLimit = 0.0939f;
        
        //set start position to start in the middle
        prevPosition = gameObject.transform.localPosition;
        updatePosition(prevPosition);
        // Get the particle system component
        particleSystem = GetComponent<ParticleSystem>();
        // Set the initial start speed
        SetStartSpeed(startSpeed);
    }

    void Update()
    {
        curPosition = gameObject.transform.localPosition;
        if (curPosition != prevPosition)
        {
            updatePosition(curPosition);
            // Update the start speed based on the scaled value
            UpdateStartSpeed(scaledVal);
        }
    }

    private void updatePosition(Vector3 newPos)
    {
        if (newPos.x > limit)
        {
            xval = limit;
        }
        else if (newPos.x < backLimit)
        {
            xval = backLimit;
        }
        else
        {
            xval = newPos.x;
        }
        prevPosition = new Vector3(0f, -0.004f, xval);
        gameObject.transform.localPosition = prevPosition;
        scaledVal = (xval + limit) / (2 * limit);
        Debug.Log(curPosition);
    }

    private void UpdateStartSpeed(float value)
    {
        // Remap the scaled value to the start speed range
        float remappedSpeed = remap(10f, 15f, sliderRange.x, sliderRange.y, value);
        SetStartSpeed(remappedSpeed);
    }

    private void SetStartSpeed(float speed)
    {
        var main = particleSystem.main;
        main.startSpeed = speed;
    }

    private float remap(float origFrom, float origTo, float targetFrom, float targetTo, float value)
    {
        float rel = Mathf.InverseLerp(origFrom, origTo, value);
        return Mathf.Lerp(targetFrom, targetTo, rel);
    }
}
