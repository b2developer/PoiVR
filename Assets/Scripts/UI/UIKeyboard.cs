using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* class UIKeyboard 
* 
* manages callbacks for an array of buttons
* that represents a QWERTY keyboard 
* 
* @author: Bradley Booth, Academy of Interactive Entertainment, 2018
*/
public class UIKeyboard : MonoBehaviour
{
    public delegate void StringFunc(string str);

    //call-backs for when a key is pressed
    public StringFunc OnKeyPressed;

    public bool capsLock = false;

    public GameObject lowerVisuals;
    public GameObject upperVisuals;

	// Use this for initialization
	void Start ()
    {
        //toggle lower and upper keyboards depending on the capslock state
        if (capsLock)
        {
            lowerVisuals.SetActive(false);
            upperVisuals.SetActive(true);
        }
        else
        {
            lowerVisuals.SetActive(true);
            upperVisuals.SetActive(false);
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    /*
    * OnKeyPressInternal 
    * 
    * internal call-back for when a key is pressed
    * 
    * @param string str - the character / command the key represents
    * @returns void
    */ 
    public void OnKeyPressInternal(string str)
    {
        //caps lock
        if (str == "caps")
        {
            capsLock = !capsLock;
        }

        //toggle lower and upper keyboards depending on the capslock state
        if (capsLock)
        {
            lowerVisuals.SetActive(false);
            upperVisuals.SetActive(true);
        }
        else
        {
            lowerVisuals.SetActive(true);
            upperVisuals.SetActive(false);
        }

        //caps lock formatting
        str = capsLock && str.Length == 1 ? str.ToUpper() : str.ToLower();

        OnKeyPressed(str);
    }
}
