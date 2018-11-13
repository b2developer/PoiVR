using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class ShittyMirrorToggle : MonoBehaviour
{

    public Animator animator;
    public int toggleValue = 0;

    public GameObject hudGraphics;

    public Transform mimicTransform;
    public Transform mimicInside;
    public Transform mimicOutside;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        animator.SetInteger("Switch", toggleValue);

        //enable/disable hud graphics
        hudGraphics.SetActive(!MenuStack.instance.isGame && MenuStack.instance.stack[0] != MenuStack.instance.defaultMenu);
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
