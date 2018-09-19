using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* class UINotification
* child class of UIElement
* 
* a 3D image that appears for a finite amount of time, can be
* used to notify the player
* 
* @author: Bradley Booth, Academy of Interactive Entertainment, 2018
*/
public class UINotification : UIElement
{

    //public flag for readability
    public bool visible;

    //how long the element stays visible for
    public float visibleTime = 1.0f;

    //keeps track of how long the notification has been enabled for
    private float timer = 0.0f;

    //reference to the component to enable/disable
    private MeshRenderer meshRenderer;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        timer = visibleTime;
        SetVisible(false);
    }

    void Update()
    {
        //increment the timer if needed
        if (timer < visibleTime)
        {
            timer += Time.unscaledDeltaTime;
        }

        //disable the timer
        if (timer > visibleTime && visible)
        {
            SetVisible(false);
        }
    }

    /*
    * SetVisible 
    * 
    * enables/disables the renderer
    * 
    * @param bool visible_ - the visibility flag
    * @returns void
    */
    public void SetVisible(bool visible_)
    {
        visible = visible_;
        meshRenderer.enabled = visible;

        //reset the timer
        if (visible)
        {
            timer = 0.0f;
        }
    }

    /*
    * OnClick 
    * overrides UIElement's OnClick()
    * 
    * triggered when the user clicks on this element
    * 
    * @param Pointer pointer - the pointer that triggered the event
    * @returns void
    */
    public override void OnClick(Pointer pointer)
    {
       
    }

    /*
    * OnRelease 
    * overrides UIElement's OnClick()
    * 
    * triggered when the user releases the remote off this element
    * 
    * @returns void
    */
    public override void OnRelease()
    {
        
    }

    /*
    * OnHover 
    * overrides UIElement's OnClick()
    * 
    * triggered when the user's raycast enters the area without pressing it
    * 
    * @returns void
    */
    public override void OnHover()
    {
        
    }

    /*
    * OnNotHover 
    * overrides UIElement's OnNotHover()
    * 
    * triggered when the user's raycast exits the area without pressing it
    * 
    * @returns void
    */
    public override void OnNotHover()
    {
        
    }

}
