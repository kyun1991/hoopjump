using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour {

    public Transform ball;

    private Vector3 offset;

    private void Start()
    {
        offset = ball.position - transform.position;
    }

    private void Update()
    {
        transform.position = ball.position - offset;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(collision.gameObject);
    }
}
