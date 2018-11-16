using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* class SoftFollowNotifier
* 
* follows the player's vision around, notifying them of 
* the combo, it's current status and the tricks being performed
* also applys some visual effects
* 
* @author: Bradley Booth, Academy of Interactive Entertainment, 2018 
*/
public class SoftFollowNotifier : MonoBehaviour
{
    public const int INIT_PIORITY = 0;

    public Transform head = null;
    public Vector3 untransformedOffset = new Vector3(0, -0.2f, 1.0f);

    public TextMesh comboMesh;
    public GameObject comboBar;
    public TextMesh[] trickMeshes;

    //list for tracking trick data
    public List<float> trickTimes;
    public List<string> trickHistory;

    public int maxTrickDisplays;

    //timing for fading out tricks
    public float trickAppearanceTime = 0.0f;
    public float trickFadeTime = 0.0f;

    //timing for bumping (when a combo trick is completed)
    public float bumpValue = 0.0f;
    public float bumpOnTrick = 0.5f;

    //flash variables
    public int flashSwitches = 10;
    public float flashTimer = 0.0f;
    public float maxFlashTime = 0.1f;

    private Vector3 comboBarOriginalScale = Vector3.zero;

    private void AwakePiority()
    {
        trickTimes = new List<float>();
        trickHistory = new List<string>();

        maxTrickDisplays = trickMeshes.GetLength(0);

        comboBarOriginalScale = comboBar.transform.localScale;

        comboMesh.text = "";

        foreach (TextMesh tm in trickMeshes)
        {
            tm.text = "";
        }
    }

	// Use this for initialization
	void Start ()
    {
        

    }
	
	// Update is called once per frame
	void Update ()
    {

        bumpValue = Mathf.Lerp(bumpValue, 0.0f, 0.05f);

        transform.rotation = Quaternion.Lerp(transform.rotation, head.rotation, 0.1f);
        transform.position = head.position + transform.rotation * (untransformedOffset - untransformedOffset * bumpValue);

        int tc = trickMeshes.GetLength(0);

        //update meshes, timers and colours
        for (int i = 0; i < tc; i++)
        {
            if (i >= trickHistory.Count)
            {
                trickMeshes[i].text = "";
                continue;
            }

            trickMeshes[i].text = trickHistory[i];
            trickTimes[i] += Time.unscaledDeltaTime;

            float time = trickTimes[i];

            if (time < trickAppearanceTime)
            {
                Color white = new Color(1, 1, 1, 1.0f);

                trickMeshes[i].color = white;
            }
            else if (time < trickAppearanceTime + trickFadeTime)
            {
                //calculate alpha
                float ratio = Mathf.Clamp01((time - trickAppearanceTime) / trickFadeTime);

                Color white = new Color(1, 1, 1, 1 - ratio);

                trickMeshes[i].color = white;
            }
            else
            {
                trickMeshes[i].text = "";
                trickTimes.RemoveAt(i);
                trickHistory.RemoveAt(i);
            }
        }

        UpdateFlashes();

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
            case Sequence.ETrickType.STALL: OnTrickNotification("Stall"); break;
            case Sequence.ETrickType.EXTENSION: OnTrickNotification("Extension"); break;
            case Sequence.ETrickType.FLOWER: OnTrickNotification("Flower"); break;
            case Sequence.ETrickType.WEAVE3: OnTrickNotification("3-Beat Weave"); break;
            case Sequence.ETrickType.DOUBLE_STALL: OnTrickNotification("Double Stall"); break;
            case Sequence.ETrickType.WEAVE2: if (TutorialManager.instance.inSession) { OnTrickNotification("2-Beat Weave"); } break;
            default: break;
        }
    }

    /*
    * OnTrickNotification 
    * 
    * callback to initialise the display for a recently performed trick
    * 
    * @param string trick - the display name of the trick
    * @returns void
    */
    public void OnTrickNotification(string trick)
    {
        int tc = trickHistory.Count;

        if (tc >= 1)
        {
            //previous trick detection
            if (trick == trickHistory[0])
            {
                SoundLibrary.instance.PlaySound(6, 2);

                StartFlashes(2);
                bumpValue = -bumpOnTrick;
            }
            else
            {
                SoundLibrary.instance.PlaySound(14, 2);
                bumpValue = bumpOnTrick;
            }
        }
        else
        {
            SoundLibrary.instance.PlaySound(14, 2);
        }

        trickHistory.Insert(0, trick);
        trickTimes.Insert(0, 0.0f);

        //remove least recent trick when the list overflows
        if (tc > maxTrickDisplays)
        {
            trickHistory.RemoveAt(tc - 1);
            trickTimes.RemoveAt(tc - 1);
        }
    }

    /*
    * SetCombo 
    * 
    * sets the scale of the combo bar using a scalar from 0-1
    * 
    * @param float value - the value to the set the combo bar to
    * @returns void
    */
    public void SetCombo(float value)
    {
        float scale = value * comboBarOriginalScale.x;

        comboBar.transform.localScale = new Vector3(scale, comboBar.transform.localScale.y, comboBar.transform.localScale.z);
    }

    /*
    * UpdateFlashes 
    * 
    * update sub function for the flashing mechanism
    * 
    * @returns void
    */
    public void UpdateFlashes()
    {
        flashTimer -= Time.unscaledDeltaTime;

        //decrement the timer, toggle the UI's visibility at the end of every timer
        if (flashTimer <= 0.0f)
        {
            flashTimer = 0.0f;

            if (flashSwitches > 0)
            {
                flashSwitches--;
                flashTimer = maxFlashTime;

                //instead of getting the visibility flag of each UI component only get it for one as they are all equal
                bool representitive = comboMesh.gameObject.activeSelf;

                comboMesh.gameObject.SetActive(!representitive);
                comboBar.gameObject.SetActive(!representitive);
            }
        }
    }

    /*
    * StartFlashes 
    * 
    * initialises member variables to start the soft lock's flashing mechanism
    * 
    * @param int amount - the amount of flashes required
    * @retuns void
    */
    public void StartFlashes(int amount)
    {
        flashSwitches = amount * 2;
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
