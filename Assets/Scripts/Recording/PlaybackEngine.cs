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
    //origin of the playback rig
    public Transform rootTransform = null;

    //model that the rig is controlling
    public GameObject playbackModel = null;
    public GameObject modelLeftRope = null;
    public GameObject modelRightRope = null;

    //rig to control with the game-object
    public GameObject[] playbackRig = null;

    //animation to play-back from
    public RigAnimation rigAnimation = null;
    public PoiRope rigLeftRope = null;
    public PoiRope rigRightRope = null;

    //time to play-back
    public int playbackFrame = 0;
    public float accumulator = 0.0f;

    public GameObject playbackMenu = null;
    public UISlider[] playbackSliders = null;

    public TextMesh playbackTime = null;
    public TextMesh totalTime = null;

    public bool isLooping = false;

	void Start ()
    {
        playbackModel.SetActive(false);
        modelLeftRope.SetActive(false);
        modelRightRope.SetActive(false);
    }

    //current chunk being displayed
    RigAnimation.Chunk ch = null;

    [Range(0.0f, 4.0f)]
    public float playbackSpeed = 0.25f;

    void Update ()
    {
        //don't update playback engine if there is no animation to playback
        if (rigAnimation != null)
        {
            float normalisedTime = Mathf.Clamp01((float)playbackFrame / rigAnimation.chunks.GetLength(0));

            float trueTime = Mathf.FloorToInt(normalisedTime * rigAnimation.totalTime * 100.0f) / 100.0f;
            float allTime = Mathf.FloorToInt(rigAnimation.totalTime * 100.0f) / 100.0f;

            playbackTime.text = trueTime.ToString();
            totalTime.text = allTime.ToString();
        }

        //set the playback frame to loop if the animation is looping and plays all the way through
        if (rigAnimation == null || playbackFrame >= rigAnimation.chunks.GetLength(0))
        {
            if (isLooping && playbackFrame >= rigAnimation.chunks.GetLength(0))
            {
                playbackFrame = 0;
            }
            else
            {
                return;
            }
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
                if (isLooping)
                {
                    playbackFrame = 0;
                    ch = rigAnimation.chunks[playbackFrame];
                }
                else
                {
                    break;
                }
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

            float playbackPosition = Mathf.Clamp01(playbackFrame / (float)(rigAnimation.chunks.GetLength(0)));

            foreach (UISlider pbs in playbackSliders)
            {
                pbs.OverrideValue(playbackPosition);
            }
        }
	}

    /*
    * Play 
    * 
    * sets the rig's transform with the current chunk
    * 
    * @RigAnimation animation - the animation to play
    * @returns void
    */
    public void Play(RigAnimation animation)
    {
        playbackSpeed = 0.0f;

        rigLeftRope.Initialise();
        rigRightRope.Initialise();

        playbackModel.SetActive(true);
        modelLeftRope.SetActive(true);
        modelRightRope.SetActive(true);

        rigAnimation = animation;
        playbackFrame = 0;
        accumulator = 0.0f;

        foreach (UISlider pbs in playbackSliders)
        {
            pbs.OverrideValue(0.0f);
        }

        ch = animation.chunks[0];
        SetChunk(ch);
    }

    /*
    * Play 
    * 
    * sets the rig's transform with the current chunk
    * 
    * @param string animName - the name of the animation to play
    * @param float speed - the playback speed
    * @returns void
    */
    public void Play(string animName)
    {
        playbackSpeed = 0.0f;

        //null if the name isn't found
        RigAnimation searchResult = null;

        foreach (RigAnimation ra in RecordingManager.instance.animations)
        {
            //positive match
            if (ra.id == animName)
            {
                searchResult = ra;
                break;
            }
        }

        //go back one menu, no animation of that name was found
        if (searchResult == null)
        {
            MenuStack.instance.Pop(1);
            return;
        }

        rigLeftRope.Initialise();
        rigRightRope.Initialise();

        playbackModel.SetActive(true);
        modelLeftRope.SetActive(true);
        modelRightRope.SetActive(true);

        rigAnimation = searchResult;
        playbackFrame = 0;
        accumulator = 0.0f;

        foreach (UISlider pbs in playbackSliders)
        {
            pbs.OverrideValue(0.0f);
        }

        ch = searchResult.chunks[0];
        SetChunk(ch);
    }


    /*
    * Stop
    * 
    * disables the rig
    * 
    * @returns void
    */
    public void Stop()
    {
        playbackModel.SetActive(false);
        modelLeftRope.SetActive(false);
        modelRightRope.SetActive(false);
    }

    /*
    * SetPlaybackSpeed 
    * 
    * UI callback for setting the playback speed of the recording
    * 
    * @param float speed - the new playback speed
    * @returns void
    */
    public void SetPlaybackSpeed(float speed)
    {
        playbackSpeed = speed;
    }

    /*
    * OverrideTime 
    * 
    * sets the playback position of the recording
    * 
    * @param float time - the playback position to move to
    * @returns void
    */
    public void OverrrideTime(float value)
    {
        playbackSpeed = 0.0f;
        playbackFrame = Mathf.RoundToInt(value * (float)(rigAnimation.chunks.GetLength(0) - 1));

        //set new position
        RigAnimation.Chunk ch = rigAnimation.chunks[playbackFrame];
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

            playbackRig[i].transform.position = rootTransform.rotation * rkf.position + rootTransform.position;
            playbackRig[i].transform.rotation = rootTransform.rotation * rkf.rotation;
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
            rigLeftRope.m_spawnedNodes[i].gameObject.transform.position = rootTransform.rotation * rkf.position + rootTransform.position;
            rigLeftRope.m_spawnedNodes[i].gameObject.transform.rotation = rkf.rotation * rootTransform.rotation;
            rigLeftRope.m_spawnedNodes[i].gameObject.transform.localScale = rkf.scale;
        }

        RigKeyframe lrkf = ch.keyframes[poiLength + playbackRig.GetLength(0)];
        rigLeftRope.ropeStart.gameObject.transform.position = rootTransform.rotation * lrkf.position + rootTransform.position;
        rigLeftRope.ropeStart.gameObject.transform.rotation = lrkf.rotation * rootTransform.rotation;
        rigLeftRope.ropeStart.gameObject.transform.localScale = lrkf.scale;

        RigKeyframe lrkf2 = ch.keyframes[poiLength + playbackRig.GetLength(0) + 1];
        rigLeftRope.poiEnd.gameObject.transform.position = rootTransform.rotation * lrkf2.position + rootTransform.position;
        rigLeftRope.poiEnd.gameObject.transform.rotation = lrkf2.rotation * rootTransform.rotation;
        rigLeftRope.poiEnd.gameObject.transform.localScale = lrkf2.scale;

        rigLeftRope.UpdateTether();

        foreach (PoiRope.PoiBezier pb in rigLeftRope.poiBeziers)
        {
            pb.CalculateBezier();
        }

        for (int i = 0; i < poiLength; i++)
        {
            RigKeyframe rkf = ch.keyframes[i + playbackRig.GetLength(0) + poiLength + 2];
            rigRightRope.m_spawnedNodes[i].gameObject.transform.position = rootTransform.rotation * rkf.position + rootTransform.position;
            rigRightRope.m_spawnedNodes[i].gameObject.transform.rotation = rkf.rotation * rootTransform.rotation;
            rigRightRope.m_spawnedNodes[i].gameObject.transform.localScale = rkf.scale;
        }

        RigKeyframe rrkf = ch.keyframes[poiLength * 2 + playbackRig.GetLength(0) + 2];
        rigRightRope.ropeStart.gameObject.transform.position = rootTransform.rotation * rrkf.position + rootTransform.position;
        rigRightRope.ropeStart.gameObject.transform.rotation = rrkf.rotation * rootTransform.rotation;
        rigRightRope.ropeStart.gameObject.transform.localScale = rrkf.scale;

        RigKeyframe rrkf2 = ch.keyframes[poiLength * 2 + playbackRig.GetLength(0) + 3];
        rigRightRope.poiEnd.gameObject.transform.position = rootTransform.rotation * rrkf2.position + rootTransform.position;
        rigRightRope.poiEnd.gameObject.transform.rotation = rrkf2.rotation * rootTransform.rotation;
        rigRightRope.poiEnd.gameObject.transform.localScale = rrkf2.scale;

        rigRightRope.UpdateTether();

        foreach (PoiRope.PoiBezier pb in rigRightRope.poiBeziers)
        {
            pb.CalculateBezier();
        }
    }
}
