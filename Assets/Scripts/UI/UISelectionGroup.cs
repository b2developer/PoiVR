using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/*
* class UISelectionGroup 
* child class of UIElement
* 
* contains a list of UI checkboxes and ensures that only 
* one can be selected at a time
* 
* @author: Bradley Booth, Academy of Interactive Entertainment, 2018
*/
public class UISelectionGroup : UIElement
{
    public const int INIT_PIORITY = 1;

    //list of elements that the UI selection holds
    private List<UICheckbox> elements;
    public bool automaticallyFill;

    public UICheckbox activeElement = null;

    public int activeID = 0;

    [System.Serializable]
    public class IntEvent : UnityEvent<int> { }

    //callback queue when a button is clicked
    [SerializeField]
    public IntEvent OnActiveChanged;

    void AwakePiority()
    {
        //create a new list out of the immediate children
		if (automaticallyFill)
        {
            elements = new List<UICheckbox>();

            UICheckbox[] allChildren = gameObject.transform.GetComponentsInChildren<UICheckbox>();

            int id = 0;

            //get all of the immediate children from a list of all children
            foreach (UICheckbox child in allChildren)
            {
                if (child.transform.parent == transform)
                {
                    elements.Add(child);
                    child.groupValue = id;
                    child.OnValueChanged += OnValueChanged;

                    id++;
                }
            }

            activeElement = allChildren[activeID];

            allChildren[activeID].value = true;

            if (allChildren[activeID].valueDisplay != null)
            {
                allChildren[activeID].valueDisplay.SetActive(true);
            }
        }
	}
	
	void Update ()
    {

	}

    public void OnValueChanged(UICheckbox box, bool value)
    {
        //disable the previous active element, if there was one
        if (value)
        {
            if (activeElement != null)
            {
                if (activeElement.valueDisplay != null)
                {
                    activeElement.valueDisplay.gameObject.SetActive(false);
                }
                activeElement.value = false;
            }

            activeElement = box;

            activeID = activeElement.groupValue;

            OnActiveChanged.Invoke(activeID);
        }
        else
        {
            activeElement = null;
        }
    }
}
