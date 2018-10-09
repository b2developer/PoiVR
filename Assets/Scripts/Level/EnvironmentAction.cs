using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* class EnvironmentAction 
* child class of LevelAction
* 
* action for changing the reflection probe and skybox
* 
* @author: Bradley Booth, Academy of Interactive Entertainment, 2018
*/
public class EnvironmentAction : LevelAction
{
    /*
    * EnvironmentAction(Level l)
    * constructor, assigns level
    * 
    * @param Level l - the level that the action applies to
    */
    public EnvironmentAction(Level l) : base(l)
    {

    }

    void Start()
    {

    }

    void Update()
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
        LevelManager.instance.ApplyLighting(level);
        return true;
    }
}
