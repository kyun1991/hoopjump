using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {

    //Declare public variables
    public GameObject testtt;
    public Transform Arrow;
    public float ballForce;
    public bool freeBall;

    //Declare Private variables
    private Vector2 newDir;

    private Rigidbody2D rb;
    
	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "circle")
        {
            rb.velocity = new Vector3(0, 0, 0);
            rb.isKinematic = true;
            transform.SetParent(collision.transform);

            Vector3 test = transform.localPosition;
            //Debug.Log(test);

            Vector3 test2 = transform.position;
            //Debug.Log(test2);

            testtt.transform.SetParent(collision.transform);
            testtt.transform.localPosition = test;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Vector2 dir = transform.position - collision.transform.position;
        newDir = Vector2.Perpendicular(dir);
        //Reference vector, that points up
        Vector2 orig = new Vector2(0,1);
        var m_Angle = Vector2.SignedAngle(orig, newDir); //Calculate the angle between reference vector and ball newDir vector
        Arrow.eulerAngles = new Vector3(0,0,m_Angle);
        Arrow.position = transform.position;

        if (freeBall == true)
        {
            freeBall = false;
            transform.SetParent(null);
            testtt.transform.SetParent(null);
            rb.isKinematic = false;
            rb.AddForce(newDir.normalized* ballForce);
        }
    }
}
