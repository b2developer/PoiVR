using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* class RemoteManager 
* 
* manages reactions of various game-objects in the scene (enemies, poi ropes, pointers) to remote inputs
* 
* @author: Bradley Booth, Daniel Witt, Academy of Interactive Entertainment, 2018
*/
public class RemoteManager : MonoBehaviour
{
    public const int INIT_PIORITY = 0;

    //singleton pattern
    public static RemoteManager instance;

    //modes that the VR remotes can have as input for the game
    public enum RemoteMode
    {
        POI,
        UI_LASER,
    }

    public RemoteMode remoteMode = RemoteMode.UI_LASER;

    //reference to the menu manager
    public MenuStack menuStack;
    public GameObject mainMenu;

    //references to the poi ropes
    public PoiRope leftPoiRope;
    public PoiRope rightPoiRope;

    public Pointer leftPointer;
    public Pointer rightPointer;

    void AwakePiority ()
    {
        RemoteManager.instance = this;
        menuStack.OnGameResumed += OnGameResumed;

        remoteMode = RemoteMode.POI;
    }

    void Update()
    {
        if (MenuStack.instance.isGame)
        {
            int currentLeftLength = leftPoiRope.ropeLength;
            int currentRightLength = rightPoiRope.ropeLength;

            int intRopeLength = Mathf.FloorToInt(GameProperties.G_TETHERLENGTH);

            if (currentLeftLength != intRopeLength)
            {
                leftPoiRope.ropeLength = Mathf.FloorToInt(GameProperties.G_TETHERLENGTH);
            }

            if (currentRightLength != intRopeLength)
            {
                rightPoiRope.ropeLength = Mathf.FloorToInt(GameProperties.G_TETHERLENGTH);
            }
        }
    }

 
    /*
    * OnGameResumed 
    * 
    * call-back response when the game is resumed
    * 
    * @returns void
    */
    public void OnGameResumed()
    {
        remoteMode = RemoteMode.POI;

        leftPoiRope.PauseSimulation(false);
        rightPoiRope.PauseSimulation(false);

        leftPointer.Set(false);
        rightPointer.Set(false);

        leftPoiRope.ResetJoints();
        rightPoiRope.ResetJoints();
    }

    /*
    * OnGamePaused 
    * 
    * call-back response to a pause event
    * 
    * @returns void
    */
    public void OnGamePaused()
    {
        //pause the game
        if (MenuStack.instance.isGame)
        {
            remoteMode = RemoteMode.UI_LASER;

            menuStack.Add(mainMenu);

            leftPoiRope.PauseSimulation(true);
            rightPoiRope.PauseSimulation(true);

            leftPointer.Set(true);
            rightPointer.Set(true);
        }
    }

    /*
    * ForcePause 
    * 
    * function that forces the remotes to pause regardless of gamestate
    * 
    * @returns void
    */
    public void ForcePause()
    {
        remoteMode = RemoteMode.UI_LASER;

        leftPoiRope.PauseSimulation(true);
        rightPoiRope.PauseSimulation(true);

        leftPointer.Set(true);
        rightPointer.Set(true);
    }
}
