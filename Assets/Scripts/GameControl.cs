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
    public GameObject pe1;
    public GameObject pe2;
    public GameObject pesmall;
    public GameObject pemedium;
    public GameObject pelarge;
    public GameObject peringstick;
    public GameObject peword;
    public Transform camReference;
    public GameObject panelIngame;
    public GameObject panelMain;
    public GameObject panelMainGO;
    public Animator AnimCam;

    //Declare level Properties/assets
    public int ringNumber;
    public float deltaRings; // distance between first and last ring to calculate slider value
    public GameObject[] rings;
    public GameObject Ball;
    public GameObject Counter;

    //Bonus & Endgame
    public GameObject FinishLine;
    public GameObject[] Minigames;
    public GameObject Bonus1Text;
    public GameObject Bonus2Text;
    public GameObject Bonus3Text;
    public float BonusTextYPos = 5.5f;
    public float FinishLineOffset = 3.0f;
    public GameObject Touch;

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
    public CameraScript CameraScript;

    //Declare Private Variables
    private List<GameObject> ringList = new List<GameObject>();
    private GameObject spawnedDartBoard;


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

        ringList.Add(Instantiate(rings[0], new Vector3(0, 0, 0), Quaternion.identity));
        // initial spawn of rings at origin
        for (int i = 0; i < (ringNumber-1); i++)
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
        spawnedDartBoard = Instantiate(minigame, finishPos, Quaternion.identity);
        spawnedDartBoard.SetActive(false);


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

    private void Update()
    {
        if (Ball.GetComponent<Ball>().GetCurrentCircle() != null)
        {
            //Calculate slider value by looking at which circle that the ball is currently on
            slider.value = Ball.GetComponent<Ball>().GetCurrentCircle().transform.position.x / deltaRings;
        }
        else
        {
            slider.value = 0;
        }
    }

    public void Dead()
    {
        PlayerPrefs.SetInt("score", 0);
        Ball.SetActive(false);
        CameraScript.StopMoving();
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
        var temp3 = Instantiate(peword, camReference);
        temp3.transform.localPosition = new Vector3(0, 3.65f, 10);
        Destroy(temp3, 1f);
        scoreText.GetComponent<Text>().text = ChoiceWords[Random.Range(0, ChoiceWords.Length)];
        scoreText.GetComponent<Text>().color = ChoiceWordsColor;
        Destroy(scoreText, 2f);
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

    public int GetLevel()
    {
        return LevelController.GetLevel();
    }

    public void StartButton()
    {
        panelMain.SetActive(false);
        panelMainGO.SetActive(false);
        panelIngame.SetActive(true);
        AnimCam.SetTrigger("movecam");
        Destroy(AnimCam, 0.3f);
        Ball.GetComponent<Rigidbody2D>().isKinematic = false;
    }

    //Called from Ball class
    public GameObject GetLastRing()
    {
        return ringList[ringNumber-1];
    }

    //Called from Ball class when the ball lands on the last ring
    public void OnLastRing()
    {
        spawnedDartBoard.SetActive(true); //Need Animation Transition in
        CameraScript.IncreaseCameraSize();

        //Remove all other rings from gameplay
        for(int i = 0; i < ringList.Count - 1; i++)
        {
            Destroy(ringList[i]);
        }

        //Disable touch temporarily
        StartCoroutine(DisableTouchTemporarily());
    }

    IEnumerator DisableTouchTemporarily()
    {
        Touch.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        Touch.SetActive(true);
    }

    public void RewardBonus(int bonusCount)
    {
        int bonusPoints = 0;
        CameraScript.StopMoving();
        var xPos = Camera.main.transform.position.x;
        var yPos = BonusTextYPos;
        Vector3 spawnPos = new Vector3(xPos, yPos, 0);
        GameObject newobj = new GameObject();

        if (bonusCount == 0)
        {

        }
        else if(bonusCount == 1)
        {
            bonusPoints = GetLevel() * 5;
            newobj = Instantiate(Bonus1Text, spawnPos, Quaternion.identity);
        }
        else if(bonusCount == 2)
        {
            bonusPoints = GetLevel() * 10;
            newobj = Instantiate(Bonus2Text, spawnPos, Quaternion.identity);
        }
        else
        {
            bonusPoints = GetLevel() * 20;
            newobj = Instantiate(Bonus3Text, spawnPos, Quaternion.identity);
        }
    }
}
