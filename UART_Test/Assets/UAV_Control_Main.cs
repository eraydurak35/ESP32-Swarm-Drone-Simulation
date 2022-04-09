using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using UnityEngine.InputSystem;
using System;

public class UAV_Control_Main : MonoBehaviour
{
    UAV_Gamepad gamepad;
    public float pitch = 0f, roll = 0f, yaw = 0f, battery = 0f;
    public GameObject drone;

    Vector2 pitchRollInput, ThrottleYawInput;
    int ArmDisarmInput, CalibrateAccInput, CalibrateGyroInput;
    public SerialController serialController;
    
    private void Awake()
    {
        gamepad = new UAV_Gamepad();
    }
    private void OnEnable()
    {
        
        gamepad.uavControl.Enable();
        gamepad.uavControl.PitchRollControl.performed += ctx => pitchRollInput = ctx.ReadValue<Vector2>() * 100f;
        gamepad.uavControl.PitchRollControl.canceled += ctx => pitchRollInput = Vector2.zero;
        gamepad.uavControl.ThrottleYawControl.performed += ctx => ThrottleYawInput = ctx.ReadValue<Vector2>() * 100f;
        gamepad.uavControl.ThrottleYawControl.canceled += ctx => ThrottleYawInput = Vector2.zero;
        gamepad.uavControl.ArmDisarm.performed += ctx => ArmDisarmFunction();
        gamepad.uavControl.CalibrateGyro.performed += ctx => CalibrateGyrofunction();
        gamepad.uavControl.CalibrateAcc.performed += ctx => CalibrateAccfunction();
        
    }

    private void CalibrateAccfunction()
    {
        CalibrateAccInput = 1;
    }

    private void CalibrateGyrofunction()
    {
        CalibrateGyroInput = 1;
    }

    private void ArmDisarmFunction()
    {
        if (ArmDisarmInput == 1) ArmDisarmInput = 0;
        else ArmDisarmInput = 1;
    }

    void Start()
    {
        System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

        serialController = GameObject.Find("SerialControllerObj").GetComponent<SerialController>();
        /*
        pitchRollInput.x = 1.2f;
        pitchRollInput.y = 2.3f;
        ThrottleYawInput.x = 3.4f;
        ThrottleYawInput.y = 4.5f;
        ArmDisarmInput = 1;
        CalibrateGyroInput = 1;
        CalibrateAccInput = 1;
        */
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Debug.Log(pitchRollInput + "    " + ThrottleYawInput + "    " + ArmDisarmInput + "    " + CalibrateAccInput + "    " + CalibrateGyroInput);

        serialController.SendSerialMessage((-pitchRollInput.y).ToString() + '&' + pitchRollInput.x.ToString() + "&" + ThrottleYawInput.y.ToString() + "&" + ThrottleYawInput.x.ToString()
            + "&" + ArmDisarmInput + "&" + CalibrateGyroInput + "&" + CalibrateAccInput);
    }
    
    void OnMessageArrived(string msg)
    {


        if (msg != null && msg != "" && msg != "__Connected__" && msg != "__Disconnected__")
        {

            string[] words = msg.Split(',');
            //Debug.Log(words[0] + "      " + words[1]+ "      " + words[2]);
            pitch = float.Parse(words[0], System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
            roll = float.Parse(words[1], System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
            yaw = float.Parse(words[2], System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
            battery = float.Parse(words[3], System.Globalization.CultureInfo.InvariantCulture.NumberFormat);

            drone.transform.rotation = Quaternion.Euler(pitch, yaw, roll);
            
        }
    }
    
    void OnConnectionEvent(bool succes)
    {

    }
}
