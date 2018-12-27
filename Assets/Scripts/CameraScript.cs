using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {
    
    //Declare public variables
    public GameObject Ball;
    public float XOffset = 1.0f; //Camera offset by 1 units from left
    public float AdjustSpeed = 1.0f;

    //Declare private variables
    private Ball BallScript;
    private float OffsetBeforeJump = 0;

    void Start()
    {
        BallScript = Ball.GetComponent<Ball>();
    }

    // Update is called once per frame
    void Update () {
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
    }
}
