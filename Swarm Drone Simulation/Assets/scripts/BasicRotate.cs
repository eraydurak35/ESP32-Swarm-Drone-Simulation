using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicRotate : MonoBehaviour
{
    private int rawXAxis;
    private int rawYAxis;
    private int rawZAxis;

    private float pitchSensorValue;
    private float rollSensorValue;
    private float yawSensorValue;
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Rotate(Vector3.right * Time.deltaTime * 150 * Input.GetAxis("Pitch"));
        transform.Rotate(Vector3.forward * Time.deltaTime * 150 * Input.GetAxis("Roll"));
        transform.Rotate(Vector3.up * Time.deltaTime * 150 * Input.GetAxis("Yaw"));

        GetComponent<LSM6DSL_Gyro>().DPS500();
        rawXAxis = GetComponent<LSM6DSL_Gyro>().xAxisOutput;
        rawYAxis = GetComponent<LSM6DSL_Gyro>().yAxisOutput;
        rawZAxis = GetComponent<LSM6DSL_Gyro>().zAxisOutput;

        pitchSensorValue += rawXAxis * 0.000021875f;
        rollSensorValue += rawZAxis * 0.000021875f;
        yawSensorValue += rawYAxis * 0.000021875f;

        Debug.Log(pitchSensorValue + "  ,  " + rollSensorValue + "  ,  " + yawSensorValue);


    }
}
