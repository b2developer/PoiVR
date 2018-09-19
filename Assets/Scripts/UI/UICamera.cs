using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* class UICamera 
* child class of UIElement
* 
* interface for a draggable object that clamps itself to a hemi-sphere around a transform
* and constantly looks at another with free control of it's roll rotation
* 
* @author: Bradley Booth, Academy of Interactive Entertainment, 2018
*/
public class UICamera : UIElement
{
    public enum EButtonState
    {
        RELEASED,
        PRESSED,
        HOVER,
    }

    public EButtonState state = EButtonState.RELEASED;

    public Pointer draggingPointer = null;

    public Transform lockCentre;
    public float lockRadius;
   
    //zooming in the camera
    public float zoomSpeed = 3.0f;
    public float zoomMin = 1.0f;
    public float zoomMax = 7.5f;

    private Vector2 currentPadPress = Vector2.zero;

    //location of the player
    public Transform playerTransform;

    void Start()
    {
        
    }

    void Update()
    {
        //while pressed move the camera around the sphere
        if (state == EButtonState.PRESSED)
        {
            Vector3 S = draggingPointer.pointer.transform.position;
            Vector3 D = draggingPointer.pointer.transform.rotation * Vector3.forward;

            Vector3 C = lockCentre.transform.position;
            float R = lockRadius;

            //checks that a line is intersecting a sphere, two intersections are possible so the equation takes a quadratic form
            float a = (D.x * D.x + D.y * D.y + D.z * D.z);
            float b = (2 * D.x * C.x - 2 * D.x * S.x +
                        2 * D.y * C.y - 2 * D.y * S.y +
                        2 * D.z * C.z - 2 * D.z * S.z);
            float c = (S.x * S.x + C.x * C.x - 2 * S.x * C.x +
                        S.y * S.y + C.y * C.y - 2 * S.y * C.y +
                        S.z * S.z + C.z * C.z - 2 * S.z * C.z - R * R);

            Quadratic q = new Quadratic(a, b, c);

            float[] roots = q.Solve();

            for (int i = 0; i < roots.GetLength(0); i++)
            {
                roots[i] *= -1.0f;
            }
        
            int rl = roots.GetLength(0);

            Vector3 intersection = Vector3.zero;

            if (rl == 1)
            {
                //only one intersection occured (very rare edge case)
                intersection = S + D * roots[0];
            }
            else if (rl == 2)
            {
                //two intersections occured, get the one that was closest from the line origin
                float min = Mathf.Min(roots);

                //don't use negative numbers for 't'
                if (min < 0.0f)
                {
                    min = Mathf.Max(roots);
                }

                intersection = S + D * min;
            }

            transform.position = intersection;

            //clamp to the floor
            float P = lockCentre.position.y;

            float u = 1 - (intersection.y - P) / (intersection.y - S.y);

            //clamp the sphere intersection down to a hemi-sphere intersection if neccessary
            if (u > 0.0f && u < 1.0f)
            {
                float xSpan = intersection.x - S.x;
                float zSpan = intersection.z - S.z;

                float x = S.x + xSpan * u;
                float z = S.z + zSpan * u;

                intersection = new Vector3(x, lockCentre.position.y, z);

                transform.position = intersection;
            }

            //use the rolling rotation of the remote to allow furthur manipulation of the camera
            transform.up = draggingPointer.pointer.transform.up;
            transform.rotation = Quaternion.LookRotation((playerTransform.position - transform.position).normalized, draggingPointer.pointer.transform.up);
           
        }

        //detect zoom
        if (currentPadPress != Vector2.zero)
        {
            if (currentPadPress.y > 0.0f)
            {
                lockRadius += zoomSpeed * Time.deltaTime;
            }
            else if (currentPadPress.y < 0.0f)
            {
                lockRadius -= zoomSpeed * Time.deltaTime;
            }

            lockRadius = Mathf.Clamp(lockRadius, zoomMin, zoomMax);
        }
    }

    /*
    * OnPadPress 
    * 
    * triggered when the pad is pressed that zooms the camera
    * 
    * @param Vector2 point - the area of the pad pressed
    */
    public void OnPadPress(Vector2 point)
    {
        if (state == EButtonState.PRESSED)
        {
            currentPadPress = point;
        }
    }

    /*
    * OnPadPress
    * 
    * triggered when the pad zooming the camera is released
    * 
    * @returns void 
    */
    public void OnPadRelease()
    {
        currentPadPress = Vector2.zero;
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
        currentPadPress = Vector2.zero;
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
