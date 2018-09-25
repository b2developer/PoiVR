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

        foreach (Level l in levels)
        {
            //activate the gameobject if it isn't active
            if (!l.gameObject.activeSelf)
            {
                l.gameObject.SetActive(true);
            }

            l.OnExit();
        }

        LoadDeck();
    }

    void Start ()
    {
        
	}
	
	void Update ()
    {
        //remove finished level actions
        for (int i = 0; i < queue.Count; i++)
        {
            //remove the most recent finished level and execute the next animation
            if (!queue[i].level.isAnimating())
            {
                queue.RemoveAt(i);
               
                //check that there is a next item to execute
                if (i >= 0 && i < queue.Count)
                {
                    queue[i].Execute();
                }

                i--;
            }
            else
            {
                break;
            }
        }
    }

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
        LevelAction holoEnter = new LevelAction();
        holoEnter.level = holodeck;
        holoEnter.state = LevelAction.ELevelState.ENTRANCE;
        queue.Add(holoEnter);

        //only exit the current level if it is not the holodeck
        if (activeID != -1)
        {
            LevelAction activeExit = new LevelAction();
            activeExit.level = levels[activeID];
            activeExit.state = LevelAction.ELevelState.EXIT;
            queue.Add(activeExit);

        }

        activeID = id;
        LevelAction newEnter = new LevelAction();
        newEnter.level = levels[activeID];
        newEnter.state = LevelAction.ELevelState.ENTRANCE;
        queue.Add(newEnter);

        LevelAction holoExit = new LevelAction();
        holoExit.level = holodeck;
        holoExit.state = LevelAction.ELevelState.EXIT;
        queue.Add(holoExit);

        queue[0].Execute();

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
        LevelAction holoEnter = new LevelAction();
        holoEnter.level = holodeck;
        holoEnter.state = LevelAction.ELevelState.ENTRANCE;
        queue.Add(holoEnter);


        //only exit the current level if it is not the holodeck
        if (activeID != -1)
        {
            LevelAction activeExit = new LevelAction();
            activeExit.level = levels[activeID];
            activeExit.state = LevelAction.ELevelState.EXIT;
            queue.Add(activeExit);
        }

        queue[0].Execute();

        activeID = -1;
    }
}
