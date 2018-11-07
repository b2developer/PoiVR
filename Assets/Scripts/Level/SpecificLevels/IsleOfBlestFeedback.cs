using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* class IsleOfBlestFeedback 
* child class of LevelFeedback
* 
* level response to combos for the level IsleOfBlest
* utilises the animation controller 
* 
* @author: Bradley Booth, Academy of Interactive Entertainment, 2018
*/
public class IsleOfBlestFeedback : LevelFeedback
{

    public Animator[] floatTower;
    public Animator[] lightTower;

    public bool inIdle = false;

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

        //iterate through all animation clips for the towers, searching for the rising animations
        foreach (Animator animator in floatTower)
        {
            foreach (AnimationClip ac in animator.runtimeAnimatorController.animationClips)
            {
                if (ac.name == "Float_Tower_Rig_3_Rise")
                {
                    animator.Play(ac.name, 0, 0.0f);
                }
            }
        }

        foreach (Animator animator in lightTower)
        {
            foreach (AnimationClip ac in animator.runtimeAnimatorController.animationClips)
            {
                if (ac.name == "Light_tower_1")
                {
                    animator.Play(ac.name, 0, 0.0f);
                }
            }
        }
    }

    private float previousAnimationTime = 0.0f;
 

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
            if (!inIdle)
            {
                feedbackMeter -= Time.unscaledDeltaTime / feedbackTime;
            }
            else
            {
                float animationTime = (floatTower[0].GetCurrentAnimatorStateInfo(0).normalizedTime + lightTower[0].GetCurrentAnimatorStateInfo(0).normalizedTime) / 2.0f;

                //reset is possible
                if (Mathf.Floor(previousAnimationTime) < Mathf.Floor(animationTime))
                {
                    inIdle = false;
                    feedbackMeter -= Time.unscaledDeltaTime / feedbackTime;
                }

                previousAnimationTime = animationTime;
            }
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
        if (level < 1.0f)
        {
            inIdle = false;

            //iterate through all animation clips for the towers, searching for the rising animations
            foreach (Animator animator in floatTower)
            {
                foreach (AnimationClip ac in animator.runtimeAnimatorController.animationClips)
                {
                    if (ac.name == "Float_Tower_Rig_3_Rise")
                    {
                        animator.Play(ac.name, 0, level);
                    }
                }
            }

            foreach (Animator animator in lightTower)
            {
                foreach (AnimationClip ac in animator.runtimeAnimatorController.animationClips)
                {
                    if (ac.name == "Light_tower_1")
                    {
                        animator.Play(ac.name, 0, level);
                    }
                }
            }
        }
        else
        {
            if (!inIdle)
            {
                inIdle = true;
                previousAnimationTime = 0.0f;

                //iterate through all animation clips for the towers, searching for idle animations
                foreach (Animator animator in floatTower)
                {
                    foreach (AnimationClip ac in animator.runtimeAnimatorController.animationClips)
                    {
                        if (ac.name == "Float_Tower_Rig_3_Idle")
                        {
                            animator.Play(ac.name, 0, 0.0f);
                        }
                    }
                }

                foreach (Animator animator in lightTower)
                {
                    foreach (AnimationClip ac in animator.runtimeAnimatorController.animationClips)
                    {
                        if (ac.name == "Light_Tower_Idle")
                        {
                            animator.Play(ac.name, 0, 0.0f);
                        }
                    }
                }
            }
        }
    }
}
