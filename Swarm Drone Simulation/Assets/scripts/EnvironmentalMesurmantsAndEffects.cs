using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentalMeasurementsAndEffects : MonoBehaviour
{
    [HideInInspector]
    public float groundEffectBoost_LB;
    [HideInInspector]
    public float groundEffectBoost_LT;
    [HideInInspector]
    public float groundEffectBoost_RB;
    [HideInInspector]
    public float groundEffectBoost_RT;

    private RaycastHit hitLB;
    private RaycastHit hitRB;
    private RaycastHit hitLT;
    private RaycastHit hitRT;
    [HideInInspector]
    public float distanceLB;
    [HideInInspector]
    public float distanceRB;
    [HideInInspector]
    public float distanceLT;
    [HideInInspector]
    public float distanceRT;

    [HideInInspector]
    public float realAltitude;

    private Vector3 prevPosition;
    [HideInInspector]
    public float speed;
    public float updateRate = 800;

    public float groundEffecCoef = 12f;
    private float groundEffectBoostLB;
    private float groundEffectBoostLT;
    private float groundEffectBoostRB;
    private float groundEffectBoostRT;

    Rigidbody droneBody;

    Transform left_bott_prop;
    Transform left_top_prop;
    Transform right_top_prop;
    Transform right_bott_prop;

    public GameObject TopLeftMotor;
    public GameObject TopRightMotor;
    public GameObject BottomLeftMotor;
    public GameObject BottomRightMotor;

    public LineRenderer leftBottomLine;
    public LineRenderer leftTopLine;
    public LineRenderer rightBottomLine;
    public LineRenderer rightTopLine;

    public LineRenderer XAccPozitive;
    public LineRenderer XAccNegative;
    public LineRenderer YAccPozitive;
    public LineRenderer YAccNegative;
    public LineRenderer ZAccPozitive;
    public LineRenderer ZAccNegative;

    private void Start()
    {
        droneBody = GetComponent<Rigidbody>();
        left_bott_prop = transform.Find("left_bot_prop_pivot");
        left_top_prop = transform.Find("left_top_prop_pivot");
        right_top_prop = transform.Find("right_top_prop_pivot");
        right_bott_prop = transform.Find("right_bot_prop_pivot");
    }

    public void GroundEffect()
    {
        Ray rayLB = new Ray(BottomLeftMotor.transform.position, -Vector3.up);
        Ray rayRB = new Ray(BottomRightMotor.transform.position, -Vector3.up);
        Ray rayLT = new Ray(TopLeftMotor.transform.position, -Vector3.up);
        Ray rayRT = new Ray(TopRightMotor.transform.position, -Vector3.up);

        if (Physics.Raycast(rayLB, out hitLB, 10))
        {
            distanceLB = hitLB.distance;
        }
        if (Physics.Raycast(rayRB, out hitRB, 10))
        {
            distanceRB = hitRB.distance;
        }
        if (Physics.Raycast(rayLT, out hitLT, 10))
        {
            distanceLT = hitLT.distance;
        }
        if (Physics.Raycast(rayRT, out hitRT, 10))
        {
            distanceRT = hitRT.distance;
        }
    }

    public void Speed()
    {
        speed = Vector3.Distance(prevPosition, transform.position) / (1.0f / updateRate);
        prevPosition = transform.position;
    }

    public void RealAltitude()
    {
        realAltitude = transform.position.y;
    }

    public void GenerateThrustOnMotors()
    {
        GroundEffect();

        groundEffectBoostLB = GetComponent<Motors>().LBThrust * ((groundEffecCoef / 1000) / Mathf.Pow(distanceLB, 2));
        groundEffectBoostLT = GetComponent<Motors>().LTThrust * ((groundEffecCoef / 1000) / Mathf.Pow(distanceLT, 2));
        groundEffectBoostRB = GetComponent<Motors>().RBThrust * ((groundEffecCoef / 1000) / Mathf.Pow(distanceRB, 2));
        groundEffectBoostRT = GetComponent<Motors>().RTThrust * ((groundEffecCoef / 1000) / Mathf.Pow(distanceRT, 2));


        // Add force according to motor thrust
        droneBody.AddForceAtPosition(transform.up * (GetComponent<Motors>().LBThrust + groundEffectBoostLB), BottomLeftMotor.transform.position);
        droneBody.AddForceAtPosition(transform.up * (GetComponent<Motors>().LTThrust + groundEffectBoostLT), TopLeftMotor.transform.position);
        droneBody.AddForceAtPosition(transform.up * (GetComponent<Motors>().RBThrust + groundEffectBoostRB), BottomRightMotor.transform.position);
        droneBody.AddForceAtPosition(transform.up * (GetComponent<Motors>().RTThrust + groundEffectBoostRT), TopRightMotor.transform.position);

        // Add torque to drone body according to motor thrust
        droneBody.AddTorque(transform.up * ((GetComponent<Motors>().LBThrust + GetComponent<Motors>().RTThrust) - (GetComponent<Motors>().LTThrust + GetComponent<Motors>().RBThrust)) / 50);

        // Turn propellers acording to motor thrust
        left_bott_prop.Rotate(0, GetComponent<Motors>().LBThrust * -50, 0);
        left_top_prop.Rotate(0, GetComponent<Motors>().LTThrust * 50, 0);
        right_bott_prop.Rotate(0, GetComponent<Motors>().RBThrust * 50, 0);
        right_top_prop.Rotate(0, GetComponent<Motors>().RTThrust * -50, 0);

        // Visulalize motor thrust 
        leftBottomLine.transform.localScale = new Vector3(1, 1.3f, GetComponent<Motors>().LBThrust);
        leftTopLine.transform.localScale = new Vector3(1, 1.3f, GetComponent<Motors>().LTThrust);
        rightBottomLine.transform.localScale = new Vector3(1, 1.3f, GetComponent<Motors>().RBThrust);
        rightTopLine.transform.localScale = new Vector3(1, 1.3f, GetComponent<Motors>().RTThrust);
    }

    public void VisualizeAcceleration()
    {
        if (GetComponent<LSM6DSL_Accelerometer>().xAccOut > 0)
        {
            XAccPozitive.transform.localScale = new Vector3(1, 1, GetComponent<LSM6DSL_Accelerometer>().xAccOut
                / 40000f);

            XAccNegative.transform.localScale = new Vector3(1, 1, 0);
        }
        else if (GetComponent<LSM6DSL_Accelerometer>().xAccOut < 0)
        {
            XAccNegative.transform.localScale = new Vector3(1, 1, -GetComponent<LSM6DSL_Accelerometer>().xAccOut
                / 40000f);

            XAccPozitive.transform.localScale = new Vector3(1, 1, 0);
        }
        if (GetComponent<LSM6DSL_Accelerometer>().yAccOut > 0)
        {
            YAccPozitive.transform.localScale = new Vector3(1, 1, GetComponent<LSM6DSL_Accelerometer>().yAccOut
                / 40000f);

            YAccNegative.transform.localScale = new Vector3(1, 1, 0);
        }
        else if (GetComponent<LSM6DSL_Accelerometer>().yAccOut < 0)
        {
            YAccNegative.transform.localScale = new Vector3(1, 1, -GetComponent<LSM6DSL_Accelerometer>().yAccOut
                / 40000f);

            YAccPozitive.transform.localScale = new Vector3(1, 1, 0);
        }
        if (GetComponent<LSM6DSL_Accelerometer>().zAccOut > 0)
        {
            ZAccPozitive.transform.localScale = new Vector3(1, 1, GetComponent<LSM6DSL_Accelerometer>().zAccOut
                / 40000f);

            ZAccNegative.transform.localScale = new Vector3(1, 1, 0);
        }
        else if (GetComponent<LSM6DSL_Accelerometer>().zAccOut < 0)
        {
            ZAccNegative.transform.localScale = new Vector3(1, 1, -GetComponent<LSM6DSL_Accelerometer>().zAccOut
                / 40000f);

            ZAccPozitive.transform.localScale = new Vector3(1, 1, 0);
        }
    }

    public void WindDisturbance()
    {

    }
}
