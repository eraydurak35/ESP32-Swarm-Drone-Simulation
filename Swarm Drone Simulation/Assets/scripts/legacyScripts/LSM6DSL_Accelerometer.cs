using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LSM6DSL_Accelerometer : MonoBehaviour
{
    private float xVelocity, yVelocity, zVelocity;
    private float xLastVelocity, yLastVelocity, zLastVelocity;
    private float xVelocityChange, yVelocityChange, zVelocityChange;
    [HideInInspector]
    public float xAccelerationGlobal, yAccelerationGlobal, zAccelerationGlobal;
    private float xAccelerationSensor, yAccelerationSensor, zAccelerationSensor;
    private float xAccelerationLocal, zAccelerationLocal;
    [HideInInspector]
    public int xAccOut, yAccOut, zAccOut;
    private float xAccG, yAccG, zAccG;

    private float accVector;
    private float xAngle, zAngle, yAngle;
    public float updateRate = 800f;
    [HideInInspector]
    public int maxValue = 32786;

    public int noise;

    public Rigidbody droneBody;

    [HideInInspector]
    public int mesurementRange = 2;

    private void Start()
    {
        droneBody.GetComponent<Rigidbody>();
    }

    public void G2(float motorSignal)
    {
        mesurementRange = 2;

        xAngle = transform.eulerAngles.x;
        zAngle = transform.eulerAngles.z;
        yAngle = transform.eulerAngles.y;

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

        float xzVector = Mathf.Sqrt(Mathf.Pow(xAccelerationGlobal, 2) + Mathf.Pow(zAccelerationGlobal, 2));

        float x = Mathf.Asin(xAccelerationGlobal / xzVector) * Mathf.Rad2Deg;
        float z = Mathf.Asin(zAccelerationGlobal / xzVector) * Mathf.Rad2Deg;

        if (x < 0 && z > 0) // -45 +45 bölgesi
        {
            xAccelerationLocal = -xzVector * Mathf.Sin(Mathf.Deg2Rad * (yAngle - x)); // 0 da negatif   // 90 da negatif    // 180 de pozitif   // 170 te pozitif
            zAccelerationLocal = -xzVector * Mathf.Sin(Mathf.Deg2Rad * (yAngle - z)); // 0 da pozitif   // 90 da negatif    // 180 de negatif   // 270 te pozitif
        }
        else if (x > 0 && z < 0) // +45 -45 Bölgesi
        {
            xAccelerationLocal = xzVector * Mathf.Sin(Mathf.Deg2Rad * (yAngle + x)); // 0 da pozitif    // 90 da pozitif    // 180 de negatif   // 270 te negatif
            zAccelerationLocal = xzVector * Mathf.Sin(Mathf.Deg2Rad * (yAngle + z)); // 0 da negatif    // 90 da pozitif    // 180 de pozitif   // 270 te negatif 
        }
        else if (x < 0 && z < 0) // -45 -45 Bölgesi
        {
            xAccelerationLocal = -xzVector * Mathf.Sin(Mathf.Deg2Rad * (yAngle - x + 90)); // 0 da negatif   // 90 da pozitif    // 180 de pozitif   // 270 te negatif
            zAccelerationLocal = xzVector * Mathf.Sin(Mathf.Deg2Rad * (yAngle + z - 90));  // 0 da negatif   // 90 da negatif    // 180 de pozitif   // 270 te pozitif
        }
        else if (x > 0 && z > 0) // +45 +45 Bölgesi
        {
            xAccelerationLocal = xzVector * Mathf.Sin(Mathf.Deg2Rad * (yAngle + x + 90)); // 0 da pozitif   // 90 da negatif    // 180 de negatif   // 270 te pozitif
            zAccelerationLocal = -xzVector * Mathf.Sin(Mathf.Deg2Rad * (yAngle - z - 90)); // 0 da pozitif   // 90 da pozitif    // 180 de negatif   // 270 te negatif
        }
        else if (x == 0 && z == 90)
        {
            xAccelerationLocal = xzVector * Mathf.Cos(Mathf.Deg2Rad * (yAngle +90));
            zAccelerationLocal = xzVector * Mathf.Cos(Mathf.Deg2Rad * yAngle);
        }
        else if (x == 0 && z == -90)
        {
            xAccelerationLocal = -xzVector * Mathf.Cos(Mathf.Deg2Rad * (yAngle + 90));
            zAccelerationLocal = -xzVector * Mathf.Cos(Mathf.Deg2Rad * yAngle);
        }
        else if (x == -90 && z == 0)
        {
            xAccelerationLocal = -xzVector * Mathf.Cos(Mathf.Deg2Rad * yAngle);
            zAccelerationLocal = xzVector * Mathf.Cos(Mathf.Deg2Rad * (yAngle + 90));
        }
        else if (x == 90 && z == 0)
        {
            xAccelerationLocal = xzVector * Mathf.Cos(Mathf.Deg2Rad * yAngle);
            zAccelerationLocal = -xzVector * Mathf.Cos(Mathf.Deg2Rad * (yAngle + 90));
        }

        //Debug.Log(xAccelerationLocal + " , " + zAccelerationLocal);

        xAccelerationSensor = yAccelerationGlobal * Mathf.Sin(Mathf.Deg2Rad * zAngle) + xAccelerationLocal * Mathf.Cos(Mathf.Deg2Rad * zAngle);

        yAccelerationSensor = (Mathf.Cos(Mathf.Deg2Rad * zAngle)) * (Mathf.Cos(Mathf.Deg2Rad * xAngle)) * yAccelerationGlobal 
            + xAccelerationLocal * Mathf.Sin(Mathf.Deg2Rad * -zAngle) 
            + zAccelerationLocal * Mathf.Sin(Mathf.Deg2Rad * xAngle);

        zAccelerationSensor = -yAccelerationGlobal * Mathf.Sin(Mathf.Deg2Rad * xAngle) + zAccelerationLocal * Mathf.Cos(Mathf.Deg2Rad * xAngle);

        //Debug.Log(xAccelerationSensor + " , " + yAccelerationSensor + " , " + zAccelerationSensor);

        xAccG = xAccelerationSensor / -Physics.gravity.y;
        yAccG = yAccelerationSensor / -Physics.gravity.y;
        zAccG = zAccelerationSensor / -Physics.gravity.y;


        if ((int)(((xAccG * maxValue) / 2) + Random.Range((-noise * motorSignal) / 100, (noise * motorSignal) / 100)) > maxValue) xAccOut = maxValue;
        else if ((int)(((xAccG * maxValue) / 2) + Random.Range((-noise * motorSignal) / 100, (noise * motorSignal) / 100)) < -maxValue) xAccOut = -maxValue;
        else xAccOut = (int)(((xAccG * maxValue) / 2) + Random.Range((-noise * motorSignal) / 100, (noise * motorSignal) / 100));

        if ((int)(((yAccG * maxValue) / 2) + Random.Range((-noise * motorSignal) / 100, (noise * motorSignal) / 100)) > maxValue) yAccOut = maxValue;
        else if ((int)(((yAccG * maxValue) / 2) + Random.Range((-noise * motorSignal) / 100, (noise * motorSignal) / 100)) < -maxValue) yAccOut = -maxValue;
        else yAccOut = (int)(((yAccG * maxValue) / 2) + Random.Range((-noise * motorSignal) / 100, (noise * motorSignal) / 100));

        if ((int)(((zAccG * maxValue) / 2) + Random.Range((-noise * motorSignal) / 100, (noise * motorSignal) / 100)) > maxValue) zAccOut = maxValue;
        else if ((int)(((zAccG * maxValue) / 2) + Random.Range((-noise * motorSignal) / 100, (noise * motorSignal) / 100)) < -maxValue) zAccOut = -maxValue;
        else zAccOut = (int)(((zAccG * maxValue) / 2) + Random.Range((-noise * motorSignal) / 100, (noise * motorSignal) / 100));

        
        //Debug.Log(xAccOut + " , " + yAccOut + " , " + zAccOut);
    }
    public void G4(float motorSignal)
    {
        mesurementRange = 4;

        xAngle = transform.eulerAngles.x;
        zAngle = transform.eulerAngles.z;
        yAngle = transform.eulerAngles.y;

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

        float xzVector = Mathf.Sqrt(Mathf.Pow(xAccelerationGlobal, 2) + Mathf.Pow(zAccelerationGlobal, 2));

        float x = Mathf.Asin(xAccelerationGlobal / xzVector) * Mathf.Rad2Deg;
        float y = Mathf.Asin(zAccelerationGlobal / xzVector) * Mathf.Rad2Deg;

        if (x < 0 && y > 0) // -45 +45 bölgesi
        {
            xAccelerationLocal = -xzVector * Mathf.Sin(Mathf.Deg2Rad * (yAngle - x)); // 0 da negatif   // 90 da negatif    // 180 de pozitif   // 170 te pozitif
            zAccelerationLocal = -xzVector * Mathf.Sin(Mathf.Deg2Rad * (yAngle - y)); // 0 da pozitif   // 90 da negatif    // 180 de negatif   // 270 te pozitif
        }
        else if (x > 0 && y < 0) // +45 -45 Bölgesi
        {
            xAccelerationLocal = xzVector * Mathf.Sin(Mathf.Deg2Rad * (yAngle + x)); // 0 da pozitif    // 90 da pozitif    // 180 de negatif   // 270 te negatif
            zAccelerationLocal = xzVector * Mathf.Sin(Mathf.Deg2Rad * (yAngle + y)); // 0 da negatif    // 90 da pozitif    // 180 de pozitif   // 270 te negatif 
        }
        else if (x < 0 && y < 0) // -45 -45 Bölgesi
        {
            xAccelerationLocal = -xzVector * Mathf.Sin(Mathf.Deg2Rad * (yAngle - x + 90)); // 0 da negatif   // 90 da pozitif    // 180 de pozitif   // 270 te negatif
            zAccelerationLocal = xzVector * Mathf.Sin(Mathf.Deg2Rad * (yAngle + y - 90));  // 0 da negatif   // 90 da negatif    // 180 de pozitif   // 270 te pozitif
        }
        else if (x > 0 && y > 0) // +45 +45 Bölgesi
        {
            xAccelerationLocal = xzVector * Mathf.Sin(Mathf.Deg2Rad * (yAngle + x + 90)); // 0 da pozitif   // 90 da negatif    // 180 de negatif   // 270 te pozitif
            zAccelerationLocal = -xzVector * Mathf.Sin(Mathf.Deg2Rad * (yAngle - y - 90)); // 0 da pozitif   // 90 da pozitif    // 180 de negatif   // 270 te negatif
        }
        else if (x == 0 && y == 90)
        {
            xAccelerationLocal = xzVector * Mathf.Cos(Mathf.Deg2Rad * (yAngle + 90));
            zAccelerationLocal = xzVector * Mathf.Cos(Mathf.Deg2Rad * yAngle);
        }
        else if (x == 0 && y == -90)
        {
            xAccelerationLocal = -xzVector * Mathf.Cos(Mathf.Deg2Rad * (yAngle + 90));
            zAccelerationLocal = -xzVector * Mathf.Cos(Mathf.Deg2Rad * yAngle);
        }
        else if (x == -90 && y == 0)
        {
            xAccelerationLocal = -xzVector * Mathf.Cos(Mathf.Deg2Rad * yAngle);
            zAccelerationLocal = xzVector * Mathf.Cos(Mathf.Deg2Rad * (yAngle + 90));
        }
        else if (x == 90 && y == 0)
        {
            xAccelerationLocal = xzVector * Mathf.Cos(Mathf.Deg2Rad * yAngle);
            zAccelerationLocal = -xzVector * Mathf.Cos(Mathf.Deg2Rad * (yAngle + 90));
        }


        xAccelerationSensor = yAccelerationGlobal * Mathf.Sin(Mathf.Deg2Rad * zAngle) + xAccelerationLocal * Mathf.Cos(Mathf.Deg2Rad * zAngle);

        yAccelerationSensor = (Mathf.Cos(Mathf.Deg2Rad * zAngle)) * (Mathf.Cos(Mathf.Deg2Rad * xAngle)) * yAccelerationGlobal
            + xAccelerationLocal * Mathf.Sin(Mathf.Deg2Rad * -zAngle)
            + zAccelerationLocal * Mathf.Sin(Mathf.Deg2Rad * xAngle);

        zAccelerationSensor = -yAccelerationGlobal * Mathf.Sin(Mathf.Deg2Rad * xAngle) + zAccelerationLocal * Mathf.Cos(Mathf.Deg2Rad * xAngle);

        //Debug.Log(xAccelerationSensor + " , " + yAccelerationSensor + " , " + zAccelerationSensor);

        xAccG = xAccelerationSensor / -Physics.gravity.y;
        yAccG = yAccelerationSensor / -Physics.gravity.y;
        zAccG = zAccelerationSensor / -Physics.gravity.y;


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
        mesurementRange = 8;

        xAngle = transform.eulerAngles.x;
        zAngle = transform.eulerAngles.z;
        yAngle = transform.eulerAngles.y;

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

        float xzVector = Mathf.Sqrt(Mathf.Pow(xAccelerationGlobal, 2) + Mathf.Pow(zAccelerationGlobal, 2));

        float x = Mathf.Asin(xAccelerationGlobal / xzVector) * Mathf.Rad2Deg;
        float y = Mathf.Asin(zAccelerationGlobal / xzVector) * Mathf.Rad2Deg;

        if (x < 0 && y > 0) // -45 +45 bölgesi
        {
            xAccelerationLocal = -xzVector * Mathf.Sin(Mathf.Deg2Rad * (yAngle - x)); // 0 da negatif   // 90 da negatif    // 180 de pozitif   // 170 te pozitif
            zAccelerationLocal = -xzVector * Mathf.Sin(Mathf.Deg2Rad * (yAngle - y)); // 0 da pozitif   // 90 da negatif    // 180 de negatif   // 270 te pozitif
        }
        else if (x > 0 && y < 0) // +45 -45 Bölgesi
        {
            xAccelerationLocal = xzVector * Mathf.Sin(Mathf.Deg2Rad * (yAngle + x)); // 0 da pozitif    // 90 da pozitif    // 180 de negatif   // 270 te negatif
            zAccelerationLocal = xzVector * Mathf.Sin(Mathf.Deg2Rad * (yAngle + y)); // 0 da negatif    // 90 da pozitif    // 180 de pozitif   // 270 te negatif 
        }
        else if (x < 0 && y < 0) // -45 -45 Bölgesi
        {
            xAccelerationLocal = -xzVector * Mathf.Sin(Mathf.Deg2Rad * (yAngle - x + 90)); // 0 da negatif   // 90 da pozitif    // 180 de pozitif   // 270 te negatif
            zAccelerationLocal = xzVector * Mathf.Sin(Mathf.Deg2Rad * (yAngle + y - 90));  // 0 da negatif   // 90 da negatif    // 180 de pozitif   // 270 te pozitif
        }
        else if (x > 0 && y > 0) // +45 +45 Bölgesi
        {
            xAccelerationLocal = xzVector * Mathf.Sin(Mathf.Deg2Rad * (yAngle + x + 90)); // 0 da pozitif   // 90 da negatif    // 180 de negatif   // 270 te pozitif
            zAccelerationLocal = -xzVector * Mathf.Sin(Mathf.Deg2Rad * (yAngle - y - 90)); // 0 da pozitif   // 90 da pozitif    // 180 de negatif   // 270 te negatif
        }
        else if (x == 0 && y == 90)
        {
            xAccelerationLocal = xzVector * Mathf.Cos(Mathf.Deg2Rad * (yAngle + 90));
            zAccelerationLocal = xzVector * Mathf.Cos(Mathf.Deg2Rad * yAngle);
        }
        else if (x == 0 && y == -90)
        {
            xAccelerationLocal = -xzVector * Mathf.Cos(Mathf.Deg2Rad * (yAngle + 90));
            zAccelerationLocal = -xzVector * Mathf.Cos(Mathf.Deg2Rad * yAngle);
        }
        else if (x == -90 && y == 0)
        {
            xAccelerationLocal = -xzVector * Mathf.Cos(Mathf.Deg2Rad * yAngle);
            zAccelerationLocal = xzVector * Mathf.Cos(Mathf.Deg2Rad * (yAngle + 90));
        }
        else if (x == 90 && y == 0)
        {
            xAccelerationLocal = xzVector * Mathf.Cos(Mathf.Deg2Rad * yAngle);
            zAccelerationLocal = -xzVector * Mathf.Cos(Mathf.Deg2Rad * (yAngle + 90));
        }


        xAccelerationSensor = yAccelerationGlobal * Mathf.Sin(Mathf.Deg2Rad * zAngle) + xAccelerationLocal * Mathf.Cos(Mathf.Deg2Rad * zAngle);

        yAccelerationSensor = (Mathf.Cos(Mathf.Deg2Rad * zAngle)) * (Mathf.Cos(Mathf.Deg2Rad * xAngle)) * yAccelerationGlobal
            + xAccelerationLocal * Mathf.Sin(Mathf.Deg2Rad * -zAngle)
            + zAccelerationLocal * Mathf.Sin(Mathf.Deg2Rad * xAngle);

        zAccelerationSensor = -yAccelerationGlobal * Mathf.Sin(Mathf.Deg2Rad * xAngle) + zAccelerationLocal * Mathf.Cos(Mathf.Deg2Rad * xAngle);

        //Debug.Log(xAccelerationSensor + " , " + yAccelerationSensor + " , " + zAccelerationSensor);

        xAccG = xAccelerationSensor / -Physics.gravity.y;
        yAccG = yAccelerationSensor / -Physics.gravity.y;
        zAccG = zAccelerationSensor / -Physics.gravity.y;


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

        mesurementRange = 16;

        xAngle = transform.eulerAngles.x;
        zAngle = transform.eulerAngles.z;
        yAngle = transform.eulerAngles.y;

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

        float xzVector = Mathf.Sqrt(Mathf.Pow(xAccelerationGlobal, 2) + Mathf.Pow(zAccelerationGlobal, 2));

        float x = Mathf.Asin(xAccelerationGlobal / xzVector) * Mathf.Rad2Deg;
        float y = Mathf.Asin(zAccelerationGlobal / xzVector) * Mathf.Rad2Deg;

        if (x < 0 && y > 0) // -45 +45 bölgesi
        {
            xAccelerationLocal = -xzVector * Mathf.Sin(Mathf.Deg2Rad * (yAngle - x)); // 0 da negatif   // 90 da negatif    // 180 de pozitif   // 170 te pozitif
            zAccelerationLocal = -xzVector * Mathf.Sin(Mathf.Deg2Rad * (yAngle - y)); // 0 da pozitif   // 90 da negatif    // 180 de negatif   // 270 te pozitif
        }
        else if (x > 0 && y < 0) // +45 -45 Bölgesi
        {
            xAccelerationLocal = xzVector * Mathf.Sin(Mathf.Deg2Rad * (yAngle + x)); // 0 da pozitif    // 90 da pozitif    // 180 de negatif   // 270 te negatif
            zAccelerationLocal = xzVector * Mathf.Sin(Mathf.Deg2Rad * (yAngle + y)); // 0 da negatif    // 90 da pozitif    // 180 de pozitif   // 270 te negatif 
        }
        else if (x < 0 && y < 0) // -45 -45 Bölgesi
        {
            xAccelerationLocal = -xzVector * Mathf.Sin(Mathf.Deg2Rad * (yAngle - x + 90)); // 0 da negatif   // 90 da pozitif    // 180 de pozitif   // 270 te negatif
            zAccelerationLocal = xzVector * Mathf.Sin(Mathf.Deg2Rad * (yAngle + y - 90));  // 0 da negatif   // 90 da negatif    // 180 de pozitif   // 270 te pozitif
        }
        else if (x > 0 && y > 0) // +45 +45 Bölgesi
        {
            xAccelerationLocal = xzVector * Mathf.Sin(Mathf.Deg2Rad * (yAngle + x + 90)); // 0 da pozitif   // 90 da negatif    // 180 de negatif   // 270 te pozitif
            zAccelerationLocal = -xzVector * Mathf.Sin(Mathf.Deg2Rad * (yAngle - y - 90)); // 0 da pozitif   // 90 da pozitif    // 180 de negatif   // 270 te negatif
        }
        else if (x == 0 && y == 90)
        {
            xAccelerationLocal = xzVector * Mathf.Cos(Mathf.Deg2Rad * (yAngle + 90));
            zAccelerationLocal = xzVector * Mathf.Cos(Mathf.Deg2Rad * yAngle);
        }
        else if (x == 0 && y == -90)
        {
            xAccelerationLocal = -xzVector * Mathf.Cos(Mathf.Deg2Rad * (yAngle + 90));
            zAccelerationLocal = -xzVector * Mathf.Cos(Mathf.Deg2Rad * yAngle);
        }
        else if (x == -90 && y == 0)
        {
            xAccelerationLocal = -xzVector * Mathf.Cos(Mathf.Deg2Rad * yAngle);
            zAccelerationLocal = xzVector * Mathf.Cos(Mathf.Deg2Rad * (yAngle + 90));
        }
        else if (x == 90 && y == 0)
        {
            xAccelerationLocal = xzVector * Mathf.Cos(Mathf.Deg2Rad * yAngle);
            zAccelerationLocal = -xzVector * Mathf.Cos(Mathf.Deg2Rad * (yAngle + 90));
        }


        xAccelerationSensor = yAccelerationGlobal * Mathf.Sin(Mathf.Deg2Rad * zAngle) + xAccelerationLocal * Mathf.Cos(Mathf.Deg2Rad * zAngle);

        yAccelerationSensor = (Mathf.Cos(Mathf.Deg2Rad * zAngle)) * (Mathf.Cos(Mathf.Deg2Rad * xAngle)) * yAccelerationGlobal
            + xAccelerationLocal * Mathf.Sin(Mathf.Deg2Rad * -zAngle)
            + zAccelerationLocal * Mathf.Sin(Mathf.Deg2Rad * xAngle);

        zAccelerationSensor = -yAccelerationGlobal * Mathf.Sin(Mathf.Deg2Rad * xAngle) + zAccelerationLocal * Mathf.Cos(Mathf.Deg2Rad * xAngle);

        //Debug.Log(xAccelerationSensor + " , " + yAccelerationSensor + " , " + zAccelerationSensor);

        xAccG = xAccelerationSensor / -Physics.gravity.y;
        yAccG = yAccelerationSensor / -Physics.gravity.y;
        zAccG = zAccelerationSensor / -Physics.gravity.y;


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
