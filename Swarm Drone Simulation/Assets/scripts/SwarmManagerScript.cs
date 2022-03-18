using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmManagerScript : MonoBehaviour
{
    /*
    AgentControl AgentControl0;
    AgentControl AgentControl1;
    AgentControl AgentControl2;
    AgentControl AgentControl3;
    AgentControl AgentControl4;
    AgentControl AgentControl5;
    */

    AgentControl[] AgentControl = new AgentControl[10];
    [SerializeField] GameObject[] Agents = new GameObject[10];
    public bool[] ActiveAgents = new bool[10];

    public GameObject CheckPointObject;
    GameObject checkpoint;
    private bool newCheckpoint = true;
    public float checkpointX;
    public float checkpointZ;
    public float checkpointY;
    [HideInInspector]
    public float[] Agent_X = new float[10];
    [HideInInspector]
    public float[] Agent_Y = new float[10];
    [HideInInspector]
    public float[] Agent_Z = new float[10];
    [HideInInspector]
    public float[] Agent_Heading = new float[10];
    [HideInInspector]
    public float[] AgentProximity = new float[10];
    [HideInInspector]
    public float[] AgentTargetHeading = new float[10];
    [HideInInspector]
    public float[] AgentBatteryStatus = new float[10];

    public bool FormArrowHead = false;
    public bool FormTriangle = false;
    public bool TakeOff = false;
    public bool Mission = false;
    float[,] CollisionDistance = new float[9, 10];
    float[,] PrevCollisionDistance = new float[9, 10];
    float[,] CollisionDistanceChangeRate = new float[9, 10];
    [HideInInspector]
    public float[] CollisionAltChange = new float[10];
    [HideInInspector]
    public float[] CollisionPitchChange = new float[10];
    [HideInInspector]
    public float[] CollisionRollChange = new float[10];
    [HideInInspector]
    public float[,] CollisionHeading = new float[9, 10];
    public float CollisionAvoidCoeff = 1f;
    public float ch;
    public float[] AgentCollHeading = new float[10];
    private void Awake()
    {

        for (int i = 0; i < 10; i++)
        {
            AgentControl[i] = Agents[i].GetComponent<AgentControl>();
        }

    }
    // Start is called before the first frame update
    void Start()
    {
        checkpoint = Instantiate(CheckPointObject) as GameObject;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        for (int j = 0; j < 10; j++)
        {
            Agent_X[j] = Agents[j].transform.position.x;
            Agent_Y[j] = Agents[j].transform.position.y;
            Agent_Z[j] = Agents[j].transform.position.z;
            Agent_Heading[j] = AgentControl[j].GetComponent<AgentControl>().AgentHeading;

            AgentBatteryStatus[j] = Agents[j].GetComponent<AgentBattery>().RemainBattPercent;

            if (AgentBatteryStatus[j] < 30f)
            {
                ActiveAgents[j] = false;
            }

        }

        if (Agent_Y[0] >= 9f && TakeOff)
        {
            TakeOff = false;
            FormArrowHead = true;
            Mission = true;
        }



        
        

        if (newCheckpoint)
        {
            checkpointX = Random.Range(150, 350);
            checkpointZ = Random.Range(150, 350);
            checkpointY = Random.Range(5, 40);
            checkpoint.transform.position = new Vector3(checkpointX, checkpointY, checkpointZ);
            newCheckpoint = false;
        }
        else
        {
            for (int k = 0; k < 10; k++)
            {
                AgentProximity[k] = Mathf.Sqrt(Mathf.Pow((checkpointX - Agent_X[k]), 2) + Mathf.Pow((checkpointY - Agent_Y[k]), 2) + Mathf.Pow((checkpointZ - Agent_Z[k]), 2));
                AgentTargetHeading[k] = (Mathf.Atan2((checkpointX - Agent_X[k]), (Agent_Z[k] - checkpointZ)) * Mathf.Rad2Deg) * -1f;
                
                if (AgentProximity[k] < 2f)
                {
                    newCheckpoint = true;
                }
            }
        }

        AvoidCollision();
        /*
        for (int c = 0; c < 10; c++)
        {
            Debug.Log(CollisionDistance[0, c] + "   ");
        }
        */
    }
    void AvoidCollision()
    {
        for (int n = 0; n < 9; n++)
        {
            for (int k = n + 1; k < 10; k++)
            {

                CollisionDistance[n, k] = Mathf.Sqrt(Mathf.Pow((Agent_X[n] - Agent_X[k]), 2) + Mathf.Pow((Agent_Y[n] - Agent_Y[k]), 2) + Mathf.Pow((Agent_Z[n] - Agent_Z[k]), 2));
                CollisionDistanceChangeRate[n, k] = (PrevCollisionDistance[n, k] - CollisionDistance[n, k]) * 200f;
                PrevCollisionDistance[n, k] = CollisionDistance[n, k];
                CollisionHeading[n, k] = (Mathf.Atan2((Agent_X[k] - Agent_X[n]), (Agent_Z[n] - Agent_Z[k])) * Mathf.Rad2Deg) * -1f;

                if (CollisionHeading[n, k] < 0f) CollisionHeading[n, k] += 360f;
                if (Agent_Heading[n] < 0f) AgentCollHeading[n] = Agent_Heading[n] + 360f;
                CollisionHeading[n, k] = (CollisionHeading[n, k] - Agent_Heading[n]) % 360f;

                /*
                if (Agent_Heading[8] < 0f) x = Agent_Heading[8] + 360f;
                else x = Agent_Heading[8];
                */

                //CollisionHeading[8, 9] = CollisionHeading[8, 9] - Agent_Heading[8];

                /*
                if (Agent_Heading[n] < 0f) CollisionHeading[n, k] -= Agent_Heading[n] + 360f;
                else CollisionHeading[n, k] -= Agent_Heading[n];

                */
                ch = CollisionHeading[8, 9];

                ////////////////////////////   AVOID BY ALTITUDE   /////////////////////////////

                /*
                if (CollisionDistance[n,k] < 2f)
                {
                    if (Agent_Y[n] > Agent_Y[k])
                    {
                        CollisionAltChange[n] = 2f / Mathf.Pow(CollisionDistance[n, k], 2);
                        CollisionAltChange[k] = -2f / Mathf.Pow(CollisionDistance[n, k], 2);
                        //Debug.Log("1......  " +n + "  " + k + "  " + CollisionAltChange[n] + "   " + CollisionAltChange[k]);
                        //Debug.Log(CollisionDistanceChangeRate[n, k]);
                    }
                    else if (Agent_Y[n] <= Agent_Y[k])
                    {
                        CollisionAltChange[n] = -2f / Mathf.Pow(CollisionDistance[n, k], 2);
                        CollisionAltChange[k] = 2f / Mathf.Pow(CollisionDistance[n, k], 2);
                        //Debug.Log("1......  " + n + "  " + k + "  " + CollisionAltChange[n] + "   " + CollisionAltChange[k]);
                        //Debug.Log(CollisionDistanceChangeRate[n, k]);
                    }

                }
                else if (CollisionDistance[n, k] > 10f)
                {
                    CollisionAltChange[n] = 0f;
                    CollisionAltChange[k] = 0f;
                }
                */

                //////////////////////////////    AVOID BY POSITION    ////////////////////////////

                /*
                CollisionHeading = (Mathf.Atan2((Agent_X[k] - Agent_X[n]), (Agent_Z[n] - Agent_Z[k])) * Mathf.Rad2Deg) * -1f;
                if (CollisionHeading < 0f) CollisionHeading += 360f;
                if (Agent_Heading[n] < 0f) CollisionHeading -= Agent_Heading[n] + 360f;
                else CollisionHeading -= Agent_Heading[n];

                CollisionPitchChange[n] = Mathf.Cos(Mathf.Deg2Rad * CollisionHeading) * 10f;
                CollisionRollChange[n] = Mathf.Sin(Mathf.Deg2Rad * CollisionHeading) * -10f;
                CollisionPitchChange[k] = Mathf.Cos(Mathf.Deg2Rad * CollisionHeading) * -10f;
                CollisionRollChange[k] = Mathf.Sin(Mathf.Deg2Rad * CollisionHeading) * 10f;

                */
                //Debug.Log(n + "  " + k + "  " + CollisionHeading);
                //Debug.Log(CollisionPitchChange[n] + "  " + CollisionRollChange[n] + "  " + CollisionPitchChange[k] + "  " + CollisionRollChange[k]);
                //Time.timeScale = 0;
            }


        }
        

        CollisionPitchChange[0] =
              CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[0, 1]) / Mathf.Pow(CollisionDistance[0, 1], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[0, 2]) / Mathf.Pow(CollisionDistance[0, 2], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[0, 3]) / Mathf.Pow(CollisionDistance[0, 3], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[0, 4]) / Mathf.Pow(CollisionDistance[0, 4], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[0, 5]) / Mathf.Pow(CollisionDistance[0, 5], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[0, 6]) / Mathf.Pow(CollisionDistance[0, 6], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[0, 7]) / Mathf.Pow(CollisionDistance[0, 7], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[0, 8]) / Mathf.Pow(CollisionDistance[0, 8], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[0, 9]) / Mathf.Pow(CollisionDistance[0, 9], 2));

        CollisionRollChange[0] =
              CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[0, 1]) / Mathf.Pow(CollisionDistance[0, 1], 2))
            + CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[0, 2]) / Mathf.Pow(CollisionDistance[0, 2], 2))
            + CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[0, 3]) / Mathf.Pow(CollisionDistance[0, 3], 2))
            + CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[0, 4]) / Mathf.Pow(CollisionDistance[0, 4], 2))
            + CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[0, 5]) / Mathf.Pow(CollisionDistance[0, 5], 2))
            + CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[0, 6]) / Mathf.Pow(CollisionDistance[0, 6], 2))
            + CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[0, 7]) / Mathf.Pow(CollisionDistance[0, 7], 2))
            + CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[0, 8]) / Mathf.Pow(CollisionDistance[0, 8], 2))
            + CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[0, 9]) / Mathf.Pow(CollisionDistance[0, 9], 2));

        
        ///////////////////////////////////////////////////////////////////////////////////////////////////

        CollisionPitchChange[1] = 
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[0, 1]) / Mathf.Pow(CollisionDistance[0, 1], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[1, 2]) / Mathf.Pow(CollisionDistance[1, 2], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[1, 3]) / Mathf.Pow(CollisionDistance[1, 3], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[1, 4]) / Mathf.Pow(CollisionDistance[1, 4], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[1, 5]) / Mathf.Pow(CollisionDistance[1, 5], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[1, 6]) / Mathf.Pow(CollisionDistance[1, 6], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[1, 7]) / Mathf.Pow(CollisionDistance[1, 7], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[1, 8]) / Mathf.Pow(CollisionDistance[1, 8], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[1, 9]) / Mathf.Pow(CollisionDistance[1, 9], 2));

        CollisionRollChange[1] = 
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[0, 1]) / Mathf.Pow(CollisionDistance[0, 1], 2))
            + CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[1, 2]) / Mathf.Pow(CollisionDistance[1, 2], 2))
            + CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[1, 3]) / Mathf.Pow(CollisionDistance[1, 3], 2))
            + CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[1, 4]) / Mathf.Pow(CollisionDistance[1, 4], 2))
            + CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[1, 5]) / Mathf.Pow(CollisionDistance[1, 5], 2))
            + CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[1, 6]) / Mathf.Pow(CollisionDistance[1, 6], 2))
            + CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[1, 7]) / Mathf.Pow(CollisionDistance[1, 7], 2))
            + CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[1, 8]) / Mathf.Pow(CollisionDistance[1, 8], 2))
            + CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[1, 9]) / Mathf.Pow(CollisionDistance[1, 9], 2));

        ///////////////////////////////////////////////////////////////////////////////////////////////////

        CollisionPitchChange[2] =
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[0, 2]) / Mathf.Pow(CollisionDistance[0, 2], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[1, 2]) / Mathf.Pow(CollisionDistance[1, 2], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[2, 3]) / Mathf.Pow(CollisionDistance[2, 3], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[2, 4]) / Mathf.Pow(CollisionDistance[2, 4], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[2, 5]) / Mathf.Pow(CollisionDistance[2, 5], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[2, 6]) / Mathf.Pow(CollisionDistance[2, 6], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[2, 7]) / Mathf.Pow(CollisionDistance[2, 7], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[2, 8]) / Mathf.Pow(CollisionDistance[2, 8], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[2, 9]) / Mathf.Pow(CollisionDistance[2, 9], 2));

        CollisionRollChange[2] =
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[0, 2]) / Mathf.Pow(CollisionDistance[0, 2], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[1, 2]) / Mathf.Pow(CollisionDistance[1, 2], 2))
            + CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[2, 3]) / Mathf.Pow(CollisionDistance[2, 3], 2))
            + CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[2, 4]) / Mathf.Pow(CollisionDistance[2, 4], 2))
            + CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[2, 5]) / Mathf.Pow(CollisionDistance[2, 5], 2))
            + CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[2, 6]) / Mathf.Pow(CollisionDistance[2, 6], 2))
            + CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[2, 7]) / Mathf.Pow(CollisionDistance[2, 7], 2))
            + CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[2, 8]) / Mathf.Pow(CollisionDistance[2, 8], 2))
            + CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[2, 9]) / Mathf.Pow(CollisionDistance[2, 9], 2));

        /////////////////////////////////////////////////////////////////////////////////////////////////

        CollisionPitchChange[3] =
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[0, 3]) / Mathf.Pow(CollisionDistance[0, 3], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[1, 3]) / Mathf.Pow(CollisionDistance[1, 3], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[2, 3]) / Mathf.Pow(CollisionDistance[2, 3], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[3, 4]) / Mathf.Pow(CollisionDistance[3, 4], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[3, 5]) / Mathf.Pow(CollisionDistance[3, 5], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[3, 6]) / Mathf.Pow(CollisionDistance[3, 6], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[3, 7]) / Mathf.Pow(CollisionDistance[3, 7], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[3, 8]) / Mathf.Pow(CollisionDistance[3, 8], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[3, 9]) / Mathf.Pow(CollisionDistance[3, 9], 2));

        CollisionRollChange[3] =
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[0, 3]) / Mathf.Pow(CollisionDistance[0, 3], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[1, 3]) / Mathf.Pow(CollisionDistance[1, 3], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[2, 3]) / Mathf.Pow(CollisionDistance[2, 3], 2))
            + CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[3, 4]) / Mathf.Pow(CollisionDistance[3, 4], 2))
            + CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[3, 5]) / Mathf.Pow(CollisionDistance[3, 5], 2))
            + CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[3, 6]) / Mathf.Pow(CollisionDistance[3, 6], 2))
            + CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[3, 7]) / Mathf.Pow(CollisionDistance[3, 7], 2))
            + CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[3, 8]) / Mathf.Pow(CollisionDistance[3, 8], 2))
            + CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[3, 9]) / Mathf.Pow(CollisionDistance[3, 9], 2));

        ///////////////////////////////////////////////////////////////////////////////////////////////////
        
        CollisionPitchChange[4] = 
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[0, 4]) / Mathf.Pow(CollisionDistance[0, 4], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[1, 4]) / Mathf.Pow(CollisionDistance[1, 4], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[2, 4]) / Mathf.Pow(CollisionDistance[2, 4], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[3, 4]) / Mathf.Pow(CollisionDistance[3, 4], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[4, 5]) / Mathf.Pow(CollisionDistance[4, 5], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[4, 6]) / Mathf.Pow(CollisionDistance[4, 6], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[4, 7]) / Mathf.Pow(CollisionDistance[4, 7], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[4, 8]) / Mathf.Pow(CollisionDistance[4, 8], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[4, 9]) / Mathf.Pow(CollisionDistance[4, 9], 2));

        CollisionRollChange[4] =
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[0, 4]) / Mathf.Pow(CollisionDistance[0, 4], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[1, 4]) / Mathf.Pow(CollisionDistance[1, 4], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[2, 4]) / Mathf.Pow(CollisionDistance[2, 4], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[3, 4]) / Mathf.Pow(CollisionDistance[3, 4], 2))
            + CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[4, 5]) / Mathf.Pow(CollisionDistance[4, 5], 2))
            + CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[4, 6]) / Mathf.Pow(CollisionDistance[4, 6], 2))
            + CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[4, 7]) / Mathf.Pow(CollisionDistance[4, 7], 2))
            + CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[4, 8]) / Mathf.Pow(CollisionDistance[4, 8], 2))
            + CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[4, 9]) / Mathf.Pow(CollisionDistance[4, 9], 2));

        ///////////////////////////////////////////////////////////////////////////////////////////////////
        
        CollisionPitchChange[5] =
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[0, 5]) / Mathf.Pow(CollisionDistance[0, 5], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[1, 5]) / Mathf.Pow(CollisionDistance[1, 5], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[2, 5]) / Mathf.Pow(CollisionDistance[2, 5], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[3, 5]) / Mathf.Pow(CollisionDistance[3, 5], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[4, 5]) / Mathf.Pow(CollisionDistance[4, 5], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[5, 6]) / Mathf.Pow(CollisionDistance[5, 6], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[5, 7]) / Mathf.Pow(CollisionDistance[5, 7], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[5, 8]) / Mathf.Pow(CollisionDistance[5, 8], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[5, 9]) / Mathf.Pow(CollisionDistance[5, 9], 2));

        CollisionRollChange[5] =
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[0, 5]) / Mathf.Pow(CollisionDistance[0, 5], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[1, 5]) / Mathf.Pow(CollisionDistance[1, 5], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[2, 5]) / Mathf.Pow(CollisionDistance[2, 5], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[3, 5]) / Mathf.Pow(CollisionDistance[3, 5], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[4, 5]) / Mathf.Pow(CollisionDistance[4, 5], 2))
            + CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[5, 6]) / Mathf.Pow(CollisionDistance[5, 6], 2))
            + CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[5, 7]) / Mathf.Pow(CollisionDistance[5, 7], 2))
            + CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[5, 8]) / Mathf.Pow(CollisionDistance[5, 8], 2))
            + CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[5, 9]) / Mathf.Pow(CollisionDistance[5, 9], 2));

        ///////////////////////////////////////////////////////////////////////////////////////////////////
        
        CollisionPitchChange[6] =
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[0, 6]) / Mathf.Pow(CollisionDistance[0, 6], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[1, 6]) / Mathf.Pow(CollisionDistance[1, 6], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[2, 6]) / Mathf.Pow(CollisionDistance[2, 6], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[3, 6]) / Mathf.Pow(CollisionDistance[3, 6], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[4, 6]) / Mathf.Pow(CollisionDistance[4, 6], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[5, 6]) / Mathf.Pow(CollisionDistance[5, 6], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[6, 7]) / Mathf.Pow(CollisionDistance[6, 7], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[6, 8]) / Mathf.Pow(CollisionDistance[6, 8], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[6, 9]) / Mathf.Pow(CollisionDistance[6, 9], 2));

        CollisionRollChange[6] =
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[0, 6]) / Mathf.Pow(CollisionDistance[0, 6], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[1, 6]) / Mathf.Pow(CollisionDistance[1, 6], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[2, 6]) / Mathf.Pow(CollisionDistance[2, 6], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[3, 6]) / Mathf.Pow(CollisionDistance[3, 6], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[4, 6]) / Mathf.Pow(CollisionDistance[4, 6], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[5, 6]) / Mathf.Pow(CollisionDistance[5, 6], 2))
            + CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[6, 7]) / Mathf.Pow(CollisionDistance[6, 7], 2))
            + CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[6, 8]) / Mathf.Pow(CollisionDistance[6, 8], 2))
            + CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[6, 9]) / Mathf.Pow(CollisionDistance[6, 9], 2));

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        
        CollisionPitchChange[7] =
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[0, 7]) / Mathf.Pow(CollisionDistance[0, 7], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[1, 7]) / Mathf.Pow(CollisionDistance[1, 7], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[2, 7]) / Mathf.Pow(CollisionDistance[2, 7], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[3, 7]) / Mathf.Pow(CollisionDistance[3, 7], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[4, 7]) / Mathf.Pow(CollisionDistance[4, 7], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[5, 7]) / Mathf.Pow(CollisionDistance[5, 7], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[6, 7]) / Mathf.Pow(CollisionDistance[6, 7], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[7, 8]) / Mathf.Pow(CollisionDistance[7, 8], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[7, 9]) / Mathf.Pow(CollisionDistance[7, 9], 2));

        CollisionRollChange[7] =
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[0, 7]) / Mathf.Pow(CollisionDistance[0, 7], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[1, 7]) / Mathf.Pow(CollisionDistance[1, 7], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[2, 7]) / Mathf.Pow(CollisionDistance[2, 7], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[3, 7]) / Mathf.Pow(CollisionDistance[3, 7], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[4, 7]) / Mathf.Pow(CollisionDistance[4, 7], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[5, 7]) / Mathf.Pow(CollisionDistance[5, 7], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[6, 7]) / Mathf.Pow(CollisionDistance[6, 7], 2))
            + CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[7, 8]) / Mathf.Pow(CollisionDistance[7, 8], 2))
            + CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[7, 9]) / Mathf.Pow(CollisionDistance[7, 9], 2));

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        
        CollisionPitchChange[8] =
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[0, 8]) / Mathf.Pow(CollisionDistance[0, 8], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[1, 8]) / Mathf.Pow(CollisionDistance[1, 8], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[2, 8]) / Mathf.Pow(CollisionDistance[2, 8], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[3, 8]) / Mathf.Pow(CollisionDistance[3, 8], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[4, 8]) / Mathf.Pow(CollisionDistance[4, 8], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[5, 8]) / Mathf.Pow(CollisionDistance[5, 8], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[6, 8]) / Mathf.Pow(CollisionDistance[6, 8], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[7, 8]) / Mathf.Pow(CollisionDistance[7, 8], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[8, 9]) / Mathf.Pow(CollisionDistance[8, 9], 2));

        CollisionRollChange[8] =
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[0, 8]) / Mathf.Pow(CollisionDistance[0, 8], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[1, 8]) / Mathf.Pow(CollisionDistance[1, 8], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[2, 8]) / Mathf.Pow(CollisionDistance[2, 8], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[3, 8]) / Mathf.Pow(CollisionDistance[3, 8], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[4, 8]) / Mathf.Pow(CollisionDistance[4, 8], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[5, 8]) / Mathf.Pow(CollisionDistance[5, 8], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[6, 8]) / Mathf.Pow(CollisionDistance[6, 8], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[7, 8]) / Mathf.Pow(CollisionDistance[7, 8], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[8, 9]) / Mathf.Pow(CollisionDistance[8, 9], 2));

        ///////////////////////////////////////////////////////////////////////////////////////////////////

        CollisionPitchChange[9] =
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[0, 9]) / Mathf.Pow(CollisionDistance[0, 9], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[1, 9]) / Mathf.Pow(CollisionDistance[1, 9], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[2, 9]) / Mathf.Pow(CollisionDistance[2, 9], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[3, 9]) / Mathf.Pow(CollisionDistance[3, 9], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[4, 9]) / Mathf.Pow(CollisionDistance[4, 9], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[5, 9]) / Mathf.Pow(CollisionDistance[5, 9], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[6, 9]) / Mathf.Pow(CollisionDistance[6, 9], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[7, 9]) / Mathf.Pow(CollisionDistance[7, 9], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[8, 9] - (AgentCollHeading[8] - AgentCollHeading[9]))) / Mathf.Pow(CollisionDistance[8, 9], 2));

        CollisionRollChange[9] =
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[0, 9]) / Mathf.Pow(CollisionDistance[0, 9], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[1, 9]) / Mathf.Pow(CollisionDistance[1, 9], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[2, 9]) / Mathf.Pow(CollisionDistance[2, 9], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[3, 9]) / Mathf.Pow(CollisionDistance[3, 9], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[4, 9]) / Mathf.Pow(CollisionDistance[4, 9], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[5, 9]) / Mathf.Pow(CollisionDistance[5, 9], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[6, 9]) / Mathf.Pow(CollisionDistance[6, 9], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[7, 9]) / Mathf.Pow(CollisionDistance[7, 9], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[8, 9] - (AgentCollHeading[8] - AgentCollHeading[9]))) / Mathf.Pow(CollisionDistance[8, 9], 2));

        /*

        ////////////////

        CollisionPitchChange[4] = (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[4, 5]) / Mathf.Pow(CollisionDistance[4, 5], 2)) 
            + (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[4, 6]) / Mathf.Pow(CollisionDistance[4, 6], 2));

        CollisionRollChange[4] = -(Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[4, 5]) / Mathf.Pow(CollisionDistance[4, 5], 2))
            + (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[4, 6]) / Mathf.Pow(CollisionDistance[4, 6], 2));

        ////////////////

        CollisionPitchChange[5] = -(Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[4, 5]) / Mathf.Pow(CollisionDistance[4, 5], 2))
            + (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[5, 6]) / Mathf.Pow(CollisionDistance[5, 6], 2));


        CollisionRollChange[5] = (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[4, 5]) / Mathf.Pow(CollisionDistance[4, 5], 2))
            + (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[5, 6]) / Mathf.Pow(CollisionDistance[5, 6], 2));

        ////////////////

        CollisionPitchChange[6] = -(Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[4, 6]) / Mathf.Pow(CollisionDistance[4, 6], 2))
            - (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[5, 6]) / Mathf.Pow(CollisionDistance[5, 6], 2));

        CollisionRollChange[6] = -(Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[4, 6]) / Mathf.Pow(CollisionDistance[4, 6], 2))
            - (Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[5, 6]) / Mathf.Pow(CollisionDistance[5, 6], 2));

        ///////////////

        */

        //Debug.Log(CollisionDistance[4, 5] + "    " + CollisionPitchChange[4] + "   " + CollisionPitchChange[5]);
    }
}
