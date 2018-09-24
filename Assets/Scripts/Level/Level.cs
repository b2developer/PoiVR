using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public int INIT_PIORITY = 0;

    public GameObject[] assets;
    public List<AnimationTracker> activeAnimations;
    public bool isExiting = false;

    private void AwakePiority()
    {
        //automatically get all assets for the level
        Transform[] children = GetComponentsInChildren<Transform>();

        assets = new GameObject[children.GetLength(0) - 1];

        int subtractor = 0;

        //get all child game-objects
        for (int i = 0; i < children.GetLength(0); i++)
        {
            //found the parent transform that for some reason is included as part of the children search
            if (children[i] == transform)
            {
                subtractor = -1;
                continue;
            }

            assets[i + subtractor] = children[i].gameObject;
        }

        activeAnimations = new List<AnimationTracker>();
	}

    void Update()
    {
        int animCount = activeAnimations.Count;

        //remove animations that aren't playing
        for (int i = 0; i < animCount; i++)
        {
            AnimationTracker at = activeAnimations[i];
            
            if (!at.isPlaying)
            {
                activeAnimations.RemoveAt(i);
                i--;
                animCount--;
            }
        }

        //disable all game-objects if the level is dissapearing and all animations are finished
        if (animCount == 0 && isExiting)
        {
            foreach (GameObject g in assets)
            {
                g.SetActive(false);
            }
        }
	}

    /*
    * isAnimating 
    * 
    * checks if the level still has animations
    * 
    * @returns bool - flag indicating if the level is animating
    */
    public bool isAnimating()
    {
        return activeAnimations.Count > 0;
    }

    /*
    * OnEntrance 
    * 
    * callback for when the level enters the scene
    * 
    * @returns void
    */
    public void OnEntrance()
    {
        //dont execute other actions while executing
        if (isAnimating())
        {
            return;
        }

        isExiting = false;

        foreach (GameObject g in assets)
        {
            g.SetActive(true);
        }

        //activate all entering animations
        AnimationTracker a = new AnimationTracker();
        AnimationTracker a2 = new AnimationTracker();
        AnimationTracker a3 = new AnimationTracker();

        activeAnimations.Add(a);
        activeAnimations.Add(a2);
        activeAnimations.Add(a3);
    }

    /*
    * OnExit 
    * 
    * callback for when the level exits the scene
    * 
    * @returns void
    */
    public void OnExit()
    {
        //dont execute other actions while executing
        if (isAnimating())
        {
            return;
        }

        isExiting = true;

        //activate all exiting animations
        AnimationTracker a = new AnimationTracker();
        AnimationTracker a2 = new AnimationTracker();
        AnimationTracker a3 = new AnimationTracker();

        activeAnimations.Add(a);
        activeAnimations.Add(a2);
        activeAnimations.Add(a3);
    }

    public void OnIdle()
    {
        isExiting = false;
    }
}
