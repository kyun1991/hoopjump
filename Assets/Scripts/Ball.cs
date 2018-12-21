using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {

    //Declare public variables
    public GameObject ArrowGO;

    public Transform Arrow;
    public float ballForce;
    public bool freeBall;
    public bool flying;


    //Declare Private variables
    private Vector2 newDir;
    private Rigidbody2D rb;
    private bool clockwise;
    
	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
        flying = true;
    }

    private void Update()
    {
        Arrow.transform.position = transform.position;

        Vector2 orig = new Vector2(0, 1);
        var m_Angle = Vector2.SignedAngle(orig, newDir);

        if (clockwise == false)
        {
            Arrow.transform.rotation = Quaternion.Euler(0, 0, m_Angle);
        }
        if (clockwise == true)
        {
            Arrow.transform.rotation = Quaternion.Euler(0, 0, m_Angle+180);
        }

        
    
        Debug.Log(clockwise);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "circle")
        {
            flying = false;
            rb.velocity = new Vector3(0, 0, 0);
            rb.isKinematic = true;
            transform.SetParent(collision.transform);
            ArrowGO.SetActive(true);
            clockwise = collision.GetComponent<Circles>().SpinDirection();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Vector2 dir = transform.position - collision.transform.position;
        newDir = Vector2.Perpendicular(dir.normalized);

        if(clockwise)
        {
            newDir = -newDir;
        }
        if (freeBall == true)
        {
            freeBall = false;
            flying = true;
            transform.SetParent(null);
            rb.isKinematic = false;
            rb.AddForce(newDir* ballForce);
            //ArrowGO.SetActive(false);
        }
    }
}
