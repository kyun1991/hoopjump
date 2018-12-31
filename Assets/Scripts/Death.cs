using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death : MonoBehaviour {

    //Declare public variables
    public Transform ball;
    public GameObject DeathPE1;
    public GameObject DeathPE2;
    public GameObject DeathPE3;

      private void Update()
    {
        //transform.position = new Vector3(ball.position.x, transform.position.y, transform.position.z);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "ball")
        {
            Instantiate(DeathPE1, collision.gameObject.transform.position, Quaternion.identity);
            Instantiate(DeathPE2, collision.gameObject.transform.position, Quaternion.identity);
            Instantiate(DeathPE3, collision.gameObject.transform.position, Quaternion.identity);
            GameControl.instance.Dead();
        }
    }
}
