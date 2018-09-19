using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* class UIElement 
* 
* base class for an item that is part of the overall UI system
* contains all callouts that every element can call
* 
* @author: Bradley Booth, Academy of Interactive Entertainment, 2018
*/
public class UIElement : MonoBehaviour
{
	void Start ()
    {
		
	}
	
	void Update ()
    {
		
	}

    /*
    * OnHover 
    * virtual function
    * 
    * triggered when the user's raycast enters the area without pressing it
    * 
    * @returns void
    */
    public virtual void OnHover()
    {

    }

    /*
    * OnNotHover 
    * virtual function
    * 
    * triggered when the user's raycast exits the area without pressing it
    * 
    * @returns void
    */
    public virtual void OnNotHover()
    {

    }

    /*
    * OnClick 
    * virtual function
    * 
    * triggered when the user clicks on this element
    * 
    * @param Pointer pointer - the pointer that triggered the event
    * @returns void
    */
    public virtual void OnClick(Pointer pointer)
    {

    }

    /*
    * OnRelease 
    * virtual function
    * 
    * triggered when the user releases the remote off this element
    * 
    * @returns void
    */
    public virtual void OnRelease()
    {

    }
}
