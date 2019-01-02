using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public CameraScript camRef;

    //Declare public variables
    public GameObject ArrowGO;
    public Transform Arrow;
    public float ballForce;
    public float ballVel = 10.0f;
    public bool freeBall;
    public bool flying;
    public float PerfectBoundary = 10.0f; //Perfect boundary is +-n from the calculated perfect angle
    public GameObject TextPrefab;
    public float ScoreTextOffset = 0.5f;
    public Color ScoreTextColor;
    public float SafeTextOffset = 1.0f;
    public Color SafeTextColor;
    public AudioSource PopSound;
    public AudioSource PointSound;
    public AudioSource ClearSound;

    //Declare Private variables
    private GameObject CurrentCircle;
    private Vector2 dir;
    private Vector2 newDir;
    private Rigidbody2D rb;
    private bool clockwise;
    private bool within = false;
    private int passedCount = 0;
    private bool safe;
    private int bonusCount = 0;

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
            var Boundary2 = PerfectAngle + PerfectBoundary; //Second Boundary
            if (Boundary1 <= -360)
            {
                if(angle - 360 >= Boundary1)
                {
                    angle -= 360;
                }
            }

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
            //If the ball is on the last right perform a few functions
            if(collision.gameObject == GameControl.instance.GetLastRing())
            {
                GameControl.instance.OnLastRing();
            }
            //Need to declare current circle for reference in CameraScript
            CurrentCircle = collision.gameObject;
            flying = false;
            rb.velocity = new Vector3(0, 0, 0);
            rb.isKinematic = true;
            transform.SetParent(collision.transform);
            ArrowGO.SetActive(true);
            clockwise = CurrentCircle.GetComponent<Circles>().SpinDirection();
            CurrentCircle.GetComponent<Circles>().AdjustLargeArmRotation(gameObject); //Adjust Large Arm rotation
            CurrentCircle.GetComponent<Circles>().MakeCircleStop(); //if the circle was moving before, make it stop because it looks confusing
            if (collision.tag.Contains("small"))
            {
                var temp = Instantiate(GameControl.instance.pesmall, collision.transform.position, Quaternion.identity);
                Destroy(temp, 2f);
            }
            else if (collision.tag.Contains("medium"))
            {
                var temp = Instantiate(GameControl.instance.pemedium, collision.transform.position, Quaternion.identity);
                Destroy(temp, 2f);
            }
            else
            {
                var temp = Instantiate(GameControl.instance.pelarge, collision.transform.position, Quaternion.identity);
                Destroy(temp, 2f);
            }

            var temp2 = Instantiate(GameControl.instance.peringstick, transform.position, Quaternion.identity);
            Destroy(temp2, 2f);


            // set colour of ring based on passedCount number.
            if (!collision.GetComponent<Circles>().colorChanged)
            {
                if (safe && passedCount == 1)
                {
                    collision.GetComponent<Circles>().greenlight.SetActive(true);
                    var radius = collision.gameObject.GetComponent<CircleCollider2D>().radius;
                    var offset = radius + SafeTextOffset;
                    var newPos = collision.gameObject.transform.position + new Vector3(0, offset, 0);
                    InstantiateText(newPos, "Safe", SafeTextColor);
                    safe = false;
                    GameControl.instance.pe1.SetActive(true);
                    GameControl.instance.pe2.SetActive(false);
                }
                else if (!safe && passedCount == 1 || passedCount == 0)
                {
                    collision.GetComponent<Circles>().whitelight.SetActive(true);
                    GameControl.instance.pe1.SetActive(false);
                    GameControl.instance.pe2.SetActive(false);
                }
                else
                {
                    collision.GetComponent<Circles>().redlight.SetActive(true);
                    GameControl.instance.pe1.SetActive(true);
                    GameControl.instance.pe2.SetActive(true);
                }

                //Display choice Word
                if (passedCount >= 2)
                {
                    GameControl.instance.DisplayChoiceWord();
                }

                passedCount = 0;
                collision.GetComponent<Circles>().colorChanged = true;
            }        
        }

        //Detect how many circles that the ball flew over
        if(collision.tag == "counter")
        {
            //Audio
            PopSound.pitch = 1 + passedCount * 0.1f;
            PopSound.Play();
            passedCount += 1;
            Destroy(collision.gameObject);
            //See which ring the counter is referring to, and instantiate the score text above the ring when it flies over
            var ring = collision.gameObject.GetComponent<Counter>().Ring;
            var radius = ring.GetComponent<CircleCollider2D>().radius;
            var offset = radius + ScoreTextOffset;
            var spawnPos = ring.transform.position + new Vector3(0, offset, 0);
            //exponential system
            var score = GameControl.instance.ExponentialScore(passedCount);
            InstantiateText(spawnPos, "+" + score, ScoreTextColor);            
            //Display choice words when the ball flies over more than two circles
        }

        if (collision.tag.Contains("Bonus"))
        {
            Destroy(collision.gameObject);
            bonusCount += 1;
            if (bonusCount == 1)
            {
                PointSound.Play();
            }
            else if (bonusCount == 2)
            {
                PointSound.pitch += .2f;
                PointSound.Play();
            }
            else
            {
                ClearSound.Play();
            }
            if (collision.tag.Contains("1"))
            {
                Instantiate(GameControl.instance.pemask1, collision.transform.position, Quaternion.identity);
            }
            if (collision.tag.Contains("2"))
            {
                Instantiate(GameControl.instance.pemask2, collision.transform.position, Quaternion.identity);
            }
            if (collision.tag.Contains("3"))
            {
                Instantiate(GameControl.instance.pemask3, collision.transform.position, Quaternion.identity);
            }
        }            

        if(collision.tag == "gameend")
        {
            GameControl.instance.death.SetActive(false);
            GameControl.instance.LevelClear();
            GameControl.instance.RewardBonus(bonusCount);
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
            //rb.AddForce(newDir * ballForce);
            rb.velocity = newDir * ballVel;
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
        within = true;
        CurrentCircle.GetComponent<Circles>().Perfect.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = Color.white;
    }

    private void LeavePerfectZone()
    {
        //TODO: leave perfect zone logic
        CurrentCircle.GetComponent<Circles>().Perfect.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = Color.black;
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
        safe = true;
    }

    //Called when ball hits one of the endblocks
    private void PopEndBlock(GameObject obj)
    {
        Destroy(obj);
        GameControl.instance.IncrementScore();
        //TODO: Instantiate and Destroy popping animation/effect
    }

    private void InstantiateText(Vector3 spawnPos, string text, Color c)
    {
        var obj = Instantiate(TextPrefab, spawnPos, Quaternion.identity);
        Destroy(obj, 2f);
        foreach (Transform child in obj.transform)
        {
            child.gameObject.GetComponent<TextMesh>().text = text;
            child.gameObject.GetComponent<TextMesh>().color = c;
        }
    }

}
