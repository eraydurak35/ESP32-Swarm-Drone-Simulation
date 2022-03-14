using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentControl : MonoBehaviour
{

    SwarmManagerScript SwarmManager;
    [SerializeField] GameObject SwarmManagerObj;

    public int AgentID;
    private bool Active = true;
    
    private float pitchGyro;
    
    private float rollGyro;
    
    private float lastPitchSensorValue;
    private float lastRollSensorValue;
    private float lastYawChangeRate;

    public float maxPitchAngle = 25f;
    public float maxRollAngle = 25f;
    public float yawRate = 100f;

    public float pitchP = 30f;
    public float pitchI = 5f;
    public float pitchD = 200f;

    public float rollP = 30f;
    public float rollI = 5f;
    public float rollD = 200f;

    public float yawP = 20f;
    public float yawI = 1f;
    public float yawD = 0f;

    private float tau = 0.02f;

    private float pitchSetpoint = 0f;
    private float rollSetpoint = 0f;
    [HideInInspector]
    public float yawSetRate = 0f;

    private float yawChangeRate;

    private float pitchError = 0f;
    private float rollError = 0f;
    private float yawError = 0f;
    private float pitchPrevError;
    private float rollPrevError;
    private float yawPrevError;
    public float AgentHeading;

    private int maxThrottle = 4000;
    [HideInInspector]
    public float LBThrottle;
    [HideInInspector]
    public float LTThrottle;
    [HideInInspector]
    public float RBThrottle;
    [HideInInspector]
    public float RTThrottle;



    private float pitch_P_out;
    private float pitch_I_out;
    private float pitch_D_out;

    private float roll_P_out;
    private float roll_I_out;
    private float roll_D_out;

    private float yaw_P_out;
    private float yaw_I_out;
    private float yaw_D_out;

    private float pitch_PID_out;
    private float roll_PID_out;
    private float yaw_PID_out;

    private float userThrottle = 300;

    private float maxPID = 1000;
    private float maxI = 100;
    private float maxD = 300;
    private float minThrottle = 100f;
    private float minUserThrottle = 500;
    private float sampleTime = 0.00125f;

    [HideInInspector]
    public float barometerAltitude;
    private float prevBarometerAltitude;

    [HideInInspector]
    public float AltHoldSet;
    public float altHoldP = 280f;
    public float altHoldI = 100f;
    public float altHoldD = 6500;
    private float altHold_P_out;
    private float altHold_I_out;
    private float altHold_D_out;
    private float altHoldPID_out;
    private float altHoldError;
    private float altHoldPrevError;

    private float prevYaw;
    private float positionX;
    private float positionZ;
    private float prevPositionX;
    private float prevPositionZ;
    private float positionXError;
    private float positionZError;
    private float pitchPosHoldHorth;
    private float rollPosHoldHorth;
    [HideInInspector]
    public float PosHoldSetX;
    [HideInInspector]
    public float PosHoldSetZ;

    public float posHoldP = 100f;
    public float posHoldD = 30f;

    private float pitchPosHold_P_out;
    private float rollPosHold_P_out;
    private float pitchPosHold_D_out;
    private float rollPosHold_D_out;
    private float correctedPitchPosHoldAngle;
    private float correctedRollPosHoldAngle;

    public float TargetHeading;
    // Start is called before the first frame update

    private void Awake()
    {
        SwarmManager = SwarmManagerObj.GetComponent<SwarmManagerScript>();
    }
    private void FixedUpdate()
    {
        SensorFusion();
        GetDataFromSwarmManager();
        PID();
        GetComponent<AgentMotors>().MotorsState();

    }

    void Landing()
    {

    }

    void GetDataFromSwarmManager()
    {

        Active = SwarmManager.ActiveAgents[AgentID];

        if (Active)
        {
            TargetHeading = SwarmManager.AgentTargetHeading[AgentID];

            PosHoldSetX = SwarmManager.checkpointX;
            PosHoldSetZ = SwarmManager.checkpointZ;
            AltHoldSet = SwarmManager.checkpointY;

            if (TargetHeading > 180f) TargetHeading -= 360f;

            if (TargetHeading < -170f || TargetHeading > 170f)
            {
                if (TargetHeading < 0f) TargetHeading += 360f;

                if (AgentHeading < 0f) AgentHeading += 360f;
            }

            yawSetRate = (AgentHeading - TargetHeading) / 2f;
            if (yawSetRate > yawRate) yawSetRate = yawRate;
            else if (yawSetRate < -yawRate) yawSetRate = -yawRate;
        }
        
    }

    void PID()
    {
        // Error calculations
        pitchError = pitchSetpoint - pitchGyro;
        rollError = rollSetpoint - rollGyro;
        yawError = yawChangeRate - yawSetRate;

        // P calculations
        pitch_P_out = pitchP * pitchError;
        roll_P_out = rollP * rollError;
        yaw_P_out = yawP * yawError;

        // I calculations
        pitch_I_out = pitch_I_out + 0.5f * pitchI * sampleTime * (pitchError + pitchPrevError);

        if (pitch_I_out > maxI) pitch_I_out = maxI;
        else if (pitch_I_out < -maxI) pitch_I_out = -maxI;

        roll_I_out = roll_I_out + 0.5f * rollI * sampleTime * (rollError + rollPrevError);

        if (roll_I_out > maxI) roll_I_out = maxI;
        else if (roll_I_out < -maxI) roll_I_out = -maxI;

        yaw_I_out = yaw_I_out + 0.5f * yawI * sampleTime * (yawError + yawPrevError);

        if (yaw_I_out > maxI) yaw_I_out = maxI;
        else if (yaw_I_out < -maxI) yaw_I_out = -maxI;

        // D calculations
        pitch_D_out = -(2.0f * pitchD * (pitchGyro - lastPitchSensorValue)
                        + (2.0f * tau - sampleTime) * pitch_D_out)
                        / (2.0f * tau + sampleTime);

        if (pitch_D_out > maxD) pitch_D_out = maxD;
        else if (pitch_D_out < -maxD) pitch_D_out = -maxD;

        roll_D_out = -(2.0f * rollD * (rollGyro - lastRollSensorValue)
                        + (2.0f * tau - sampleTime) * roll_D_out)
                        / (2.0f * tau + sampleTime);

        if (roll_D_out > maxD) roll_D_out = maxD;
        else if (roll_D_out < -maxD) roll_D_out = -maxD;

        yaw_D_out = -(2.0f * yawD * (yawChangeRate - lastYawChangeRate)
                        + (2.0f * tau - sampleTime) * yaw_D_out)
                        / (2.0f * tau + sampleTime);

        if (yaw_D_out > maxD) yaw_D_out = maxD;
        else if (yaw_D_out < -maxD) yaw_D_out = -maxD;

        lastPitchSensorValue = pitchGyro;
        lastRollSensorValue = rollGyro;
        lastYawChangeRate = yawChangeRate;

        pitchPrevError = pitchError;
        rollPrevError = rollError;
        yawPrevError = yawError;


        // Calculating the overall and limiting PID values
        pitch_PID_out = pitch_P_out + pitch_I_out + pitch_D_out;
        if (pitch_PID_out > maxPID) pitch_PID_out = maxPID;
        else if (pitch_PID_out < -maxPID) pitch_PID_out = -maxPID;

        roll_PID_out = roll_P_out + roll_I_out + roll_D_out;
        if (roll_PID_out > maxPID) roll_PID_out = maxPID;
        else if (roll_PID_out < -maxPID) roll_PID_out = -maxPID;

        yaw_PID_out = yaw_P_out + yaw_I_out + yaw_D_out;
        if (yaw_PID_out > maxPID) yaw_PID_out = maxPID;
        else if (yaw_PID_out < -maxPID) yaw_PID_out = -maxPID;


        ////////////////////////////////////////////////////////////////////////////////////////////////  ALTITUDE HOLD  //////////////////////////////////////

        altHoldError = AltHoldSet - barometerAltitude;

        if (altHoldError > 3f) altHoldError = 3f;
        else if (altHoldError < -3f) altHoldError = -3f;

        // Altitude hold P Calculation
        altHold_P_out = altHoldError * altHoldP;

        // Altitude hold I Calculation

        altHold_I_out = altHold_I_out + 0.5f * altHoldI * sampleTime * (altHoldError + altHoldPrevError);

        if (altHold_I_out > 3000f) altHold_I_out = 3000f;
        else if (altHold_I_out < 500f) altHold_I_out = 500f;


        // Altitude hold D Calculation
        altHold_D_out = -(2.0f * altHoldD * (barometerAltitude - prevBarometerAltitude)
                    + (2.0f * tau - sampleTime) * altHold_D_out)
                    / (2.0f * tau + sampleTime);

        if (altHold_D_out > 1500f) altHold_D_out = 1500f;
        else if (altHold_D_out < -1500) altHold_D_out = -1500f;

        // Limiting Altitude Hold PID values
        altHoldPID_out = altHold_P_out + altHold_I_out + altHold_D_out;
        if (altHoldPID_out > 2000f) altHoldPID_out = 2000f;
        else if (altHoldPID_out < 0f) altHoldPID_out = 0f;

        userThrottle = altHoldPID_out + minUserThrottle;
        userThrottle += userThrottle * (Mathf.Sin(Mathf.Deg2Rad * Mathf.Abs(pitchGyro)) + Mathf.Sin(Mathf.Deg2Rad * Mathf.Abs(rollGyro)));

        altHoldPrevError = altHoldError;
        prevBarometerAltitude = barometerAltitude;

        /////////////////////////////////////////////////////////////////////////////////////////////////  POSITION HOLD ////////////////////////////////////////////////


        positionXError = PosHoldSetX - positionX;
        positionZError = PosHoldSetZ - positionZ;

        rollPosHold_P_out = -positionXError * (posHoldP / 100);
        pitchPosHold_P_out = positionZError * (posHoldP / 100);

        pitchPosHold_D_out = -(2.0f * posHoldD * (positionZ - prevPositionZ)
                    + (2.0f * tau - sampleTime) * pitchPosHold_D_out)
                    / (2.0f * tau + sampleTime);

        rollPosHold_D_out = -(2.0f * posHoldD * (positionX - prevPositionX)
                    + (2.0f * tau - sampleTime) * rollPosHold_D_out)
                    / (2.0f * tau + sampleTime);


        pitchPosHoldHorth = pitchPosHold_P_out + pitchPosHold_D_out;
        rollPosHoldHorth = rollPosHold_P_out - rollPosHold_D_out;

        if (pitchPosHoldHorth > maxPitchAngle) pitchPosHoldHorth = maxPitchAngle;
        else if (pitchPosHoldHorth < -maxPitchAngle) pitchPosHoldHorth = -maxPitchAngle;

        if (rollPosHoldHorth > maxRollAngle) rollPosHoldHorth = maxRollAngle;
        else if (rollPosHoldHorth < -maxRollAngle) rollPosHoldHorth = -maxRollAngle;

        correctedPitchPosHoldAngle = pitchPosHoldHorth * Mathf.Cos(Mathf.Deg2Rad * UnityEditor.TransformUtils.GetInspectorRotation(transform).y)
            + rollPosHoldHorth * Mathf.Cos(Mathf.Deg2Rad * (UnityEditor.TransformUtils.GetInspectorRotation(transform).y + 90f));

        correctedRollPosHoldAngle = rollPosHoldHorth * Mathf.Cos(Mathf.Deg2Rad * UnityEditor.TransformUtils.GetInspectorRotation(transform).y)
            + pitchPosHoldHorth * Mathf.Cos(Mathf.Deg2Rad * (UnityEditor.TransformUtils.GetInspectorRotation(transform).y - 90f));

        pitchSetpoint = correctedPitchPosHoldAngle;
        rollSetpoint = correctedRollPosHoldAngle;

        prevPositionX = positionX;
        prevPositionZ = positionZ;




        ////////////////////////////////////////////////////////////////////////////////////////////  LIMITING OF SIGNALS  ///////////////////////////////////////////




        // Calculating and limiting all motor signals

        LBThrottle = userThrottle - pitch_PID_out + roll_PID_out + yaw_PID_out;

        if (LBThrottle > maxThrottle) LBThrottle = maxThrottle;
        else if (LBThrottle < minThrottle) LBThrottle = minThrottle;

        LTThrottle = userThrottle + pitch_PID_out + roll_PID_out - yaw_PID_out;

        if (LTThrottle > maxThrottle) LTThrottle = maxThrottle;
        else if (LTThrottle < minThrottle) LTThrottle = minThrottle;

        RBThrottle = userThrottle - pitch_PID_out - roll_PID_out - yaw_PID_out;

        if (RBThrottle > maxThrottle) RBThrottle = maxThrottle;
        else if (RBThrottle < minThrottle) RBThrottle = minThrottle;

        RTThrottle = userThrottle + pitch_PID_out - roll_PID_out + yaw_PID_out;

        if (RTThrottle > maxThrottle) RTThrottle = maxThrottle;
        else if (RTThrottle < minThrottle) RTThrottle = minThrottle;

    }

    public void SensorFusion()
    {
        pitchGyro = UnityEditor.TransformUtils.GetInspectorRotation(transform).x;
        rollGyro = UnityEditor.TransformUtils.GetInspectorRotation(transform).z;

        AgentHeading = transform.eulerAngles.y;
        
        if (AgentHeading > 180f)
        {
            AgentHeading -= 360f;
        }
        
        yawChangeRate = (prevYaw - UnityEditor.TransformUtils.GetInspectorRotation(transform).y) / sampleTime;
        prevYaw = UnityEditor.TransformUtils.GetInspectorRotation(transform).y;
        barometerAltitude = transform.position.y;
        positionX = transform.position.x;
        positionZ = transform.position.z;
    }
}
