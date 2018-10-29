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
    public static float ANGLE_CLAMP = 90.0f;
    public static float DECAY_RATE = 0.94f;
    public static float VELOCITY_EPSILON = 0.01f;
    public static float CLAMP_EPSILON = 1e-7f;

    public enum EButtonState
    {
        RELEASED,
        PRESSED,
        HOVER,
    }

    public EButtonState state = EButtonState.RELEASED;

    //reset transform
    public Vector3 resetPosition = Vector3.zero;
    public Quaternion resetRotation = Quaternion.identity;

    public Pointer draggingPointer = null;

    //desired distance for the UI to place itself at when dragged
    public float setDistance = 2.0f;

    public Vector3 velocity = Vector3.zero;

    //cylindrical clamping method
    public Vector3 cylinderCentre = Vector3.zero;
    public float cylinderRadius = 0.0f;
    public float cylinderHeight = 0.0f;

  
    void Start()
    {
        resetPosition = transform.parent.position;
        resetRotation = transform.parent.rotation;

        MenuStack.instance.OnGameResumed += ResetTransform;
    }

    void Update()
    {
        //while pressed move the camera around the sphere
        if (state == EButtonState.PRESSED)
        {
            Vector3 previousPosition = transform.parent.transform.position;
            Quaternion previousRotaion = transform.parent.transform.rotation;

            transform.parent.rotation = draggingPointer.pointer.transform.rotation * resetRotation;

            Vector3 forward = transform.parent.rotation * Vector3.forward;

            float dot = Mathf.Sin(ANGLE_CLAMP * Mathf.Deg2Rad);

            if (forward.y <= dot)
            {
                forward.y = dot;
            }

            transform.parent.rotation = Quaternion.LookRotation(forward, -draggingPointer.pointer.transform.forward);

            Vector3 scaledLocal = new Vector3(transform.localPosition.x * transform.parent.lossyScale.x, transform.localPosition.y * transform.parent.lossyScale.y, transform.localPosition.z * transform.parent.lossyScale.z);

            Vector3 relativeGrab = -scaledLocal;
            relativeGrab = transform.parent.rotation * relativeGrab;

            Vector3 pointerPosition = Vector3.forward * setDistance;
            pointerPosition = draggingPointer.pointer.transform.rotation * pointerPosition;

            transform.parent.position = draggingPointer.transform.position + relativeGrab + pointerPosition;

            Vector3 relativeV = transform.parent.transform.position - previousPosition;

            velocity = relativeV / Time.unscaledDeltaTime;

            Vector3 untransformedV = Quaternion.Inverse(draggingPointer.pointer.transform.rotation) * velocity;

            //amplify pulling
            if (untransformedV.z <= 0.0f)
            {
                untransformedV.z *= 2.0f;
            }

            velocity = draggingPointer.pointer.transform.rotation * untransformedV;

            Vector3 clampFactor = ClampToCylinder(transform.position, cylinderCentre, cylinderRadius, cylinderHeight) - transform.position;

            transform.parent.transform.position += clampFactor;


        }
        else
        {

            transform.parent.transform.position += velocity * Time.unscaledDeltaTime;
            velocity *= DECAY_RATE;

            float ve2 = VELOCITY_EPSILON * VELOCITY_EPSILON;

            //set to zero if too small
            if (velocity.sqrMagnitude < ve2)
            {
                velocity = Vector3.zero;
            }

            Vector3 clampFactor = ClampToCylinder(transform.position, cylinderCentre, cylinderRadius, cylinderHeight) - transform.position;

            transform.parent.transform.position += clampFactor;
        }

    }

    /*
    * ClampToCylinder
    * 
    * clamps a given point to a cylinder in 3D space
    * 
    * @param Vector3 cyliCentre - the position of the cylinder
    * @param float cyliRadius - the XZ extent
    * @param float cyliHeight - the Y axis range
    * @returns Vector3 - the position clamped to the cylinder 
    */
    public Vector3 ClampToCylinder(Vector3 position, Vector3 cyliCentre, float cyliRadius, float cyliHeight)
    {
        Vector3 relative = position - cyliCentre;

        //clamp to the height range
        relative.y = Mathf.Clamp(relative.y, -cyliHeight * 0.5f, cyliHeight * 0.5f);

        Vector2 disk = new Vector2(relative.x, relative.z);

        disk = Vector2.ClampMagnitude(disk, cyliRadius);

        relative = new Vector3(disk.x, relative.y, disk.y);
 
        return cyliCentre + relative;
    }

    /*
    * ResetTransform
    * 
    * places the UI projection back at the spawn position
    * 
    * @returns void
    */
    public void ResetTransform()
    {
        transform.parent.position = resetPosition;
        transform.parent.rotation = resetRotation;
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

