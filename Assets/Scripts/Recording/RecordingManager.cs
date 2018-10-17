using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* class RecordingManager 
* 
* loads recorded files, applies data to mimics and
* initialises UI that can manipulate the data through playback and recording
* 
* @author: Bradley Booth, Academy of Interactive Entertainment, 2018
*/
public class RecordingManager : MonoBehaviour
{
    public static RecordingManager instance = null;

    //all loaded animations
    public List<RigAnimation> animations;

    //pelvis rig root 
    public GameObject rigPelvis;

    //pelvis mimic root
    public GameObject mimicPelvis;

	void Start ()
    {
        instance = this;
        animations = new List<RigAnimation>();

        RigAnimation ra = new RigAnimation();
        ra.GenerateRandom(50);

        string data = ra.Serialise();

        RigAnimation rac = new RigAnimation();
        rac.Deserialise(data);

        string dataClone = rac.Serialise();

        bool test = data == dataClone;

        int a = 0;
	}
	
	void Update ()
    {
		
	}
}
