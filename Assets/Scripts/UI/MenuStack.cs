using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
* class MenuStack 
* 
* contains a stack of menu parents that ensures that only the uppermost 
* menu is active (being rendered and taking input)
* 
* @author: Bradley Booth, Academy of Interactive Entertainment, 2018
*/
public class MenuStack : MonoBehaviour
{
    public delegate void VoidFunc();

    public static MenuStack instance = null;

    //delegate for when the menu enters the game-state
    public VoidFunc OnGameResumed;

    public bool isGame = false;

    public GameObject gameState = null;
    public List<GameObject> stack;

    void Start()
    {
        instance = this;

        Add(gameState);
    }

    void Update()
    {
        
    }

    /*
    * Add 
    * 
    * adds a menu to the top of the stack
    * disables the previously uppermost menu (if there is one)
    * 
    * @param GameObject n - the menu to add
    * @returns void
    */
    public void Add(GameObject n)
    {
        if (stack.Count > 0)
        {
            stack[0].SetActive(false);
        }

        stack.Insert(0, n);

        n.SetActive(true);

        isGame = stack[0] == gameState;
    }

    /*
    * Pop 
    * 
    * removes menus from the top of the stack
    * 
    * @param int layers - the amount of layers to remove
    * @returns void
    */
    public void Pop(int layers)
    {
        for (int i = 0; i < layers; i++)
        {
            //disable the uppermost and remove it
            if (stack.Count > 0)
            {
                stack[0].SetActive(false);
                stack.RemoveAt(0);

                //enable the new uppermost (if there is one)
                if (stack.Count > 0)
                {
                    stack[0].SetActive(true);
                }
            }
            else
            {
                break;
            }
        }

        if (stack.Count == 0)
        {
            //leave the game
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #elif UNITY_STANDALONE
            Application.Quit();
            #endif
        }
        else
        {
            isGame = stack[0] == gameState;

            if (isGame)
            {
                OnGameResumed();
            }
        }
       
    }

    /*
    * PopToTop 
    * 
    * essentially unpauses the game by popping all states but the last
    * 
    * @returns void
    */
    public void PopToTop()
    {
        Pop(stack.Count - 1);
    }

}
