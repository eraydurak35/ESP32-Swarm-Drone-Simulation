using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentEMAE : MonoBehaviour
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
    public float updateRate = 200;

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


    public float XwindVelocity = 0;
    public float YwindVelocity = 0;
    public float ZwindVelocity = 0;
    // Start is called before the first frame update
    void Start()
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

    public void GenerateThrustOnMotors()
    {
        GroundEffect();

        groundEffectBoostLB = GetComponent<AgentMotors>().LBThrust * ((groundEffecCoef / 1000) / Mathf.Pow(distanceLB, 2));
        groundEffectBoostLT = GetComponent<AgentMotors>().LTThrust * ((groundEffecCoef / 1000) / Mathf.Pow(distanceLT, 2));
        groundEffectBoostRB = GetComponent<AgentMotors>().RBThrust * ((groundEffecCoef / 1000) / Mathf.Pow(distanceRB, 2));
        groundEffectBoostRT = GetComponent<AgentMotors>().RTThrust * ((groundEffecCoef / 1000) / Mathf.Pow(distanceRT, 2));


        // Add force according to motor thrust
        droneBody.AddForceAtPosition(transform.up * (GetComponent<AgentMotors>().LBThrust + groundEffectBoostLB), BottomLeftMotor.transform.position);
        droneBody.AddForceAtPosition(transform.up * (GetComponent<AgentMotors>().LTThrust + groundEffectBoostLT), TopLeftMotor.transform.position);
        droneBody.AddForceAtPosition(transform.up * (GetComponent<AgentMotors>().RBThrust + groundEffectBoostRB), BottomRightMotor.transform.position);
        droneBody.AddForceAtPosition(transform.up * (GetComponent<AgentMotors>().RTThrust + groundEffectBoostRT), TopRightMotor.transform.position);

        // Add torque to drone body according to motor thrust
        droneBody.AddTorque(transform.up * ((GetComponent<AgentMotors>().LBThrust + GetComponent<AgentMotors>().RTThrust) - (GetComponent<AgentMotors>().LTThrust + GetComponent<AgentMotors>().RBThrust)) / 50);

        // Turn propellers acording to motor thrust
        left_bott_prop.Rotate(0, GetComponent<AgentMotors>().LBThrust * -200, 0);
        left_top_prop.Rotate(0, GetComponent<AgentMotors>().LTThrust * 200, 0);
        right_bott_prop.Rotate(0, GetComponent<AgentMotors>().RBThrust * 200, 0);
        right_top_prop.Rotate(0, GetComponent<AgentMotors>().RTThrust * -200, 0);

        // Visulalize motor thrust 
        /*
        leftBottomLine.transform.localScale = new Vector3(1, 1.3f, GetComponent<AgentMotors>().LBThrust);
        leftTopLine.transform.localScale = new Vector3(1, 1.3f, GetComponent<AgentMotors>().LTThrust);
        rightBottomLine.transform.localScale = new Vector3(1, 1.3f, GetComponent<AgentMotors>().RBThrust);
        rightTopLine.transform.localScale = new Vector3(1, 1.3f, GetComponent<AgentMotors>().RTThrust);
        */
    }
}
