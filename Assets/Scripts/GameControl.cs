using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameControl : MonoBehaviour
{

    public static GameControl instance;

    public int ringNumber;
    public GameObject[] rings;

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
        for (int i = 0; i < ringNumber; i++)
        {
            ringList.Add(Instantiate(rings[Random.Range(0,rings.Length)],new Vector3(i*4,0,0),Quaternion.identity));
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
