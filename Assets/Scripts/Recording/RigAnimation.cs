using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/*
* class RigAnimation 
* 
* collects rig keyframes into chunks that define the positions of a rig over time
* capable of loading and saving the animations into external storage
* 
* @author: Bradley Booth, Academy of Interactive Entertainment, 2018
*/
public class RigAnimation
{
    public const char CHUNK_SEPERATOR = ';';
    public const char VALUE_SEPERATOR = ',';

    /*
    * class Chunk 
    * 
    * contains keyframes at a specific point in time
    * 
    * @author: Bradley Booth, Academy of Interactive Entertainment, 2018
    */
    public class Chunk
    {
        public float deltaTime = 0.0f;
        public RigKeyframe[] keyframes;

        /*
        * public Chunk() 
        * default constructor
        */
        public Chunk()
        {

        }
        
    }

    public string id = "";

    //flag indicating if the animation should be written to external storage or not
    public bool writeFlag = false;

    //list of all freeze-frames of the rig to play through with accompanying time frames
    public Chunk[] chunks = null;

    //playback time of the recording
    public float totalTime = 0.0f;

    //flag indicating if the animation is considered a tutorial
    public bool lockFlag = false;

    /*
    * public RigAnimation() 
    * default constructor
    */
    public RigAnimation()
    {

    }

    /*
    * GenerateRandom
    * 
    * creates a completely random but reasonable animation example
    * 
    * @param int sequenceLength - the amount of keyframes (chunks) in the example
    * @param int rigNodes - the amount of manipulable nodes simulated
    * @returns void
    */
    public void GenerateRandom(int sequenceLength, int rigNodes = 40)
    {
        chunks = new Chunk[sequenceLength];

        //for each timestep in the sequence
        for (int i = 0; i < sequenceLength; i++)
        {
            chunks[i] = new Chunk();
            Chunk chunk = chunks[i];
            chunk.keyframes = new RigKeyframe[rigNodes];

            //for each node in the rig
            for (int j = 0; j < rigNodes; j++)
            {
                chunk.keyframes[j] = new RigKeyframe();
                RigKeyframe rkf = chunk.keyframes[j];

                //random positional values
                rkf.position = Random.insideUnitSphere;
                rkf.rotation = Random.rotationUniform;
                rkf.scale = Random.insideUnitSphere;
                
            }

            //random delta-time
            chunk.deltaTime = Random.Range(0.01f, 0.1f);
        }
    }

    /*
    * Deserialise 
    * 
    * takes a serialised rig animation object and converts it using this
    * 
    * @returns void
    */
    public void Deserialise(string data)
    {
        string[] chunkStrings = data.Split(CHUNK_SEPERATOR);

        //first 'chunk' is the name
        id = chunkStrings[0];

        totalTime = float.Parse(chunkStrings[1]);

        lockFlag = bool.Parse(chunkStrings[2]);

        int chunkLength = chunkStrings.GetLength(0) - 3;
        chunks = new Chunk[chunkLength];

        //create chunks
        for (int i = 0; i < chunkLength; i++)
        {
            chunks[i] = new Chunk();
            Chunk ch = chunks[i];

            string[] keyStrings = chunkStrings[i+3].Split(VALUE_SEPERATOR);
            int keyLength = keyStrings.GetLength(0) - 1;

            //last value is the delta-time
            ch.deltaTime = float.Parse(keyStrings[keyLength]);

            int k10 = keyLength / 10;

            //10 numbers per keyframe
            ch.keyframes = new RigKeyframe[k10];

            //create keyframes
            for (int j = 0; j < k10; j++)
            {
                ch.keyframes[j] = new RigKeyframe();
                RigKeyframe rkf = ch.keyframes[j];

                int j10 = j * 10;

                //deserialise each individual part
                rkf.position = new Vector3(float.Parse(keyStrings[j10]), float.Parse(keyStrings[j10 + 1]), float.Parse(keyStrings[j10 + 2]));
                rkf.rotation = new Quaternion(float.Parse(keyStrings[j10 + 3]), float.Parse(keyStrings[j10 + 4]), float.Parse(keyStrings[j10 + 5]), float.Parse(keyStrings[j10 + 6]));
                rkf.scale = new Vector3(float.Parse(keyStrings[j10 + 7]), float.Parse(keyStrings[j10 + 8]), float.Parse(keyStrings[j10 + 9]));
            }
        }
    }

    /*
    * Serialise 
    * 
    * creates a string representation of the rig animation
    * 
    * @returns string - serialised data
    */
    public string Serialise()
    {
        int chunkLength = chunks.GetLength(0);

        StringBuilder sb = new StringBuilder();

        string b = "";

        sb.Append(id);
        sb.Append(CHUNK_SEPERATOR);
        sb.Append(totalTime);
        sb.Append(CHUNK_SEPERATOR);
        sb.Append(lockFlag);
        sb.Append(CHUNK_SEPERATOR);

        //serialise each chunk individually
        for (int i = 0; i < chunkLength; i++)
        {
            Chunk ch = chunks[i];

            int keyLength = ch.keyframes.GetLength(0);

            //serialise each keyframe individually (each one has 10 numbers, this is how the individual keyframes are recreated)
            for (int j = 0; j < keyLength; j++)
            {
                RigKeyframe rkf = ch.keyframes[j];

                sb.Append(rkf.position.x);
                sb.Append(VALUE_SEPERATOR);
                sb.Append(rkf.position.y);
                sb.Append(VALUE_SEPERATOR);
                sb.Append(rkf.position.z);
                sb.Append(VALUE_SEPERATOR);
                sb.Append(rkf.rotation.x);
                sb.Append(VALUE_SEPERATOR);
                sb.Append(rkf.rotation.y);
                sb.Append(VALUE_SEPERATOR);
                sb.Append(rkf.rotation.z);
                sb.Append(VALUE_SEPERATOR);
                sb.Append(rkf.rotation.w);
                sb.Append(VALUE_SEPERATOR);
                sb.Append(rkf.scale.x);
                sb.Append(VALUE_SEPERATOR);
                sb.Append(rkf.scale.y);
                sb.Append(VALUE_SEPERATOR);
                sb.Append(rkf.scale.z);
                sb.Append(VALUE_SEPERATOR);
            }

            sb.Append(ch.deltaTime);

            if (i < chunkLength - 1)
            {
                sb.Append(CHUNK_SEPERATOR);
            }
        }

        return sb.ToString();
    }
}
