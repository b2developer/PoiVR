using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
* class UIProjection 
* child class of UIElement
* 
* interface for a draggable object that itself represent is a menu
* 
* @author: Bradley Booth, Academy of Interactive Entertainment, 2018
*/
public class UIProjection : UIElement
{
    public enum EButtonState
    {
        RELEASED,
        PRESSED,
        HOVER,
    }

    public EButtonState state = EButtonState.RELEASED;

    public Pointer draggingPointer = null;

    //desired distance for the UI to place itself at when dragged
    public float setDistance = 2.0f;

    Vector3 relativePosition = Vector3.zero;
    Quaternion relativeRotation = Quaternion.identity;

    public Vector3 velocity = Vector3.zero;
    public float decayRate = 0.94f;
    public float velocityEpsilon = 0.01f;

    void Start()
    {

    }

    void Update()
    {
        //while pressed move the camera around the sphere
        if (state == EButtonState.PRESSED)
        {
            Vector3 previousPosition = transform.parent.transform.position;
            Quaternion previousRotaion = transform.parent.transform.rotation;

            transform.parent.transform.position = draggingPointer.pointer.transform.position + draggingPointer.pointer.transform.rotation * relativePosition;
            transform.parent.transform.rotation = draggingPointer.pointer.transform.rotation * relativeRotation;

            Vector3 relativeV = transform.parent.transform.position - previousPosition;

            velocity = relativeV / Time.unscaledDeltaTime;

            Vector3 untransformedV = Quaternion.Inverse(draggingPointer.pointer.transform.rotation) * velocity;

            //amplify pulling
            if (untransformedV.z <= 0.0f)
            {
                untransformedV.z *= 2.0f;
            }

            velocity = draggingPointer.pointer.transform.rotation * untransformedV;
        }
        else
        {

            transform.parent.transform.position += velocity * Time.unscaledDeltaTime;
            velocity *= decayRate;

            float ve2 = velocityEpsilon * velocityEpsilon;

            //set to zero if too small
            if (velocity.sqrMagnitude < ve2)
            {
                velocity = Vector3.zero;
            }
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
        state = EButtonState.PRESSED;

        draggingPointer = pointer;

        relativePosition = Quaternion.Inverse(pointer.pointer.transform.rotation) * (transform.parent.position - pointer.pointer.transform.position);
        relativeRotation = Quaternion.Inverse(pointer.pointer.transform.rotation) * transform.parent.rotation;

        //clamp if required
        float se2 = setDistance * setDistance;

        if (se2 < relativePosition.sqrMagnitude)
        {
            float minus = relativePosition.magnitude - setDistance;
            relativePosition = Quaternion.Inverse(pointer.pointer.transform.rotation) * (transform.parent.position - pointer.pointer.transform.position + pointer.pointer.transform.rotation * Vector3.back * minus);

            //relativePosition.Normalize();
            //relativePosition *= setDistance;
        }
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
        state = EButtonState.RELEASED;

        draggingPointer = null;
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
        //only react to the hover event if the button isn't pressed
        if (state != EButtonState.PRESSED)
        {
            state = EButtonState.HOVER;
        }
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
        //only release if was hoverin
        if (state == EButtonState.HOVER)
        {
            state = EButtonState.RELEASED;
        }
    }
}

