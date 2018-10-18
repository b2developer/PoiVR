using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* class PrimitiveDetection 
* 
* searches for specific primitive movements in poi. 
* This includes, revolutions of the shoulder and poi, extensions and stalls
* 
* @author: Bradley Booth, Academy of Interactive Entertainment, 2018
*/
public class PrimitiveDetection : MonoBehaviour
{
    public delegate void GestureFunc(BaseGesture g);

    //constants
    public const float MIN_CIRCLE_RADIUS = 0.1f;
    public const float CIRCLE_CONFIDENCE = 0.85f;
    public const float SHOULDER_CIRCLE_CONFIDENCE = 0.65f;
    public const float DOWN_DOT = 0.33f;

    public const float LINEAR_STALL_SPEED_EPSILON = 4.5f;
    public const float MIN_STALL_TIME = 0.1f;
    public const float MIN_NEGATIVE_STALL_TIME = 1.5f;

    public const float MIN_EXTENSION_TIME = 0.5f;
    public const float EXTENSION_CONFIDENCE = 0.80f;

    //circular motion analyser of the remote
    public MotionDetection detector;

    //circular motion analyser of the shoulder remote system
    public MotionDetection shoulderDetector;

    //position of the chest, used to determine the plane
    public Transform bodyIK;

    //revolution variables
    public GestureFunc gestureCallbacks;

    public bool revolutionBeingCompleted = false;
    public bool revolutionInitialExit = false;
    public float revolutionTime = 0.0f;
    public Vector3 revolutionDirection = Vector3.zero;
    public Vector3 revolutionStart = Vector3.zero;

    //shoulder revolution variables
    public bool sh_revolutionBeingCompleted = false;
    public bool sh_revolutionInitialExit = false;
    public float sh_revolutionTime = 0.0f;
    public Vector3 sh_revolutionDirection = Vector3.zero;

    //stall variables
    public bool inRevolution = false;
    public bool inMotion = false;
    public bool stallSwitch = false;

    //extension variables
    public bool inExtension = false;
    public float extensionDuration = 0.0f;

    public float timeSinceMotion = 0.0f;
    public float timeInMotion = 0.0f;

    void Start()
    {

    }

    void Update()
    {
        AnalyseGestures(ref detector, ref shoulderDetector);
    }

    /*
    * DeltaNormalise 
    * 
    * gets the change of value between each entry in the list
    * and then normalises it between -1 and 1
    * 
    * @param float[] X - the array of data points
    * @returns float[] - the array of derivatives
    */ 
    public float[] DeltaNormalise(float[] X)
    {
        int size = X.GetLength(0);

        float[] DX = new float[size - 1];

        //used to normalise the values
        float max = -10000.0f;
        float min = 10000.0f;

        for (int i = 1; i < size; i++)
        {
            DX[i - 1] = X[i] - X[i - 1];

            if (min > DX[i - 1])
            {
                min = DX[i - 1];
            }

            if (max < DX[i - 1])
            {
                max = DX[i - 1];
            }

        }

        float span = (max - min);

        //the span must be larger than zero to stop undefined values
        if (span > 0.0f)
        {
            //normalise all values
            for (int i = 0; i < size - 1; i++)
            {
                DX[i] -= min;
                DX[i] /= span;
                DX[i] = 2.0f * DX[i] - 1.0f;
            }

            return DX;
        }
        else
        {
            //no span, all entries are zero
            for (int i =0; i < size - 1; i++)
            {
                DX[i] = 0.0f;
            }

            return DX;
        }
    }

    /*
    * Normalise 
    * 
    * normalises the array
    * 
    * @param float[] X - the array of data points
    * @returns float[] - the normalised array
    */
    public float[] Normalise(float[] X)
    {
        int size = X.GetLength(0);

        //used to normalise the values
        float max = -10000.0f;
        float min = 10000.0f;

        for (int i = 1; i < size; i++)
        {
            if (min > X[i - 1])
            {
                min = X[i - 1];
            }

            if (max < X[i - 1])
            {
                max = X[i - 1];
            }

        }

        float span = (max - min);

        //the span must be larger than zero to stop undefined values
        if (span > 0.0f)
        {
            //normalise all values
            for (int i = 0; i < size - 1; i++)
            {
                X[i] -= min;
                X[i] /= span;
                X[i] = 2.0f * X[i] - 1.0f;
            }

            return X;
        }
        else
        {
            return X;
        }
    }

