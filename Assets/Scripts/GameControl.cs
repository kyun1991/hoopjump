using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameControl : MonoBehaviour
{

    public static GameControl instance;

    public int ringNumber;
    public GameObject[] rings;
    public float smallsmall;
    public float smallmedium;
    public float smalllarge;
    public float mediumsmall;
    public float mediummedium;
    public float mediumlarge;
    public float largesmall;
    public float largemedium;
    public float largelarge;

    private List<GameObject> ringList = new List<GameObject>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // initial spawn of rings at origin
        for (int i = 0; i < ringNumber; i++)
        {
            ringList.Add(Instantiate(rings[Random.Range(0,rings.Length)],new Vector3(0,0,0),Quaternion.identity));
        }

        // arranges rings by x offset depending on ring size
        for (int i = 0; i < ringNumber-1; i++)
        {
            if (ringList[i].tag == "ringsmall") // current ring size
            {
                if(ringList[i + 1].tag == "ringsmall") // next ring size
                {
                    ringList[i + 1].transform.position =ringList[i].transform.position + new Vector3(smallsmall, 0, 0);                    
                }
                else if (ringList[i + 1].tag == "ringmedium") // next ring size
                {
                    ringList[i + 1].transform.position = ringList[i].transform.position + new Vector3(smallmedium, 0, 0);
                }
                else if (ringList[i + 1].tag == "ringlarge") // next ring size
                {
                    ringList[i + 1].transform.position = ringList[i].transform.position + new Vector3(smalllarge, 0, 0);
                }

            }
            else if (ringList[i].tag == "ringmedium")
            {
                if (ringList[i + 1].tag == "ringsmall") // next ring size
                {
                    ringList[i + 1].transform.position = ringList[i].transform.position + new Vector3(mediumsmall, 0, 0);
                }
                else if (ringList[i + 1].tag == "ringmedium") // next ring size
                {
                    ringList[i + 1].transform.position = ringList[i].transform.position + new Vector3(mediummedium, 0, 0);
                }
                else if (ringList[i + 1].tag == "ringlarge") // next ring size
                {
                    ringList[i + 1].transform.position = ringList[i].transform.position + new Vector3(mediumlarge, 0, 0);
                }
            }
            else if (ringList[i].tag == "ringlarge")
            {
                if (ringList[i + 1].tag == "ringsmall") // next ring size
                {
                    ringList[i + 1].transform.position = ringList[i].transform.position + new Vector3(largesmall, 0, 0);
                }
                else if (ringList[i + 1].tag == "ringmedium") // next ring size
                {
                    ringList[i + 1].transform.position = ringList[i].transform.position + new Vector3(largemedium, 0, 0);
                }
                else if (ringList[i + 1].tag == "ringlarge") // next ring size
                {
                    ringList[i + 1].transform.position = ringList[i].transform.position + new Vector3(largelarge, 0, 0);
                }
            }
        }

    }

    public void Dead()
    {
        StartCoroutine(Delay(1f));
    }

    IEnumerator Delay(float time)
    {
        yield return new WaitForSeconds(time);
        SceneManager.LoadScene(0);
    }

}
