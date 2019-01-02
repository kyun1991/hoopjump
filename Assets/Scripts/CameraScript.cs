using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {
    
    //Declare public variables
    public GameObject Ball;
    public float XOffset = 1.0f; //Camera offset by 1 units from left
    public float AdjustSpeed = 1.0f;
    public bool Stop;
    public float EndCamSize = 11.0f;

    //Declare private variables
    private Ball BallScript;
    private float OffsetBeforeJump = 0;
    private bool EndReached = false;
    private float OrigOrthographicSize;

    void Start()
    {
        BallScript = Ball.GetComponent<Ball>();
        OrigOrthographicSize = Camera.main.orthographicSize;
    }

    // Update is called once per frame
    void Update () {
        if (!Stop)
        {
            //While the ball is flying to the right adjust the camera to follow the ball
            if (BallScript.flying)
            {
                transform.position = new Vector3(Ball.transform.position.x + OffsetBeforeJump, transform.position.y, transform.position.z);
            }
            else
            {
                //Need to store the current circle that the ball is sitting on, and adjust the camera if its not on that position
                var circlePos = BallScript.GetCurrentCircle().GetComponent<Transform>().position;
                var target = new Vector3(circlePos.x + XOffset, circlePos.y, transform.position.z);
                var step = AdjustSpeed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, target, step);
            }
            //Store the offset between the ballpos and camera pos every frame to prevent sudden change in camera position when the ball jumps
            OffsetBeforeJump = transform.position.x - Ball.transform.position.x;

            if (EndReached) {
                if (Camera.main.orthographicSize <= EndCamSize)
                {
                    Camera.main.orthographicSize += (EndCamSize - OrigOrthographicSize)/2 * Time.deltaTime;
                }
            }
        }
    }

    public void IncreaseCameraSize()
    {
        EndReached = true;
    }


    //Camera stop moving
    public void StopMoving()
    {
        Stop = true;
    }
}
