using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* class DelayActtion 
* child class of LevelAction
* 
* action for creating a delay in time
* 
* @author: Bradley Booth, Academy of Interactive Entertainment, 2018
*/
public class DelayAction : LevelAction
{

    /*
    * DelayAction(Level l, float time)
    * constructor, assigns level and delay time
    * 
    * @param Level l - the level that the action applies to
    * @param float time - total time to day
    */
    public DelayAction(Level l, float time) : base(l)
    {
        remainingTime = time;
    }

    public float remainingTime = 0.0f;

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
        remainingTime -= Time.deltaTime;

        if (remainingTime < 0.0f)
        {
            remainingTime = 0.0f;
        }

        return remainingTime <= 0.0f;
    }
}
