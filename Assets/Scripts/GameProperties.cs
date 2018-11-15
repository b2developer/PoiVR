using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* class GameProperties 
* 
* central object for configurable variables
* 
* @author: Bradley Booth, Academy of Interactive Entertainment, 2018
*/
public class GameProperties : MonoBehaviour
{
    //singleton instance
    public static GameProperties instance;

    //properties
    public static float G_ELASTICITY;
    public static float G_DRAG;
    public static float G_TIMESCALE;
    public static float G_GRAVITY;
    public static float G_TETHERLENGTH;
    public static float G_HANDLEOFFSET;

    public float G_ELASTICITY_GS
    {
        get 
        {
            return G_ELASTICITY;
        }

        set 
        {
            G_ELASTICITY = value * (PoiProfile.MAX_ELASTICITY - PoiProfile.MIN_ELASTICITY) + PoiProfile.MAX_ELASTICITY;
        }
    }

    public float G_DRAG_GS
    {
        get
        {
            return G_DRAG;
        }

        set
        {
            G_DRAG = value * (PoiProfile.MAX_DRAG - PoiProfile.MIN_DRAG) + PoiProfile.MIN_DRAG;
        }
    }

    public float G_TIMESCALE_GS
    {
        get
        {
            return G_TIMESCALE;
        }

        set
        {
            G_TIMESCALE = value * (PoiProfile.MAX_TIMESCALE - PoiProfile.MIN_TIMESCALE) + PoiProfile.MIN_TIMESCALE;
        }
    }

    public float G_GRAVITY_GS
    {
        get
        {
            return G_GRAVITY;
        }

        set
        {
            G_GRAVITY = value * (PoiProfile.MAX_GRAVITY - PoiProfile.MIN_GRAVITY) + PoiProfile.MIN_GRAVITY;
        }
    }

    public float G_TETHERLENGTH_GS
    {
        get
        {
            return G_TETHERLENGTH;
        }

        set
        {
            G_TETHERLENGTH = value * (PoiProfile.MAX_TETHERLENGTH - PoiProfile.MIN_TETHERLENGTH) + PoiProfile.MIN_TETHERLENGTH;
        }
    }

    public float G_HANDLEOFFSET_GS
    {
        get
        {
            return G_HANDLEOFFSET;
        }

        set
        {
            G_HANDLEOFFSET = value * (PoiProfile.MAX_HANDLEOFFSET - PoiProfile.MIN_HANDLEOFFSET) + PoiProfile.MIN_HANDLEOFFSET;
        }
    }


    // Use this for initialization
    void Start()
    {
        instance = this;

        G_ELASTICITY_GS = 0.5f;
        G_DRAG_GS = 0.5f;
        G_TIMESCALE_GS = 0.5f;
        G_GRAVITY_GS = 0.5f;
        G_TETHERLENGTH_GS = 0.5f;
        G_HANDLEOFFSET_GS = 0.5f;
    }
	
	// Update is called once per frame
	void Update ()
    { 
		
	}
}
