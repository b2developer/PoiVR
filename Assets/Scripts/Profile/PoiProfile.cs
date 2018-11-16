using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* class PoiProfile 
* 
* serialisable object that assigns all of the properties of the poi-rope
* and can be modified using UI in the real-time
* 
* @author: Bradley Booth, Daniel Witt, Academy of Interactive Entertainment, 2018
*/
public class PoiProfile
{
    //static constraints to make sure poi profile values aren't ridiculous
    public const float MIN_ELASTICITY = 12500.0f;
    public const float MAX_ELASTICITY = 22500.0f;
    public const float MIN_DRAG = 0.5f;
    public const float MAX_DRAG = 1.5f;
    public const float MIN_TIMESCALE = 0.5f;
    public const float MAX_TIMESCALE = 1.0f;
    public const float MIN_GRAVITY = 7.0f;
    public const float MAX_GRAVITY = 14.0f;
    public const int MIN_TETHERLENGTH = 3;
    public const int MAX_TETHERLENGTH = 10;
    public const float MIN_HANDLEOFFSET = -0.075f;
    public const float MAX_HANDLEOFFSET = 0.075f;

    //profile's properties
    public float elasticity;
    public float drag;
    public float timeScale;
    public float gravity;
    public float tetherLength;
    public float handleOffset;

    /*
    * PoiProfile(string data)
    * constructor, deserialises the string
    * 
    * @param string data - the string representation
    */
    public PoiProfile(string data)
    {
        string[] parts = data.Split(',');

        //extract all data
        elasticity = float.Parse(parts[0]);
        drag = float.Parse(parts[1]);
        timeScale = float.Parse(parts[2]);
        gravity = float.Parse(parts[3]);
        tetherLength = float.Parse(parts[4]);
        handleOffset = float.Parse(parts[5]);
    }

    /*
    * Serialise 
    * 
    * creates a string representation of the PoiProfile
    * 
    * @returns string - the serialised object
    */
    public string Serialise()
    {
        string b = "";

        b += elasticity.ToString();
        b += ",";
        b += drag.ToString();
        b += ",";
        b += timeScale.ToString();
        b += ",";
        b += gravity.ToString();
        b += ",";
        b += tetherLength.ToString();
        b += ",";
        b += handleOffset.ToString();

        return b;

    }

    /*
    * Constrain 
    * 
    * clamps the PoiProfile's to static constraint variables 
    *
    * @returns void
    */
    public void Constrain()
    {
        elasticity = Mathf.Clamp01(elasticity);
        drag = Mathf.Clamp01(drag);
        timeScale = Mathf.Clamp01(timeScale);
        gravity = Mathf.Clamp01(gravity);
        tetherLength = Mathf.Clamp01(tetherLength);
        handleOffset = Mathf.Clamp01(handleOffset);
    }

    /*
    * Apply 
    * 
    * sets all of the PoiProfile properties
    *
    * @returns void
    */
    public void Apply()
    {
        GameProperties.instance.G_ELASTICITY_GS = elasticity;
        GameProperties.instance.G_DRAG_GS = drag;
        GameProperties.instance.G_TIMESCALE_GS = timeScale;
        GameProperties.instance.G_GRAVITY_GS = gravity;
        GameProperties.instance.G_TETHERLENGTH_GS = tetherLength;
        GameProperties.instance.G_HANDLEOFFSET_GS = handleOffset;
    }
}
