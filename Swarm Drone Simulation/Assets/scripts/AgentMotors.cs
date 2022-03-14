using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentMotors : MonoBehaviour
{

    public float motorResponse;

    [HideInInspector]
    public float LBCurrent, LTCurrent, RBCurrent, RTCurrent, sumCurrent = 0f;
    private float LBDesiredThrottle, LTDesiredThrottle, RBDesiredThrottle, RTDesiredThrottle;
    private float LBActualThrottle, LTActualThrottle, RBActualThrottle, RTActualThrottle;
    private float prevLBThrottle, prevLTThrottle, prevRBThrottle, prevRTThrottle;
    [HideInInspector]
    public float avrMotorThrottle;

    [HideInInspector]
    public float LBThrust;
    [HideInInspector]
    public float LTThrust;
    [HideInInspector]
    public float RBThrust;
    [HideInInspector]
    public float RTThrust;

    private float LBVoltage;
    private float LTVoltage;
    private float RBVoltage;
    private float RTVoltage;

    private float motorStepValue;
    public float updateRate = 800;
    // Start is called before the first frame update
    private void Start()
    {
        motorStepValue = 4095 / ((motorResponse * updateRate) / 1000f);
    }

    public void MotorsState()
    {
        LBActualThrottle = GetComponent<AgentControl>().LBThrottle;
        LTActualThrottle = GetComponent<AgentControl>().LTThrottle;
        RBActualThrottle = GetComponent<AgentControl>().RBThrottle;
        RTActualThrottle = GetComponent<AgentControl>().RTThrottle;
        /*
        //if (LBActualThrottle - LBDesiredThrottle <= motorStepValue || LBActualThrottle - LBDesiredThrottle >= -motorStepValue) LBActualThrottle = LBDesiredThrottle;
        if (LBDesiredThrottle > LBActualThrottle) LBActualThrottle += motorStepValue;
        else if (LBDesiredThrottle < LBActualThrottle) LBActualThrottle -= motorStepValue;

        //if (LTActualThrottle - LTDesiredThrottle <= motorStepValue || LTActualThrottle - LTDesiredThrottle >= -motorStepValue) LTActualThrottle = LTDesiredThrottle;
        if (LTDesiredThrottle > LTActualThrottle) LTActualThrottle += motorStepValue;
        else if (LTDesiredThrottle < LTActualThrottle) LTActualThrottle -= motorStepValue;

        //if (RBActualThrottle - RBDesiredThrottle <= motorStepValue || RBActualThrottle - RBDesiredThrottle >= -motorStepValue) RBActualThrottle = RBDesiredThrottle;
        if (RBDesiredThrottle > RBActualThrottle) RBActualThrottle += motorStepValue;
        else if (RBDesiredThrottle < RBActualThrottle) RBActualThrottle -= motorStepValue;

        //if (RTActualThrottle - RTDesiredThrottle <= motorStepValue || RTActualThrottle - RTDesiredThrottle >= -motorStepValue) RTActualThrottle = RTDesiredThrottle;
        if (RTDesiredThrottle > RTActualThrottle) RTActualThrottle += motorStepValue;
        else if (RTDesiredThrottle < RTActualThrottle) RTActualThrottle -= motorStepValue;

        */
        avrMotorThrottle = (LBActualThrottle + LTActualThrottle + RTActualThrottle + RBActualThrottle) / 4;

        // Calculate motor volatages from motor signals
        LBVoltage = LBActualThrottle * (GetComponent<AgentBattery>().voltage / 4095f);
        LTVoltage = LTActualThrottle * (GetComponent<AgentBattery>().voltage / 4095f);
        RBVoltage = RBActualThrottle * (GetComponent<AgentBattery>().voltage / 4095f);
        RTVoltage = RTActualThrottle * (GetComponent<AgentBattery>().voltage / 4095f);

        // Calculate each motors thrust (Newton) from motor voltages 
        LBThrust = (float)((Mathf.Pow(LBVoltage, 2f) * 3.5524f) + (LBThrust * 1.5921f)) * 0.0098f;
        LTThrust = (float)((Mathf.Pow(LTVoltage, 2f) * 3.5524f) + (LTThrust * 1.5921f)) * 0.0098f;
        RBThrust = (float)((Mathf.Pow(RBVoltage, 2f) * 3.5524f) + (RBThrust * 1.5921f)) * 0.0098f;
        RTThrust = (float)((Mathf.Pow(RTVoltage, 2f) * 3.5524f) + (RTThrust * 1.5921f)) * 0.0098f;

        GetComponent<AgentEMAE>().GenerateThrustOnMotors();

        LBCurrent = (Mathf.Pow(LBVoltage, 3) * 0.1142f) - (Mathf.Pow(LBVoltage, 2) * 0.0261f) + (LBVoltage * 0.3555f);
        LTCurrent = (Mathf.Pow(LTVoltage, 3) * 0.1142f) - (Mathf.Pow(LTVoltage, 2) * 0.0261f) + (LTVoltage * 0.3555f);
        RBCurrent = (Mathf.Pow(RBVoltage, 3) * 0.1142f) - (Mathf.Pow(RBVoltage, 2) * 0.0261f) + (RBVoltage * 0.3555f);
        RTCurrent = (Mathf.Pow(RTVoltage, 3) * 0.1142f) - (Mathf.Pow(RTVoltage, 2) * 0.0261f) + (RTVoltage * 0.3555f);

        GetComponent<AgentBattery>().BatteryState();

        sumCurrent = LBCurrent + LTCurrent + RBCurrent + RTCurrent;

    }
}
