﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circles : MonoBehaviour {

    private float spinSpeed=100;

    private void Update()
    {
        transform.Rotate(0, 0, spinSpeed*Time.deltaTime);
    }
}
