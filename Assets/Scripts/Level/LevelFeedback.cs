using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* class LevelFeedback 
* 
* base class for a level's response to the player producing combos
* or a lack of combos; includes animation controllers, animation positioning
* particle effects and shader effects
* 
* @author: Bradley Booth, Academy of Interactive Entertainment, 2018
*/
public class LevelFeedback : MonoBehaviour
{

    public float feedbackMeter = 0.0f;
    public float feedbackTime = 10.0f;

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {

    }

    /*
    * Reset
    * virtual function
    * 
    * sets all internal variables back to their initial state before
    * the playthough of a game
    * 
    * @returns void 
    */
    public virtual void Reset()
    {

    }

    /*
    * Loop
    * virtual function
    * 
    * update loop that is controlled by the level being activated or not 
    * 
    * @param bool comboActive - checks if a combo is being performed
    * @returns void 
    */
    public virtual void Loop(bool comboActive)
    {

    }

    /*
    * SetFeedbackLevel 
    * virtual function
    * 
    * sets how much the level's intensity through various animations and effects
    * 
    * @param float level - the feedback meter
    * @returns void
    */
    public virtual void SetFeedbackLevel(float level)
    {

    }
}
