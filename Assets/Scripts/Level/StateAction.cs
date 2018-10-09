using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* class StateAction 
* child class of LevelAction
* 
* action for changing the appearances of levels
* 
* @author: Bradley Booth, Academy of Interactive Entertainment, 2018
*/
public class StateAction : LevelAction
{
    //custom type for exit
    public enum EStateType
    {
        ENTRANCE,
        EXIT,
        EXIT_ANIM
    }

    public EStateType state;

    /*
    * StateAction(Level l, EStateType s)
    * constructor, assigns level and state type
    * 
    * @param Level l - the level that the action applies to
    * @param EStateType s - the type of state-action to apply
    */
    public StateAction(Level l, EStateType s) : base(l)
    {
        state = s;
    }

	void Start ()
    {
		
	}

	void Update ()
    {
		
	}

    /*
    * Execute 
    * overrides LevelAction's Execute()
    * 
    * executes the specified action
    * 
    * @returns bool - should the action be removed?
    */
    public override bool Execute()
    {
        if (state == EStateType.ENTRANCE)
        {
            level.OnEntrance();
        }
        else if (state == EStateType.EXIT)
        {
            level.OnExit();
        }
        else if (state == EStateType.EXIT_ANIM)
        {
            level.OnExitAnimation();
        }

        return true;
    }
}
