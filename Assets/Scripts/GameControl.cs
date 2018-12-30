using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameControl : MonoBehaviour
{
    //Declare public variables
    public static GameControl instance;
    public float AdjustPerfectClkwise = 15f;
    public float AdjustPerfectAntiClkwise = 15f;
    public float CircleMoveChance = 0.1f;
    public bool movingRingsLevel = false;

    //Declare level Properties/assets
    public int ringNumber;
    public GameObject[] rings;
    public GameObject Counter;
    public GameObject FinishLine;
    public GameObject[] Minigames;
    public float FinishLineOffset = 3.0f;
    public Text textCurrentscore;

    //Declare Offset Parameters
    public float smallsmall;
    public float smallmedium;
    public float smalllarge;
    public float mediumsmall;
    public float mediummedium;
    public float mediumlarge;
    public float largesmall;
    public float largemedium;
    public float largelarge;

    //Declare controllers
    public LevelControl LevelController;

    //Declare Private Variables
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
        //Declare all level related parameters - How many rings? Are the rings moving?
        LevelController.SetLevel();

        // initial spawn of rings at origin
        for (int i = 0; i < ringNumber; i++)
        {
            ringList.Add(Instantiate(rings[Random.Range(0,rings.Length)],new Vector3(0,0,0),Quaternion.identity));
        }
        // arranges rings by x offset depending on ring size
        AddOffset();
        //Add finishline after the last circle
        var finishPos = ringList[ringNumber - 1].transform.position + new Vector3(FinishLineOffset,0,0);

        //Instantiate a random minigame at the end
        Instantiate(FinishLine, finishPos, Quaternion.identity);
        var minigame = Minigames[Random.Range(0, Minigames.Length)];
        Instantiate(minigame, finishPos, Quaternion.identity);


        for (int i = 1; i < ringNumber; i++)
        {
            //Create a counter object for each circle
            float offset = 0;
            if (ringList[i].tag == "ringsmall")
            {
                offset = 0.3f;
            } else if (ringList[i].tag == "ringmedium")
            {
                offset = 0.5f;
            } else if (ringList[i].tag == "ringlarge")
            {
                offset = 0.7f;
            }
            Instantiate(Counter, ringList[i].transform.position - new Vector3(offset, 0, 0), Quaternion.identity);

            //Check if its moving Rings level
            if (movingRingsLevel)
            {
                //Randomly make the circles move
                var ran = Random.Range(0, 1f);
                if (ran <= CircleMoveChance)
                {
                    ringList[i].GetComponent<Circles>().MakeCircleMove();
                }
            }
        }

        textCurrentscore.text = PlayerPrefs.GetInt("score", 0).ToString();
    }

    public void Dead()
    {
        PlayerPrefs.SetInt("score", 0);
        StartCoroutine(Delay(1f));
    }

    IEnumerator Delay(float time)
    {
        yield return new WaitForSeconds(time);
        SceneManager.LoadScene(0);
    }

    // increments level by 1 when stage clear
    public void LevelClear()
    {
        LevelController.LevelUp();
        StartCoroutine(Delay(1f));
    }

    //Sam please retrieve the current level from Levelcontroller and increment score by its current level
    public void IncrementScore()
    {
        PlayerPrefs.SetInt("score", PlayerPrefs.GetInt("score", 0) + 1);
        textCurrentscore.text = PlayerPrefs.GetInt("score", 0).ToString();
    }

    public void ExponentialScore(int passedcount)
    {
        // exponential algooorithm coem up with here.
        PlayerPrefs.SetInt("score", PlayerPrefs.GetInt("score", 0) + 1);
        textCurrentscore.text = PlayerPrefs.GetInt("score", 0).ToString();
    }

    private void AddOffset()
    {
        // arranges rings by x offset depending on ring size
        for (int i = 0; i < ringNumber - 1; i++)
        {
            if (ringList[i].tag == "ringsmall") // current ring size
            {
                if (ringList[i + 1].tag == "ringsmall") // next ring size
                {
                    ringList[i + 1].transform.position = new Vector3(ringList[i].transform.position.x,0,0) + new Vector3(smallsmall, Random.Range(-2f, 2f), 0);
                }
                else if (ringList[i + 1].tag == "ringmedium") // next ring size
                {
                    ringList[i + 1].transform.position = new Vector3(ringList[i].transform.position.x, 0, 0) + new Vector3(smallmedium, Random.Range(-2f, 2f), 0);
                }
                else if (ringList[i + 1].tag == "ringlarge") // next ring size
                {
                    ringList[i + 1].transform.position = new Vector3(ringList[i].transform.position.x, 0, 0) + new Vector3(smalllarge, Random.Range(-2f, 2f), 0);
                }

            }
            else if (ringList[i].tag == "ringmedium")
            {
                if (ringList[i + 1].tag == "ringsmall") // next ring size
                {
                    ringList[i + 1].transform.position = new Vector3(ringList[i].transform.position.x, 0, 0) + new Vector3(mediumsmall, Random.Range(-2f, 2f), 0);
                }
                else if (ringList[i + 1].tag == "ringmedium") // next ring size
                {
                    ringList[i + 1].transform.position = new Vector3(ringList[i].transform.position.x, 0, 0) + new Vector3(mediummedium, Random.Range(-2f, 2f), 0);
                }
                else if (ringList[i + 1].tag == "ringlarge") // next ring size
                {
                    ringList[i + 1].transform.position = new Vector3(ringList[i].transform.position.x, 0, 0) + new Vector3(mediumlarge, Random.Range(-2f, 2f), 0);
                }
            }
            else if (ringList[i].tag == "ringlarge")
            {
                if (ringList[i + 1].tag == "ringsmall") // next ring size
                {
                    ringList[i + 1].transform.position = new Vector3(ringList[i].transform.position.x, 0, 0) + new Vector3(largesmall, Random.Range(-2f, 2f), 0);
                }
                else if (ringList[i + 1].tag == "ringmedium") // next ring size
                {
                    ringList[i + 1].transform.position = new Vector3(ringList[i].transform.position.x, 0, 0) + new Vector3(largemedium, Random.Range(-2f, 2f), 0);
                }
                else if (ringList[i + 1].tag == "ringlarge") // next ring size
                {
                    ringList[i + 1].transform.position = new Vector3(ringList[i].transform.position.x, 0, 0) + new Vector3(largelarge, Random.Range(-2f, 2f), 0);
                }
            }

            //Calculate the perfect angle
            ringList[i].GetComponent<Circles>().AddNextCircle(ringList[i + 1]);
        }
    }
}
