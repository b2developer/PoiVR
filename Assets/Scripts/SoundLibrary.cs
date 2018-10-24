using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

/*
* class SoundLibrary 
* 
* singleton component that holds references and useful functions
* for all audio related resources
* 
* @author: Bradley Booth, Academy of Interactive Entertainment, 2018
*/
public class SoundLibrary : MonoBehaviour
{

    public static SoundLibrary instance = null;

    private AudioSource soundPlayer;

    public AudioClip buttonHover;
    public AudioClip buttonPress;

	// Use this for initialization
	void Start ()
    {
        instance = this;
        soundPlayer = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        
	}

    /*
    * PlaySound
    * 
    * initialises the sound player in vr with the new sound
    * then commands the player to emit it
    * 
    * @param AudioClip clip - the sound file to play
    * @returns void
    */
    public void PlaySound(AudioClip clip)
    {
        Debug.Log("PLAY SOUND!");

        soundPlayer.clip = clip;
        soundPlayer.Play();
    }
}
