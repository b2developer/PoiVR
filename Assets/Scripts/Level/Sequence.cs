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
        WEAVE2,
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

            //combo registration, new trick must be unique and be present before the time runs out
            if (Sequence.instance.currentTrick != Sequence.instance.previousTrick && timer <= Sequence.instance.comboMaxTime)
            {
                tricks++;

                //reward combo points for the previous trick when the combo starts
                if (points == 0)
                {
                    points += Sequence.instance.CalculatePoints(Sequence.instance.previousTrick);
                }

                points += pointsAwarded;

                trickHistory.Add(trick);

                timer = 0.0f;

                Sequence.instance.DebugDisplayFunction("COMBO ADD!");

                SoundLibrary.instance.PlaySound(5, 2);
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
            if (tricks > 0)
            {
                timer += Time.deltaTime;

                //combo award
                if (timer > Sequence.instance.comboMaxTime)
                {
                    Reset(points > 0);
                }
            }
            else
            {
                timer = 0.0f;
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
                //award combo points (points earned in combo * tricks done in combo)
                Sequence.instance.points += points * tricks;
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

                SoundLibrary.instance.PlaySound(4, 2);
            }

            points = 0;
            timer = 0.0f;
            tricks = 0;

            //reset the state of the tricks to allow duplicates to count to a combo after a combo dies
            Sequence.instance.previousTrick = ETrickType.NONE;
            Sequence.instance.currentTrick = ETrickType.NONE;

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
    public Animator timerNotifier = null;

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

        sequenceType = ESequenceType.PRACTICE;
        softNotifier.comboMesh.text = "";
        softNotifier.SetCombo(0.0f);
    }

	void Update ()
    {
        //only trigger sequence updates in the game-state
        if (!MenuStack.instance.isGame && !TutorialManager.instance.inSession)
        {
            timerNotifier.speed = 0.0f;

            //turn the notifier off
            if (softNotifier.gameObject.activeSelf)
            {
                softNotifier.gameObject.SetActive(false);
            }

            return;
        }

        //turn the notifier on
        if (!softNotifier.gameObject.activeSelf)
        {
            softNotifier.gameObject.SetActive(true);
        }

        //update loop for each mode
        if (sequenceType == ESequenceType.PRACTICE)
        {
            softNotifier.comboMesh.text = "";
            softNotifier.SetCombo(0.0f);
        }
        else if (sequenceType == ESequenceType.NORMAL)
        {
            combo.Check();

            softNotifier.comboMesh.text = "x" + combo.tricks.ToString();

            if (combo.tricks > 0)
            {
                softNotifier.SetCombo(1.0f - combo.timer / comboMaxTime);
            }
            else
            {
                softNotifier.SetCombo(0.0f);
            }

            //apply feedback
            if (LevelManager.instance.activeID >= 0)
            {
                Level activeLevel = LevelManager.instance.levels[LevelManager.instance.activeID];

                activeLevel.levelFeedback.Loop(combo.tricks >= 2);
            }

            //decrement timer until it reaches 0
            if (timer > 0.0f)
            {
                timerNotifier.speed = 45.0f / 60.0f;

                float prevTimer = timer;

                timer -= Time.unscaledDeltaTime;

                //remainder for the countdown
                if (timer > matchTime)
                {
                    float remainder = timer - matchTime;

                    softNotifier.comboMesh.text = (Mathf.Floor(remainder * 10.0f) / 10.0f).ToString();
                }

                //sound for starting the game
                if (prevTimer > matchTime && timer <= matchTime)
                {
                    SoundLibrary.instance.PlaySound(13, 2);

                    timerNotifier.transform.parent.gameObject.SetActive(true);

                    softNotifier.comboMesh.text = "x0";
                }

                //3, 2, 1 countdown
                if (prevTimer > 3.0f && timer <= 3.0f)
                {
                    SoundLibrary.instance.PlaySound(8, 2);
                }

                if (timer < 0.0f)
                {
                    timer = 0.0f;

                    combo.Reset(true);

                    timerNotifier.transform.parent.gameObject.SetActive(false);

                    pointsDisplay.text = points.ToString();

                    menuStack.Add(summaryMenu);

                    //switch to practice mode
                    OnModeStart(1);

                    SoundLibrary.instance.PlaySound(7, 2);

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
            timerNotifier.transform.parent.gameObject.SetActive(false);
            timerNotifier.playbackTime = 0.0f;
        }
        else if (sequenceType == ESequenceType.PRACTICE)
        {
            //apply feedback
            if (LevelManager.instance.activeID >= 0)
            {
                Level activeLevel = LevelManager.instance.levels[LevelManager.instance.activeID];

                activeLevel.levelFeedback.Reset();
            }
        }
        else if (sequenceType == ESequenceType.NORMAL)
        {

            timerNotifier.transform.parent.gameObject.SetActive(false);
            timerNotifier.playbackTime = 0.0f;

            timer = matchTime + 3.0f;
            points = 0;

            currentTrick = ETrickType.NONE;
            previousTrick = ETrickType.NONE;

            combo.Reset(false);

            //apply reset to the active level
            if (LevelManager.instance.activeID >= 0)
            {
                Level activeLevel = LevelManager.instance.levels[LevelManager.instance.activeID];

                activeLevel.levelFeedback.Reset();
            }

            SoundLibrary.instance.PlaySound(8, 2);

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
            if (timer > matchTime)
            {
                return;
            }

            //must be a trick that can award points
            bool isComplexTrick = trick != ETrickType.REVOLUTION && trick != ETrickType.SHOULDER_REVOLUTION && trick != ETrickType.WEAVE2;

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
