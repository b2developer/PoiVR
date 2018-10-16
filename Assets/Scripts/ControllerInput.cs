using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* class ControllerInput 
* 
* organises callbacks and provides a central area to access input from the VR systems
* 
* @author: Bradley Booth, Academy of Interactive Entertainment, 2018
*/
public class ControllerInput : MonoBehaviour
{
    public static ControllerInput instance = null;

    public RemoteManager manager;

    public SteamVR_TrackedController leftController;
    public SteamVR_TrackedController rightController;

    public PoiRope leftRope;
    public PoiRope rightRope;

    public float leftVibrationOverrideStrength = 0.0f;
    public float leftVibrationTimer = 0.0f;

    public float rightVibrationOverrideStrength = 0.0f;
    public float rightVibrationTimer = 0.0f;

    public Pointer leftPointer;
    public Pointer rightPointer;

    public UICamera spectatorCamera;

    public int maxNodes = 10;
    public int minNodes = 3;
    public float vibrationPerTensionUnit = 600.0f;
    public float restBias = 2.0f;

    void Start ()
    {
        instance = this;

        Time.timeScale = 1.0f;

        SteamVR_Render.instance.lockPhysicsUpdateRateToRenderFrequency = false;

        leftController.TriggerClicked += LeftTrigger;
        rightController.TriggerClicked += RightTrigger;

        leftController.TriggerUnclicked += LeftTriggerReleased;
        rightController.TriggerUnclicked += RightTriggerReleased;

        leftController.MenuButtonClicked += MenuButtonClicked;
        rightController.MenuButtonClicked += MenuButtonClicked;

        leftController.PadClicked += LeftPadClicked;
        leftController.PadUnclicked += LeftPadReleased;

        rightController.PadClicked += LeftPadClicked;
        rightController.PadUnclicked += LeftPadReleased;

        SteamVR_Events.InputFocus.AddListener(SteamFocus);

        
    }
	
	void Update ()
    {

        Time.timeScale = GameProperties.G_TIMESCALE;
        Physics.gravity = new Vector3(0.0f, -GameProperties.G_GRAVITY, 0.0f);

        //decrement the left timer
        if (leftVibrationTimer > 0.0f)
        {
            leftVibrationTimer -= Time.unscaledDeltaTime;
        }

        //reset state of the left timer
        if (leftVibrationTimer <= 0.0f)
        {
            leftVibrationTimer = 0.0f;
            leftVibrationOverrideStrength = 0.0f;
        }

        float leftTension = leftRope.CalculateTension();

        //left controller haptic feedback
        if (leftTension > restBias || leftVibrationOverrideStrength > 0.0f)
        {
            float clampedTension = Mathf.Clamp(leftTension - restBias, 0.0f, leftRope.stretchScalar) + leftVibrationOverrideStrength;
            SteamVR_Controller.Input((int)leftController.controllerIndex).TriggerHapticPulse((ushort)(clampedTension * vibrationPerTensionUnit));
        }

        //decrement the right timer
        if (rightVibrationTimer > 0.0f)
        {
            rightVibrationTimer -= Time.unscaledDeltaTime;
        }

        //reset state of the right timer
        if (rightVibrationTimer <= 0.0f)
        {
            rightVibrationTimer = 0.0f;
            rightVibrationOverrideStrength = 0.0f;
        }

        float rightTension = rightRope.CalculateTension() + rightVibrationOverrideStrength;

        //right controller haptic feedback
        if (rightTension > restBias || rightVibrationOverrideStrength > 0.0f)
        {
            float clampedTension = Mathf.Clamp(rightTension - restBias, 0.0f, rightRope.stretchScalar) + rightVibrationOverrideStrength;
            SteamVR_Controller.Input((int)rightController.controllerIndex).TriggerHapticPulse((ushort)(clampedTension * vibrationPerTensionUnit));
        }

    }

    /*
    * LeftTrigger 
    * 
    * callback for when the Steam VR's trigger is pressed
    * 
    * @param object sender - the object that triggered the event
    * @param ClickedEventArgs e - information about the event
    * @returns void
    */
    void LeftTrigger(object sender, ClickedEventArgs e)
    {
        if (manager.remoteMode == RemoteManager.RemoteMode.POI)
        {
            if (leftRope.ropeLength > minNodes)
            {
                leftRope.ropeLength--;
            }

            if (rightRope.ropeLength > minNodes)
            {
                rightRope.ropeLength--;
            }
        }
        else if (manager.remoteMode == RemoteManager.RemoteMode.UI_LASER)
        {
            leftPointer.OnClick();
        }
    }

    /*
    * RightTrigger 
    * 
    * callback for when the Steam VR's trigger is pressed
    * 
    * @param object sender - the object that triggered the event
    * @param ClickedEventArgs e - information about the event
    * @returns void
    */
    void RightTrigger(object sender, ClickedEventArgs e)
    {
        if (manager.remoteMode == RemoteManager.RemoteMode.POI)
        {
            if (leftRope.ropeLength < maxNodes)
            {
                leftRope.ropeLength++;
            }

            if (rightRope.ropeLength < maxNodes)
            {
                rightRope.ropeLength++;
            }
        }
        else if (manager.remoteMode == RemoteManager.RemoteMode.UI_LASER)
        {
            rightPointer.OnClick();
        }
    }

    /*
    * LeftTriggerReleased 
    * 
    * callback for when the Steam VR's trigger is released
    * 
    * @param object sender - the object that triggered the event
    * @param ClickedEventArgs e - information about the event
    * @returns void
    */
    void LeftTriggerReleased(object sender, ClickedEventArgs e)
    {
        if (manager.remoteMode == RemoteManager.RemoteMode.UI_LASER)
        {
            leftPointer.OnRelease();
        }
    }

    /*
    * RightTriggerReleased 
    * 
    * callback for when the Steam VR's trigger is released
    * 
    * @param object sender - the object that triggered the event
    * @param ClickedEventArgs e - information about the event
    * @returns void
    */
    void RightTriggerReleased(object sender, ClickedEventArgs e)
    {
        if (manager.remoteMode == RemoteManager.RemoteMode.UI_LASER)
        {
            rightPointer.OnRelease();
        }
    }

    /*
    * LeftPadClicked 
    * 
    * callback for when the Steam VR's pad is pressed
    * 
    * @param object sender - the object that triggered the event
    * @param ClickedEventArgs e - information about the event
    * @returns void
    */
    void LeftPadClicked(object sender, ClickedEventArgs e)
    {
        Vector2 area = new Vector2(e.padX, e.padY);
       
        spectatorCamera.OnPadPress(area);
    }

    /*
    * LeftPadReleased
    * 
    * callback for when the Steam VR's pad is released
    * 
    * @param object sender - the object that triggered the event
    * @param ClickedEventArgs e - information about the event
    * @returns void
    */
    void LeftPadReleased(object sender, ClickedEventArgs e)
    {
        spectatorCamera.OnPadRelease();
    }

    /*
    * MenuButtonClicked 
    * 
    * callback for when the Steam VR's regular menu button is pressed
    * 
    * @param object sender - the object that triggered the event
    * @param ClickedEventArgs e - information about the event
    * @returns void
    */
    void MenuButtonClicked(object sender, ClickedEventArgs e)
    {
        manager.OnGamePaused();
    }

    /*
    * SteamFocus 
    * 
    * callback for when steam overlay takes over / loses focus of the controllers
    * 
    * @param bool focus - flag indicating if the remotes have control of the unity application
    * @returns void
    */
    void SteamFocus(bool focus)
    {
        if (!focus)
        {
            manager.OnGamePaused();
        }
    }

}
