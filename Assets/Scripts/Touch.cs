﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Touch : MonoBehaviour, IPointerClickHandler
{
    public Ball Ball;

    public void OnPointerClick(PointerEventData eventData)
    {
        Ball.freeBall = true;
    }
}
