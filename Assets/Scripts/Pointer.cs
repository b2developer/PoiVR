using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* class Pointer 
* 
* watches callbacks from the remote manager and sends raycasts out
* to trigger UI events
* 
* @author: Bradley Booth, Academy of Interactive Entertainment, 2018
*/
public class Pointer : MonoBehaviour
{
    public bool isOn = true;

    //graphical representation of the ray
    public LineRenderer laser;

    public GameObject pointer;

    private UIElement selectedElement = null;
    private UIElement hoveredElement = null;

    //distance of the ray
    public float castDistance = 5.0f;

    public enum EDirection
    {
        LEFT,
        RIGHT,
    }

    public EDirection direction;

	void Start ()
    {
        Set(false);
	}
	
	void Update ()
    {
        //don't do anything if off
        if (!isOn)
        {
            return;
        }

        laser.SetPositions(new Vector3[] { pointer.transform.position, pointer.transform.position + pointer.transform.rotation * Vector3.forward * castDistance });

        //raycast to detect hover events
        RaycastHit hitInfo;

        if (Physics.Raycast(pointer.transform.position, pointer.transform.rotation * Vector3.forward, out hitInfo, castDistance))
        {
            UIElement element = hitInfo.collider.GetComponent<UIElement>();

            //trigger a click event
            if (element != null)
            {
                //detect when something isn't hovered over
                if (hoveredElement != null)
                {
                    hoveredElement.OnNotHover();
                    hoveredElement = null;
                }

                hoveredElement = element;
                hoveredElement.OnHover();
            }

        }
        else
        {
            //detect when something isn't hovered over
            if (hoveredElement != null)
            {
                hoveredElement.OnNotHover();
                hoveredElement = null;
            }
        }
    }

    /*
    * OnClick 
    * 
    * call-back invoked when the VR remote triggers are pressed
    * 
    * @returns void
    */
    public void OnClick()
    {
        //don't do anything if off
        if (!isOn)
        {
            return;
        }

        RaycastHit hitInfo;

        if (Physics.Raycast(pointer.transform.position, pointer.transform.rotation * Vector3.forward, out hitInfo, castDistance))
        {
            UIElement element = hitInfo.collider.GetComponent<UIElement>();

            //trigger a click event
            if (element != null)
            {
                selectedElement = element;
                selectedElement.OnNotHover();

                //automatically disable hovering events
                if (hoveredElement != null)
                {
                    hoveredElement.OnNotHover();
                    hoveredElement = null;
                }

                selectedElement.OnClick(this);
            }

        }
    }

    /*
    * OnRelease 
    * 
    * call-back invoked when the VR remote triggers are released
    * 
    * @returns void
    */
    public void OnRelease()
    {
        if (selectedElement != null)
        {
            selectedElement.OnRelease();
            selectedElement = null;
        }
    }

    /*
    * Set 
    * 
    * turns the pointer on or off
    * 
    * @param bool value - the switch
    * @returns void
    */
    public void Set(bool value)
    {
        isOn = value;
        laser.enabled = value;
    }
}
