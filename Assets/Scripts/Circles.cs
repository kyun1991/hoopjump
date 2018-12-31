using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circles : MonoBehaviour
{
    //Declare public variables
    public GameObject LargeArm;
    public GameObject Perfect;
    public GameObject greenlight;
    public GameObject redlight;
    public GameObject whitelight;
    public bool colorChanged;

    //Declare private variables
    private int spin = 0;
    private float spinSpeed = 150;
    private float PerfectAngle = 0;
    private CircleCollider2D cc;
    private GameObject NextCircle;
    private Vector3 NextCirclePrevFramePos;

    //Declare variables used for move
    public float speed = 1;
    private bool isMove = false;
    private bool moveUp = true;
    private Vector3 topMovePos;
    private Vector3 bottomMovePos;

    private void Awake()
    {
        spin = Random.Range(0, 2);
    }

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

        //Check if the position of the next circle changed, if so, adjust the perfect zone
        if (NextCircle != null)
        {
            if (!NextCirclePrevFramePos.Equals(NextCircle.transform.position))
            {
                CalculatePerfectAngle();
            }
            NextCirclePrevFramePos = NextCircle.transform.position;
        }
        //Perfect locator also rotates with the circle without below code, we'll offset the rotation so that it stays on the same place
        Perfect.transform.Rotate(0, 0, -rotateAmount);

        if (isMove)
        {
            var step = speed * Time.deltaTime;
            Vector3 newPos;
            if (moveUp)
            {
                newPos = topMovePos;
            }
            else
            {
                newPos = bottomMovePos;
            }
            transform.position = Vector3.MoveTowards(transform.position, newPos, step);
            if(transform.position == newPos)
            {
                moveUp = !moveUp;
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

    public void AddNextCircle(GameObject nextCircle)
    {
        NextCircle = nextCircle;
        CalculatePerfectAngle();
    }

    //Calculate the rotation where the ball launch will result in perfect - called from Gamecontrol class
    public void CalculatePerfectAngle()
    {
        var pos1 = transform.position;
        var pos2 = NextCircle.transform.position;
        float newYPos = pos2.y - pos1.y;

        var hypotenuse = Mathf.Sqrt(Mathf.Pow(pos2.x - pos1.x, 2) + Mathf.Pow(pos2.y - pos1.y, 2));
        cc = GetComponent<CircleCollider2D>();
        var radius = cc.radius;
        var theta = Mathf.Acos(radius / hypotenuse);
        if (spin == 1)//If clockwise angle is different
        {
            theta = 2 * Mathf.PI - theta;
            theta -= GameControl.instance.AdjustPerfectClkwise * Mathf.PI / 180;
        }
        else
        {
            theta -= GameControl.instance.AdjustPerfectAntiClkwise * Mathf.PI / 180;
        }
        var upVector = new Vector2(0, 1); //Reference vector
        var diffVector = pos2 - pos1;
        var alpha = Vector2.SignedAngle(upVector, diffVector);
        PerfectAngle = (-theta) / Mathf.PI * 180 + alpha; //Convert to degrees and add alpha
        if (PerfectAngle <= -360)
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

    //Called from the Gamecontrol class to indicate that this circle is going to be moving
    public void MakeCircleMove()
    {
        isMove = true;
        if (isMove)
        {
            var distance = Random.Range(0.5f, 1.0f);
            topMovePos = transform.position + new Vector3(0, distance, 0);
            bottomMovePos = transform.position - new Vector3(0, distance, 0);
        }
    }

    //Called when the ball lands on this circle, stop moving the circle because it looks very messy
    public void MakeCircleStop()
    {
        isMove = false;
    }

    public void AdjustLargeArmRotation(GameObject Ball)
    {
        var upVector = new Vector2(0, 1); //Reference vector
        var newVector = Ball.transform.position - transform.position;
        var newAngle = Vector2.SignedAngle(upVector, newVector);
        LargeArm.transform.rotation = Quaternion.Euler(0, 0, newAngle + 90);
    }
}
