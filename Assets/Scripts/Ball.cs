using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {

    public GameObject testtt;

    public float ballForce;

    public bool freeBall;

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
