using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

/*
* Initialiser 
* 
* creates a piority queue for other components that need to be
* initialised in a certain order. Components can subscribe to the 
* piority queue by including the INIT_PIORITY variable and the AwakePiority function
* which is detected using reflection
* 
* @author: Bradley Booth, Academy of Interactive Entertainment, 2018
*/
public class Initialiser : MonoBehaviour
{

    public static string VAR_STRING = "INIT_PIORITY";
    public static string FUNC_STRING = "AwakePiority";

    /*
    * class InitialisedItem
    * 
    * package that stores a MonoBehaviour, an invokable function from it
    * and it's piority value. This is used for sorting
    * 
    * @author: Bradley Booth, Academy of Interactive Entertainment, 2018
    */
    public class InitialisedItem
    {
        public MonoBehaviour behaviour;
        public MethodInfo function;
        public int piority = 0;
    }

    /*
    * Compare 
    * 
    * compares the piority value of each item
    * 
    * @param InitialisedItem a 
    * @param InitialisedItem b
    * @returns int - result of the compasiron
    */
    public int Compare(InitialisedItem a, InitialisedItem b)
    {
        if (a.piority < b.piority)
        {
            return -1;
        }
        else if (a.piority > b.piority)
        {
            return 1;
        }

        //identical piority values
        return 0;
    }

	void Awake ()
    {
        //get all components in the scene
        MonoBehaviour[] behaviours = Resources.FindObjectsOfTypeAll<MonoBehaviour>();

        List<InitialisedItem> queue = new List<InitialisedItem>();

        //check each mono behaviour for the INIT_PIORITY variable and AwakePiority function
        foreach (MonoBehaviour mb in behaviours)
        {
            System.Type type = mb.GetType();

            //get the variable
            FieldInfo fi = type.GetField(VAR_STRING);
            MethodInfo mi = type.GetMethod(FUNC_STRING, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

            //don't add it to the queue if one of these properties isn't present
            if (fi == null || mi == null)
            {
                continue;
            }

            //couple the item with some useful information for invoking the initialsiation function and accessing the piority value
            InitialisedItem item = new InitialisedItem();

            item.behaviour = mb;
            item.function = mi;
            item.piority = (int)fi.GetValue(mb);

            queue.Add(item);
        }

        queue.Sort(Compare);

        //invoke all of the initialisable items
        foreach (InitialisedItem item in queue)
        {
            item.function.Invoke(item.behaviour, new object[] { });
        }
	}
}
