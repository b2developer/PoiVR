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

    //left and right poi must be offset by 50% of a spin to form a perfect 3 beat weave, this is the different in beats allowed as a decimal percentage 
    public const float WEAVE3_BEAT_UNSYNC_EPSILON = 0.15f;

    //left and right stalls can be performed within this timeframe of each other to trigger a double stall
    float DOUBLE_STALL_TIME_EPSILON = 0.25f;

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

        FlowerDetection(ref leftGestures, ref leftShoulderGestures);
        FlowerDetection(ref rightGestures, ref rightShoulderGestures);
        Weave3Detection(ref leftGestures, ref rightGestures);
        DoubleStallDetection(ref leftGestures, ref rightGestures);
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
            //not a revolution or rest, this can't be a flower
            else
            {
                return;
            }
        }

        //no shoulder revolution was performed
        if (!shoulderRevFound)
        {
            return;
        }

        BaseGesture.ETrickLevels flagTest = shoulderGesture.examinationStatus & BaseGesture.ETrickLevels.FLOWER;

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
            Sequence.instance.OnTrickPerformed(Sequence.ETrickType.FLOWER);
        }
        else if (dot < 0.0f)
        {
            Debug.Log("FLOWER");
            Sequence.instance.OnTrickPerformed(Sequence.ETrickType.FLOWER);
        }
    }


    /*
    * Compare
    * 
    * compares the values of 2 lists to see if each is equal
    * 
    * @param int[] arr1 - the first list
    * @param int[] arr2 - the second list
    * @returns bool - true if arr1 == arr2 false otherwise
    */
    public bool Compare(int[] arr1, int[] arr2)
    {
        //arrays aren't the same if they aren't the same length
        if (arr1.GetLength(0) != arr2.GetLength(0))
        {
            return false;
        }

        //all elements must be equal
        for (int i = 0; i < arr1.GetLength(0); i++)
        {
            //compare individual elements
            if (arr1[i] != arr2[i])
            {
                return false;
            }
        }

        //no elements weren't the same therefore the lists are equal
        return true;
    }

    /*
    * Weave3Detection
    * 
    * detects if a 3-beat weave is performed using specific rules:
    * 
    * - each poi must perform 3 revolutions
    * - each poi must follow a specific spin pattern relative to the body (spin once on one side, twice on the opposite)
    * - the left and right spin patterns must be within a certain percentage of being half a revolution out of sync
    * - each gesture can not have already tested positive for a 3-beat weave (creates edge detection)
    * 
    * @param List<BaseGesture> leftGestures - list of actions performed by the left poi
    * @param List<BaseGesture> rightGestures - list of actions performed by the right poi
    * @returns void 
    */
    public void Weave3Detection(ref List<BaseGesture> leftGestures, ref List<BaseGesture> rightGestures)
    {
        int leftCount = leftGestures.Count;
        int rightCount = rightGestures.Count;

        //3 beat weave can't have taken place due to the lack of gestures
        if (leftCount < 3 || rightCount < 3)
        {
            return;
        }

        List<RevolutionGesture> leftRevs = new List<RevolutionGesture>();
        List<RevolutionGesture> rightRevs = new List<RevolutionGesture>();

        //count 3 revolutions and track the time (left side)
        int leftRevCount = 0;
        float leftTime = 0.0f;
        float leftDelay = 0.0f;

        for (int i = leftCount -1; i >= 0; i--)
        {
            BaseGesture bg = leftGestures[i];

            RevolutionGesture rvg = bg as RevolutionGesture;
            RestGesture rg = bg as RestGesture;

            leftTime += bg.duration;

            if (rvg != null)
            {
                leftRevCount++;
                leftRevs.Add(rvg);
                BaseGesture.ETrickLevels flagTest = bg.examinationStatus & BaseGesture.ETrickLevels.WEAVE3;

                //revolutions belong to a 3-beat weave pattern already
                if (flagTest > 0)
                {
                    return;
                }
            }
            else if (rg != null && leftRevCount == 0)
            {
                leftDelay = bg.duration;
            }
            //not a rest or revolution, this can't be a 3-beat weave
            else if (rvg == null && rg == null)
            {
                return;
            }

            if (leftRevCount >= 3)
            {
                break;
            }
        }

        //count 3 revolutions and track the time (right side)
        int rightRevCount = 0;
        float rightTime = 0.0f;
        float rightDelay = 0.0f;

        for (int i = rightCount - 1; i >= 0; i--)
        {
            BaseGesture bg = rightGestures[i];

            RevolutionGesture rvg = bg as RevolutionGesture;
            RestGesture rg = bg as RestGesture;

            rightTime += bg.duration;

            if (rvg != null)
            {
                rightRevCount++;
                rightRevs.Add(rvg);
                BaseGesture.ETrickLevels flagTest = bg.examinationStatus & BaseGesture.ETrickLevels.WEAVE3;

                //revolutions belong to a 3-beat weave pattern already
                if (flagTest > 0)
                {
                    return;
                }
            }
            else if (rg != null && rightRevCount == 0)
            {
                rightDelay = bg.duration;
            }

            if (rightRevCount >= 3)
            {
                break;
            }
        }

        //not enough revolutions to count as a 3 beat weave
        if (leftRevCount < 3 || rightRevCount < 3)
        {
            return;
        }

        //get each directional pattern
        int[] leftPattern = new int[3];
        int[] rightPattern = new int[3];

        for (int i = 0; i < 3; i++)
        {
            leftPattern[i] = leftRevs[i].spinPlane;
            rightPattern[i] = rightRevs[i].spinPlane;
        }

        float startUnsync = Mathf.Abs(leftDelay - rightDelay);
        float endUnsync = Mathf.Abs(leftTime - rightTime);

        //divide by 2 for the average and then 3 due to 3 spins
        float spinAverageDuration = (leftTime - leftDelay) + (rightTime - rightDelay);
        spinAverageDuration /= 6.0f;

        //correct permulations for the left pattern array
        int[] lc1 = new int[3] { -1, 1, 1 };
        int[] lc2 = new int[3] { 1, -1, 1 };
        int[] lc3 = new int[3] { 1, 1, -1 };

        //correct permulations for the right pattern array
        int[] rc1 = new int[3] { 1, -1, -1 };
        int[] rc2 = new int[3] { -1, 1, -1 };
        int[] rc3 = new int[3] { -1, -1, 1 };

        bool leftTest = Compare(leftPattern, lc1) || Compare(leftPattern, lc2) || Compare(leftPattern, lc3);
        bool rightTest = Compare(rightPattern, rc1) || Compare(rightPattern, rc2) || Compare(rightPattern, rc3);

        float totalUnsync = startUnsync / spinAverageDuration - 0.5f;

        bool beatTest = Mathf.Abs(totalUnsync) <= WEAVE3_BEAT_UNSYNC_EPSILON;

        //Debug.Log("left: " + leftTest.ToString() + "right: " + rightTest.ToString() + totalUnsync);

        if (leftTest && rightTest && beatTest)
        {
            //all revolutions involved need to be flagged as part of a trick
            foreach (RevolutionGesture rg in leftRevs)
            {
                rg.examinationStatus = rg.examinationStatus | BaseGesture.ETrickLevels.WEAVE3;
            }

            foreach (RevolutionGesture rg in rightRevs)
            {
                rg.examinationStatus = rg.examinationStatus | BaseGesture.ETrickLevels.WEAVE3;
            }

            Debug.Log("THREE BEAT WEAVE");
            Sequence.instance.OnTrickPerformed(Sequence.ETrickType.WEAVE3);
        }

        //Debug.Log("Left Pattern: " + leftPattern[0] + ", " + leftPattern[1] + ", " + leftPattern[2]);
        //Debug.Log("Right Pattern: " + rightPattern[0] + ", " + rightPattern[1] + ", " + rightPattern[2]);

        
    }

    /*
    * DoubleStallDetection
    * 
    * detects if two stalls were done within a certain timeframe
    * 
    * @param List<BaseGesture> leftGestures - list of actions performed by the left poi
    * @param List<BaseGesture> rightGestures - list of actions performed by the right poi
    * @returns void 
    */
    public void DoubleStallDetection(ref List<BaseGesture> leftGestures, ref List<BaseGesture> rightGestures)
    {
        int leftCount = leftGestures.Count;
        int rightCount = rightGestures.Count;

        //double stall can't have taken place due to the lack of gestures
        if (leftCount < 1 || rightCount < 1)
        {
            return;
        }

        float leftDelay = 0.0f;
        StallGesture leftStall = null;

        float rightDelay = 0.0f;
        StallGesture rightStall = null;

        //search for stalls in both gesture lists, accept one rest action of delay
        for (int i = leftCount - 1; i >= 0; i--)
        {
            RestGesture rg = leftGestures[i] as RestGesture;
            StallGesture sg = leftGestures[i] as StallGesture;

            if (rg != null)
            {
                leftDelay += rg.duration;
            }
            else if (sg != null)
            {
                leftStall = sg;
                break;
            }
        }

        for (int i = rightCount - 1; i >= 0; i--)
        {
            RestGesture rg = rightGestures[i] as RestGesture;
            StallGesture sg = rightGestures[i] as StallGesture;

            if (rg != null)
            {
                rightDelay += rg.duration;
            }
            else if (sg != null)
            {
                rightStall = sg;
                break;
            }
        }

        //check that both gestures have stalls present
        if (leftStall != null && rightStall != null)
        {
            BaseGesture.ETrickLevels leftFlagTest = leftStall.examinationStatus & BaseGesture.ETrickLevels.DOUBLE_STALL;
            BaseGesture.ETrickLevels rightFlagTest = rightStall.examinationStatus & BaseGesture.ETrickLevels.DOUBLE_STALL;

            //one or both of the stalls were involved in a double stall in the past
            if (leftFlagTest > 0 || rightFlagTest > 0)
            {
                return;
            }

            float errorTime = Mathf.Abs(leftDelay - rightDelay);

            //stalls must both occur within a certain timeframe
            if (errorTime <= DOUBLE_STALL_TIME_EPSILON)
            {
                leftStall.examinationStatus = leftStall.examinationStatus | BaseGesture.ETrickLevels.DOUBLE_STALL;
                rightStall.examinationStatus = rightStall.examinationStatus | BaseGesture.ETrickLevels.DOUBLE_STALL;
                Debug.Log("DOUBLE STALL");
                Sequence.instance.OnTrickPerformed(Sequence.ETrickType.DOUBLE_STALL);
            }
        }

    }
}
