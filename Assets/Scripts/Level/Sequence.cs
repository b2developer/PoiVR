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
    public delegate void StringFunc(string data);

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
        NONE,
        REVOLUTION,
        STALL,
        EXTENSION,
        SHOULDER_REVOLUTION,
        FLOWER,
        ANTI_FLOWER,
        WEAVE3,
        DOUBLE_STALL,
    }

    /*
    * class Combo
    * 
    * manages the state of a combo internally
    * 
    * @author: Bradley Booth, Academy of Interactive Entertainment, 2018 
    */
    public class Combo
    {
        public int points = 0;
        public int tricks = 0;
        public float timer = 0.0f;

        public List<ETrickType> trickHistory;

        /*
        * Combo 
        * default constructor
        */
        public Combo()
        {
            trickHistory = new List<ETrickType>();
        }

        /*
        * Update
        * 
        * callback sub-routine for when a trick is performed, updates the combo state
        * 
        * @param ETrickType trick - the type of trick that was performed
        * @param int pointsAwarded - calculated points (stored to avoid multiple redundant function calls)
        * @returns void
        */
        public void Update(ETrickType trick, int pointsAwarded)
        {
            if (Sequence.instance.currentTrick == Sequence.instance.previousTrick)
            {
                Sequence.instance.softNotifier.mesh.color = Color.red;
                Sequence.instance.softNotifier.colourTimer = Sequence.instance.softNotifier.flashTime;
            }

            //combo registration, new trick must be unique and be present before the time runs out
            if (Sequence.instance.currentTrick != Sequence.instance.previousTrick && timer <= Sequence.instance.comboMaxTime)
            {
                //reward combo points for the previous trick when the combo starts
                if (points == 0)
                {
                    points += Sequence.instance.CalculatePoints(Sequence.instance.previousTrick);
                    tricks++;
                    
                }

                Sequence.instance.softNotifier.mesh.color = Color.blue;
                Sequence.instance.softNotifier.colourTimer = Sequence.instance.softNotifier.flashTime;

                points += pointsAwarded;
                tricks++;

                trickHistory.Add(trick);

                timer = 0.0f;

                Sequence.instance.DebugDisplayFunction("COMBO ADD!");
            }
        }

        /*
        * Check
        * 
        * sub-routine when checking if combo points should be awarded
        * 
        * @returns void
        */
        public void Check()
        {
            timer += Time.deltaTime;

            //combo award
            if (timer > Sequence.instance.comboMaxTime)
            {
                Reset(points > 0);
            }
        }

        /*
        * Reset
        * 
        * resets the combo state, awards points of applicable
        * 
        * @param bool award - should points be awarded?
        * @returns void
        */
        public void Reset(bool award)
        {
            //notify the player they achieved a combo
            if (award)
            {
                //award combo points
                Sequence.instance.points += points;
                Sequence.instance.DebugDisplayFunction("COMBO x" + tricks.ToString() + "!");

                List<ETrickType> uniqueTricks = new List<ETrickType>();
                
                //create list of all of the unique tricks
                foreach (ETrickType trick in trickHistory)
                {
                    if (!uniqueTricks.Contains(trick))
                    {
                        uniqueTricks.Add(trick);
                    }
                }

                Sequence.instance.softNotifier.mesh.color = Color.green;
                Sequence.instance.softNotifier.colourTimer = Sequence.instance.softNotifier.flashTime;
            }

            points = 0;
            timer = 0.0f;
            tricks = 0;

            trickHistory.Clear();
        }
    }


    //references for automated menu interaction
    public MenuStack menuStack = null;
    public GameObject summaryMenu = null;

    //singleton reference
    public static Sequence instance = null;
   
    public TrickFunc OnTrickPerformedCallback = null;
    public TrickNotifier trickNotifier = null;
    public SoftFollowNotifier softNotifier = null;

    public StringFunc DebugDisplayFunction = null;

    public ESequenceType sequenceType = ESequenceType.NONE;

    //NORMAL MODE MEMBERS
    //-----------------------------------
    public float timer = 0.0f;
    public float matchTime = 0.0f;
    public int points = 0;

    public ETrickType currentTrick;
    public ETrickType previousTrick;
    public float trickTimer = 0.0f;

    public Combo combo;

    public float comboMaxTime = 10.0f;

    public TextMesh pointsDisplay = null;
    //-----------------------------------

    void Start ()
    {
        instance = this;
        OnTrickPerformedCallback += trickNotifier.OnTrickPerformed;
        OnTrickPerformedCallback += softNotifier.OnTrickPerformed;
        DebugDisplayFunction += trickNotifier.AddTrickNotifier;
        combo = new Combo();
	}

	void Update ()
    {
        //only trigger sequence updates in the game-state
        if (!MenuStack.instance.isGame)
        {
            return;
        }

        //update loop for each mode
        if (sequenceType == ESequenceType.PRACTICE)
        {

        }
        else if (sequenceType == ESequenceType.NORMAL)
        {
            combo.Check();

            //apply feedback
            if (LevelManager.instance.activeID >= 0)
            {
                Level activeLevel = LevelManager.instance.levels[LevelManager.instance.activeID];

                activeLevel.levelFeedback.Loop(combo.tricks >= 2);
            }

            //increment timer until it reaches 0
            if (timer > 0.0f)
            {
                timer -= Time.unscaledDeltaTime;

                if (timer < 0.0f)
                {
                    timer = 0.0f;

                    combo.Reset(true);

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
            softNotifier.scoreLerp = 0.0f;

            currentTrick = ETrickType.NONE;
            previousTrick = ETrickType.NONE;

            combo.Reset(false);

            //apply reset to the active level
            if (LevelManager.instance.activeID >= 0)
            {
                Level activeLevel = LevelManager.instance.levels[LevelManager.instance.activeID];

                activeLevel.levelFeedback.Reset();

            }

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
            bool isComplexTrick = trick != ETrickType.REVOLUTION && trick != ETrickType.SHOULDER_REVOLUTION;

            //award points
            int pointsAwarded = CalculatePoints(trick);
            points += pointsAwarded;

            //update combo (for complex tricks)
            if (isComplexTrick)
            {
                previousTrick = currentTrick;
                currentTrick = trick;

                combo.Update(currentTrick, pointsAwarded);
            }
          
        }
    }

    /*
    * CalculatePoints 
    * 
    * gets the amount of points awarded for each trick
    * 
    * @param ETrickType trick - the type of trick that was performed
    * @returns int - the amount of points for that specific trick
    */
    public int CalculatePoints(ETrickType trick)
    {
        switch (trick)
        {
            case ETrickType.REVOLUTION: return 5;
            case ETrickType.STALL: return 5;
            case ETrickType.EXTENSION: return 10;
            case ETrickType.SHOULDER_REVOLUTION: return 5;
            case ETrickType.FLOWER: return 40;
            case ETrickType.WEAVE3: return 25;
            case ETrickType.DOUBLE_STALL: return 20;
        }

        return 0;
    }
}
