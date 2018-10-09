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
    //reference to the level to change
    public Level level = null;

    /*
    * LevelAction(Level l)
    * constructor, sets level
    * 
    * @param Level l - the level that the action applies to
    */
    public LevelAction(Level l)
    {
        level = l;
    }

	void Start ()
    {
		
	}
	
	void Update ()
    {
		
	}

    /*
    * Execute 
    * virtual function
    * 
    * executes the specified action
    * 
    * @returns bool - should the action be removed?
    */
    public virtual bool Execute()
    {
        return true;
    }
}
