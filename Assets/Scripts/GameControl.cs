using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameControl : MonoBehaviour
{

    public static GameControl instance;

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
