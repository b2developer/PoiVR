using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* class StartupTooltip
* 
* UI element that sticks around during the first running of the game
* until the game is finally paused again, dissapearing forever after that
* 
* @author: Bradley Booth, Academy of Interactive Entertainment, 2018 
*/
public class StartupTooltip : MonoBehaviour
{
    public GameObject menu = null;
    public GameObject button = null;

    public bool gameRunning = false;

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update() {

        //enable and then disable the menu after first pause
        if (MenuStack.instance.isGame && !gameRunning)
        {
            menu.SetActive(true);
            gameRunning = true;
        }
        else if (!MenuStack.instance.isGame && gameRunning)
        {
            menu.SetActive(false);
        }

        //turn the button off
        if (MenuStack.instance.isGame)
        {
            button.SetActive(false);
        }
	}
}
