using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* class FixedTransform 
* 
* clamps translations and rotations, even under the effects of transform parenting
* 
* @author: Bradley Booth, Academy of Interactive Entertainment, 2018
*/
public class FixedTransform : MonoBehaviour
{

    private Quaternion originalRotation;
    public bool fixRotation = false;

	// Use this for initialization
	void Start ()
    {
        originalRotation = transform.localRotation;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (fixRotation)
        {
            transform.rotation = originalRotation;
        }
	}
}
