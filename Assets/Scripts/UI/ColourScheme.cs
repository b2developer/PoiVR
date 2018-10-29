using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* class ColourScheme 
* 
* contains a library of colours that various UI Elements can use
* to set their appearance, used to synchronise colours
* 
* @author: Bradley Booth, Academy of Interactive Entertainment, 2018
*/
public class ColourScheme : MonoBehaviour
{
    public const int INIT_PIORITY = 0;

    public static ColourScheme instance = null;

    //custom type for supported display colours
    public enum EColour
    {
        NONE,
        RED,
        BLUE,
        GREEN,
        WHITE,
    }

    public Color whiteIdle;
    public Color whiteHover;
    public Color whitePress;

    public Color blueIdle;
    public Color blueHover;
    public Color bluePress;

    public Color redIdle;
    public Color redHover;
    public Color redPress;

    public Color greenIdle;
    public Color greenHover;
    public Color greenPress;

    private void AwakePiority()
    {
        instance = this;
    }

    // Use this for initialization
    void Start ()
    {
        
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    /*
    * SetButtonColours 
    * 
    * assigns colour properties of a button, changing it's appearance in-game
    * 
    * @param UIButton button - the button to apply the colour to
    * @param EColour colour - the colour to apply
    * @returns void
    */
    public void SetButtonColours(UIButton button, EColour colour)
    {
        //apply the selected colour scheme
        switch (colour)
        {
            case EColour.NONE: break;

            case EColour.WHITE:
                button.inactiveColor = whiteIdle;
                button.hoverColor = whiteHover;
                button.pressedColor = whitePress;
                break;

            case EColour.RED:
                button.inactiveColor = redIdle;
                button.hoverColor = redHover;
                button.pressedColor = redPress;
                break;

            case EColour.GREEN:
                button.inactiveColor = greenIdle;
                button.hoverColor = greenHover;
                button.pressedColor = greenPress;
                break;

            case EColour.BLUE:
                button.inactiveColor = blueIdle;
                button.hoverColor = blueHover;
                button.pressedColor = bluePress;
                break;
        }
    }

    /*
    * SetCheckboxColours 
    * 
    * assigns colour properties of a checkbox, changing it's appearance in-game
    * 
    * @param UICheckbox checkbox - the checkbox to apply the colour to
    * @param EColour colour - the colour to apply
    * @returns void
    */
    public void SetCheckboxColours(UICheckbox checkbox, EColour colour)
    {
        //apply the selected colour scheme
        switch (colour)
        {
            case EColour.NONE: break;

            case EColour.WHITE:
                checkbox.inactiveColor = whiteIdle;
                checkbox.hoverColor = whiteHover;
                checkbox.pressedColor = whitePress;
                break;

            case EColour.RED:
                checkbox.inactiveColor = redIdle;
                checkbox.hoverColor = redHover;
                checkbox.pressedColor = redPress;
                break;

            case EColour.GREEN:
                checkbox.inactiveColor = greenIdle;
                checkbox.hoverColor = greenHover;
                checkbox.pressedColor = greenPress;
                break;

            case EColour.BLUE:
                checkbox.inactiveColor = blueIdle;
                checkbox.hoverColor = blueHover;
                checkbox.pressedColor = bluePress;
                break;
        }
    }

    /*
    * SetSliderColours 
    * 
    * assigns colour properties of a slider, changing it's appearance in-game
    * 
    * @param UISlider slider - the checkbox to apply the colour to
    * @param EColour colour - the colour to apply
    * @returns void
    */
    public void SetSliderColours(UISlider slider, EColour colour)
    {
        //apply the selected colour scheme
        switch (colour)
        {
            case EColour.NONE: break;

            case EColour.WHITE:
                slider.inactiveColor = whiteIdle;
                slider.hoverColor = whiteHover;
                slider.pressedColor = whitePress;
                break;

            case EColour.RED:
                slider.inactiveColor = redIdle;
                slider.hoverColor = redHover;
                slider.pressedColor = redPress;
                break;

            case EColour.GREEN:
                slider.inactiveColor = greenIdle;
                slider.hoverColor = greenHover;
                slider.pressedColor = greenPress;
                break;

            case EColour.BLUE:
                slider.inactiveColor = blueIdle;
                slider.hoverColor = blueHover;
                slider.pressedColor = bluePress;
                break;
        }
    }
}
