using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circles : MonoBehaviour {

    private int spin=0;
    private float spinSpeed=150;

    private void Start()
    {
        spin=Random.Range(0, 2);
    }

    private void Update()
    {
        if (spin == 0) // anticlockwise
        {
            transform.Rotate(0, 0, spinSpeed * Time.deltaTime);
        }
        else // clockwise
        {
            transform.Rotate(0, 0, -spinSpeed * Time.deltaTime);
        }
    }

    public bool SpinDirection()
    {
        if (spin == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
