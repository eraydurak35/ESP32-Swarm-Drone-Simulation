using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LSM6DSL_Accelerometer : MonoBehaviour
{
    private float xVelocity, yVelocity, zVelocity;
    private float xLastVelocity, yLastVelocity, zLastVelocity;
    private float xVelocityChange, yVelocityChange, zVelocityChange;
    private float xAccelerationGlobal, yAccelerationGlobal, zAccelerationGlobal;
    private float xAccelerationSensor, yAccelerationSensor, zAccelerationSensor;
    [HideInInspector]
    public int xAccOut, yAccOut, zAccOut;
    private float xAccG, yAccG, zAccG;

    private float accVector;
    private float xAngle, zAngle;
    public float updateRate = 800f;
    private int maxValue = 32786;

    public int noise;

    public Rigidbody droneBody;

    private void Start()
    {
        droneBody.GetComponent<Rigidbody>();
    }

    public void G2(float motorSignal)
    {

        xAngle = UnityEditor.TransformUtils.GetInspectorRotation(transform).x;
        zAngle = UnityEditor.TransformUtils.GetInspectorRotation(transform).z;

        xVelocity = droneBody.velocity.x;
        yVelocity = droneBody.velocity.y;
        zVelocity = droneBody.velocity.z;

        xVelocityChange = xVelocity - xLastVelocity;
        yVelocityChange = yVelocity - yLastVelocity;
        zVelocityChange = zVelocity - zLastVelocity;

        xLastVelocity = xVelocity;
        yLastVelocity = yVelocity;
        zLastVelocity = zVelocity;

        xAccelerationGlobal = xVelocityChange / (1.0f / updateRate);
        yAccelerationGlobal = (yVelocityChange / (1.0f / updateRate)) + Physics.gravity.y;
        zAccelerationGlobal = zVelocityChange / (1.0f / updateRate);

        accVector = Mathf.Sqrt(Mathf.Pow(xAccelerationGlobal, 2) + Mathf.Pow(yAccelerationGlobal, 2) + Mathf.Pow(zAccelerationGlobal, 2));

        xAccelerationSensor = accVector * Mathf.Sin(Mathf.Deg2Rad * zAngle);
        yAccelerationSensor = (Mathf.Cos(Mathf.Deg2Rad * zAngle)) * (Mathf.Cos(Mathf.Deg2Rad * xAngle)) * -accVector;
        zAccelerationSensor = accVector * Mathf.Sin(Mathf.Deg2Rad * xAngle);

        xAccG = xAccelerationSensor / Physics.gravity.y;
        yAccG = yAccelerationSensor / Physics.gravity.y;
        zAccG = zAccelerationSensor / Physics.gravity.y;


        if ((int)(((xAccG * maxValue) / 2) + Random.Range((-noise * motorSignal) / 100, (noise * motorSignal) / 100)) > maxValue) xAccOut = maxValue;
        else if ((int)(((xAccG * maxValue) / 2) + Random.Range((-noise * motorSignal) / 100, (noise * motorSignal) / 100)) < -maxValue) xAccOut = -maxValue;
        else xAccOut = (int)(((xAccG * maxValue) / 2) + Random.Range((-noise * motorSignal) / 100, (noise * motorSignal) / 100));

        if ((int)(((yAccG * maxValue) / 2) + Random.Range((-noise * motorSignal) / 100, (noise * motorSignal) / 100)) > maxValue) yAccOut = maxValue;
        else if ((int)(((yAccG * maxValue) / 2) + Random.Range((-noise * motorSignal) / 100, (noise * motorSignal) / 100)) < -maxValue) yAccOut = -maxValue;
        else yAccOut = (int)(((yAccG * maxValue) / 2) + Random.Range((-noise * motorSignal) / 100, (noise * motorSignal) / 100));

        if ((int)(((zAccG * maxValue) / 2) + Random.Range((-noise * motorSignal) / 100, (noise * motorSignal) / 100)) > maxValue) zAccOut = maxValue;
        else if ((int)(((zAccG * maxValue) / 2) + Random.Range((-noise * motorSignal) / 100, (noise * motorSignal) / 100)) < -maxValue) zAccOut = -maxValue;
        else zAccOut = (int)(((zAccG * maxValue) / 2) + Random.Range((-noise * motorSignal) / 100, (noise * motorSignal) / 100));
    }
    public void G4(float motorSignal)
    {
        xAngle = UnityEditor.TransformUtils.GetInspectorRotation(transform).x;
        zAngle = UnityEditor.TransformUtils.GetInspectorRotation(transform).z;

        xVelocity = droneBody.velocity.x;
        yVelocity = droneBody.velocity.y;
        zVelocity = droneBody.velocity.z;

        xVelocityChange = xVelocity - xLastVelocity;
        yVelocityChange = yVelocity - yLastVelocity;
        zVelocityChange = zVelocity - zLastVelocity;

        xLastVelocity = xVelocity;
        yLastVelocity = yVelocity;
        zLastVelocity = zVelocity;

        xAccelerationGlobal = xVelocityChange / (1.0f / updateRate);
        yAccelerationGlobal = (yVelocityChange / (1.0f / updateRate)) + Physics.gravity.y;
        zAccelerationGlobal = zVelocityChange / (1.0f / updateRate);

        accVector = Mathf.Sqrt(Mathf.Pow(xAccelerationGlobal, 2) + Mathf.Pow(yAccelerationGlobal, 2) + Mathf.Pow(zAccelerationGlobal, 2));

        xAccelerationSensor = accVector * Mathf.Sin(Mathf.Deg2Rad * zAngle);
        yAccelerationSensor = (Mathf.Cos(Mathf.Deg2Rad * zAngle)) * (Mathf.Cos(Mathf.Deg2Rad * xAngle)) * -accVector;
        zAccelerationSensor = accVector * Mathf.Sin(Mathf.Deg2Rad * xAngle);


        xAccG = xAccelerationSensor / Physics.gravity.y;
        yAccG = yAccelerationSensor / Physics.gravity.y;
        zAccG = zAccelerationSensor / Physics.gravity.y;


        if ((int)(((xAccG * maxValue) / 4) + Random.Range((-noise * motorSignal) / 100, (noise * motorSignal) / 100)) > maxValue) xAccOut = maxValue;
        else if ((int)(((xAccG * maxValue) / 4) + Random.Range((-noise * motorSignal) / 100, (noise * motorSignal) / 100)) < -maxValue) xAccOut = -maxValue;
        else xAccOut = (int)(((xAccG * maxValue) / 4) + Random.Range((-noise * motorSignal) / 100, (noise * motorSignal) / 100));

        if ((int)(((yAccG * maxValue) / 4) + Random.Range((-noise * motorSignal) / 100, (noise * motorSignal) / 100)) > maxValue) yAccOut = maxValue;
        else if ((int)(((yAccG * maxValue) / 4) + Random.Range((-noise * motorSignal) / 100, (noise * motorSignal) / 100)) < -maxValue) yAccOut = -maxValue;
        else yAccOut = (int)(((yAccG * maxValue) / 4) + Random.Range((-noise * motorSignal) / 100, (noise * motorSignal) / 100));

        if ((int)(((zAccG * maxValue) / 4) + Random.Range((-noise * motorSignal) / 100, (noise * motorSignal) / 100)) > maxValue) zAccOut = maxValue;
        else if ((int)(((zAccG * maxValue) / 4) + Random.Range((-noise * motorSignal) / 100, (noise * motorSignal) / 100)) < -maxValue) zAccOut = -maxValue;
        else zAccOut = (int)(((zAccG * maxValue) / 4) + Random.Range((-noise * motorSignal) / 100, (noise * motorSignal) / 100));
    }
    public void G8(float motorSignal)
    {
        xAngle = UnityEditor.TransformUtils.GetInspectorRotation(transform).x;
        zAngle = UnityEditor.TransformUtils.GetInspectorRotation(transform).z;

        xVelocity = droneBody.velocity.x;
        yVelocity = droneBody.velocity.y;
        zVelocity = droneBody.velocity.z;

        xVelocityChange = xVelocity - xLastVelocity;
        yVelocityChange = yVelocity - yLastVelocity;
        zVelocityChange = zVelocity - zLastVelocity;

        xLastVelocity = xVelocity;
        yLastVelocity = yVelocity;
        zLastVelocity = zVelocity;

        xAccelerationGlobal = xVelocityChange / (1.0f / updateRate);
        yAccelerationGlobal = (yVelocityChange / (1.0f / updateRate)) + Physics.gravity.y;
        zAccelerationGlobal = zVelocityChange / (1.0f / updateRate);

        accVector = Mathf.Sqrt(Mathf.Pow(xAccelerationGlobal, 2) + Mathf.Pow(yAccelerationGlobal, 2) + Mathf.Pow(zAccelerationGlobal, 2));

        xAccelerationSensor = accVector * Mathf.Sin(Mathf.Deg2Rad * zAngle);
        yAccelerationSensor = (Mathf.Cos(Mathf.Deg2Rad * zAngle)) * (Mathf.Cos(Mathf.Deg2Rad * xAngle)) * -accVector;
        zAccelerationSensor = accVector * Mathf.Sin(Mathf.Deg2Rad * xAngle);

        xAccG = xAccelerationSensor / Physics.gravity.y;
        yAccG = yAccelerationSensor / Physics.gravity.y;
        zAccG = zAccelerationSensor / Physics.gravity.y;


        if ((int)(((xAccG * maxValue) / 8) + Random.Range((-noise * motorSignal) / 100, (noise * motorSignal) / 100)) > maxValue) xAccOut = maxValue;
        else if ((int)(((xAccG * maxValue) / 8) + Random.Range((-noise * motorSignal) / 100, (noise * motorSignal) / 100)) < -maxValue) xAccOut = -maxValue;
        else xAccOut = (int)(((xAccG * maxValue) / 8) + Random.Range((-noise * motorSignal) / 100, (noise * motorSignal) / 100));

        if ((int)(((yAccG * maxValue) / 8) + Random.Range((-noise * motorSignal) / 100, (noise * motorSignal) / 100)) > maxValue) yAccOut = maxValue;
        else if ((int)(((yAccG * maxValue) / 8) + Random.Range((-noise * motorSignal) / 100, (noise * motorSignal) / 100)) < -maxValue) yAccOut = -maxValue;
        else yAccOut = (int)(((yAccG * maxValue) / 8) + Random.Range((-noise * motorSignal) / 100, (noise * motorSignal) / 100));

        if ((int)(((zAccG * maxValue) / 8) + Random.Range((-noise * motorSignal) / 100, (noise * motorSignal) / 100)) > maxValue) zAccOut = maxValue;
        else if ((int)(((zAccG * maxValue) / 8) + Random.Range((-noise * motorSignal) / 100, (noise * motorSignal) / 100)) < -maxValue) zAccOut = -maxValue;
        else zAccOut = (int)(((zAccG * maxValue) / 8) + Random.Range((-noise * motorSignal) / 100, (noise * motorSignal) / 100));
    }
    public void G16(float motorSignal)
    {
        xAngle = UnityEditor.TransformUtils.GetInspectorRotation(transform).x;
        zAngle = UnityEditor.TransformUtils.GetInspectorRotation(transform).z;

        xVelocity = droneBody.velocity.x;
        yVelocity = droneBody.velocity.y;
        zVelocity = droneBody.velocity.z;

        xVelocityChange = xVelocity - xLastVelocity;
        yVelocityChange = yVelocity - yLastVelocity;
        zVelocityChange = zVelocity - zLastVelocity;

        xLastVelocity = xVelocity;
        yLastVelocity = yVelocity;
        zLastVelocity = zVelocity;

        xAccelerationGlobal = xVelocityChange / (1.0f / updateRate);
        yAccelerationGlobal = (yVelocityChange / (1.0f / updateRate)) + Physics.gravity.y;
        zAccelerationGlobal = zVelocityChange / (1.0f / updateRate);

        accVector = Mathf.Sqrt(Mathf.Pow(xAccelerationGlobal, 2) + Mathf.Pow(yAccelerationGlobal, 2) + Mathf.Pow(zAccelerationGlobal, 2));

        xAccelerationSensor = accVector * Mathf.Sin(Mathf.Deg2Rad * zAngle);
        yAccelerationSensor = (Mathf.Cos(Mathf.Deg2Rad * zAngle)) * (Mathf.Cos(Mathf.Deg2Rad * xAngle)) * -accVector;
        zAccelerationSensor = accVector * Mathf.Sin(Mathf.Deg2Rad * xAngle);

        xAccG = xAccelerationSensor / Physics.gravity.y;
        yAccG = yAccelerationSensor / Physics.gravity.y;
        zAccG = zAccelerationSensor / Physics.gravity.y;


        if ((int)(((xAccG * maxValue) / 16) + Random.Range((-noise * motorSignal) / 100, (noise * motorSignal) / 100)) > maxValue) xAccOut = maxValue;
        else if ((int)(((xAccG * maxValue) / 16) + Random.Range((-noise * motorSignal) / 100, (noise * motorSignal) / 100)) < -maxValue) xAccOut = -maxValue;
        else xAccOut = (int)(((xAccG * maxValue) / 16) + Random.Range((-noise * motorSignal) / 100, (noise * motorSignal) / 100));

        if ((int)(((yAccG * maxValue) / 16) + Random.Range((-noise * motorSignal) / 100, (noise * motorSignal) / 100)) > maxValue) yAccOut = maxValue;
        else if ((int)(((yAccG * maxValue) / 16) + Random.Range((-noise * motorSignal) / 100, (noise * motorSignal) / 100)) < -maxValue) yAccOut = -maxValue;
        else yAccOut = (int)(((yAccG * maxValue) / 16) + Random.Range((-noise * motorSignal) / 100, (noise * motorSignal) / 100));

        if ((int)(((zAccG * maxValue) / 16) + Random.Range((-noise * motorSignal) / 100, (noise * motorSignal) / 100)) > maxValue) zAccOut = maxValue;
        else if ((int)(((zAccG * maxValue) / 16) + Random.Range((-noise * motorSignal) / 100, (noise * motorSignal) / 100)) < -maxValue) zAccOut = -maxValue;
        else zAccOut = (int)(((zAccG * maxValue) / 16) + Random.Range((-noise * motorSignal) / 100, (noise * motorSignal) / 100));
    }
}
