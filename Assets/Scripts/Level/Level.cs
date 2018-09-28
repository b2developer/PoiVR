using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public int INIT_PIORITY = 0;

    public GameObject[] assets;
    public List<MeshFilter> filters;
    public List<AnimationTracker> activeAnimations;
    public bool isExiting = false;

    public Animator levelAnimator = null;
    public Material skyboxMaterial = null;
    public Cubemap reflectionProbe = null;
    public LightmapData lightmapData = null;

    private void AwakePiority()
    {
        //automatically get all assets for the level
        Transform[] children = GetComponentsInChildren<Transform>();

        assets = new GameObject[children.GetLength(0) - 1];
        filters = new List<MeshFilter>();

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

            MeshFilter filter = children[i].gameObject.GetComponent<MeshFilter>();

            if (filter != null)
            {
                filters.Add(filter);
            }
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

            at.Update();
            
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

            isExiting = false;
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

        RenderSettings.skybox = skyboxMaterial;
        RenderSettings.customReflection = reflectionProbe;
        DynamicGI.UpdateEnvironment();

        //activate all entering animations
        AnimationTracker a = new AnimationTracker();

        activeAnimations.Add(a);

        if (levelAnimator != null)
        {
            levelAnimator.SetInteger("Switch", 1);
        }
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

        activeAnimations.Add(a);

        if (levelAnimator != null)
        {
            levelAnimator.SetInteger("Switch", 0);
        }
    }

    public void OnIdle()
    {
        isExiting = false;
    }
}
