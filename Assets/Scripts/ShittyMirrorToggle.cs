using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class ShittyMirrorToggle : MonoBehaviour
{

    public Animator animator;
    public int toggleValue = 0;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        animator.SetInteger("Switch", toggleValue);

    }


    public void Toggle()
    {
        toggleValue = toggleValue == 0 ? 1 : 0;
    }
}
