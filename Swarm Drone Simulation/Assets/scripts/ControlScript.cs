using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlScript : MonoBehaviour
{

    GamePadControl control;

    private float realYawAngle;

    [HideInInspector]
    public float pitchGyro;
    [HideInInspector]
    public float rollGyro;
    private float yawGyro;
    private float pitchAcc;
    private float rollAcc;
    private float accTotalVector;
    private int xAccRaw, yAccRaw, zAccRaw;
    private float linearizedYawSensorValue;
    private int laps;
    private float lastYaw;

    private float lastPitchSensorValue;
    private float lastRollSensorValue;
    private float lastYawSensorValue;
    private float lastLinearizedSensorValue;

    public float maxPitchAngle = 5f;
    public float maxRollAngle = 5f;
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
    private float minUsrerThrottle = 500;
    private float sampleTime = 0.00125f;

    public float barometerAltitude;

    bool armed = false;
    bool gamePad = false;
    bool fastMode = false;
    Vector2 pitchRollInput;
    Vector2 thrYawInput;

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
    }
    private void FS_Mode()
    {
        fastMode = !fastMode;
    }
    void Start()
    {

    }

    void FixedUpdate()
    {
        SensorFusion();
        GetComponent<UI>().updateUI();
        Inputs();
        PID();
        GetComponent<Motors>().MotorsState();
    }

    void Inputs()
    {
        

        if (gamePad)
        {
            if (armed) userThrottle = (Mathf.Abs(thrYawInput.y + 1) * 1200) + minUsrerThrottle;
            if (thrYawInput.y < -0.8) armed = true;

            if (fastMode)
            {
                pitchSetpoint = pitchRollInput.y * -maxPitchAngle * 1.5f;
                rollSetpoint = pitchRollInput.x * maxRollAngle * 1.5f;
            }
            else
            {
                pitchSetpoint = pitchRollInput.y * -maxPitchAngle;
                rollSetpoint = pitchRollInput.x * maxRollAngle;
            } 

            yawSetpoint = thrYawInput.x * -yawRate;
        }
        else
        {
            if (Input.GetKey(KeyCode.C) && userThrottle < 3000) userThrottle += 0.3f;
            if (Input.GetKey(KeyCode.X) && userThrottle > minUsrerThrottle) userThrottle -= 0.3f;

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
        if (Input.GetKeyDown(KeyCode.G)) gamePad = !gamePad;

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

        lastPitchSensorValue = pitchGyro;
        lastRollSensorValue = rollGyro;
        lastYawSensorValue = linearizedYawSensorValue;

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
        
        GetComponent<LSM6DSL_Gyro>().DPS500();
        pitchGyro += GetComponent<LSM6DSL_Gyro>().xAxisOutput * 0.000021875f;
        rollGyro += GetComponent<LSM6DSL_Gyro>().zAxisOutput * 0.000021875f;
        yawChangeRate = GetComponent<LSM6DSL_Gyro>().yAxisOutput * -0.0175f;
        
        /*
        GetComponent<LSM6DSL_Gyro>().DPS250();
        pitchGyro += GetComponent<LSM6DSL_Gyro>().xAxisOutput * 0.0000109375f;
        rollGyro += GetComponent<LSM6DSL_Gyro>().zAxisOutput * 0.0000109375f;
        yawChangeRate = GetComponent<LSM6DSL_Gyro>().yAxisOutput * -0.00875f;
        */

        GetComponent<LSM6DSL_Accelerometer>().G2(GetComponent<Motors>().avrMotorThrottle);
        xAccRaw = GetComponent<LSM6DSL_Accelerometer>().xAccOut;
        yAccRaw = GetComponent<LSM6DSL_Accelerometer>().yAccOut;
        zAccRaw = GetComponent<LSM6DSL_Accelerometer>().zAccOut;

        barometerAltitude = GetComponent<BMP280>().GetAltitude();

        accTotalVector = Mathf.Sqrt(Mathf.Pow(xAccRaw, 2) + Mathf.Pow(yAccRaw, 2) + Mathf.Pow(zAccRaw, 2));
        pitchAcc = Mathf.Asin(zAccRaw / accTotalVector) * -Mathf.Rad2Deg;
        rollAcc = Mathf.Asin(xAccRaw / accTotalVector) * -Mathf.Rad2Deg;

        pitchGyro = (pitchGyro * 0.995f) + (pitchAcc * 0.005f);
        rollGyro = (rollGyro * 0.995f) + (rollAcc * 0.005f);

        //Debug.Log(pitchAcc + " , " + rollAcc + "   ,   " + pitchGyro + " , " + rollGyro);
        //Debug.Log(finalPitch + " , " + finalRoll);

        
        //Debug.Log(yawChangeRate);
        // Altitude
        
    }
}
