using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death : MonoBehaviour {

    public Transform ball;

      private void Update()
    {
        transform.position = new Vector3(ball.position.x, transform.position.y, transform.position.z);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "ball")
        {
            GameControl.instance.Dead();
        }
    }
}
