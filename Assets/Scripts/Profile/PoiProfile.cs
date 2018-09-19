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
    public const float MIN_ELASTICITY = 15000.0f;
    public const float MAX_ELASTICITY = 20000.0f;
    public const float MIN_DRAG = 0.5f;
    public const float MAX_DRAG = 1.5f;
    public const float MIN_TIMESCALE = 0.5f;
    public const float MAX_TIMESCALE = 1.0f;

    //profile's properties
    public float elasticity;
    public float drag;
    public float timeScale;

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
    }
}
