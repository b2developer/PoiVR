using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* class UIGraph 
* child class of UIElement
* 
* a 3D plane that plots a set of points
* 
* @author: Bradley Booth, Academy of Interactive Entertainment, 2018
*/
public class UIGraph : UIElement
{
    public const float FORWARD_EPSILON = 0.1f;

    public LineRenderer line = null;
    public float[] dataPoints;

    public float yMin = -1.0f;
    public float yMax = 1.0f;
    
	void Start ()
    {
        line = GetComponent<LineRenderer>();

	}
	
	void Update ()
    {

        Vector3 up = transform.rotation * Vector3.forward * (transform.localScale.z * 10.0f);
        Vector3 right = transform.rotation * Vector3.right * (transform.localScale.x * 10.0f);
        Vector3 forward = transform.rotation * Vector3.up * FORWARD_EPSILON;

        float xStep = 1 / (float)(dataPoints.GetLength(0) - 1);

        line.positionCount = dataPoints.GetLength(0);

        //draw all of the graph points
        for (int i = 0; i < dataPoints.GetLength(0); i++)
        {
            float step = (float)i * xStep;
            float value = dataPoints[i];

            value -= yMin;
            value /= (yMax - yMin);

            //bottom right corner
            Vector3 root = transform.position - (right + up) * 0.5f;

            //calculate graph position
            Vector3 nextPosition = root + up * value + right * step + forward;

            line.SetPosition(i, nextPosition);
        }
	}

    /*
    * SinWave 
    * 
    * generates data points for a sine wave with specified properties
    * 
    * @param float frequency - the amount of wave peaks in a second
    * @param float amplitude - the height of the wave
    * @param float timeOffset - time offset
    * @int segments - the amount of segments in the wave
    * @returns void
    */
    public void SinWave(float frequency, float amplitude, float timeOffset, int segments)
    {
        dataPoints = new float[segments];

        float timeScale = 1.0f / (segments - 1);

        for (int i = 0; i < segments; i++)
        {
            float t = i * (float)timeScale;

            float v = amplitude * Mathf.Sin(t * frequency + timeOffset);

            dataPoints[i] = v;
        }
    }
}
