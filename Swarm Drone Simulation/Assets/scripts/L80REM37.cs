using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class L80REM37 : MonoBehaviour
{
    public float updateRate = 800;
    private float accLatitude; // Global Z axis
    private float accLongtitude; // Global X axis
    private float accVelocityX;
    private float accVelocityY;
    private float accVelocityZ;
    [HideInInspector]
    public float accGlobalizedXraw = 1;
    [HideInInspector]
    public float accGlobalizedYraw = 1;
    [HideInInspector]
    public float accGlobalizedZraw = 1;
    private float accVector;
    private int n = 0,k = 0;
    public int dataPerSecond = 5;
    public float deviation = 0;
    public float resolution = 0.11f;
    private float[] LatitudesGPS = new float[5];
    private float[] LongtitudesGPS = new float[5];
    [HideInInspector]
    public float meanLatitude;
    [HideInInspector]
    public float meanLongtitude;

    public float accx;
    public float accz;

    public GameObject dataPointGPS;
    public GameObject meanGpsSphere;

    GameObject[] locationsGPS = new GameObject[5];
    GameObject meanLocationGps;

    private void Start()
    {
        for (int i = 0; i<5;i++)
        {
            locationsGPS[i] = Instantiate(dataPointGPS) as GameObject;
        }

        meanLocationGps = Instantiate(meanGpsSphere) as GameObject;

    }
    public void getLocation()
    {
        if (n == updateRate / dataPerSecond)
        {
            n = 0;
            LatitudesGPS[k] = Random.Range(-deviation, deviation) + transform.position.z;
            LongtitudesGPS[k] = Random.Range(-deviation, deviation) + transform.position.x;
            k++;
            if (k == 5) k = 0;
            locationsGPS[k].transform.position = new Vector3(LongtitudesGPS[k], transform.position.y, LatitudesGPS[k]);

            meanLatitude = 0;
            meanLongtitude = 0;
            for (int i = 0; i < 5; i++)
            {
                meanLatitude += LatitudesGPS[i] / 5f;
                meanLongtitude += LongtitudesGPS[i] / 5f;
            }

            meanLocationGps.transform.position = new Vector3(meanLongtitude, transform.position.y, meanLatitude);

        }
        n++;


        accGlobalizedXraw = (int)(GetComponent<ControlScript>().xAccRaw - (GetComponent<ControlScript>().yAccRaw * Mathf.Sin(Mathf.Deg2Rad * GetComponent<ControlScript>().rollGyro)));
        accGlobalizedZraw = (int)(GetComponent<ControlScript>().zAccRaw - (GetComponent<ControlScript>().yAccRaw * Mathf.Sin(Mathf.Deg2Rad * -GetComponent<ControlScript>().pitchGyro)));


        if (transform.eulerAngles.y >= 0 && transform.localEulerAngles.y < 90)
        {
            accx = accGlobalizedZraw * Mathf.Cos(Mathf.Deg2Rad * transform.eulerAngles.y) + accGlobalizedXraw * Mathf.Sin(Mathf.Deg2Rad * transform.eulerAngles.y);
            accz = accGlobalizedXraw * Mathf.Cos(Mathf.Deg2Rad * transform.eulerAngles.y) + accGlobalizedZraw * -Mathf.Sin(Mathf.Deg2Rad * transform.eulerAngles.y);
        }
        else if (transform.eulerAngles.y >= 90 && transform.eulerAngles.y < 180)
        {
            accx = accGlobalizedXraw * Mathf.Cos(Mathf.Deg2Rad * transform.eulerAngles.y) + accGlobalizedZraw * Mathf.Sin(Mathf.Deg2Rad * transform.eulerAngles.y);
            accz = accGlobalizedZraw * Mathf.Cos(Mathf.Deg2Rad * transform.eulerAngles.y) + accGlobalizedXraw * -Mathf.Sin(Mathf.Deg2Rad * transform.eulerAngles.y);
        }
        else if (transform.eulerAngles.y >= 180 && transform.eulerAngles.y < 270)
        {

        }
        else if (transform.eulerAngles.y >= 270 && transform.eulerAngles.y < 360)
        {
            accx = accGlobalizedXraw * Mathf.Cos(Mathf.Deg2Rad * transform.eulerAngles.y) + accGlobalizedZraw * Mathf.Sin(Mathf.Deg2Rad * transform.eulerAngles.y);
            accz = accGlobalizedZraw * Mathf.Cos(Mathf.Deg2Rad * transform.eulerAngles.y) + accGlobalizedXraw * -Mathf.Sin(Mathf.Deg2Rad * transform.eulerAngles.y);
        }
        Debug.Log(accGlobalizedXraw + " , " + accGlobalizedZraw);

        /*

        accx = accGlobalizedXraw * Mathf.Cos(Mathf.Deg2Rad * transform.localEulerAngles.y) + accGlobalizedZraw * Mathf.Cos(Mathf.Deg2Rad * (transform.localEulerAngles.y - 90));
        accy = accGlobalizedZraw * Mathf.Cos(Mathf.Deg2Rad * transform.localEulerAngles.y) + accGlobalizedXraw * Mathf.Cos(Mathf.Deg2Rad * (transform.localEulerAngles.y - 90));

        */
        /*
        accVector = Mathf.Sqrt(Mathf.Pow(accGlobalizedXraw, 2) + Mathf.Pow(accGlobalizedZraw, 2));

        float x = Mathf.Asin(accGlobalizedXraw / accVector) * Mathf.Rad2Deg;
        float y = Mathf.Asin(accGlobalizedZraw / accVector) * Mathf.Rad2Deg;

        Debug.Log(x + " , " + y);
        
        if (x < 0 && y < 0) // -45 -45 Bölgesi
        {
            accGlobalizedXraw = (int)(-accVector * Mathf.Sin(Mathf.Deg2Rad * (transform.eulerAngles.y - x + 90))); // 0 da negatif   // 90 da pozitif    // 180 de pozitif   // 270 te negatif
            accGlobalizedZraw = (int)(accVector * Mathf.Sin(Mathf.Deg2Rad * (transform.eulerAngles.y + y - 90)));  // 0 da negatif   // 90 da negatif    // 180 de pozitif   // 270 te pozitif
        }

        */

        /*
        if (accGlobalizedXraw < 0) accGlobalizedXraw = (int)(-accVector * Mathf.Cos(Mathf.Deg2Rad * (GetComponent<ControlScript>().headingGyro + 90)));
        else accGlobalizedXraw = (int)(accVector * Mathf.Cos(Mathf.Deg2Rad * (GetComponent<ControlScript>().headingGyro + 90)));

        if (accGlobalizedZraw < 0) accGlobalizedZraw = (int)(-accVector * Mathf.Cos(Mathf.Deg2Rad * GetComponent<ControlScript>().headingGyro));
        else accGlobalizedZraw = (int)(accVector * Mathf.Cos(Mathf.Deg2Rad * GetComponent<ControlScript>().headingGyro));
        */

















        //accVector = Mathf.Sqrt(Mathf.Pow(GetComponent<LSM6DSL_Accelerometer>().xAccOut, 2) + Mathf.Pow(GetComponent<LSM6DSL_Accelerometer>().yAccOut, 2) + Mathf.Pow(GetComponent<LSM6DSL_Accelerometer>().zAccOut, 2)) - 16393;
        /*
        accNormalizedX = (GetComponent<LSM6DSL_Accelerometer>().xAccOut + (accVector * Mathf.Sin(Mathf.Deg2Rad * UnityEditor.TransformUtils.GetInspectorRotation(transform).z))) * Mathf.Cos(Mathf.Deg2Rad * UnityEditor.TransformUtils.GetInspectorRotation(transform).y);
        accNormalizedZ = (GetComponent<LSM6DSL_Accelerometer>().zAccOut + (accVector * Mathf.Sin(Mathf.Deg2Rad * UnityEditor.TransformUtils.GetInspectorRotation(transform).x))) * Mathf.Sin(Mathf.Deg2Rad * UnityEditor.TransformUtils.GetInspectorRotation(transform).y);
        accNormalizedY = Mathf.Cos(Mathf.Deg2Rad * UnityEditor.TransformUtils.GetInspectorRotation(transform).x) * Mathf.Cos(Mathf.Deg2Rad * UnityEditor.TransformUtils.GetInspectorRotation(transform).z) * accVector;
        */
        //accNormalizedX = GetComponent<ControlScript>().xAccRaw - Mathf.Sin(Mathf.Deg2Rad * GetComponent<ControlScript>().rollGyro) * (GetComponent<ControlScript>().yAccRaw + 16393);

        //accNormalizedX = GetComponent<LSM6DSL_Accelerometer>().xAccOut * Mathf.Sin(Mathf.Deg2Rad * UnityEditor.TransformUtils.GetInspectorRotation(transform).y);
        //accNormalizedZ = GetComponent<LSM6DSL_Accelerometer>().zAccOut * Mathf.Cos(Mathf.Deg2Rad * UnityEditor.TransformUtils.GetInspectorRotation(transform).y);

        //Debug.Log(accNormalizedX + " , " + accNormalizedY + " , " + accNormalizedZ);
        //Debug.Log(GetComponent<LSM6DSL_Accelerometer>().zAccOut + " , " + " , ");

    }
}
