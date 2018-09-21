using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* class RevolutionGesture 
* child class of BaseGesture
* 
* gesture of a full revolution, contains duration, true normal and spin direction
*  
* @author: Bradley Booth, Academy of Interactive Entertainment, 2018
*/
public class RevolutionGesture : BaseGesture
{
    public enum ESpinDirection
    {
        UNDEFINED,
        WALL,  //x dominant
        FLOOR, //y dominant
        WHEEL, //z dominant
    }

    public enum ESystemType
    {
        UNDEFINED,
        POI,
        SHOULDER,
    }

    public ESpinDirection spinDirection = ESpinDirection.UNDEFINED;
    public ESystemType systemType = ESystemType.UNDEFINED;
    public Vector3 normal = Vector3.zero;
    public int spinPlane = 0; //-1 LEFT, 1 RIGHT
}
