using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* class BaseGesture 
* 
* abstract object that represents a motion pattern or lack of it
*  
* @author: Bradley Booth, Academy of Interactive Entertainment, 2018
*/
public class BaseGesture
{
    public float duration = 0.0f;

    public enum ETrickLevels
    {
        NONE =   0x0000,
        FLOWER = 0x0001,
        WEAVE3 = 0x0002,
        CAPS =   0x0004,
    }

    //flag describing which systems tested the base gesture positively
    //allows the gesture to be only test positive once
    public ETrickLevels examinationStatus = ETrickLevels.NONE;
}
