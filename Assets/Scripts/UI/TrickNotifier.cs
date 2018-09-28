using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
* class TrickNotifier
* 
* displays notifications of certain types of tricks that 
* have been performed can display multiple in a list like fashion
* 
* @author: Bradley Booth, Academy of Interactive Entertainment, 2018
*/
public class TrickNotifier : MonoBehaviour 
{

    public TextMesh pointsDisplay;
    public TextMesh[] trickDisplays;
    public TextMesh timeDisplay;

    void Start ()
    {
        Reset();   
	}
	
	void Update ()
    {
        pointsDisplay.text = "Score: " + Sequence.instance.points.ToString();
        timeDisplay.text = "Time: " + FormatTimeString((int)Sequence.instance.timer);
	}

    /*
    * OnTrickPerformed 
    * 
    * callback for when a trick is performed, adds points and triggers notifications and combos
    * 
    * @param ETrickType trick - the type of trick that was performed
    * @returns void
    */
    public void OnTrickPerformed(Sequence.ETrickType trick)
    {
        switch (trick)
        {
            case Sequence.ETrickType.STALL: AddTrickNotifier("Stall"); break;
            case Sequence.ETrickType.EXTENSION: AddTrickNotifier("Extension"); break;
            case Sequence.ETrickType.FLOWER: AddTrickNotifier("Flower"); break;
            case Sequence.ETrickType.WEAVE3: AddTrickNotifier("3-Beat Weave"); break;
            case Sequence.ETrickType.DOUBLE_STALL: AddTrickNotifier("Double Stall"); break;
            default:break;
        }
    }

    /*
    * AddTrickNotifier 
    * 
    * registers a new trick on the score board
    * 
    * @param string displayName - what to show on the display
    * @returns void
    */
    public void AddTrickNotifier(string displayName)
    {
        for (int i = trickDisplays.GetLength(0) - 2; i >= 0; i--)
        {
            trickDisplays[i + 1].text = trickDisplays[i].text;
        }

        trickDisplays[0].text = displayName;
    }

    /*
    * Reset
    * 
    * resets all of the trick displays
    * 
    * @returns void
    */
    public void Reset()
    {
        //set all of the trick displays to null
        for (int i = 0; i < trickDisplays.GetLength(0); i++)
        {
            trickDisplays[i].text = "";
        }
    }

    /*
    * FormatTimeString 
    * 
    * @param int seconds - the total amont of seconds left in the mode
    * @returns string - the seconds in the format (minutes):(seconds)
    */
    public string FormatTimeString(int seconds)
    {
        float minutes = seconds / 60.0f;

        float wholeMinutes = Mathf.Floor(minutes);
        float secondsDecimal = (minutes - wholeMinutes) * 60.0f;

        int mins = (int)wholeMinutes;
        int secs = Mathf.FloorToInt(secondsDecimal);

        string zeroes = "";

        //add an additional zero for formating
        if (secs.ToString().Length < 2)
        {
            zeroes = "0";
        }

        return mins.ToString() + ":" + zeroes + secs.ToString();
    }
}
