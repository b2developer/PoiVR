using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* class RecordingManipulator 
* 
* manages UI callbacks and menu states to allow dynamic manipulation
* of recording files through renaming, deleting and playback
* 
* @author: Bradley Booth, Academy of Interactive Entertainment, 2018
*/
public class RecordingManipulator : MonoBehaviour
{
    public static int MAX_CHARS = 20;

    //current rig animation in the dynamic menu
    public RigAnimation rigAnimation = null;
    public int dynamicID = 0;

    //display components
    public TextMesh nameMesh = null;
    public TextMesh timeMesh = null;

    //modification components
    public UIKeyboard keyboard = null;

    //list that contains all recordings
    public UIScrollableList list;

    //prefab for a dynamic button menu that sets up automatically for each recording
    public GameObject buttonPrefab;

    //list of spawned buttons, ID is the list position (corresponds with recording array in RecordingManager)
    public List<GameObject> buttons;


	void Start ()
    {
        keyboard.OnKeyPressed += ModifyStringName;
	}
	
	void Update ()
    {

    }
    
    /*
    * ModifyStringName 
    * 
    * accepts callbacks from a keyboard, modifies the name of an animation
    * 
    * @param string input - the string that the keyboard returned
    * @returns void
    */
    public void ModifyStringName(string input)
    {
        if (input == "back")
        {
            //subtract the last character from the string
            if (rigAnimation.id.Length > 0)
            {
                rigAnimation.id = rigAnimation.id.Substring(0, rigAnimation.id.Length - 1);
            }
        }
        else
        {
            //add the inputed character to the string
            if (rigAnimation.id.Length < MAX_CHARS)
            {
                rigAnimation.id += input;
            }
        }

        nameMesh.text = rigAnimation.id;
    }

    /*
    * SetDynamicMenu
    * 
    * sets up the dynamic menu with the desired information
    * 
    * @param int id - the id of the button
    * @returns void 
    */
    public void SetDynamicMenu(int id)
    {
        rigAnimation = RecordingManager.instance.animations[id];

        nameMesh.text = rigAnimation.id;
        timeMesh.text = rigAnimation.totalTime.ToString();

        dynamicID = id;
    }

    /*
    * DynamicSave
    * 
    * saves the current recording to external storage
    * 
    * @returns void 
    */
    public void DynamicSave()
    {
        RecordingManager.instance.Save(RecordingManager.instance.animations[dynamicID]);
    }

    /*
    * DynamicDelete
    * 
    * removes the current recording loaded in the dynamic menu
    * 
    * @returns void 
    */
    public void DynamicDelete()
    {
        RecordingManager.instance.Delete(RecordingManager.instance.animations[dynamicID]);
        DeleteButton(dynamicID);
    }

    /*
    * DeleteButton
    * 
    * deletes a new button from the array
    * 
    * @param int position - the list position to remove
    * @returns void 
    */
    public void DeleteButton(int position)
    {
        //despawn item
        Destroy(buttons[position]);
        buttons.RemoveAt(position);

        UpdateButtons();
    }

    /*
    * SpawnNewButton
    * 
    * creates a new button prefab for the additional recording
    * 
    * @param RigAnimation animation - the animation to make a new dynamic button for
    * @returns void 
    */
    public void SpawnNewButton(RigAnimation animation)
    {
        //spawn item
        GameObject nb = Instantiate<GameObject>(buttonPrefab);
        buttons.Add(nb);

        //buttons were spawned in, trigger their initialisation manually
        nb.GetComponent<UIButton>().AwakePiority();
        nb.GetComponent<UIButton>().groupValue = buttons.Count - 1;
        nb.GetComponent<UIButton>().OnClickedDelegate += SetDynamicMenu;

        nb.transform.SetParent(list.transform);

        UpdateButtons();
    }

    /*
    * UpdateButtons
    * 
    * ensures that the information that the buttons
    * have represents the current recordings array
    * 
    * @returns void
    */
    public void UpdateButtons()
    {
        list.items.Clear();

        int bl = buttons.Count;

        //update all information and assign the UIScrollableList properly
        for (int i = 0; i < bl; i++)
        {
            TextMesh[] texts = buttons[i].GetComponentsInChildren<TextMesh>();

            texts[0].text = RecordingManager.instance.animations[i].id;
            texts[1].text = RecordingManager.instance.animations[i].totalTime.ToString();

            buttons[i].GetComponent<UIButton>().groupValue = i;

            list.items.Add(buttons[i]);
        }

        //update list
        list.SetDisplayed(list.scroller.unscaledValue);
    }
}
