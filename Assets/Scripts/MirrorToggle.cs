using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

/*
* class MirrorToggle 
* 
* toggles the mirror on or off when told to
* additionally contains various small pieces of functionality
* 
* @author: Bradley Booth, Daniel Witt, Academy of Interactive Entertainment, 2018
*/
public class MirrorToggle : MonoBehaviour
{

    public Animator animator;
    public int toggleValue = 0;

    public GameObject hudGraphics;

    public Transform mimicTransform;
    public Transform mimicInside;
    public Transform mimicOutside;

    public GameObject secondDefaultMenu = null;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per framez
	void Update ()
    {
        animator.SetInteger("Switch", toggleValue);

        //enable/disable hud graphics
        hudGraphics.SetActive(!MenuStack.instance.isGame && MenuStack.instance.stack[0] != MenuStack.instance.defaultMenu && MenuStack.instance.stack[0] != secondDefaultMenu);
    }


    public void Toggle()
    {
        toggleValue = toggleValue == 0 ? 1 : 0;
    }

    public void MoveMimicInside()
    {
        mimicTransform.position = mimicInside.position;
        mimicTransform.rotation = mimicInside.rotation;
    }

    public void MoveMimicOutside()
    {
        mimicTransform.position = mimicOutside.position;
        mimicTransform.rotation = mimicOutside.rotation;
    }
}
