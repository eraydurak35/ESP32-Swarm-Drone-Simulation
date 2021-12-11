using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicTranslate : MonoBehaviour
{
    Rigidbody m_Rigidbody;
    float force = 0f;
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        GetComponent<LSM6DSL_Accelerometer>().G2(100f);

        //Debug.Log(GetComponent<LSM6DSL_Accelerometer>().xAccOut + "  ,  " + GetComponent<LSM6DSL_Accelerometer>().yAccOut + "  ,  " + GetComponent<LSM6DSL_Accelerometer>().zAccOut);

        //Debug.Log(Physics.gravity.y);
        //Debug.Log(GetComponent<LSM6DSL_Accelometer>().xAcceleration + " , " + GetComponent<LSM6DSL_Accelometer>().yAcceleration + " , " + GetComponent<LSM6DSL_Accelometer>().zAcceleration);
        force += 0.0001f;
        //m_Rigidbody.AddForce(Vector3.forward * force);
    }
}
