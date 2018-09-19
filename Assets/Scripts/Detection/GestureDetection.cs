using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* class GestureDetection 
* 
* searches for advanced movements of poi that are combinations of specific primitive movements
* this includes 3 beat weaves, flowers and CAPs
* 
* @author: Bradley Booth, Academy of Interactive Entertainment, 2018
*/
public class GestureDetection : MonoBehaviour
{
    //difference in time allowed between the shoulder revolution and 3/4 regular revolutions in between
    public const float FLOWER_TIME_EPSILON = 1.5f;

    public const float MAX_RECORDED_TIME = 10.0f;

    //primitive detectors
    public PrimitiveDetection leftPrimitive;
    public PrimitiveDetection rightPrimitive;

    //list of completed gestures
    public List<BaseGesture> leftGestures;
    public List<BaseGesture> rightGestures;
    public List<BaseGesture> leftShoulderGestures;
    public List<BaseGesture> rightShoulderGestures;

    public float shoulderRevolutionStart = 0.0f;

    void Start ()
    {
        leftGestures = new List<BaseGesture>();
        rightGestures = new List<BaseGesture>();
        leftShoulderGestures = new List<BaseGesture>();
        rightShoulderGestures = new List<BaseGesture>();

        leftPrimitive.gestureCallbacks = AddLeftGesture;
        rightPrimitive.gestureCallbacks = AddRightGesture;
	}
	
	void Update ()
    {
        ProgressGesture(ref leftGestures);
        ProgressGesture(ref rightGestures);
        ProgressGesture(ref leftShoulderGestures);
        ProgressGesture(ref rightShoulderGestures);

        FlowerDetection(ref rightGestures, ref rightShoulderGestures);
    }

    /*
    * ProgressGesture 
    * 
    * increments the list through time, removing gestures that
    * no longer fit on the limited timeline and incrementing the 
    * mandatory rest gesture
    * 
    * @param List<BaseGesture> gestures - container of gestures to progress
    * @returns void
    */
    public void ProgressGesture(ref List<BaseGesture> gestures)
    {
        //add a rest gesture or increment the most recent last item in the list (a rest gesture)
        if (gestures.Count == 0)
        {
            RestGesture rg = new RestGesture();
            rg.duration = Time.unscaledDeltaTime;

            gestures.Add(rg);
        }
        else
        {
            int recent = gestures.Count - 1;

            RestGesture rg = gestures[recent] as RestGesture;

            if (rg == null)
            {
                //add a new rest gesture to increment
                RestGesture nrg = new RestGesture();
                nrg.duration = Time.deltaTime;

                gestures.Add(nrg);
            }
            else
            {
                //increment the existing rest gesture
                rg.duration += Time.unscaledDeltaTime;
            }

        }

        //test total time
        float totalTime = 0.0f;

        for (int i = gestures.Count - 1; i >= 0; i--)
        {
            totalTime += gestures[i].duration;

            //remove the uniterated part of the list
            if (totalTime > MAX_RECORDED_TIME)
            {
                gestures.RemoveRange(0, i + 1);
                break;
            }
        }
    }

    /*
    * AddLeftGesture 
    * 
    * adds a new gesture to the left list
    * 
    * @param BaseGesture g - the new gesture to add to the list
    * @returns void
    */
    public void AddLeftGesture(BaseGesture g)
    {
        RevolutionGesture rg = g as RevolutionGesture;

        if (rg == null)
        {
            AddGesture(ref leftGestures, g);
        }
        else
        {
            //determine correct list to add the the gesture to
            if (rg.systemType == RevolutionGesture.ESystemType.POI)
            {
                AddGesture(ref leftGestures, g);
            }
            else if (rg.systemType == RevolutionGesture.ESystemType.SHOULDER)
            {
                AddGesture(ref leftShoulderGestures, g);
            }
        }
    }

    /*
    * AddRightGesture 
    * 
    * adds a new gesture to the right list
    * 
    * @param BaseGesture g - the new gesture to add to the list
    * @returns void
    */
    public void AddRightGesture(BaseGesture g)
    {
        RevolutionGesture rg = g as RevolutionGesture;

        if (rg == null)
        {
            AddGesture(ref rightGestures, g);
        }
        else
        {
            //determine correct list to add the the gesture to
            if (rg.systemType == RevolutionGesture.ESystemType.POI)
            {
                AddGesture(ref rightGestures, g);
            }
            else if (rg.systemType == RevolutionGesture.ESystemType.SHOULDER)
            {
                AddGesture(ref rightShoulderGestures, g);
            }
        }
    }

    /*
    * AddGesture 
    * 
    * adds a new gesture to the specified list
    * 
    * @param List<BaseGesture> gestures - container of gestures to add the new gesture to
    * @param BaseGesture g - the new gesture to add to the list
    * @returns void
    */
    public void AddGesture(ref List<BaseGesture> gestures, BaseGesture g)
    {
        if (gestures.Count == 0)
        {
            gestures.Add(g);
        }
        else
        {
            int recent = gestures.Count - 1;

            RestGesture rg = gestures[recent] as RestGesture;

            //check if a rest was the most recent gesture, this forces a time subtraction as the trick was being performed during what was thought to be the rest
            if (rg == null)
            {
                gestures.Add(g);
            }
            else
            {
                rg.duration -= g.duration;

                //no rest occured
                if( rg.duration <= 0.0f)
                {
                    gestures.RemoveAt(recent);
                }

                gestures.Add(g);
            }
        }
    }

    public void FlowerDetection(ref List<BaseGesture> gestures, ref List<BaseGesture> sh_gestures)
    {
        int gestureCount = gestures.Count;
        int sh_gestureCount = sh_gestures.Count;

        if (gestureCount < 3 || sh_gestureCount < 1)
        {
            return;
        }

        RestGesture rg = gestures[0] as RestGesture;
        RestGesture srg = sh_gestures[0] as RestGesture;

        float rd = 0.0f;
        float sd = 0.0f;

        if (rg != null)
        {
            rd = rg.duration;
        }

        if (srg != null)
        {
            sd = srg.duration;
        }

        float syncErrorStart = Mathf.Abs(rd - sd);

        Debug.Log("Regular Rest: " + rd + ", Shoulder Rest: " + sd);
    }
}