    /*
    * HarmonicError 
    * 
    * calculates how much a data point and measured delta to the next point
    * represent a harmonic
    * 
    * @param float data - the data point
    * @param float delta - the discrete delta measurement
    * @returns float - the error of the sine wave data points
    */
    public float HarmonicError(float data, float delta)
    {
        float absData = data * 0.5f + 1.0f;
        float absDelta = delta * 0.5f + 1.0f;

        return absDelta - absData;
    }

    /*
    * GaussianMatrix 
    * 
    * gaussian blur matrix applied to a single line data set
    * 
    * @param float variance
    * @returns int size - the amount of entries that can influence their neighbours
    */
    public float[] GaussianMatrix(float variance, int size)
    {
        float[] GM = new float[size];

        float varianceSqr = variance * variance;

        //calculate gaussian values
        for (int i = 0; i < size; i++)
        {
            int dist = i - size / 2;

            float gaussianValue = 1 / (2 * Mathf.PI * varianceSqr) * Mathf.Exp(-((dist * dist) / (2 * varianceSqr)));

            GM[i] = gaussianValue;
        }

        return GM;
    }

    /*
    * SmoothData
    * 
    * applies a gaussian weight matrix algorithm to the points
    * 
    * @param float[] dataSet - the data set to smooth
    * @returns void
    */
    public void SmoothData(ref float[] dataSet, float[] gaussianWeights)
    {
        int size = dataSet.GetLength(0);
        int gHalf = gaussianWeights.GetLength(0) / 2;

        float[] smoothedDataSet = new float[size];
        
        for (int i = 0; i < size; i++)
        {
            float gaussianEffect = 0.0f;
            float sum = 0;

            //calculate gaussian average
            for (int j = -gHalf; j <= gHalf; j++)
            {
                if (i + j < 0 || i + j >= size)
                {
                    continue;
                }

                float contribution = dataSet[i + j] * gaussianWeights[j + gHalf];

                gaussianEffect += gaussianWeights[j + gHalf];
                sum += contribution;
            }

            sum /= gaussianEffect;

            smoothedDataSet[i] = sum;
        }

        dataSet = smoothedDataSet;
    }

