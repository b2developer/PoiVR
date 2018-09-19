using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/*
* class UISlider 
* child class of UIElement
* 
* a 3D collider that can be dragged along a 3D line to modify a scalar value
* 
* @author: Bradley Booth, Academy of Interactive Entertainment, 2018
*/
public class UISlider : UIElement
{
    public const int INIT_PIORITY = 1;

    public float FORWARD_EPSILON = 0.1f;

    public enum EButtonState
    {
        RELEASED,
        PRESSED,
        HOVER,
    }

    public EButtonState state = EButtonState.RELEASED;

    private Material currentMaterial;
    private float time = 0.0f;

    public float transitionTime = 0.1f;

    public ColourScheme.EColour colour = ColourScheme.EColour.NONE;

    public Color inactiveColor;
    public Color hoverColor;
    public Color pressedColor;

    public Color currentColor;
    public Color previousColor;

    //pointer that triggered drag events
    public Pointer draggingPointer = null;

    //ends of the slidable area
    public Transform startLock;
    public Transform endLock;

    //between 0 and 1 representing each end of the slider
    public float unscaledValue = 0.0f;

    [System.Serializable]
    public class FloatEvent : UnityEvent<float> { }

    //callback queue when the button is clicked
    [SerializeField]
    public FloatEvent OnValueChanged;

    void AwakePiority()
    {
        ColourScheme.instance.SetSliderColours(this, colour);

        currentMaterial = GetComponent<MeshRenderer>().material;
        currentColor = inactiveColor;

        previousColor = new Color(0, 0, 0, 0);

        //don't trigger transition
        time = transitionTime;
    }

    void Update()
    {
        //simple timer script
        if (time < transitionTime)
        {
            time += Time.deltaTime;

            if (time > transitionTime)
            {
                time = transitionTime;
            }
        }

        float ratio = time / transitionTime;

        currentMaterial.color = Color.Lerp(previousColor, currentColor, ratio);

        //while pressed move the handle
        if (state == EButtonState.PRESSED)
        {
            Vector3 rayStart = draggingPointer.pointer.transform.position;
            Vector3 rayEnd = draggingPointer.pointer.transform.position + draggingPointer.pointer.transform.rotation * Vector3.forward * draggingPointer.castDistance;

            Plane sliderPlane = new Plane(startLock.rotation * Vector3.up, startLock.transform.position);

            Ray pointer = new Ray(rayStart, rayEnd - rayStart);

            float distance = 0.0f;

            //collide the point with the slider's plane
            if (sliderPlane.Raycast(pointer, out distance))
            {
                Vector3 planar = pointer.GetPoint(distance);

                Debug.DrawLine(rayStart, planar, Color.green);
                Debug.DrawLine(startLock.position, endLock.position, Color.cyan);

                Vector3 D = (endLock.position - startLock.position).normalized;
                Vector3 E = endLock.position - startLock.position;

                float endDot = Vector3.Dot(D, E);
                float dot = Vector3.Dot(D, planar - startLock.position);

                dot = Mathf.Clamp(dot, 0.0f, endDot);

                Vector3 NP = startLock.position + D * dot;

                transform.position = NP + transform.up * FORWARD_EPSILON;

                float mappedValue = dot / endDot;
                unscaledValue = mappedValue;

                if (OnValueChanged != null)
                {
                    OnValueChanged.Invoke(unscaledValue);
                }
            }
        }
    }

    /*
    * OverrideValue
    * 
    * graphically overrides the slider's current value
    * 
    * @param float value - the new value to give the slider
    * @returns void
    */
    public void OverrideValue(float value)
    {
        unscaledValue = value;

        Vector3 E = endLock.position - startLock.position;

        Vector3 NP = startLock.position + E * value;

        transform.position = NP + transform.up * FORWARD_EPSILON;
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

        previousColor = currentMaterial.color;
        currentColor = pressedColor;
        time = 0.0f;

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
        previousColor = currentMaterial.color;
        currentColor = inactiveColor;
        time = 0.0f;

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
            previousColor = currentMaterial.color;
            currentColor = hoverColor;
            time = 0.0f;

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
            previousColor = currentMaterial.color;
            currentColor = inactiveColor;
            time = 0.0f;

            state = EButtonState.RELEASED;
        }
    }

}
