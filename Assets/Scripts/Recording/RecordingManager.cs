using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

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
    //singleton instance
    public static RecordingManager instance = null;

    public static string folderPath = "";

    //all loaded animations
    public List<RigAnimation> animations;

    public PlaybackEngine playbackEngine = null;
    public RecordingManipulator recordingManipulator = null;

    //pelvis rig root 
    public GameObject rigPelvis;
    public GameObject[] playerRig;
    public PoiRope playerPoiLeft;
    public PoiRope playerPoiRight;

    //pelvis mimic root
    public GameObject mimicPelvis;
    public GameObject[] mimicRig;
    public PoiRope mimicPoiLeft;
    public PoiRope mimicPoiRight;

    public RigAnimation rigAnimation;
    public List<RigAnimation.Chunk> chunks;
    public float recordingLimit = 20.0f;
    public float recordingTime = 0.0f;
    public bool recording = false;

	void Start ()
    {

        GetInternalDataPath();

        instance = this;
        animations = new List<RigAnimation>();

        LoadAll();

        //initialise rig arrays
        ExamineRig(rigPelvis, ref playerRig);
        ExamineRig(mimicPelvis, ref mimicRig);
        mimicPoiLeft.PauseSimulation(true);
        mimicPoiRight.PauseSimulation(true);

        playbackEngine.playbackRig = mimicRig;
        playbackEngine.rigLeftRope = mimicPoiLeft;
        playbackEngine.rigRightRope = mimicPoiRight;

        //debug unit test
        //--------------------
        RigAnimation ra = new RigAnimation();
        ra.GenerateRandom(150);

        string data = ra.Serialise();

        RigAnimation rac = new RigAnimation();
        rac.Deserialise(data);

        string dataClone = rac.Serialise();

        bool test = data == dataClone;
        //--------------------

        rigAnimation = new RigAnimation();
        rigAnimation.writeFlag = true;

        chunks = new List<RigAnimation.Chunk>();
	}

    /*
    * GetInternalDataPath
    * 
    * gets the folder directory of the application
    * 
    * @returns void
    */
    public static void GetInternalDataPath()
    {
        folderPath = Application.dataPath + "/RigAnimations/";
    }

    void Update ()
    {
        //don't record if not in the game-state
        if (!MenuStack.instance.isGame)
        {
            return;
        }

        //only record if allowed
        if (!recording)
        {
            return;
        }

        if (recordingTime < recordingLimit)
        {
            recordingTime += Time.unscaledDeltaTime;

            RigAnimation.Chunk ch = CaptureChunk(ref playerRig, ref playerPoiLeft, ref playerPoiRight);
            chunks.Add(ch);

            if (recordingTime >= recordingLimit)
            {
                FinishRecording();
            }
        }
	}

    /*
    * OnPadPress 
    * 
    * call-back for when either of the two remotes pad's are pressed
    * 
    * @param Vector2 area - the section of the pad that was pressed
    * @returns void
    */
    public void OnPadPress(Vector2 area)
    {
        if (!MenuStack.instance.isGame)
        {
            return;
        }

        float dot = Vector2.Dot(area.normalized, Vector2.left);

        //correct part of the pad was pressed to trigger a callback
        if (dot > 0.5f)
        {
            if (recording)
            {
                FinishRecording();
            }
            else
            {
                //start recording
                recording = true;

                chunks.Clear();
            }
        }
    }

    /*
    * ExamineRig 
    * 
    * examines child object tree to form a list that makes up
    * all of the nodes in the entire rig that can be manipulated
    * 
    * @param GameObject root - the first node, typically acts as a transform
    * @param GameObject[] result - the array to store all children in
    * @returns void
    */
    public void ExamineRig(GameObject root, ref GameObject[] result)
    {
        Transform[] allTransforms = root.GetComponentsInChildren<Transform>();
        result = new GameObject[allTransforms.GetLength(0)];

        for (int i = 0; i < allTransforms.GetLength(0); i++)
        {
            result[i] = allTransforms[i].gameObject;
        }
    }

    /*
    * CaptureChunk
    * 
    * records the position, rotation, scale of all transforms in a rig
    * and the respective time-frame in which it was captured
    * 
    * @param GameObject[] rig - the list of game objects in the rig being captured
    * @param PoiRope leftPoi - the left poi rope being captured
    * @param PoiRope rightPoi - the right poi rope being captured
    * @returns RigAnimation.Chunk - animation data for one timestep
    */
    public RigAnimation.Chunk CaptureChunk(ref GameObject[] rig, ref PoiRope leftPoi, ref PoiRope rightPoi)
    {
        RigAnimation.Chunk ch = new RigAnimation.Chunk();

        int leftLength = leftPoi.m_spawnedNodes.Count;
        int rightLength = rightPoi.m_spawnedNodes.Count;

        ch.keyframes = new RigKeyframe[rig.GetLength(0) + leftLength + rightLength + 4];

        //capture each node
        for (int i = 0; i < rig.GetLength(0); i++)
        {
            RigKeyframe rkf = new RigKeyframe();

            rkf.position = rig[i].transform.position;
            rkf.rotation = rig[i].transform.rotation;
            rkf.scale = rig[i].transform.localScale;

            ch.keyframes[i] = rkf;
        }

        //capture each left poi node
        for (int i = 0; i < leftLength; i++)
        {
            RigKeyframe rkf = new RigKeyframe();

            rkf.position = leftPoi.m_spawnedNodes[i].transform.position;
            rkf.rotation = leftPoi.m_spawnedNodes[i].transform.rotation;
            rkf.scale = leftPoi.m_spawnedNodes[i].transform.localScale;

            ch.keyframes[i + rig.GetLength(0)] = rkf;
        }

        RigKeyframe lrfk = new RigKeyframe();

        lrfk.position = leftPoi.ropeStart.transform.position;
        lrfk.rotation = leftPoi.ropeStart.transform.rotation;
        lrfk.scale = leftPoi.ropeStart.transform.localScale;

        ch.keyframes[leftLength + rig.GetLength(0)] = lrfk;

        RigKeyframe lrfk2 = new RigKeyframe();

        lrfk2.position = leftPoi.poiEnd.transform.position;
        lrfk2.rotation = leftPoi.poiEnd.transform.rotation;
        lrfk2.scale = leftPoi.poiEnd.transform.localScale;

        ch.keyframes[leftLength + rig.GetLength(0) + 1] = lrfk2;

        //capture each right poi node
        for (int i = 0; i < rightLength; i++)
        {
            RigKeyframe rkf = new RigKeyframe();

            rkf.position = rightPoi.m_spawnedNodes[i].transform.position;
            rkf.rotation = rightPoi.m_spawnedNodes[i].transform.rotation;
            rkf.scale = rightPoi.m_spawnedNodes[i].transform.localScale;

            ch.keyframes[i + rig.GetLength(0) + leftLength + 2] = rkf;
        }

        RigKeyframe rrfk = new RigKeyframe();

        rrfk.position = rightPoi.ropeStart.transform.position;
        rrfk.rotation = rightPoi.ropeStart.transform.rotation;
        rrfk.scale = rightPoi.ropeStart.transform.localScale;

        ch.keyframes[leftLength + rightLength + rig.GetLength(0) + 2] = rrfk;

        RigKeyframe rrfk2 = new RigKeyframe();

        rrfk2.position = rightPoi.poiEnd.transform.position;
        rrfk2.rotation = rightPoi.poiEnd.transform.rotation;
        rrfk2.scale = rightPoi.poiEnd.transform.localScale;

        ch.keyframes[leftLength + rightLength + rig.GetLength(0) + 3] = rrfk2;

        ch.deltaTime = Time.unscaledDeltaTime;

        return ch;
    }

    /*
    * FinishRecording
    * 
    * packages the recording with the time and all captured chunks
    * triggers menu callbacks that display the recording
    * 
    * @returns void 
    */
    public void FinishRecording()
    {
        rigAnimation.chunks = chunks.ToArray();
        rigAnimation.id = "New";
        rigAnimation.totalTime = recordingTime;
        rigAnimation.writeFlag = true;
        //playbackEngine.Play(rigAnimation);

        animations.Add(rigAnimation);
        recordingManipulator.SpawnNewButton(rigAnimation);
        recordingManipulator.UpdateButtons();

        rigAnimation = new RigAnimation();

        RemoteManager.instance.ForcePause();

        //set the latest animation as the dynamic menu's target
        recordingManipulator.SetDynamicMenu(animations.Count - 1);
        MenuStack.instance.Add(recordingManipulator.dynamicMenu);

        recordingTime = 0.0f;

        recording = false;
    }

    /*
    * Save 
    * 
    * writes all new recordings to memory, removes deleted ones
    * 
    * @returns void
    */
    public void Save()
    {
        foreach (RigAnimation anim in animations)
        {
            //don't write if not required
            if (!anim.writeFlag)
            {
                continue;
            }

            anim.writeFlag = false;

            string path = folderPath + anim.id + ".txt";

            StreamWriter sw = new StreamWriter(path);

            //big string
            string serial = anim.Serialise();

            sw.Write(serial);
            sw.Close();
        }
    }

    /*
    * Save 
    * 
    * writes an individual new recording to memory
    * 
    * @param RigAnimation animation - the animation to write
    * @param string originalFile - the name could change this ensures the name of the file is changed
    * @returns void
    */
    public void Save(RigAnimation animation, string originalFile)
    {
        //don't write if not required
        if (!animation.writeFlag)
        {
            return;
        }

        rigAnimation.writeFlag = false;

        string originalPath = folderPath + originalFile + ".txt";

        string path = folderPath + animation.id + ".txt";

        //this renames the file, if it exists already
        if (originalPath != path && File.Exists(originalPath))
        {
            File.Move(originalPath, path);
        }

        StreamWriter sw = new StreamWriter(path);

        //big string
        string serial = animation.Serialise();

        sw.Write(serial);
        sw.Close();
    }

    /*
    * Delete 
    * 
    * removes an individual new recording from memory
    * 
    * @param RigAnimation animation - the animation to remove
    * @returns void
    */
    public void Delete(RigAnimation animation)
    {
        //remove the directory (if needed and if it exists)
        string expectedPath = folderPath + animation.id + ".txt";

        if (File.Exists(expectedPath))
        {
            File.Delete(expectedPath);
        }

        Debug.Log("Removed Animation: " + animation.id);

        animations.Remove(animation);
    }

    /*
    * LoadAll 
    * 
    * loads all of the recordings from the recordings folder
    * they can be stored in the build or editor
    * 
    * @returns void
    */
    public void LoadAll()
    {
        string[] allFiles = Directory.GetFiles(folderPath);
      
        int afLen = allFiles.GetLength(0);

        for (int i = 0; i < afLen; i++)
        {
            string path = allFiles[i];

            //file type check
            string extension = path.Substring(path.Length - 4);

            if (extension != ".txt")
            {
                continue;
            }

            StreamReader sr = new StreamReader(path);
            string serial = sr.ReadToEnd();

            sr.Close();

            //recreate serialised animation
            RigAnimation ra = new RigAnimation();
            ra.Deserialise(serial);

            animations.Add(ra);
            recordingManipulator.SpawnNewButton(ra);
            
        }
    }
}
