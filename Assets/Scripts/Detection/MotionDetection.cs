using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* class MotionDetection 
* 
* analyses the movement of a poi relative 
* to it's anchor for circular movement
* 
* @author: Bradley Booth, Academy of Interactive Entertainment, 2018
*/
public class MotionDetection : MonoBehaviour
{
    //the start and end of the poi
    public Transform anchor;
    public Transform end;

    //how many points are stored in memory from previous frames
    public int bufferSize = 125;
    public Vector3[] localPoints;
    public MotionSnapshot[] motionSnapshots;

    public float[] dataPointsX;
    public float[] dataPointsY;

	void Awake ()
    {
        localPoints = new Vector3[bufferSize];
        motionSnapshots = new MotionSnapshot[bufferSize];
        dataPointsX = new float[bufferSize];
        dataPointsY = new float[bufferSize];

        ZeroMemory();

    }
	
	void Update ()
    {
        //don't do trick recognition when the game is paused
        if (!MenuStack.isGame)
        {
            return;
        }

        Vector3 local = end.position - anchor.position;

        ShuffleForward();

        localPoints[0] = local;

        CalculateArc();
	}

    /*
    * ZeroMemory 
    * 
    * fills arrays with refernces that aren't null
    * 
    * @returns void
    */
    public void ZeroMemory()
    {
        for (int i = 0; i < bufferSize; i++)
        {
            localPoints[i] = Vector3.zero;
            motionSnapshots[i] = new MotionSnapshot();
            dataPointsX[i] = 0.0f;
            dataPointsY[i] = 0.0f;
        }
    }

    /*
    * ShuffleForward 
    * 
    * removes the last element of the array and moves the rest forward
    * 
    * @returns void
    */
    private void ShuffleForward()
    {
        //'remove' last element
        localPoints[localPoints.GetLength(0) - 1] = Vector3.zero;

        //iterate backwards through the 
        for (int i = localPoints.GetLength(0) - 1; i >= 1; i--)
        {
            localPoints[i] = localPoints[i - 1];
        }

        localPoints[0] = Vector3.zero;

        //'remove' last element
        motionSnapshots[motionSnapshots.GetLength(0) - 1] = null;

        //iterate backwards through the 
        for (int i = motionSnapshots.GetLength(0) - 1; i >= 1; i--)
        {
            motionSnapshots[i] = motionSnapshots[i - 1];
        }

        motionSnapshots[0] = null;
    }

    public void CalculateArc()
    {
        Vector3 averageNormal = Vector3.zero;
        Vector3 averagePoint = Vector3.zero;
        int pointLen = localPoints.GetLength(0);

        //duplicate the points array
        Vector3[] points = new Vector3[pointLen];
        System.Array.Copy(localPoints, points, localPoints.Length);

        Vector2[] points2D = new Vector2[pointLen];

        //calculate the average circluar plane of the points
        for (int i = 0; i < pointLen - 1; i++)
        {
            Vector3 current = localPoints[i];
            Vector3 next = localPoints[i + 1];

            Vector3 normal = Vector3.Cross(current, next).normalized;

            averagePoint += current;
            averageNormal += normal;
        }

        //add the last point to the average
        averagePoint += points[pointLen - 1];

        averagePoint /= (float)pointLen;
        averageNormal /= (float)pointLen;

        //inverse the rotation of all of the points
        Quaternion rotation = Quaternion.Inverse(Quaternion.LookRotation(averageNormal));

        Vector3 originalDirection = points[0];

        for (int i = 0; i < pointLen; i++)
        {
            points[i] = rotation * points[i];
            points2D[i] = new Vector2(points[i].x, points[i].y);
        }

        //calculate the plane direction of all transformed points points
        for (int i = 0; i < pointLen - 1; i++)
        {
            Vector3 current = points[i];
            Vector3 next = points[i + 1];

            Vector3 normal = Vector3.Cross(current, next).normalized;

            Debug.DrawLine(points[i + 1], points[i], Color.blue);
            Debug.DrawLine(points[i] + normal * 0.1f, points[i], Color.red);
        }

        for (int i = 0; i < pointLen; i++)
        {
            points2D[i] -= new Vector2(averagePoint.x, averagePoint.y);
        }

        dataPointsX = new float[bufferSize];
        dataPointsY = new float[bufferSize];

        for (int i = 0; i < bufferSize; i++)
        {
            dataPointsX[i] = points2D[i].normalized.x;
            dataPointsY[i] = points2D[i].normalized.y;
        }

        MotionSnapshot ms = new MotionSnapshot();

        ms.spinLocation = anchor.position;
        ms.velocity = (points[1] - points[0]) / Time.deltaTime;
        ms.direction = originalDirection;
        ms.localDirection = points2D[0];
        ms.plane = averageNormal;
        ms.localRadius = points2D[0].magnitude;

        motionSnapshots[0] = ms;
    }

}
