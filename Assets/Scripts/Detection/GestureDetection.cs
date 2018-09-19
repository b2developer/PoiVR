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
    public const float FLOWER_TIME_EPSILON = 0.25f;

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

    /*
    * FlowerDetection
    * 
    * detects if a flower is performed using specific rules:
    * 
    * - shoulder revolution must be present
    * - it must encompass 3 regular revolutions
    * - the start and end of each of these actions must be within FLOWER_TIME_EPSILON of each other
    * - each regular revolution must have the same plane
    * - each gesture can not have already tested positive for a flower (creates edge detection)
    * 
    * the type of flower is determined from the relative average plane of the regular gestures and the shoulder gesture (either IN-SPIN or ANTI-SPIN)
    * 
    * @param List<BaseGesture> gestures - list of actions performed
    * @param List<BaseGesture> sh_gestures - list of shoulder actions performed
    * @returns void 
    */
    public void FlowerDetection(ref List<BaseGesture> gestures, ref List<BaseGesture> sh_gestures)
    {
        int gestureCount = gestures.Count;
        int sh_gestureCount = sh_gestures.Count;

        if (gestureCount < 3 || sh_gestureCount < 2)
        {
            return;
        }

        //when each distinct action of the flower starts and ends (the shoulder revolution and the 3 regular revolutions)
        float rd = 0.0f;
        float re = 0.0f;
        float sd = 0.0f;
        float se = 0.0f;

        bool shoulderRevFound = false;

        //remember the spin from the shoulder revolution
        RevolutionGesture shoulderGesture = null;

        //search shoulder gestures to see if a revolution has been completed
        for (int i = sh_gestureCount - 1; i >= 0; i--)
        {
            RevolutionGesture rvg = sh_gestures[i] as RevolutionGesture;
            RestGesture rg = sh_gestures[i] as RestGesture;

            //revolution, set the ending time
            if (rvg != null)
            {
                se = sd + rvg.duration;
                shoulderRevFound = true;
                shoulderGesture = rvg;
                break;
            }
            //rest, add the time taken
            else if (rg != null)
            {
                sd += rg.duration;
            }
        }

        //no shoulder revolution was performed
        if (!shoulderRevFound)
        {
            return;
        }

        BaseGesture.ETrickLevels flagTest = shoulderGesture.examinationStatus | BaseGesture.ETrickLevels.FLOWER;

        //tested positive as part of a flower before
        if (flagTest > 0)
        {
            return;
        }

        //Debug.Log("sd = " + sd + ", se: " + se);
        int revAmount = 0;

        List<RevolutionGesture> regularGestures = new List<RevolutionGesture>();

        //search regular gestures to see if at least 3 revolutions were performed
        for (int i = gestureCount - 1; i >= 0; i--)
        {
            RevolutionGesture rvg = gestures[i] as RevolutionGesture;
            RestGesture rg = gestures[i] as RestGesture;

            //revolution
            if (rvg != null)
            {
                regularGestures.Add(rvg);

                re += rvg.duration;
                revAmount++;

                //enough revolutions were done
                if (revAmount >= 3)
                {
                    break;
                }
            }
            //rest, add the time taken
            if (rg != null)
            {
                if (rd <= 0.0f)
                {
                    rd += rg.duration;
                    re += rg.duration;
                }
                else
                {
                    //add to total revolution time
                    re += rg.duration;
                }
            }
        }

        //insufficient amount of revs
        if (revAmount < 3)
        {
            return;
        }

        float startError = Mathf.Abs(rd - sd);
        float endError = Mathf.Abs(re - se);

        //flowers were too out of sync
        if (startError > FLOWER_TIME_EPSILON || endError > FLOWER_TIME_EPSILON)
        {
            return;
        }

        Vector3 regularPlaneSum = Vector3.zero;

        //check that all revolutions face the same way
        foreach (RevolutionGesture rvg in regularGestures)
        {
            float testDot = Vector3.Dot(rvg.normal, regularPlaneSum);

            //all revolutions must be facing the same way, including the shoulder revolution
            if (testDot < 0.0f || rvg.spinDirection != shoulderGesture.spinDirection)
            {
                return;
            }

            regularPlaneSum += rvg.normal;
        }

        float dot = Vector3.Dot(regularPlaneSum, shoulderGesture.normal);

        //set revolution flags
        shoulderGesture.examinationStatus = shoulderGesture.examinationStatus | BaseGesture.ETrickLevels.FLOWER;

        foreach (RevolutionGesture gesture in regularGestures)
        {
            gesture.examinationStatus = shoulderGesture.examinationStatus | BaseGesture.ETrickLevels.FLOWER;
        }

        //determine flower type
        if (dot > 0.0f)
        {
            Debug.Log("ANTI-FLOWER");
        }
        else if (dot < 0.0f)
        {
            Debug.Log("FLOWER");
        }
    }
}
