using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circles : MonoBehaviour {
    //Declare public variables
    public GameObject Perfect;

    //Declare private variables
    private int spin = 0;
    private float spinSpeed = 150;
    private float PerfectAngle = 0;
    private CircleCollider2D cc;


    private void Update()
    {
        float rotateAmount;
        if (spin == 0) // anticlockwise
        {
            rotateAmount = spinSpeed * Time.deltaTime;
        }
        else // clockwise
        {
            rotateAmount = -spinSpeed * Time.deltaTime;
        }

        transform.Rotate(0, 0, rotateAmount);

        Perfect.transform.Rotate(0, 0, -rotateAmount);
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

    //Calculate the rotation where the ball launch will result in perfect - called from Gamecontrol class
    public void CalculatePerfectAngle(Vector3 pos2)
    {
        var pos1 = transform.position;
        float newYPos = pos2.y - pos1.y;
        
        var hypotenuse = Mathf.Sqrt(Mathf.Pow(pos2.x - pos1.x, 2) + Mathf.Pow(pos2.y - pos1.y, 2));
        cc = GetComponent<CircleCollider2D>();
        var radius = cc.radius;
        var theta = Mathf.Acos(radius / hypotenuse);
        spin = Random.Range(0, 2);
        if (spin == 1)//If clockwise angle is different
        {
            theta = 2 * Mathf.PI - theta;
            theta -= GameControl.instance.AdjustPerfectClkwise * Mathf.PI/180;
        }
        else
        {
            theta -= GameControl.instance.AdjustPerfectAntiClkwise * Mathf.PI / 180;
        }
        var upVector = new Vector2(0, 1); //Reference vector
        var diffVector = pos2 - pos1;
        var alpha = Vector2.SignedAngle(upVector, diffVector);
        PerfectAngle = (-theta) / Mathf.PI * 180 + alpha; //Convert to degrees and add alpha
        if(PerfectAngle <= -360)
        {
            PerfectAngle += 360;
        }
        Perfect.transform.rotation = Quaternion.Euler(0, 0, PerfectAngle);
    }

    //Called from Ball class
    public float GetPerfectAngle()
    {
        return PerfectAngle;
    }
}
