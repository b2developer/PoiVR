using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* class AnimationTracker 
* 
* used to track the progress of an animation in real-time
* 
* @author: Bradley Booth, Academy of Interactive Entertainment, 2018
*/
public class AnimationTracker
{
    public bool isPlaying = false;

    public float timer = 4.33f;

    public void Update()
    {
        timer -= Time.unscaledDeltaTime;


        Debug.Log(timer.ToString());
        isPlaying = true;

        if (timer <= 0.0f)
        {
            isPlaying = false;

            timer = 0.0f;

           
        }
    }
}
