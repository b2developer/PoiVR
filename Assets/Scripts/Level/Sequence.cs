using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
* class Sequence 
* 
* singleton that controls the flow of the game
* practice mode simply registers tricks
* normal mode features a count-down, combos and end of match reports
*
* @author: Bradley Booth, Academy of Interactive Entertainment, 2018
*/
public class Sequence : MonoBehaviour
{
    public delegate void TrickFunc(ETrickType type);

    //references for automated menu interaction
    public MenuStack menuStack = null;
    public GameObject summaryMenu = null;

    //singleton reference
    public static Sequence instance = null;

    //custom type for the type of game being run
    public enum ESequenceType
    {
        NONE = 0,
        PRACTICE = 1,
        NORMAL = 2,
    }

    //custom type for types of tricks to perform
    public enum ETrickType
    {
        REVOLUTION,
        STALL,
        EXTENSION,
        SHOULDER_REVOLUTION,
        FLOWER,
        ANTI_FLOWER,
        WEAVE3,
        DOUBLE_STALL,
    }

    public TrickFunc OnTrickPerformedCallback = null;
    public TrickNotifier trickNotifier = null;

    public ESequenceType sequenceType = ESequenceType.NONE;

    //NORMAL MODE MEMBERS
    //-----------------------------------
    public float timer = 0.0f;
    public float matchTime = 0.0f;
    public int points = 0;

    public TextMesh pointsDisplay = null;
    //-----------------------------------

    void Start ()
    {
        instance = this;
        OnTrickPerformedCallback += trickNotifier.OnTrickPerformed;
	}

	void Update ()
    {
        //only trigger sequence updates in the game-state
        if (!MenuStack.isGame)
        {
            return;
        }

        //update loop for each mode
        if (sequenceType == ESequenceType.PRACTICE)
        {

        }
        else if (sequenceType == ESequenceType.NORMAL)
        {
            //increment timer until it reaches 0
            if (timer > 0.0f)
            {
                timer -= Time.unscaledDeltaTime;

                if (timer < 0.0f)
                {
                    timer = 0.0f;

                    pointsDisplay.text = points.ToString();

                    menuStack.Add(summaryMenu);
                    RemoteManager.instance.ForcePause();
                }
            }
        }
    }

    /*
    * OnModeStart 
    * 
    * callback for when a gamemode is started
    * 
    * @param int modeEnum - int representation of the enum that describes the current game-mode
    * @returns void
    */
    public void OnModeStart(int modeEnum)
    {
        sequenceType = (ESequenceType)modeEnum;

        //initialisation for each mode
        if (sequenceType == ESequenceType.NONE)
        {

        }
        else if (sequenceType == ESequenceType.PRACTICE)
        {

        }
        else if (sequenceType == ESequenceType.NORMAL)
        {
            timer = matchTime;
            points = 0;
        }
             
    }

    /*
    * OnTrickPerformed 
    * 
    * callback for when a trick is performed, adds points and triggers notifications and combos
    * 
    * @param ETrickType trick - the type of trick that was performed
    * @returns void
    */
    public void OnTrickPerformed(ETrickType trick)
    {
        OnTrickPerformedCallback(trick);

        if (sequenceType == ESequenceType.NORMAL)
        {
            switch (trick)
            {
                case ETrickType.REVOLUTION:                 points += 5; break;
                case ETrickType.STALL:                      points += 5; break;
                case ETrickType.EXTENSION:                  points += 10; break;
                case ETrickType.SHOULDER_REVOLUTION:        points += 5; break;
                case ETrickType.FLOWER:                     points += 40; break;
                case ETrickType.WEAVE3:                     points += 25; break;
                case ETrickType.DOUBLE_STALL:               points += 20; break;
            }
        }
    }
}
