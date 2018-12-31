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
    public GameObject TextPrefab;
    public float ScoreTextOffset = 0.5f;
    public float SafeTextOffset = 1.0f;

    //Declare Private variables
    private GameObject CurrentCircle;
    private Vector2 dir;
    private Vector2 newDir;
    private Rigidbody2D rb;
    private bool clockwise;
    private bool within = false;
    private int passedCount = 0;
    private bool safe;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        flying = true;
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
                //Only if the ball is within the pefect zone
                if (!within)
                {
                    EnterPerfectZone();
                }
            }
            else
            {
                //If the ball is outside the perfect zone
                if (within)
                {
                    LeavePerfectZone();
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
            CurrentCircle.GetComponent<Circles>().MakeCircleStop(); //if the circle was moving before, make it stop because it looks confusing
           
            if(safe && passedCount==1)
            {
                collision.GetComponent<Circles>().greenlight.SetActive(true);
                var radius = collision.gameObject.GetComponent<CircleCollider2D>().radius;
                var offset = radius + SafeTextOffset;
                var newPos = collision.gameObject.transform.position + new Vector3(0, offset, 0);
                GameObject obj = InstantiateText(newPos, "Safe");
                //obj.GetComponent<TextMesh>().color = SafeTextColor;
                safe = false;
            }
            else if(!safe && passedCount == 1 || passedCount ==0)
            {
                collision.GetComponent<Circles>().whitelight.SetActive(true);
            }
            else
            {
                collision.GetComponent<Circles>().redlight.SetActive(true);
            }
            passedCount = 0;
        }

        //Detect how many circles that the ball flew over
        if(collision.tag == "counter")
        {
            passedCount += 1;
            FlyOver();
            Destroy(collision.gameObject);

            //See which ring the counter is referring to, and instantiate the score text above the ring when it flies over
            var ring = collision.gameObject.GetComponent<Counter>().Ring;
            var radius = ring.GetComponent<CircleCollider2D>().radius;
            var offset = radius + ScoreTextOffset;
            var spawnPos = ring.transform.position + new Vector3(0, offset, 0);
            //exponential system
            var score = GameControl.instance.ExponentialScore(passedCount);
            InstantiateText(spawnPos, "+" + score);
        }

        if(collision.tag == "gameend")
        {
            GameControl.instance.death.SetActive(false);
            GameControl.instance.LevelClear();
        }

        if(collision.tag == "endblock")
        {
            PopEndBlock(collision.gameObject);
        }

        if(collision.tag == "heartblock")
        {
            foreach (Transform child in collision.gameObject.transform)
            {
                PopEndBlock(child.gameObject);
            }
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
       // Debug.Log("Enter Perfect Zone");
        within = true;
    }

    private void LeavePerfectZone()
    {
        //TODO: leave perfect zone logic
        //Debug.Log("Leave Perfect Zone");
        within = false;
    }

    public void FreeBall()
    {
        freeBall = true;
        if (within)
        {
            JumpWithinPerfect();
            LeavePerfectZone();
        }
    }

    public void JumpWithinPerfect()
    {
        Debug.Log("Perfect!!");
        safe = true;
    }

    private void FlyOver()
    {
        Debug.Log("Flew over: " + passedCount + " circles");
    }

    //Called when ball hits one of the endblocks
    private void PopEndBlock(GameObject obj)
    {
        Destroy(obj);
        GameControl.instance.IncrementScore();
        //TODO: Instantiate and Destroy popping animation/effect
    }

    private GameObject InstantiateText(Vector3 spawnPos, string text)
    {
        var obj = Instantiate(TextPrefab, spawnPos, Quaternion.identity);
        Destroy(obj, 0.8f);
        GameObject toReturn = new GameObject();
        foreach (Transform child in obj.transform)
        {
            toReturn = child.gameObject;
            child.gameObject.GetComponent<TextMesh>().text = text;
        }
        return toReturn;
    }
}
