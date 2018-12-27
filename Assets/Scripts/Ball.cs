using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{

    //Declare public variables
    public GameObject ArrowGO;

    public Transform Arrow;
    public float ballForce;
    public bool freeBall;
    public bool flying;
    public float PerfectBoundary = 10.0f; //Perfect boundary is +-n from the calculated perfect angle

    //Declare Private variables
    private GameObject CurrentCircle;
    private Animator ArrowAnimator;
    private Vector2 dir;
    private Vector2 newDir;
    private Rigidbody2D rb;
    private bool clockwise;
    private bool within = false;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        flying = true;
        ArrowAnimator = ArrowGO.GetComponent<Animator>();
    }

    private void Update()
    {
        // arrow follows balls position
        Arrow.transform.position = transform.position;

        // calculates angle between reference vector and perpendicular vector to change direction of arrow
        Vector2 orig = new Vector2(0, 1);
        var m_Angle = Vector2.SignedAngle(orig, newDir);
        Arrow.transform.rotation = Quaternion.Euler(0, 0, m_Angle);

        // calculates the angle between the ball and the circle's transform with reference to (0,1) vector
        var angle = Vector2.SignedAngle(orig, dir);
        //adjust angle to same orientation as perfectangles
        if(angle > 0)
        {
            angle -= 360; //i.e. if angle = 150 thats -210
        }

        if (CurrentCircle != null)
        {
            var PerfectAngle = CurrentCircle.GetComponent<Circles>().GetPerfectAngle();
            var Boundary1 = PerfectAngle - PerfectBoundary;//First Boundary
            if (Boundary1 <= -360)
            {
                Boundary1 += 360; //Adjust angle it needs to be within 0 ~ -360
            }
            var Boundary2 = PerfectAngle + PerfectBoundary; //Second Boundary

            if (angle <= Boundary2 && angle >= Boundary1)
            {
                if (!within)
                {
                    EnterPerfectZone();
                    within = true;
                }
            }
            else
            {
                if (within)
                {
                    LeavePerfectZone();
                    within = false;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Contains("ring"))
        {
            //Need to declare current circle for reference in CameraScript
            CurrentCircle = collision.gameObject;
            flying = false;
            rb.velocity = new Vector3(0, 0, 0);
            rb.isKinematic = true;
            transform.SetParent(collision.transform);
            ArrowGO.SetActive(true);
            clockwise = CurrentCircle.GetComponent<Circles>().SpinDirection();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // works out direction perpendicular to balls spinning direction
        dir = transform.position - collision.transform.position;
        newDir = Vector2.Perpendicular(dir.normalized);

        if (clockwise)
        {
            newDir = -newDir;
        }
        if (freeBall == true)
        {
            freeBall = false;
            flying = true;
            transform.SetParent(null);
            rb.isKinematic = false;
            rb.AddForce(newDir * ballForce);
            ArrowGO.SetActive(false);
        }
    }

    //Called from the CameraScript Class
    public GameObject GetCurrentCircle()
    {
        return CurrentCircle;
    }

    private void EnterPerfectZone()
    {
        //TODO: enter perfect zone logic
        Debug.Log("Enter Perfect Zone");
        ArrowAnimator.SetTrigger("Enlarge");
    }

    private void LeavePerfectZone()
    {
        //TODO: leave perfect zone logic
        Debug.Log("Leave Perfect Zone");
        ArrowAnimator.SetTrigger("Decrease");
    }

    public void FreeBall()
    {
        freeBall = true;
        LeavePerfectZone();
    }
}
