using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicTranslate : MonoBehaviour
{
    Rigidbody m_Rigidbody;
    float force = 0f;

    float accGlobalizedXraw;
    float accGlobalizedZraw;
    public float accx;
    public float accz;
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        GetComponent<LSM6DSL_Accelerometer>().G2(10f);
        //GetComponent<L80REM37>().getLocation();
        GetComponent<UI>().updateUI();
        //Debug.Log(GetComponent<LSM6DSL_Accelerometer>().xAccOut + "  ,  " + GetComponent<LSM6DSL_Accelerometer>().yAccOut + "  ,  " + GetComponent<LSM6DSL_Accelerometer>().zAccOut);

        //Debug.Log(Physics.gravity.y);
        //Debug.Log(GetComponent<LSM6DSL_Accelometer>().xAcceleration + " , " + GetComponent<LSM6DSL_Accelometer>().yAcceleration + " , " + GetComponent<LSM6DSL_Accelometer>().zAcceleration);
        force += 0.0001f;
        m_Rigidbody.AddForce(Vector3.left * force);
        m_Rigidbody.AddForce(-Vector3.forward * force);



        accGlobalizedXraw = (int)(GetComponent<LSM6DSL_Accelerometer>().xAccOut - (GetComponent<LSM6DSL_Accelerometer>().yAccOut * Mathf.Sin(Mathf.Deg2Rad * transform.eulerAngles.z)));
        accGlobalizedZraw = (int)(GetComponent<LSM6DSL_Accelerometer>().zAccOut - (GetComponent<LSM6DSL_Accelerometer>().yAccOut * Mathf.Sin(Mathf.Deg2Rad * -transform.eulerAngles.x)));


        if (transform.eulerAngles.y >= 0 && transform.localEulerAngles.y < 90)
        {
            accx = accGlobalizedXraw * Mathf.Cos(Mathf.Deg2Rad * transform.eulerAngles.y) + accGlobalizedZraw * Mathf.Sin(Mathf.Deg2Rad * transform.eulerAngles.y);
            accz = accGlobalizedZraw * Mathf.Cos(Mathf.Deg2Rad * transform.eulerAngles.y) + accGlobalizedXraw * -Mathf.Sin(Mathf.Deg2Rad * transform.eulerAngles.y);
        }
        else if (transform.eulerAngles.y >= 90 && transform.eulerAngles.y < 180)
        {
            accx = accGlobalizedXraw * Mathf.Cos(Mathf.Deg2Rad * transform.eulerAngles.y) + accGlobalizedZraw * Mathf.Sin(Mathf.Deg2Rad * transform.eulerAngles.y);
            accz = accGlobalizedZraw * Mathf.Cos(Mathf.Deg2Rad * transform.eulerAngles.y) + accGlobalizedXraw * -Mathf.Sin(Mathf.Deg2Rad * transform.eulerAngles.y);
        }
        else if (transform.eulerAngles.y >= 180 && transform.eulerAngles.y < 270)
        {
            accx = accGlobalizedXraw * Mathf.Cos(Mathf.Deg2Rad * transform.eulerAngles.y) + accGlobalizedZraw * Mathf.Sin(Mathf.Deg2Rad * transform.eulerAngles.y);
            accz = accGlobalizedZraw * Mathf.Cos(Mathf.Deg2Rad * transform.eulerAngles.y) + accGlobalizedXraw * -Mathf.Sin(Mathf.Deg2Rad * transform.eulerAngles.y);
        }
        else if (transform.eulerAngles.y >= 270 && transform.eulerAngles.y < 360)
        {
            accx = accGlobalizedXraw * Mathf.Cos(Mathf.Deg2Rad * transform.eulerAngles.y) + accGlobalizedZraw * Mathf.Sin(Mathf.Deg2Rad * transform.eulerAngles.y);
            accz = accGlobalizedZraw * Mathf.Cos(Mathf.Deg2Rad * transform.eulerAngles.y) + accGlobalizedXraw * -Mathf.Sin(Mathf.Deg2Rad * transform.eulerAngles.y);
        }
        Debug.Log(accx + " , " + accz);
    }
}
