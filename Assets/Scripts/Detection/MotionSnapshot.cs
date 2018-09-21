using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* class MotionSnapshot 
* 
* detailed information about the discrete timestep of a poi's motion
* filled in by the MotionDetection and PrimitiveDetection objects
* 
* @author: Bradley Booth, Academy of Interactive Entertainment, 2018
*/
public class MotionSnapshot
{

    //type for rotation over time
    public enum ESpinDirection
    {
        NONE,
        CLOCKWISE,
        COUNTER_CLOCKWISE,
    }

    public float circularConfidence = 0.0f;
    public Vector3 spinLocation = Vector3.zero;
    public Vector3 velocity = Vector3.zero;
    public Vector3 direction = Vector3.zero;
    public Vector2 localDirection = Vector2.zero;
    public Vector3 plane = Vector3.zero;
    public float localRadius = 0.0f;
    public float angle = 0.0f;
	
}
