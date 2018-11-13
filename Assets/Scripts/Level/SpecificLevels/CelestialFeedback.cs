using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* class CelestialFeedback 
* child class of LevelFeedback
* 
* level response to combos for the level IsleOfBlest
* utilises the animation controller 
* 
* @author: Bradley Booth, Academy of Interactive Entertainment, 2018
*/
public class CelestialFeedback : LevelFeedback
{
    public Material starMaterial;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /*
    * Reset
    * overrides LevelFeedback's Reset()
    * 
    * sets all internal variables back to their initial state before
    * the playthough of a game
    * 
    * @returns void 
    */
    public override void Reset()
    {
        feedbackMeter = 0.0f;

        starMaterial.SetFloat("_Cutoff", 1.0f);
    }

    /*
    * Loop
    * overrides LevelFeedback's Loop()
    * 
    * update loop that is controlled by the level being activated or not 
    * 
    * @param bool comboActive - checks if a combo is being performed
    * @returns void 
    */
    public override void Loop(bool comboActive)
    {
        //feedback increase
        if (comboActive)
        {
            feedbackMeter += Time.unscaledDeltaTime / feedbackTime;
        }
        //feedback decrease
        else
        {
            feedbackMeter -= Time.unscaledDeltaTime / feedbackTime;
        }

        feedbackMeter = Mathf.Clamp01(feedbackMeter);

        SetFeedbackLevel(feedbackMeter);
    }

    /*
    * SetFeedbackLevel 
    * overrides LevelFeedback's SetFeedbackLevel(float level)
    * 
    * sets how much the level's intensity through various animations and effects
    * 
    * @param float level - the feedback meter
    * @returns void
    */
    public override void SetFeedbackLevel(float level)
    {
        float cutoff = 1 - level;

        starMaterial.SetFloat("_Cutoff", cutoff);
        
    }
}
