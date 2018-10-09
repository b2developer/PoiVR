using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* class LevelManager
* 
* singleton class that holds all references to level base objects and manages their transitions
* 
* @author: Bradley Booth, Academy of Interactive Entertainment, 2018 
*/
public class LevelManager : MonoBehaviour
{
    public int INIT_PIORITY = 1;

    //singleton instance
    public static LevelManager instance = null;

    //level that switches between others
    public Level holodeck = null;
    public Level[] levels;
    public int activeID = 0;

    public List<LevelAction> queue;

    private void AwakePiority()
    {
        instance = this;
        queue = new List<LevelAction>();

        //activates disabled level folders (they should always be active but may not be for display and debugging purposes)
        foreach (Level l in levels)
        {
            //activate the gameobject if it isn't active
            if (!l.gameObject.activeSelf)
            {
                l.gameObject.SetActive(true);
            }

        }

        //turn on the holodeck
        foreach (GameObject g in holodeck.assets)
        {
            g.SetActive(true);
        }

        //turn off all other levels
        foreach (Level l in levels)
        {
            foreach (GameObject g in l.assets)
            {
                g.SetActive(false);
            }
        }

        //instantaneously switch to the holodeck
        ApplyLighting(holodeck);
    }

    void Start ()
    {
        
	}
	
	void Update ()
    {
        //don't run the queue if it is empty
        if (queue.Count == 0)
        {
            return;
        }

        LevelAction action = queue[0];

        //execute the next action in the queue, clear it when it is finished
        while (action.Execute())
        {
            queue.RemoveAt(0);

            if (queue.Count > 0)
            {
                action = queue[0];
            }
            else
            {
                break;
            }
        }
    }

    public static float HOLO_ANIMATION_DURATION = 3.375f + 0.1f;

    /*
    * Load 
    * 
    * loads a new level by loading the holodeck, unloading the current level
    * loading the new level and finally unloading the holodeck
    * 
    * @param int id - the new level to be loaded
    * @returns void
    */
    public void Load(int id)
    {
        if (queue.Count > 0 || id == activeID)
        {
            return;
        }

        if (activeID == -1)
        {
            EnvironmentAction levelLighting = new EnvironmentAction(levels[id]);
            StateAction levelEntry = new StateAction(levels[id], StateAction.EStateType.ENTRANCE);
            StateAction holoExitAnim = new StateAction(holodeck, StateAction.EStateType.EXIT_ANIM);
            DelayAction holoDelay2 = new DelayAction(holodeck, HOLO_ANIMATION_DURATION);
            StateAction holoExit = new StateAction(holodeck, StateAction.EStateType.EXIT);
            
            queue.Add(levelLighting);
            queue.Add(levelEntry);
            queue.Add(holoExitAnim);
            queue.Add(holoDelay2);
            queue.Add(holoExit);
            
        }
        else
        {
            StateAction holoEntry = new StateAction(holodeck, StateAction.EStateType.ENTRANCE);
            DelayAction holoDelay = new DelayAction(holodeck, HOLO_ANIMATION_DURATION);
            StateAction levelExit = new StateAction(levels[activeID], StateAction.EStateType.EXIT);
            EnvironmentAction levelLighting = new EnvironmentAction(levels[id]);
            StateAction levelEntry = new StateAction(levels[id], StateAction.EStateType.ENTRANCE);
            StateAction holoExitAnim = new StateAction(holodeck, StateAction.EStateType.EXIT_ANIM);
            DelayAction holoDelay2 = new DelayAction(holodeck, HOLO_ANIMATION_DURATION);
            StateAction holoExit = new StateAction(holodeck, StateAction.EStateType.EXIT);            
           
            queue.Add(holoEntry);
            queue.Add(holoDelay);
            queue.Add(levelExit);
            queue.Add(levelLighting);
            queue.Add(levelEntry);
            queue.Add(holoExitAnim);
            queue.Add(holoDelay2);
            queue.Add(holoExit);      
            
        }

        activeID = id;
    }

    /*
    * LoadDeck 
    * 
    * loads the holodeck
    * 
    * @returns void
    */
    public void LoadDeck()
    {
        if (queue.Count > 0 || activeID == -1)
        {
            return;
        }
       
        StateAction holoEntrance = new StateAction(holodeck, StateAction.EStateType.ENTRANCE);
        DelayAction holoDelay = new DelayAction(holodeck, HOLO_ANIMATION_DURATION);
        StateAction levelExit = new StateAction(levels[activeID], StateAction.EStateType.EXIT);
        EnvironmentAction holoLighting = new EnvironmentAction(holodeck);

        queue.Add(holoEntrance);
        queue.Add(holoDelay);
        queue.Add(levelExit);
        queue.Add(holoLighting);

        activeID = -1;
    }

    /*
    * ApplyLighting 
    * 
    * sets a specific level's reflection probe and skybox lighting
    * 
    * @param Level l - the level to get apply lighting from
    * @returns void
    */
    public void ApplyLighting(Level l)
    {
        RenderSettings.skybox = l.skyboxMaterial;
        RenderSettings.customReflection = l.reflectionProbe;

        DynamicGI.UpdateEnvironment();
    }
}