    /*
    * AnalyseGestures
    * 
    * reads each revolution output from a circular motion detector and
    * deducts more information from them eg. what plane it uses
    * 
    * @param CircleDetection detector - the circle detection object
    * @returns void
    */
    public void AnalyseGestures(ref MotionDetection detector, ref MotionDetection shoulderDetector)
    {
        //don't do trick recognition when the game is paused
        if (!MenuStack.isGame)
        {
            return;
        }

        //REVOLUTION ALGORITHM ***************************************************
        //------------------------------
        float[] deltaY = DeltaNormalise(detector.dataPointsY);
        float[] deltaX = DeltaNormalise(detector.dataPointsX);
        float[] G = GaussianMatrix(2.0f, 21);

        Vector2 currentMotion = new Vector2(deltaX[0], deltaY[0]);

        if (deltaY != null)
        {
            SmoothData(ref deltaY, G);
            deltaY = Normalise(deltaY);
        }

        if (deltaX != null)
        {
            SmoothData(ref deltaX, G);
            deltaX = Normalise(deltaX);
        }

        float errorX = 0.0f;

        for (int i = 1; i < detector.bufferSize; i++)
        {
            errorX += Mathf.Abs(HarmonicError(detector.dataPointsX[i], deltaY[i - 1]));
        }

        float errorY = 0.0f;

        for (int i = 1; i < detector.bufferSize; i++)
        {
            errorY += Mathf.Abs(HarmonicError(detector.dataPointsY[i], -deltaX[i - 1]));
        }
        //------------------------------

        //SHOULDER REVOLUTION ALGORITHM ***************************************************
        //------------------------------
        float[] sh_deltaY = DeltaNormalise(shoulderDetector.dataPointsY);
        float[] sh_deltaX = DeltaNormalise(shoulderDetector.dataPointsX);

        Vector2 sh_currentMotion = new Vector2(sh_deltaX[0], sh_deltaY[0]);

        if (sh_deltaY != null)
        {
            SmoothData(ref sh_deltaY, G);
            sh_deltaY = Normalise(sh_deltaY);
        }

        if (sh_deltaX != null)
        {
            SmoothData(ref sh_deltaX, G);
            sh_deltaX = Normalise(sh_deltaX);
        }

        float sh_errorX = 0.0f;

        for (int i = 1; i < shoulderDetector.bufferSize; i++)
        {
            sh_errorX += Mathf.Abs(HarmonicError(shoulderDetector.dataPointsX[i], sh_deltaY[i - 1]));
        }

        float sh_errorY = 0.0f;

        for (int i = 1; i < shoulderDetector.bufferSize; i++)
        {
            sh_errorY += Mathf.Abs(HarmonicError(shoulderDetector.dataPointsY[i], -sh_deltaX[i - 1]));
        }
        //------------------------------

        //REVOLUTION DETECTION ALGORITHM ***************************************************
        //------------------------------
        float totalError = ((errorX + errorY) / 2.0f) / detector.bufferSize;

        detector.motionSnapshots[0].circularConfidence = totalError;

        MotionSnapshot currentSnap = detector.motionSnapshots[0];

        float downwardsDot = Vector2.Dot(currentSnap.localDirection.normalized, Vector2.down);

        bool previousInRevolution = inRevolution;

        bool confidenceTest = 1 - currentSnap.circularConfidence < CIRCLE_CONFIDENCE;
        inRevolution = !confidenceTest && currentSnap.localRadius > MIN_CIRCLE_RADIUS;


        //edge detection for motion time
        if (!previousInRevolution && inRevolution)
        {
            timeInMotion = 0.0f;
        }

        //failed revolution (plane flipped or not a circle)
        if (revolutionBeingCompleted)
        {
            bool planeTest = Vector3.Dot(currentSnap.plane, revolutionDirection) <= 0.0f || currentSnap.localRadius < MIN_CIRCLE_RADIUS;

            revolutionTime += Time.unscaledDeltaTime;

            if (planeTest || confidenceTest)
            {
                revolutionBeingCompleted = false;
                revolutionInitialExit = false;
            }

            if (planeTest)
            {
                //Debug.Log("REVOLUTION FAILED: Plane switched.");
            }

            if (confidenceTest)
            {
                //Debug.Log("REVOLUTION FAILED: Confidence must surpass: " + (CIRCLE_CONFIDENCE * 100.0f).ToString() + "%.");
            }
        }

        //poi is pointing down
        if (downwardsDot > 1 - DOWN_DOT)
        {
            if (!revolutionBeingCompleted)
            {
                //revolution is started
                revolutionBeingCompleted = true;
                revolutionDirection = currentSnap.plane;
                revolutionTime = 0.0f;
                revolutionStart = currentSnap.spinLocation;
            }
            else if (revolutionInitialExit)
            {
                //completed revolution
                revolutionInitialExit = false;
                revolutionBeingCompleted = false;

                //determine plane
                Vector3 plane = currentSnap.plane;
                Vector3 arm = (revolutionStart - bodyIK.position).normalized;

                RevolutionGesture rg = new RevolutionGesture();
                rg.duration = revolutionTime;
                rg.normal = plane;
                rg.systemType = RevolutionGesture.ESystemType.POI;

                Quaternion modelRotation = Quaternion.Inverse(Quaternion.LookRotation(bodyIK.up));

                //inverse the rotation of the plane
                plane = modelRotation * plane;
                arm = modelRotation * arm;

                float x = Mathf.Abs(plane.z);
                float y = Mathf.Abs(plane.y);
                float z = Mathf.Abs(plane.x);

                //wall plane
                if (x > y && x > z)
                {
                    Debug.Log("WALL REVOLUTION COMPLETED.");
                    rg.spinDirection = RevolutionGesture.ESpinDirection.WALL;

                    if (arm.z > 0.0f)
                    {
                        rg.spinPlane = 1;
                    }
                    else if (arm.z < 0.0f)
                    {
                        rg.spinPlane = -1;
                    }
                    else
                    {
                        rg.spinPlane = 0;
                    }
                }

                //floor plane
                if (y > x && y > z)
                {
                    Debug.Log("FLOOR REVOLUTION COMPLETED.");
                    rg.spinDirection = RevolutionGesture.ESpinDirection.FLOOR;

                    rg.spinPlane = 0;
                }

                //wheel plane
                if (z > x && z > y)
                {
                    Debug.Log("WHEEL REVOLUTION COMPLETED.");
                    rg.spinDirection = RevolutionGesture.ESpinDirection.WHEEL;

                    if (arm.x > 0.0f)
                    {
                        rg.spinPlane = 1;
                    }
                    else if (arm.x < 0.0f)
                    {
                        rg.spinPlane = -1;
                    }
                    else
                    {
                        rg.spinPlane = 0;
                    }
                }

                Sequence.instance.OnTrickPerformed(Sequence.ETrickType.REVOLUTION);
                gestureCallbacks(rg);
            }
        }
        else if (revolutionBeingCompleted)
        {
            //revolution is entering the other part of the revolution
            revolutionInitialExit = true;
        }
        //------------------------------


        //STALL DETECTION ALGORITHM ***************************************************
        //------------------------------
        MotionSnapshot ms = detector.motionSnapshots[0];

        inMotion = ms.velocity.magnitude > LINEAR_STALL_SPEED_EPSILON;

        if (confidenceTest || inMotion || inExtension)
        {
            timeSinceMotion = 0.0f;
            
            //must be in a revolution before another stall can be triggered
            if (inRevolution && inMotion)
            {
                timeInMotion += Time.unscaledDeltaTime;
                stallSwitch = false;
            }
        }
        else
        { 

            timeSinceMotion += Time.unscaledDeltaTime;
            
            //stall trigger switch
            if (!stallSwitch && timeSinceMotion > MIN_STALL_TIME && timeInMotion > MIN_NEGATIVE_STALL_TIME)
            {
                stallSwitch = true;
                Debug.Log("STALL COMPLETED in direction: " + ms.localDirection.normalized.ToString());
                Sequence.instance.OnTrickPerformed(Sequence.ETrickType.STALL);

                StallGesture sg = new StallGesture();
                sg.duration = 0.0f;
                sg.direction = ms.localDirection.normalized;

                timeInMotion = 0.0f;

                gestureCallbacks(sg);
            }

            
        }
        //------------------------------


        //EXTENSION DETECTION ALGORITHM ***************************************************
        //------------------------------
        float extensionError = 0.0f;

        for (int i = 0; i < detector.bufferSize; i++)
        {
            float shoulderET = (Vector3.Dot(detector.motionSnapshots[i].direction.normalized, -shoulderDetector.motionSnapshots[i].direction.normalized) + 1.0f) / 2.0f;
            extensionError += shoulderET;
        }

        extensionError /= detector.bufferSize;

        float extensionConfidence = 1 - extensionError;

        //increment the extension timer
        if (extensionConfidence >= EXTENSION_CONFIDENCE && inRevolution)
        {
            inExtension = true;
            extensionDuration += Time.unscaledDeltaTime;
        }
        
        //reset the extension timer
        if ((extensionConfidence < EXTENSION_CONFIDENCE || !inRevolution) && inExtension)
        {
            inExtension = false;

            if (extensionDuration > MIN_EXTENSION_TIME)
            {
                Debug.Log("Extension was performed for: " + extensionDuration.ToString() + "s.");
                Sequence.instance.OnTrickPerformed(Sequence.ETrickType.EXTENSION);
            }

            extensionDuration = 0.0f;
        }
        //------------------------------

        //SHOULDER REVOLUTION DETECTION ALGORITHM ***************************************************
        //------------------------------

        float sh_totalError = ((sh_errorX + sh_errorY) / 2.0f) / shoulderDetector.bufferSize;

        shoulderDetector.motionSnapshots[0].circularConfidence = sh_totalError;

        MotionSnapshot sh_currentSnap = shoulderDetector.motionSnapshots[0];

        float sh_downwardsDot = Vector2.Dot(sh_currentSnap.localDirection.normalized, Vector2.down);

        bool sh_confidenceTest = 1 - sh_currentSnap.circularConfidence < SHOULDER_CIRCLE_CONFIDENCE;

        //failed revolution (plane flipped or not a circle)
        if (sh_revolutionBeingCompleted)
        {
            bool sh_planeTest = Vector3.Dot(sh_currentSnap.plane, sh_revolutionDirection) <= 0.0f || sh_currentSnap.localRadius < MIN_CIRCLE_RADIUS;

            sh_revolutionTime += Time.unscaledDeltaTime;

            if (sh_planeTest || sh_confidenceTest)
            {
                sh_revolutionBeingCompleted = false;
                sh_revolutionInitialExit = false;
            }

            if (sh_planeTest)
            {
                //Debug.Log("REVOLUTION FAILED: Plane switched.");
            }

            if (sh_confidenceTest)
            {
                //Debug.Log("REVOLUTION FAILED: Confidence must surpass: " + (CIRCLE_CONFIDENCE * 100.0f).ToString() + "%.");
            }
        }

        //shoulder is pointing down
        if (sh_downwardsDot > 1 - DOWN_DOT)
        {
            if (!sh_revolutionBeingCompleted)
            {
                //revolution is started
                sh_revolutionBeingCompleted = true;
                sh_revolutionDirection = sh_currentSnap.plane;
                sh_revolutionTime = 0.0f;
            }
            else if (sh_revolutionInitialExit)
            {
                //completed revolution
                sh_revolutionInitialExit = false;
                sh_revolutionBeingCompleted = false;

                //determine plane
                Vector3 plane = sh_currentSnap.plane;
                Vector3 arm = sh_revolutionDirection;

                RevolutionGesture rg = new RevolutionGesture();
                rg.duration = sh_revolutionTime;
                rg.normal = plane;
                rg.systemType = RevolutionGesture.ESystemType.SHOULDER;

                Quaternion modelRotation = Quaternion.Inverse(Quaternion.LookRotation(bodyIK.up));

                //inverse the rotation of the plane
                plane = modelRotation * plane;
                arm = modelRotation * arm;

                float x = Mathf.Abs(plane.z);
                float y = Mathf.Abs(plane.y);
                float z = Mathf.Abs(plane.x);

                //wall plane
                if (x > y && x > z)
                {
                    Debug.Log("SHOULDER WALL REVOLUTION COMPLETED.");
                    rg.spinDirection = RevolutionGesture.ESpinDirection.WALL;
                }

                //floor plane
                if (y > x && y > z)
                {
                    Debug.Log("SHOULDER FLOOR REVOLUTION COMPLETED.");
                    rg.spinDirection = RevolutionGesture.ESpinDirection.FLOOR;

                    rg.spinPlane = 0;
                }

                //wheel plane
                if (z > x && z > y)
                {
                    Debug.Log("SHOULDER WHEEL REVOLUTION COMPLETED.");
                    rg.spinDirection = RevolutionGesture.ESpinDirection.WHEEL;
                }

                Sequence.instance.OnTrickPerformed(Sequence.ETrickType.SHOULDER_REVOLUTION);
                gestureCallbacks(rg);
            }
        }
        else if (sh_revolutionBeingCompleted)
        {
            //revolution is entering the other part of the revolution
            sh_revolutionInitialExit = true;
        }
        //------------------------------

        //Debug.Log("Current Motion is " + ((1 - totalError) * 100.0f).ToString() + "% Circular Motion.");
        //Debug.Log("Current Motion is " + ((1 - sh_totalError) * 100.0f).ToString() + "% Shoulder Circular Motion.");
        //Debug.Log("Current Motion is " + ((1 - extensionError) * 100.0f).ToString() + "% Extension Motion.");
    }
}
