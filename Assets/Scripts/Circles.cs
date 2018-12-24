using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circles : MonoBehaviour {

    private int spin=0;
    private float spinSpeed=150;
    private TrailRenderer tr;

    private float angleTracker = 0;
    private bool startAngleCalculation;

    private void Start()
    {
        spin=Random.Range(0, 2);
        tr = GetComponentInChildren<TrailRenderer>();
        Debug.Log(tr.name);
    }

    private void Update()
    {
        if (spin == 0) // anticlockwise
        {
            transform.Rotate(0, 0, spinSpeed * Time.deltaTime);
        }
        else // clockwise
        {
            transform.Rotate(0, 0, -spinSpeed * Time.deltaTime);
        }

        // calculating if a single rotation has occured since ball attached to this circle
        if (startAngleCalculation)
        {
            angleTracker = angleTracker + spinSpeed * Time.deltaTime;
            if (angleTracker > 360)
            {
                startAngleCalculation = false;
                DeactivateLine();
            }
        }

    }

    public bool SpinDirection()
    {
        if (spin == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void ActivateLine()
    {
        tr.enabled = true;
    }

    public void DeactivateLine()
    {
        tr.enabled = false;
    }

    public void StartAngleCalculation()
    {
        ActivateLine();
        startAngleCalculation = true;
    }
}
