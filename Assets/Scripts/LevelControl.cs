using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelControl : MonoBehaviour
{
    //Declare Public variables Level variants
    public int Level1RingCount = 10;
    public int RingCountPerLevel = 2; //How many rings increase per level?
    public int RingCountCap = 30; //We don't want too many rings in a level
    public int MovingRingsFrom = 5; //Moving rings appear from level 5 onwards (default)

    //Declare Private variables
    private int Level;

    // Start is called before the first frame update
    void Awake()
    {
        Level = PlayerPrefs.GetInt("Level", 1);
        Debug.Log("Current level is: " + Level);
    }

    //Called from GameController class
    public void LevelUp()
    {
        var currentLevel = PlayerPrefs.GetInt("Level", 1);
        PlayerPrefs.SetInt("Level", currentLevel + 1);
    }

    //Called from GameControl class - this must be called at the beginning of the Start function
    public void SetLevel()
    {
        var ringCount = Level1RingCount + Level * RingCountPerLevel;
        //If calculated ringcount value is greater than ringcount cap, then set the value equal to cap
        if(ringCount > RingCountCap)
        {
            ringCount = RingCountCap;
        }
        GameControl.instance.ringNumber = ringCount;
        if (Level >= MovingRingsFrom)
        {
            GameControl.instance.movingRingsLevel = true; //Indicate whether this level should have moving rings in it
            //TODO: Maybe we want to set chance for each ring to move, but for now its default is 0.5 from gamecontroller
        }
    }
}
