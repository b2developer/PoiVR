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

    public AudioSource[] sources;

    public AudioClip buttonHover;
    public AudioClip buttonPress;

    public AudioClip[] mappedClips;

	// Use this for initialization
	void Start ()
    {
        instance = this;
        sources = GetComponents<AudioSource>();
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

        sources[0].clip = clip;
        sources[0].Play();
    }

    /*
    * PlaySound
    * 
    * initialises the sound player in vr with the new sound
    * then commands the player to emit it
    * 
    * @param int id - index of the audio array
    * @param int track - the track to play it on
    * @returns void
    */
    public void PlaySound(int id, int track)
    {
        sources[track].clip = mappedClips[id];
        sources[track].Play();
    }

    /*
    * PlaySound
    * 
    * initialises the sound player in vr with the new sound
    * then commands the player to emit it
    * 
    * @param int id - index of the audio array
    * @param int track - the track to play it on
    * @returns void
    */
    public void PlaySound(int id)
    {
        sources[0].clip = mappedClips[id];
        sources[0].Play();
    }
}
