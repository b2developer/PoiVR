using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* class LevelAction 
* 
* object that represents a level in motion
* 
* @author: Bradley Booth, Academy of Interactive Entertainment, 2018
*/
public class LevelAction
{

    public enum ELevelState
    {
        ENTRANCE,
        EXIT,
    }

    public Level level = null;
    public ELevelState state = ELevelState.ENTRANCE;

	void Start ()
    {
		
	}
	
	void Update ()
    {
		
	}

    /*
    * Execute 
    * 
    * executes the specified action
    * 
    * @returns void
    */
    public void Execute()
    {
        if (state == ELevelState.ENTRANCE)
        {
            level.OnEntrance();
        }
        else if (state == ELevelState.EXIT)
        {
            level.OnExit();
        }
    }
}
