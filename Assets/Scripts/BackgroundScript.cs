using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScript : MonoBehaviour
{
    //Declare background objects
    public GameObject[] Bgs;

    //Declare Color Set;
    public Color[] BackgroundColors;
    public Color[] BackgroundObjColors;
    // Start is called before the first frame update
    void Start()
    {
        var level = GameControl.instance.GetLevel();
        var index = level % BackgroundColors.Length;
        for (int i = 0; i < Bgs.Length; i++)
        {
            var Speed = Random.Range(-60f,60f);
            Bgs[i].GetComponent<Rigidbody2D>().angularVelocity = Speed;
            Bgs[i].GetComponent<SpriteRenderer>().color = BackgroundObjColors[index];
        }
        

        Camera.main.backgroundColor = BackgroundColors[index];
        
    }
}
