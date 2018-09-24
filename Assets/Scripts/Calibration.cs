using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* class Calibration 
* 
* uses measurements from the left remote and headset
* to determine the most appropriate settings for the IK rig
* 
* @author: Bradley Booth, Academy of Interactive Entertainment, 2018
*/
public class Calibration : MonoBehaviour
{
    //true height of the model
    public float modelDefaultHeight = 1.75f; //1.8288f;
    public float modelDefaultArmLength = 0.958f / 1.33f;

    public Transform headset = null;
    public Transform targetHand = null;

    public Transform ikModel = null;
    public RootMotion.FinalIK.VRIK ikScript = null;

	void Start ()
    {
		
	}
	
	void Update ()
    {
        Debug.DrawLine(headset.position, targetHand.position, Color.green);
	}

    /*
    * OnCalibration
    * 
    * callback that is triggered when the calibration button is pressed
    * 
    * @returns void
    */
    public void OnCalibration()
    {
        float height = headset.position.y;

        //scale to the set the vertical axis of the model on
        float scale = height / modelDefaultHeight;

        //set the height of the model
        ikModel.transform.localScale = new Vector3(ikModel.transform.localScale.x, scale, ikModel.transform.localScale.z);

        //arms vertical position and the headset's x and z position mixed together
        Vector3 neck = new Vector3(headset.position.x, targetHand.position.y, headset.position.z);

        float armLength = (neck - targetHand.position).magnitude;

        //set the IK arm multiplier
        float armMultiplier = armLength / modelDefaultArmLength;

        ikScript.solver.leftArm.armLengthMlp = armMultiplier;
        ikScript.solver.rightArm.armLengthMlp = armMultiplier;
       
        Debug.Log("Height: " + height + ", Arm Length: " + armLength);
    }
}
