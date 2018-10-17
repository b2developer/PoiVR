using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* class RigKeyframe 
* 
* stores the positional information of a 
* specific transform at a single point in time
* 
* @author: Bradley Booth, Academy of Interactive Entertainment, 2018
*/
public class RigKeyframe
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;

    /*
    * public RigKeyframe() 
    * default constructor
    */
    public RigKeyframe()
    {

    }

    /*
    * public RigKeyframe()
    * constructor, passes in parameters
    * 
    * @param Vector3 _position - the position of the keyframe
    * @param Quaternion _rotation - the rotation of the keyframe
    * @param Vector3 _scale - the scale of the keyframe
    */
    public RigKeyframe(Vector3 _posiiton, Quaternion _rotation, Vector3 _scale)
    {
        position = _posiiton;
        rotation = _rotation;
        scale = _scale;
    }
	
}
