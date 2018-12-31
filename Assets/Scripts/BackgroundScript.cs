using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScript : MonoBehaviour
{

    //Declare background objects
    public GameObject Bg1;
    public GameObject Bg2;
    public GameObject Bg3;
    public GameObject Bg4;
    public GameObject Bg5;

    //Declare Color Set;
    public Color[] BackgroundColors;
    public Color[] BackgroundObjColors;

    //Declare spin speed
    public float Bg1Speed = 10.0f;
    public float Bg2Speed = -15.0f;
    public float Bg3Speed = 20.0f;
    public float Bg4Speed = -18.0f;
    public float Bg5Speed = 30.0f;

    // Start is called before the first frame update
    void Start()
    {
        Bg1.GetComponent<Rigidbody2D>().angularVelocity = Bg1Speed;
        Bg2.GetComponent<Rigidbody2D>().angularVelocity = Bg2Speed;
        Bg3.GetComponent<Rigidbody2D>().angularVelocity = Bg3Speed;
        Bg4.GetComponent<Rigidbody2D>().angularVelocity = Bg4Speed;
        Bg5.GetComponent<Rigidbody2D>().angularVelocity = Bg5Speed;

        var level = GameControl.instance.GetLevel();
        var index = level % BackgroundColors.Length;

        Camera.main.backgroundColor = BackgroundColors[index];
        Bg1.GetComponent<SpriteRenderer>().color = BackgroundObjColors[index];
        Bg2.GetComponent<SpriteRenderer>().color = BackgroundObjColors[index];
        Bg3.GetComponent<SpriteRenderer>().color = BackgroundObjColors[index];
        Bg4.GetComponent<SpriteRenderer>().color = BackgroundObjColors[index];
        Bg5.GetComponent<SpriteRenderer>().color = BackgroundObjColors[index];
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(Camera.main.transform.position.x, transform.position.y, transform.position.z);
    }
}
