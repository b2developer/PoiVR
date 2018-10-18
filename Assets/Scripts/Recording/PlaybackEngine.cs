using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* class PlaybackEngine 
* 
* uses a rig animation and a rig and controls the rig over the timeframe
* 
* @author: Bradley Booth, Academy of Interactive Entertainment, 2018
*/
public class PlaybackEngine : MonoBehaviour
{

    //rig to control with the game-object
    public GameObject[] playbackRig = null;

    //animation to play-back from
    public RigAnimation rigAnimation = null;
    public PoiRope rigLeftRope = null;
    public PoiRope rigRightRope = null;

    //time to play-back
    public int playbackFrame = 0;
    public float accumulator = 0.0f;
    

	void Start ()
    {
		
	}

    //current chunk being displayed
    RigAnimation.Chunk ch = null;

    [Range(0.0f, 4.0f)]
    public float playbackSpeed = 0.25f;

    void Update ()
    {
        //don't update playback engine if there is no animation to playback
        if (rigAnimation == null)
        {
            return;
        }

        

        accumulator += Time.unscaledDeltaTime * playbackSpeed;

        //skip frames if required
        while (accumulator >= ch.deltaTime)
        {
            accumulator -= ch.deltaTime;
            playbackFrame++;

            //over playback limit?
            if (playbackFrame >= rigAnimation.chunks.GetLength(0))
            {
                playbackFrame = 0;
                ch = rigAnimation.chunks[playbackFrame];
                //break;
            }
            else
            {
                ch = rigAnimation.chunks[playbackFrame];
            }

            //change rig when deltatime accumulator can no longer skip frames
            if (accumulator < ch.deltaTime)
            {
                SetChunk(ch);
            }
        }
	}

    /*
    * Play 
    * 
    * sets the rig's transform with the current chunk
    * 
    * @returns void
    */
    public void Play(RigAnimation animation)
    {
        rigAnimation = animation;
        playbackFrame = 0;
        accumulator = 0.0f;

        ch = animation.chunks[0];
        SetChunk(ch);
    }

    /*
    * SetChunk 
    * 
    * sets the rig's transform with the current chunk
    * 
    * @returns void
    */
    public void SetChunk(RigAnimation.Chunk ch)
    {
        //set each node
        for (int i = 0; i < playbackRig.GetLength(0); i++)
        {
            RigKeyframe rkf = ch.keyframes[i];

            playbackRig[i].transform.position = rkf.position;
            playbackRig[i].transform.rotation = rkf.rotation;
            playbackRig[i].transform.localScale = rkf.scale;
        }

        //amount of keyframes for each poi
        int poiLength = (ch.keyframes.GetLength(0) - playbackRig.GetLength(0) - 4) / 2;

        rigLeftRope.SetLength(poiLength);
        rigRightRope.SetLength(poiLength);


        //set each poi node
        for (int i = 0; i < poiLength; i++)
        {
            RigKeyframe rkf = ch.keyframes[i + playbackRig.GetLength(0)];
            rigLeftRope.m_spawnedNodes[i].gameObject.transform.position = rkf.position;
            rigLeftRope.m_spawnedNodes[i].gameObject.transform.rotation = rkf.rotation;
            rigLeftRope.m_spawnedNodes[i].gameObject.transform.localScale = rkf.scale;
        }

        RigKeyframe lrkf = ch.keyframes[poiLength + playbackRig.GetLength(0)];
        rigLeftRope.ropeStart.gameObject.transform.position = lrkf.position;
        rigLeftRope.ropeStart.gameObject.transform.rotation = lrkf.rotation;
        rigLeftRope.ropeStart.gameObject.transform.localScale = lrkf.scale;

        RigKeyframe lrkf2 = ch.keyframes[poiLength + playbackRig.GetLength(0) + 1];
        rigLeftRope.poiEnd.gameObject.transform.position = lrkf2.position;
        rigLeftRope.poiEnd.gameObject.transform.rotation = lrkf2.rotation;
        rigLeftRope.poiEnd.gameObject.transform.localScale = lrkf2.scale;

        rigLeftRope.UpdateTether();

        foreach (PoiRope.PoiBezier pb in rigLeftRope.poiBeziers)
        {
            pb.CalculateBezier();
        }

        for (int i = 0; i < poiLength; i++)
        {
            RigKeyframe rkf = ch.keyframes[i + playbackRig.GetLength(0) + poiLength + 2];
            rigRightRope.m_spawnedNodes[i].gameObject.transform.position = rkf.position;
            rigRightRope.m_spawnedNodes[i].gameObject.transform.rotation = rkf.rotation;
            rigRightRope.m_spawnedNodes[i].gameObject.transform.localScale = rkf.scale;
        }

        RigKeyframe rrkf = ch.keyframes[poiLength * 2 + playbackRig.GetLength(0) + 2];
        rigRightRope.ropeStart.gameObject.transform.position = rrkf.position;
        rigRightRope.ropeStart.gameObject.transform.rotation = rrkf.rotation;
        rigRightRope.ropeStart.gameObject.transform.localScale = rrkf.scale;

        RigKeyframe rrkf2 = ch.keyframes[poiLength * 2 + playbackRig.GetLength(0) + 3];
        rigRightRope.poiEnd.gameObject.transform.position = rrkf2.position;
        rigRightRope.poiEnd.gameObject.transform.rotation = rrkf2.rotation;
        rigRightRope.poiEnd.gameObject.transform.localScale = rrkf2.scale;

        rigRightRope.UpdateTether();

        foreach (PoiRope.PoiBezier pb in rigRightRope.poiBeziers)
        {
            pb.CalculateBezier();
        }
    }
}
