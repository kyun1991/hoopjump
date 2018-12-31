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
    public float deltaRings; // distance between first and last ring to calculate slider value
    public GameObject[] rings;
    public GameObject Counter;
    public GameObject FinishLine;
    public GameObject[] Minigames;
    public float FinishLineOffset = 3.0f;

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
    public Slider slider;
    public GameObject death;

    //Canvas Properties
    public GameObject Canvas;
    public GameObject ChoiceWordsTextPrefab;
    public string[] ChoiceWords;
    public Color ChoiceWordsColor;
    public Text textCurrentscore;
    public Text textLevelCurrent;
    public Text textLevelNext;

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

        deltaRings = ringList[ringNumber-1].transform.position.x - ringList[0].transform.position.x;
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
                offset = 0.75f;
            } else if (ringList[i].tag == "ringmedium")
            {
                offset = 0.8f;
            } else if (ringList[i].tag == "ringlarge")
            {
                offset = 0.9f;
            }
            var counter = Instantiate(Counter, ringList[i].transform.position - new Vector3(offset, 0, 0), Quaternion.identity);
            counter.GetComponent<Counter>().Ring = ringList[i]; //Hold reference to the ring

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
        textLevelCurrent.text = LevelController.GetLevel().ToString();
        textLevelNext.text = (LevelController.GetLevel() + 1).ToString();

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

    //Increment score by its level
    public void IncrementScore()
    {
        AddScore(LevelController.GetLevel());
    }

    //Called from the ball class while flying over multiple circles
    public int ExponentialScore(int passedcount)
    {
        var sumOfScore = passedcount * LevelController.GetLevel();
        AddScore(sumOfScore);
        return sumOfScore;
    }

    private void AddScore(int score)
    {
        PlayerPrefs.SetInt("score", PlayerPrefs.GetInt("score", 0) + score);
        textCurrentscore.text = PlayerPrefs.GetInt("score", 0).ToString();
    }

    //Call this method whenever you want to display score increment on screen.
    public void DisplayChoiceWord()
    {
        var scoreText = Instantiate(ChoiceWordsTextPrefab, Canvas.transform);
        scoreText.GetComponent<Text>().text = ChoiceWords[Random.Range(0, ChoiceWords.Length)];
        scoreText.GetComponent<Text>().color = ChoiceWordsColor;
        Destroy(scoreText, 0.8f);
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("score", 0);
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
