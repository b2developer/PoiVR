using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* class UIScrollableList 
* child class of UIElement
* 
* uses a slider and transform to create a scrollable list of displayable objects
* 
* @author: Bradley Booth, Academy of Interactive Entertainment, 2018
*/
public class UIScrollableList : UIElement
{
    //scrollable value
    public UISlider scroller;

    //places items can be displayed at
    public List<Transform> displays;

    //items to display depending on the scroller value
    public List<GameObject> items;

	void Start ()
    {
        SetDisplayed(0.0f);
    }
	
	void Update ()
    {
		
	}

    /*
    * SetDisplay
    * 
    * enables / disables items depending on the scrollable region
    * 
    * @param float value - allows the function to be a value
    * @returns void 
    */
    public void SetDisplayed(float value)
    {
        //disable all items
        foreach (GameObject item in items)
        {
            item.SetActive(false);
        }

        int scrollExtent = items.Count - displays.Count;
        
        if (scrollExtent > 0)
        {
            int startingValue = Mathf.RoundToInt(scroller.unscaledValue * scrollExtent);

            //map the items to the locations
            for (int i = startingValue; i < startingValue + displays.Count; i++)
            {
                items[i].SetActive(true);
                items[i].transform.SetParent(displays[i - startingValue]);
                items[i].transform.localPosition = Vector3.zero;
            }
        }
        else
        {
            //map the small amount of items to appropriate locations
            for (int i = 0; i < items.Count; i++)
            {
                items[i].SetActive(true);
                items[i].transform.SetParent(displays[i]);
                items[i].transform.localPosition = Vector3.zero;
            }
        }
    }
}
