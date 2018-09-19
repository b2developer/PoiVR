using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/*
* class UICheckbox 
* child class of UIElement
* 
* a 3D collider that can be toggled using pushbacks
* 
* @author: Bradley Booth, Academy of Interactive Entertainment, 2018
*/
public class UICheckbox : UIElement
{
    public const int INIT_PIORITY = 1;

    public delegate void ButtonEvent(UICheckbox c, bool v);

    public ButtonEvent OnValueChanged = null;

    //value given to the checkbox relative to it's neighbours in a selection group
    public int groupValue = -1;

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

    public bool value = false;
    public GameObject valueDisplay = null;

    public Color currentColor;
    public Color previousColor;

    //callback queue when the button is clicked
    public UnityEvent OnClicked;

    void AwakePiority()
    {
        ColourScheme.instance.SetCheckboxColours(this, colour);

        currentMaterial = GetComponent<MeshRenderer>().material;
        currentColor = inactiveColor;

        //don't trigger transition
        time = transitionTime;

        valueDisplay.SetActive(value);
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
        OnClicked.Invoke();

        previousColor = currentMaterial.color;
        currentColor = pressedColor;
        time = 0.0f;

        state = EButtonState.PRESSED;

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

        //an active checkbox can't be directly turned off when it is part of a selection group
        if (groupValue != -1 && value)
        {
            return;   
        }

        value = !value;
        valueDisplay.SetActive(value);

        if (OnValueChanged != null)
        {
            OnValueChanged.Invoke(this, value);
        }

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