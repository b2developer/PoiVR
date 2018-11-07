using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* class Level 
* 
* manages the assets corresponding to a level including it's gameobjects,
* mesh filters and animators
* 
* automatically swaps lighting features such as the skybox and reflection probe
* 
* @author: Bradley Booth, Academy of Interactive Entertainment, 2018
*/
public class Level : MonoBehaviour
{
    public int INIT_PIORITY = 0;

    public GameObject[] assets;
    public List<MeshFilter> filters;

    public bool levelActive = false;

    public LevelFeedback levelFeedback = null;
    public Animator levelAnimator = null;

    public Material skyboxMaterial = null;
    public Cubemap reflectionProbe = null;

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
	}

    void Update()
    {

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
        foreach (GameObject g in assets)
        {
            g.SetActive(true);
        }

        if (levelAnimator != null)
        {
            levelAnimator.SetInteger("Switch", 1);
        }

        levelActive = true;
    }

    /*
    * OnExitAnimation 
    * 
    * callback for when the level should play it's exit animation 
    *
    * @returns void
    */
    public void OnExitAnimation()
    {
        if (levelAnimator != null)
        {
            levelAnimator.SetInteger("Switch", 0);
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
        foreach (GameObject g in assets)
        {
            g.SetActive(false);
        }

        levelActive = false;
    }
}
