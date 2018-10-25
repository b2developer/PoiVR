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

	// Use this for initialization
	void Start ()
    {
        OnKeyPressed += OnKeyPressInternal;
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
        Debug.Log("KEY PRESS DETECTED: " + str);
    }
}
