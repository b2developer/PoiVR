using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoftFollowNotifier : MonoBehaviour
{

    public Transform head = null;
    public Vector3 untransformedOffset = Vector3.zero;

    public TextMesh mesh;

    public float colourTimer = 0.0f;
    public float flashTime = 2.0f;

    public float timer = 0.0f;
    public float displayTime = 1.0f;

    public string lastTrick = "";

    public float scoreLerp = 0.0f;

	// Use this for initialization
	void Start ()
    {
        untransformedOffset = transform.position - Vector3.up * 2.1f;
	}
	
	// Update is called once per frame
	void Update ()
    {

        Vector3 transformedOffset = head.rotation * untransformedOffset;

        transform.position = Vector3.Lerp(transform.position, head.position + transformedOffset, 0.1f);
        transform.rotation = Quaternion.LookRotation(transformedOffset);


        if (timer > 0.0f)
        {
            timer -= Time.unscaledDeltaTime;

            if (timer <= 0.0f)
            {
                lastTrick = "";
                timer = 0.0f;
            }
        }

        /*
        if (colourTimer > 0.0f)
        {
            colourTimer -= Time.unscaledDeltaTime;

            if (colourTimer <= 0.0f)
            {
                mesh.color = Color.white;
                colourTimer = 0.0f;
            }
        }
        */

        if (MenuStack.instance.isGame)
        {
            scoreLerp = Mathf.Lerp(scoreLerp, Sequence.instance.points, 0.1f);
        }
        else
        {
            scoreLerp = Mathf.Lerp(scoreLerp, Sequence.instance.points, 0.5f);
        }

        string comboString = "";

        if (Sequence.instance.combo.tricks > 2)
        {
            comboString = "(x" + Sequence.instance.combo.tricks.ToString() + ")";
        }
        else
        {
            mesh.color = Color.white;
        }

        string displayString = comboString + "\n" + lastTrick;

        mesh.text = displayString;
	}

    /*
    * OnTrickPerformed 
    * 
    * callback for when a trick is performed, adds points and triggers notifications and combos
    * 
    * @param ETrickType trick - the type of trick that was performed
    * @returns void
    */
    public void OnTrickPerformed(Sequence.ETrickType trick)
    {
        switch (trick)
        {
            case Sequence.ETrickType.STALL: lastTrick ="Stall"; break;
            case Sequence.ETrickType.EXTENSION: lastTrick ="Extension"; break;
            case Sequence.ETrickType.FLOWER: lastTrick = "Flower"; break;
            case Sequence.ETrickType.WEAVE3: lastTrick = "3-Beat Weave"; break;
            case Sequence.ETrickType.DOUBLE_STALL: lastTrick = "Double Stall"; break;
            default: break;
        }

        timer = displayTime;
    }


    /*
    * FormatTimeString 
    * 
    * @param int seconds - the total amont of seconds left in the mode
    * @returns string - the seconds in the format (minutes):(seconds)
    */
    public string FormatTimeString(int seconds)
    {
        float minutes = seconds / 60.0f;

        float wholeMinutes = Mathf.Floor(minutes);
        float secondsDecimal = (minutes - wholeMinutes) * 60.0f;

        int mins = (int)wholeMinutes;
        int secs = Mathf.FloorToInt(secondsDecimal);

        string zeroes = "";

        //add an additional zero for formating
        if (secs.ToString().Length < 2)
        {
            zeroes = "0";
        }

        return mins.ToString() + ":" + zeroes + secs.ToString();
    }
}
