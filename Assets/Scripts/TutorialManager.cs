using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* class TutorialManager 
* 
* manages the tutorial recordings loading in and handles the special cases
* required for game state management and poi physics for tutorials
* 
* @author: Bradley Booth, Academy of Interactive Entertainment, 2018
*/
public class TutorialManager : MonoBehaviour
{
    //singleton reference
    public static TutorialManager instance = null;

    //used to determine if the tutorial manager is running a tutoring session
    public bool inSession = false;

    //used to restore the menu after a tutoring session
    public List<GameObject> oldStack = null;

    //used to sync the poi to the tutor's playback speed
    public float secondaryTimeScale = 1.0f;

	// Use this for initialization
	void Start ()
    {
        instance = this;
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    /*
    * StartTutoringSession 
    * 
    * turns remotes on, sets up the game to be partially unpaused but pause back to the tutoring menu
    * 
    * @param float timeScale - the time scale of the playback
    * @returns void
    */
    public void StartTutoringSession(float timeScale)
    {
        inSession = true;

        RecordingManager.instance.playbackEngine.isLooping = true;

        //oldStack = MenuStack.instance.stack;

        secondaryTimeScale = timeScale;

        RemoteManager.instance.remoteMode = RemoteManager.RemoteMode.POI;

        RemoteManager.instance.leftPoiRope.PauseSimulation(false);
        RemoteManager.instance.rightPoiRope.PauseSimulation(false);

        RemoteManager.instance.leftPointer.Set(false);
        RemoteManager.instance.rightPointer.Set(false);

        RemoteManager.instance.leftPoiRope.ResetJoints();
        RemoteManager.instance.rightPoiRope.ResetJoints();

        //start practice mode, used to clear the session if the tutorial was started during a time-attack or other gamemode
        Sequence.instance.OnModeStart(1);
    }

    /*
    * EndTutoringSession 
    * 
    * turns remotes off, sets the game back to the tutoring menu
    * 
    * @returns void
    */
    public void EndTutoringSession()
    {
        if (inSession)
        {
            inSession = false;

            RemoteManager.instance.remoteMode = RemoteManager.RemoteMode.UI_LASER;

            RemoteManager.instance.leftPoiRope.PauseSimulation(true);
            RemoteManager.instance.rightPoiRope.PauseSimulation(true);

            RemoteManager.instance.leftPointer.Set(true);
            RemoteManager.instance.rightPointer.Set(true);

            secondaryTimeScale = 1.0f;
        }
    }
}
