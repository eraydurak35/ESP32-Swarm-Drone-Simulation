using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlScript : MonoBehaviour
{

    GamePadControl control;

    [HideInInspector]
    public float pitchGyro;
    [HideInInspector]
    public float rollGyro;
    [HideInInspector]
    public float headingGyro;
    [HideInInspector]
    public float accTotalVector;
    [HideInInspector]
    public int xAccRaw, yAccRaw, zAccRaw;

    private float lastPitchSensorValue;
    private float lastRollSensorValue;
    private float lastYawChangeRate;

    public float maxPitchAngle = 25f;
    public float maxRollAngle = 25f;
    public float yawRate = 100f;

    public float pitchP = 0f;
    public float pitchI = 0f;
    public float pitchD = 0f;

    public float rollP = 0f;
    public float rollI = 0f;
    public float rollD = 0f;

    public float yawP = 0f;
    public float yawI = 0f;
    public float yawD = 0f;

    public float tau;

    private float pitchSetpoint = 0f;
    private float rollSetpoint = 0f;
    private float yawSetpoint = 0f;

    private float yawChangeRate;

    private float pitchError = 0f;
    private float rollError = 0f;
    private float yawError = 0f;
    private float pitchPrevError;
    private float rollPrevError;
    private float yawPrevError;

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

    bool armed = false;
    bool gamePad = false;
    bool fastMode = false;
    Vector2 pitchRollInput;
    Vector2 thrYawInput;

    public bool altitudeHold = false;
    public bool positionHold = false;
    public float altHoldSetPoint;
    public float altHoldP;
    public float altHoldI;
    public float altHoldD;
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
    private float PosHoldSetX;
    private float PosHoldSetZ;

    public float posHoldP = 0f;
    public float posHoldD = 0f;

    private float pitchPosHold_P_out;
    private float rollPosHold_P_out;
    private float pitchPosHold_D_out;
    private float rollPosHold_D_out;
    private float correctedPitchPosHoldAngle;
    private float correctedRollPosHoldAngle;

    private void Awake()
    {
        control = new GamePadControl();
    }
    private void OnEnable()
    {
        control.ActionMap.Enable();
        control.ActionMap.PitchRoll.performed += ctx => pitchRollInput = ctx.ReadValue<Vector2>();
        control.ActionMap.PitchRoll.canceled += ctx => pitchRollInput = Vector2.zero;
        control.ActionMap.ThrYaw.performed += ctx => thrYawInput = ctx.ReadValue<Vector2>();
        control.ActionMap.ThrYaw.canceled += ctx => thrYawInput = Vector2.zero;
        control.ActionMap.FS_Mode.performed += ctx => FS_Mode();
        control.ActionMap.ControlType.performed += ctx => ControlMode();
    }
    private void FS_Mode()
    {
        fastMode = !fastMode;
    }
    private void ControlMode()
    {
        gamePad = !gamePad;
    }

    void FixedUpdate()
    {
        SensorFusion();
        GetComponent<UI>().updateUI();
        Inputs();
        PID();
        GetComponent<Motors>().MotorsState();
        GetComponent<EnvironmentalMeasurementsAndEffects>().WindDisturbance();
    }

    void Inputs()
    {
        

        if (gamePad)
        {
            if (armed)
            {
                if (thrYawInput.y != 0)
                {
                    userThrottle = (Mathf.Abs(thrYawInput.y + 1) * 1200) + minUserThrottle;
                    altitudeHold = false;
                    altHold_I_out = 500f;
                    altHoldSetPoint = barometerAltitude;
                }
                else
                {
                    altitudeHold = true;
                } 
                if (pitchRollInput.x !=0 || pitchRollInput.y != 0)
                {
                    positionHold = false;
                }
                else
                {
                    positionHold = true;
                }

                
            } 
            if (thrYawInput.y < -0.8) armed = true;

            if (fastMode)
            {
                pitchSetpoint = pitchRollInput.y * -maxPitchAngle * 1.5f;
                rollSetpoint = pitchRollInput.x * maxRollAngle * 1.5f;
            }
            else
            {

                if (pitchSetpoint < pitchRollInput.y * -maxPitchAngle) pitchSetpoint += maxPitchAngle / 200f;
                else if (pitchSetpoint > pitchRollInput.y * -maxPitchAngle) pitchSetpoint -= maxPitchAngle / 200f;

                if (rollSetpoint < pitchRollInput.x * maxRollAngle) rollSetpoint += maxRollAngle / 200f;
                else if (rollSetpoint > pitchRollInput.x * maxRollAngle) rollSetpoint -= maxRollAngle / 200f;

                //pitchSetpoint = pitchRollInput.y * -maxPitchAngle;
                //rollSetpoint = pitchRollInput.x * maxRollAngle;
            } 

            yawSetpoint = thrYawInput.x * -yawRate;
        }
        else
        {
            if (Input.GetKey(KeyCode.C) && userThrottle < 3000) userThrottle += 0.3f;
            if (Input.GetKey(KeyCode.X) && userThrottle > minUserThrottle) userThrottle -= 0.3f;

            if (Input.GetKey(KeyCode.LeftShift))
            {
                pitchSetpoint = Input.GetAxis("Pitch") * -maxPitchAngle * 1.5f;
            }
            else pitchSetpoint = Input.GetAxis("Pitch") * -maxPitchAngle;

            if (Input.GetKey(KeyCode.LeftShift))
            {
                rollSetpoint = Input.GetAxis("Roll") * maxRollAngle * 1.5f;
            }
            else rollSetpoint = Input.GetAxis("Roll") * maxRollAngle;

            yawSetpoint = Input.GetAxis("Yaw") * -yawRate;
        }
        

        
    }

    void PID()
    {
        // Error calculations
        pitchError = pitchSetpoint - pitchGyro;
        rollError = rollSetpoint - rollGyro;
        yawError = yawChangeRate - yawSetpoint;

        // P calculations
        pitch_P_out = pitchP * pitchError;
        roll_P_out = rollP * rollError;
        yaw_P_out = yawP * yawError;

        // I calculations
        if (pitch_I_out + 0.5f * pitchI * sampleTime * (pitchError + pitchPrevError) > maxI) pitch_I_out = maxI;
        else if (pitch_I_out + 0.5f * pitchI * sampleTime * (pitchError + pitchPrevError) < -maxI) pitch_I_out = -maxI;
        else pitch_I_out = pitch_I_out + 0.5f * pitchI * sampleTime * (pitchError + pitchPrevError);

        if (roll_I_out + 0.5f * rollI * sampleTime * (rollError + rollPrevError) > maxI) roll_I_out = maxI;
        else if (roll_I_out + 0.5f * rollI * sampleTime * (rollError + rollPrevError) < -maxI) roll_I_out = -maxI;
        else roll_I_out = roll_I_out + 0.5f * rollI * sampleTime * (rollError + rollPrevError);

        if (yaw_I_out + 0.5f * yawI * sampleTime * (yawError + yawPrevError) > maxI) yaw_I_out = maxI;
        else if (yaw_I_out + 0.5f * yawI * sampleTime * (yawError + yawPrevError) < -maxI) yaw_I_out = -maxI;
        else yaw_I_out = yaw_I_out + 0.5f * yawI * sampleTime * (yawError + yawPrevError);

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
        if (pitch_P_out + pitch_I_out + pitch_D_out > maxPID) pitch_PID_out = maxPID;
        else if (pitch_P_out + pitch_I_out + pitch_D_out < -maxPID) pitch_PID_out = -maxPID;
        else pitch_PID_out = pitch_P_out + pitch_I_out + pitch_D_out;

        if (roll_P_out + roll_I_out + roll_D_out > maxPID) roll_PID_out = maxPID;
        else if (roll_P_out + roll_I_out + roll_D_out < -maxPID) roll_PID_out = -maxPID;
        else roll_PID_out = roll_P_out + roll_I_out + roll_D_out;

        if (yaw_P_out + yaw_I_out + yaw_D_out > maxPID) yaw_PID_out = maxPID;
        else if (yaw_P_out + yaw_I_out + yaw_D_out < -maxPID) yaw_PID_out = -maxPID;
        else yaw_PID_out = yaw_P_out + yaw_I_out + yaw_D_out;



        if (altitudeHold)
        {
            altHoldSetPoint = 7f;
            // Altitude hold PID Controller
            altHoldError = altHoldSetPoint - barometerAltitude;

            // Altitude hold P Calculation
            altHold_P_out = altHoldError * altHoldP;

            // Altitude hold I Calculation
            if (altHold_I_out + 0.5f * altHoldI * sampleTime * (altHoldError + altHoldPrevError) > 3000f) altHold_I_out = 3000f;
            else if (altHold_I_out + 0.5f * altHoldI * sampleTime * (altHoldError + altHoldPrevError) < 500f) altHold_I_out = 500f;
            else altHold_I_out = altHold_I_out + 0.5f * altHoldI * sampleTime * (altHoldError + altHoldPrevError);

            // Altitude hold D Calculation
            altHold_D_out = -(2.0f * altHoldD * (barometerAltitude - prevBarometerAltitude)
                        + (2.0f * tau - sampleTime) * altHold_D_out)
                        / (2.0f * tau + sampleTime);

            if (altHold_D_out > 1500f) altHold_D_out = 1500f;
            else if (altHold_D_out < -1500) altHold_D_out = -1500f;

            // Limiting Altitude Hold PID values
            if (altHold_P_out + altHold_I_out + altHold_D_out > 3000f) altHoldPID_out = 3000f;
            else if (altHold_P_out + altHold_I_out + altHold_D_out < 0) altHoldPID_out = 0;
            else altHoldPID_out = altHold_P_out + altHold_I_out + altHold_D_out;

            userThrottle = altHoldPID_out + minUserThrottle;
            userThrottle += userThrottle * (Mathf.Sin(Mathf.Deg2Rad * Mathf.Abs(pitchGyro)) + Mathf.Sin(Mathf.Deg2Rad * Mathf.Abs(rollGyro)));

            altHoldPrevError = altHoldError;
            prevBarometerAltitude = barometerAltitude;

            //Debug.Log(altHold_P_out + " , " + altHold_I_out + " , " + altHold_D_out +" , " + userThrottle);

        }

        if (positionHold)
        {
            PosHoldSetZ = 248f;
            PosHoldSetX = 248f;

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

            //Debug.Log(rollPosHold_P_out + "    " + rollPosHold_D_out);

            pitchPosHoldHorth = pitchPosHold_P_out + pitchPosHold_D_out;
            rollPosHoldHorth = rollPosHold_P_out + rollPosHold_D_out;

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
        }

        // Calculating and limiting all motor signals
        if (userThrottle - pitch_PID_out + roll_PID_out + yaw_PID_out > maxThrottle) LBThrottle = maxThrottle;
        else if (userThrottle - pitch_PID_out + roll_PID_out + yaw_PID_out < minThrottle) LBThrottle = minThrottle;
        else LBThrottle = userThrottle - pitch_PID_out + roll_PID_out + yaw_PID_out;

        if (userThrottle + pitch_PID_out + roll_PID_out - yaw_PID_out > maxThrottle) LTThrottle = maxThrottle;
        else if (userThrottle + pitch_PID_out + roll_PID_out - yaw_PID_out < minThrottle) LTThrottle = minThrottle;
        else LTThrottle = userThrottle + pitch_PID_out + roll_PID_out - yaw_PID_out;

        if (userThrottle - pitch_PID_out - roll_PID_out - yaw_PID_out > maxThrottle) RBThrottle = maxThrottle;
        else if (userThrottle - pitch_PID_out - roll_PID_out - yaw_PID_out < minThrottle) RBThrottle = minThrottle;
        else RBThrottle = userThrottle - pitch_PID_out - roll_PID_out - yaw_PID_out;

        if (userThrottle + pitch_PID_out - roll_PID_out + yaw_PID_out > maxThrottle) RTThrottle = maxThrottle;
        else if (userThrottle + pitch_PID_out - roll_PID_out + yaw_PID_out < minThrottle) RTThrottle = minThrottle;
        else RTThrottle = userThrottle + pitch_PID_out - roll_PID_out + yaw_PID_out;
    }
    
    public void SensorFusion()
    {
        pitchGyro = UnityEditor.TransformUtils.GetInspectorRotation(transform).x;
        rollGyro = UnityEditor.TransformUtils.GetInspectorRotation(transform).z;
        yawChangeRate = (prevYaw - UnityEditor.TransformUtils.GetInspectorRotation(transform).y) / sampleTime;
        prevYaw = UnityEditor.TransformUtils.GetInspectorRotation(transform).y;
        barometerAltitude = transform.position.y;
        positionX = transform.position.x;
        positionZ = transform.position.z;


        //Debug.Log(yawChangeRate + "  " + yawSetpoint);


        /*

        GetComponent<LSM6DSL_Gyro>().DPS500();
        pitchGyro += GetComponent<LSM6DSL_Gyro>().xAxisOutput * 0.000021875f;
        rollGyro += GetComponent<LSM6DSL_Gyro>().zAxisOutput * 0.000021875f;
        yawChangeRate = GetComponent<LSM6DSL_Gyro>().yAxisOutput * -0.0175f;
        headingGyro += GetComponent<LSM6DSL_Gyro>().yAxisOutput * 0.000021875f;

        if (headingGyro > 360) headingGyro = 0;
        else if (headingGyro < 0) headingGyro = 360;

        //Debug.Log(headingGyro);

        /*
        GetComponent<LSM6DSL_Gyro>().DPS250();
        pitchGyro += GetComponent<LSM6DSL_Gyro>().xAxisOutput * 0.0000109375f;
        rollGyro += GetComponent<LSM6DSL_Gyro>().zAxisOutput * 0.0000109375f;
        yawChangeRate = GetComponent<LSM6DSL_Gyro>().yAxisOutput * -0.00875f;
        */
        /*
        GetComponent<LSM6DSL_Accelerometer>().G2(GetComponent<Motors>().avrMotorThrottle);
        xAccRaw = GetComponent<LSM6DSL_Accelerometer>().xAccOut;
        yAccRaw = GetComponent<LSM6DSL_Accelerometer>().yAccOut;
        zAccRaw = GetComponent<LSM6DSL_Accelerometer>().zAccOut;

        barometerAltitude = GetComponent<BMP280>().GetAltitude();

        accTotalVector = Mathf.Sqrt(Mathf.Pow(xAccRaw, 2) + Mathf.Pow(yAccRaw, 2) + Mathf.Pow(zAccRaw, 2));
        pitchAcc = Mathf.Asin(zAccRaw / accTotalVector) * Mathf.Rad2Deg;
        rollAcc = Mathf.Asin(xAccRaw / accTotalVector) * -Mathf.Rad2Deg;

        pitchGyro = (pitchGyro * 1f) + (pitchAcc * 0f);
        rollGyro = (rollGyro * 1f) + (rollAcc * 0f);
        */
        //Debug.Log(pitchAcc + " , " + rollAcc + "   ,   " + pitchGyro + " , " + rollGyro);
        //Debug.Log(finalPitch + " , " + finalRoll);

        //yAccRaw += 16393;
        //Debug.Log(xAccRaw + " , " + yAccRaw + " , " + zAccRaw);


        //Debug.Log(yawChangeRate);
        // Altitude
        /*
        GetComponent<EnvironmentalMeasurementsAndEffects>().VisualizeAcceleration();
        GetComponent<L80REM37>().getLocation();

        */
    }
}
