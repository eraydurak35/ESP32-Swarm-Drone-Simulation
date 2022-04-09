using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmManagerScript : MonoBehaviour
{

    AgentControl[] AgentControl = new AgentControl[14];
    [SerializeField] GameObject[] Agents = new GameObject[14];
    public bool[] ActiveAgents = new bool[14];

    public GameObject CheckPointObject;

    public GameObject[] Waypoints = new GameObject[3];
    public int ReachedCheckPoints = 0;
    GameObject checkpoint;
    private bool newCheckpoint = true;
    //public float checkpointX;
    //public float checkpointZ;
    //public float checkpointY;


    [HideInInspector]
    public float[] Agent_X = new float[14];
    [HideInInspector]
    public float[] Agent_Y = new float[14];
    [HideInInspector]
    public float[] Agent_Z = new float[14];
    [HideInInspector]
    public float[] Agent_Heading = new float[14];
    [HideInInspector]
    public float[] AgentProximity = new float[14];
    [HideInInspector]
    public float[] AgentTargetHeading = new float[14];
    [HideInInspector]
    public float[] AgentBatteryStatus = new float[14];


    //public bool FormArrowHead = false;
    //public bool FormTriangle = false;
    //public bool FormTrianglePrizm = false;
    
    


    public bool TakeOff = false;
    public bool Mission = false;
    float[,] CollisionDistance = new float[13, 14];
    float[,] PrevCollisionDistance = new float[13, 14];
    float[,] CollisionDistanceChangeRate = new float[13, 14];
    [HideInInspector]
    public float[] CollisionAltChange = new float[14];
    [HideInInspector]
    public float[] CollisionPitchChange = new float[14];
    [HideInInspector]
    public float[] CollisionRollChange = new float[14];
    [HideInInspector]
    public float[,] CollisionHeading = new float[13, 14];
    public float CollisionAvoidCoeff = 1f;
    public float NoAvoidUntil = 1f;
    [HideInInspector]
    public float[] AgentCollHeading = new float[14];
    //public float angsq = 60f;



    public float takeOffHight = 10f;


    [HideInInspector]
    public bool[] AgentTakeOffSucces = new bool[14];
    public bool takeOffSucces = false;
    [HideInInspector]
    public bool[] AgentLandedSucces = new bool[14];
    public bool LandedSucces = false;





    public float formHeadDegSn = 10f;
    [HideInInspector]
    public float FormationHeading = 0f;
    public float FormationHeadingSet = 0f;





    public float formationTargetMiddleX,formationTargetMiddleZ,formationTargetMiddleHeight;
    public float formationDelay = 5f;
    public float sampleTime = 0.005f;
    public float swarmSpeed = 1;
    float swarmXspeed = 0f, swarmZspeed = 0f;
    float diffX, diffZ;
    bool NavMissionSucces;

    /// SQUARE PRIZM FORMATION
    public float squarePrizmEdge = 3f, squarePrizmHeight = 3f;
    [HideInInspector]
    public bool[] AgentFormSquarePrizmSucces = new bool[14];
    public bool FormSquarePrizmSucces = false;
    public bool FormSquarePrizm = false;
    [HideInInspector]
    public int SquarePrizmAgentRequired = 8;
    [HideInInspector]
    public bool[] SquarePrizmInFormation = new bool[14];
    [HideInInspector]
    public int[] SquarePrizmAssignedLocationNumber = new int[14];
    [HideInInspector]
    public bool[] SquarePrizmPositionFull = new bool[8];

    /// PYRAMID SQUARE PRIZM FORMATION
    public float pyramidSquarePrizmEdge = 3f, pyramidSquarePrizmHeight = 3f;
    [HideInInspector]
    public bool[] AgentFormPyramidSquarePrizmSucces = new bool[14];
    public bool FormPyramidSquarePrizmSucces = false;
    public bool FormPyramidSquarePrizm = false;
    [HideInInspector]
    public int PyramidSquarePrizmAgentRequired = 5;
    [HideInInspector]
    public bool[] PyramidSquarePrizmInFormation = new bool[14];
    [HideInInspector]
    public int[] PyramidSquarePrizmAssignedLocationNumber = new int[14];
    [HideInInspector]
    public bool[] PyramidSquarePrizmPositionFull = new bool[5];

    /// TRIANGLE FORMATION
    public float triangleEdge = 3f;
    [HideInInspector]
    public bool[] AgentFormTriangleSucces = new bool[14];
    public bool FormTriangleSucces = false;
    public bool FormTriangle = false;
    [HideInInspector]
    public int TriangleAgentRequired = 3;
    [HideInInspector]
    public bool[] TriangleInFormation = new bool[14];
    [HideInInspector]
    public int[] TriangleAssignedLocationNumber = new int[14];
    [HideInInspector]
    public bool[] TrianglePositionFull = new bool[3];

    /// TRIANGLE PRIZM FORMATION
    public float trianglePrizmEdge = 3f, trianglePrizmHeight = 3f;
    [HideInInspector]
    public bool[] AgentFormTrianglePrizmSucces = new bool[14];
    public bool FormTrianglePrizmSucces = false;
    public bool FormTrianglePrizm = false;
    [HideInInspector]
    public int TrianglePrizmAgentRequired = 6;
    [HideInInspector]
    public bool[] TrianglePrizmInFormation = new bool[14];
    [HideInInspector]
    public int[] TrianglePrizmAssignedLocationNumber = new int[14];
    [HideInInspector]
    public bool[] TrianglePrizmPositionFull = new bool[6];

    /// SQUARE FORMATION
    public float squareEdge = 3f;
    [HideInInspector]
    public bool[] AgentFormSquareSucces = new bool[14];
    public bool FormSquareSucces = false;
    public bool FormSquare = false;
    [HideInInspector]
    public int SquareAgentRequired = 4;
    [HideInInspector]
    public bool[] SquareInFormation = new bool[14];
    [HideInInspector]
    public int[] SquareAssignedLocationNumber = new int[14];
    [HideInInspector]
    public bool[] SquarePositionFull = new bool[4];


    /// HEXAGON PRIZM FORMATION
    public float hexagonPrizmEdge = 3f, hexagonPrizmHeight = 3f;
    [HideInInspector]
    public bool[] AgentFormHexagonPrizmSucces = new bool[14];
    public bool FormHexagonPrizmSucces = false;
    public bool FormHexagonPrizm = false;
    [HideInInspector]
    public int HexagonPrizmAgentRequired = 12;
    //[HideInInspector]
    public bool[] HexagonPrizmInFormation = new bool[14];
    //[HideInInspector]
    public int[] HexagonPrizmAssignedLocationNumber = new int[14];
    //[HideInInspector]
    public bool[] HexagonPrizmPositionFull = new bool[12];

    [HideInInspector]
    public int takeoffcounter;
    [HideInInspector]
    public int squarePrizmCounter;
    [HideInInspector]
    public int pyramidSquarePrizmCounter;
    [HideInInspector]
    public int triangleCounter;
    [HideInInspector]
    public int trianglePrizmCounter;
    [HideInInspector]
    public int squareCounter;
    [HideInInspector]
    public int hexagonPrizmCounter;

    private void Awake()
    {

        for (int i = 0; i < 14; i++)
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

        getAgentData();
        Formation3DMission();
        AvoidCollision();
        //SwarmNavigate();

        if (FormationHeadingSet > FormationHeading)
        {
            FormationHeading += formHeadDegSn * sampleTime;
        }
        else if (FormationHeadingSet < FormationHeading)
        {
            FormationHeading -= formHeadDegSn * sampleTime;
        }


        /*

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
        */
        
    }

    void Formation3DMission()
    {
        if (!takeOffSucces)
        {
            TakeOff = true;
        }
        else
        {
            TakeOff = false;
            
            if (!FormHexagonPrizmSucces && !FormHexagonPrizm)
            {
                FormHexagonPrizm = true;

                //formationTargetMiddleX = checkpointX;
                //formationTargetMiddleZ = checkpointZ;
                //formationTargetMiddleHeight = checkpointY;
                //Debug.Log("1");
            }
            else if (FormHexagonPrizmSucces && formationDelay > 0f)
            {
                formationDelay -= Time.deltaTime;
                //FormPyramidSquarePrizm = false;
                //Debug.Log("2");
            }
            else if (FormHexagonPrizmSucces && formationDelay <= 0 && !NavMissionSucces)
            {
                SwarmNavigate();
                //Debug.Log("3");
            }

            /*
            else if (FormSquarePrizmSucces && formationDelay > 0f)
            {
                formationDelay -= Time.deltaTime;
                FormSquarePrizm = false;
                Debug.Log("4");
            }*/
            
            
        }
    }

    void SwarmNavigate()
    {
        diffX = Mathf.Abs(Mathf.Abs(Waypoints[ReachedCheckPoints].transform.position.x) - Mathf.Abs(formationTargetMiddleX));
        diffZ = Mathf.Abs(Mathf.Abs(Waypoints[ReachedCheckPoints].transform.position.z) - Mathf.Abs(formationTargetMiddleZ));

        for (int k = 0; k < 14; k++)
        {
            AgentProximity[k] = Mathf.Sqrt(Mathf.Pow((Waypoints[ReachedCheckPoints].transform.position.x - Agent_X[k]), 2) 
                + Mathf.Pow((Waypoints[ReachedCheckPoints].transform.position.y - Agent_Y[k]), 2) 
                + Mathf.Pow((Waypoints[ReachedCheckPoints].transform.position.z - Agent_Z[k]), 2));

            if (AgentProximity[k] < 2f)
            {
                ReachedCheckPoints++;
                if (ReachedCheckPoints == 3)
                {
                    NavMissionSucces = true;
                    ReachedCheckPoints--;
                }
            }
        }

        float ratio1 = diffX / diffZ;
        float ratio2 = diffZ / diffX;

        if (ratio1 > 89f) ratio1 = 89f;
        if (ratio2 > 89f) ratio2 = 89f;

        swarmXspeed = (swarmSpeed * sampleTime) * Mathf.Cos(Mathf.Deg2Rad * (Mathf.Tan(ratio1)));
        swarmZspeed = (swarmSpeed * sampleTime) * Mathf.Cos(Mathf.Deg2Rad * (Mathf.Tan(ratio2)));

        if (Waypoints[ReachedCheckPoints].transform.position.x > formationTargetMiddleX)
        {
            formationTargetMiddleX += swarmXspeed;
        }
        else if (Waypoints[ReachedCheckPoints].transform.position.x < formationTargetMiddleX)
        {
            formationTargetMiddleX -= swarmXspeed;
        }
        if (Waypoints[ReachedCheckPoints].transform.position.z > formationTargetMiddleZ)
        {
            formationTargetMiddleZ += swarmZspeed;
        }
        else if (Waypoints[ReachedCheckPoints].transform.position.z < formationTargetMiddleZ)
        {
            formationTargetMiddleZ -= swarmZspeed;
        }
        if (Waypoints[ReachedCheckPoints].transform.position.y > formationTargetMiddleHeight)
        {
            formationTargetMiddleHeight += (swarmSpeed * sampleTime);
        }
        else if (Waypoints[ReachedCheckPoints].transform.position.y < formationTargetMiddleHeight)
        {
            formationTargetMiddleHeight -= (swarmSpeed * sampleTime);
        }
        

    }

    void getAgentData()
    {
        for (int j = 0; j < 14; j++)
        {
            Agent_X[j] = Agents[j].transform.position.x;
            Agent_Y[j] = Agents[j].transform.position.y;
            Agent_Z[j] = Agents[j].transform.position.z;
            Agent_Heading[j] = AgentControl[j].GetComponent<AgentControl>().AgentHeading;

            AgentBatteryStatus[j] = Agents[j].GetComponent<AgentBattery>().RemainBattPercent;

            /*
            if (AgentBatteryStatus[j] < 30f)
            {
                ActiveAgents[j] = false;
            }
            else ActiveAgents[j] = true;
            */
        }
    }

    void AvoidCollision()
    {
        for (int n = 0; n < 13; n++)
        {
            for (int k = n + 1; k < 14; k++)
            {

                CollisionDistance[n, k] = Mathf.Sqrt(Mathf.Pow((Agent_X[n] - Agent_X[k]), 2) + Mathf.Pow((Agent_Y[n] - Agent_Y[k]), 2) + Mathf.Pow((Agent_Z[n] - Agent_Z[k]), 2));
                CollisionDistanceChangeRate[n, k] = (PrevCollisionDistance[n, k] - CollisionDistance[n, k]) * 200f;
                PrevCollisionDistance[n, k] = CollisionDistance[n, k];
                CollisionHeading[n, k] = (Mathf.Atan2((Agent_X[k] - Agent_X[n]), (Agent_Z[n] - Agent_Z[k])) * Mathf.Rad2Deg) * -1f;

                if (CollisionHeading[n, k] < 0f) CollisionHeading[n, k] += 360f;
                if (Agent_Heading[n] < 0f) AgentCollHeading[n] = Agent_Heading[n] + 360f;
                CollisionHeading[n, k] = (CollisionHeading[n, k] - Agent_Heading[n]) % 360f;

                if (CollisionDistance[n, k] > NoAvoidUntil) CollisionDistance[n, k] = 200f; 
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
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[0, 9]) / Mathf.Pow(CollisionDistance[0, 9], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[0, 10]) / Mathf.Pow(CollisionDistance[0, 10], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[0, 11]) / Mathf.Pow(CollisionDistance[0, 11], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[0, 12]) / Mathf.Pow(CollisionDistance[0, 12], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[0, 13]) / Mathf.Pow(CollisionDistance[0, 13], 2));////////

        CollisionRollChange[0] =
              CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[0, 1]) / Mathf.Pow(CollisionDistance[0, 1], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[0, 2]) / Mathf.Pow(CollisionDistance[0, 2], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[0, 3]) / Mathf.Pow(CollisionDistance[0, 3], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[0, 4]) / Mathf.Pow(CollisionDistance[0, 4], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[0, 5]) / Mathf.Pow(CollisionDistance[0, 5], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[0, 6]) / Mathf.Pow(CollisionDistance[0, 6], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[0, 7]) / Mathf.Pow(CollisionDistance[0, 7], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[0, 8]) / Mathf.Pow(CollisionDistance[0, 8], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[0, 9]) / Mathf.Pow(CollisionDistance[0, 9], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[0, 10]) / Mathf.Pow(CollisionDistance[0, 10], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[0, 11]) / Mathf.Pow(CollisionDistance[0, 11], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[0, 12]) / Mathf.Pow(CollisionDistance[0, 12], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[0, 13]) / Mathf.Pow(CollisionDistance[0, 13], 2));//////



        ///////////////////////////////////////////////////////////////////////////////////////////////////

        CollisionPitchChange[1] =
            -CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[0, 1] - (AgentCollHeading[0] - AgentCollHeading[1]))) / Mathf.Pow(CollisionDistance[0, 1], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[1, 2]) / Mathf.Pow(CollisionDistance[1, 2], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[1, 3]) / Mathf.Pow(CollisionDistance[1, 3], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[1, 4]) / Mathf.Pow(CollisionDistance[1, 4], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[1, 5]) / Mathf.Pow(CollisionDistance[1, 5], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[1, 6]) / Mathf.Pow(CollisionDistance[1, 6], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[1, 7]) / Mathf.Pow(CollisionDistance[1, 7], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[1, 8]) / Mathf.Pow(CollisionDistance[1, 8], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[1, 9]) / Mathf.Pow(CollisionDistance[1, 9], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[1, 10]) / Mathf.Pow(CollisionDistance[1, 10], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[1, 11]) / Mathf.Pow(CollisionDistance[1, 11], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[1, 12]) / Mathf.Pow(CollisionDistance[1, 12], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[1, 13]) / Mathf.Pow(CollisionDistance[1, 13], 2));//////

        CollisionRollChange[1] =
            -CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[0, 1] - (AgentCollHeading[0] - AgentCollHeading[1]))) / Mathf.Pow(CollisionDistance[0, 1], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[1, 2]) / Mathf.Pow(CollisionDistance[1, 2], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[1, 3]) / Mathf.Pow(CollisionDistance[1, 3], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[1, 4]) / Mathf.Pow(CollisionDistance[1, 4], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[1, 5]) / Mathf.Pow(CollisionDistance[1, 5], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[1, 6]) / Mathf.Pow(CollisionDistance[1, 6], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[1, 7]) / Mathf.Pow(CollisionDistance[1, 7], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[1, 8]) / Mathf.Pow(CollisionDistance[1, 8], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[1, 9]) / Mathf.Pow(CollisionDistance[1, 9], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[1, 10]) / Mathf.Pow(CollisionDistance[1, 10], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[1, 11]) / Mathf.Pow(CollisionDistance[1, 11], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[1, 12]) / Mathf.Pow(CollisionDistance[1, 12], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[1, 13]) / Mathf.Pow(CollisionDistance[1, 13], 2));///////

        ///////////////////////////////////////////////////////////////////////////////////////////////////

        CollisionPitchChange[2] =
            -CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[0, 2] - (AgentCollHeading[0] - AgentCollHeading[2]))) / Mathf.Pow(CollisionDistance[0, 2], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[1, 2] - (AgentCollHeading[1] - AgentCollHeading[2]))) / Mathf.Pow(CollisionDistance[1, 2], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[2, 3]) / Mathf.Pow(CollisionDistance[2, 3], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[2, 4]) / Mathf.Pow(CollisionDistance[2, 4], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[2, 5]) / Mathf.Pow(CollisionDistance[2, 5], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[2, 6]) / Mathf.Pow(CollisionDistance[2, 6], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[2, 7]) / Mathf.Pow(CollisionDistance[2, 7], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[2, 8]) / Mathf.Pow(CollisionDistance[2, 8], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[2, 9]) / Mathf.Pow(CollisionDistance[2, 9], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[2, 10]) / Mathf.Pow(CollisionDistance[2, 10], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[2, 11]) / Mathf.Pow(CollisionDistance[2, 11], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[2, 12]) / Mathf.Pow(CollisionDistance[2, 12], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[2, 13]) / Mathf.Pow(CollisionDistance[2, 13], 2));//////

        CollisionRollChange[2] =
            -CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[0, 2] - (AgentCollHeading[0] - AgentCollHeading[2]))) / Mathf.Pow(CollisionDistance[0, 2], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[1, 2] - (AgentCollHeading[1] - AgentCollHeading[2]))) / Mathf.Pow(CollisionDistance[1, 2], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[2, 3]) / Mathf.Pow(CollisionDistance[2, 3], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[2, 4]) / Mathf.Pow(CollisionDistance[2, 4], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[2, 5]) / Mathf.Pow(CollisionDistance[2, 5], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[2, 6]) / Mathf.Pow(CollisionDistance[2, 6], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[2, 7]) / Mathf.Pow(CollisionDistance[2, 7], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[2, 8]) / Mathf.Pow(CollisionDistance[2, 8], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[2, 9]) / Mathf.Pow(CollisionDistance[2, 9], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[2, 10]) / Mathf.Pow(CollisionDistance[2, 10], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[2, 11]) / Mathf.Pow(CollisionDistance[2, 11], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[2, 12]) / Mathf.Pow(CollisionDistance[2, 12], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[2, 13]) / Mathf.Pow(CollisionDistance[2, 13], 2));/////

        /////////////////////////////////////////////////////////////////////////////////////////////////

        CollisionPitchChange[3] =
            -CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[0, 3] - (AgentCollHeading[0] - AgentCollHeading[3]))) / Mathf.Pow(CollisionDistance[0, 3], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[1, 3] - (AgentCollHeading[1] - AgentCollHeading[3]))) / Mathf.Pow(CollisionDistance[1, 3], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[2, 3] - (AgentCollHeading[2] - AgentCollHeading[3]))) / Mathf.Pow(CollisionDistance[2, 3], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[3, 4]) / Mathf.Pow(CollisionDistance[3, 4], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[3, 5]) / Mathf.Pow(CollisionDistance[3, 5], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[3, 6]) / Mathf.Pow(CollisionDistance[3, 6], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[3, 7]) / Mathf.Pow(CollisionDistance[3, 7], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[3, 8]) / Mathf.Pow(CollisionDistance[3, 8], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[3, 9]) / Mathf.Pow(CollisionDistance[3, 9], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[3, 10]) / Mathf.Pow(CollisionDistance[3, 10], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[3, 11]) / Mathf.Pow(CollisionDistance[3, 11], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[3, 12]) / Mathf.Pow(CollisionDistance[3, 12], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[3, 13]) / Mathf.Pow(CollisionDistance[3, 13], 2));/////

        CollisionRollChange[3] =
            -CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[0, 3] - (AgentCollHeading[0] - AgentCollHeading[3]))) / Mathf.Pow(CollisionDistance[0, 3], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[1, 3] - (AgentCollHeading[1] - AgentCollHeading[3]))) / Mathf.Pow(CollisionDistance[1, 3], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[2, 3] - (AgentCollHeading[2] - AgentCollHeading[3]))) / Mathf.Pow(CollisionDistance[2, 3], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[3, 4]) / Mathf.Pow(CollisionDistance[3, 4], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[3, 5]) / Mathf.Pow(CollisionDistance[3, 5], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[3, 6]) / Mathf.Pow(CollisionDistance[3, 6], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[3, 7]) / Mathf.Pow(CollisionDistance[3, 7], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[3, 8]) / Mathf.Pow(CollisionDistance[3, 8], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[3, 9]) / Mathf.Pow(CollisionDistance[3, 9], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[3, 10]) / Mathf.Pow(CollisionDistance[3, 10], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[3, 11]) / Mathf.Pow(CollisionDistance[3, 11], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[3, 12]) / Mathf.Pow(CollisionDistance[3, 12], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[3, 13]) / Mathf.Pow(CollisionDistance[3, 13], 2));/////

        ///////////////////////////////////////////////////////////////////////////////////////////////////

        CollisionPitchChange[4] =
            -CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[0, 4] - (AgentCollHeading[0] - AgentCollHeading[4]))) / Mathf.Pow(CollisionDistance[0, 4], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[1, 4] - (AgentCollHeading[1] - AgentCollHeading[4]))) / Mathf.Pow(CollisionDistance[1, 4], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[2, 4] - (AgentCollHeading[2] - AgentCollHeading[4]))) / Mathf.Pow(CollisionDistance[2, 4], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[3, 4] - (AgentCollHeading[3] - AgentCollHeading[4]))) / Mathf.Pow(CollisionDistance[3, 4], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[4, 5]) / Mathf.Pow(CollisionDistance[4, 5], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[4, 6]) / Mathf.Pow(CollisionDistance[4, 6], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[4, 7]) / Mathf.Pow(CollisionDistance[4, 7], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[4, 8]) / Mathf.Pow(CollisionDistance[4, 8], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[4, 9]) / Mathf.Pow(CollisionDistance[4, 9], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[4, 10]) / Mathf.Pow(CollisionDistance[4, 10], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[4, 11]) / Mathf.Pow(CollisionDistance[4, 11], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[4, 12]) / Mathf.Pow(CollisionDistance[4, 12], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[4, 13]) / Mathf.Pow(CollisionDistance[4, 13], 2));/////


        CollisionRollChange[4] =
            -CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[0, 4] - (AgentCollHeading[0] - AgentCollHeading[4]))) / Mathf.Pow(CollisionDistance[0, 4], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[1, 4] - (AgentCollHeading[1] - AgentCollHeading[4]))) / Mathf.Pow(CollisionDistance[1, 4], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[2, 4] - (AgentCollHeading[2] - AgentCollHeading[4]))) / Mathf.Pow(CollisionDistance[2, 4], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[3, 4] - (AgentCollHeading[3] - AgentCollHeading[4]))) / Mathf.Pow(CollisionDistance[3, 4], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[4, 5]) / Mathf.Pow(CollisionDistance[4, 5], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[4, 6]) / Mathf.Pow(CollisionDistance[4, 6], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[4, 7]) / Mathf.Pow(CollisionDistance[4, 7], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[4, 8]) / Mathf.Pow(CollisionDistance[4, 8], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[4, 9]) / Mathf.Pow(CollisionDistance[4, 9], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[4, 10]) / Mathf.Pow(CollisionDistance[4, 10], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[4, 11]) / Mathf.Pow(CollisionDistance[4, 11], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[4, 12]) / Mathf.Pow(CollisionDistance[4, 12], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[4, 13]) / Mathf.Pow(CollisionDistance[4, 13], 2));/////

        ///////////////////////////////////////////////////////////////////////////////////////////////////

        CollisionPitchChange[5] =
            -CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[0, 5] - (AgentCollHeading[0] - AgentCollHeading[5]))) / Mathf.Pow(CollisionDistance[0, 5], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[1, 5] - (AgentCollHeading[1] - AgentCollHeading[5]))) / Mathf.Pow(CollisionDistance[1, 5], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[2, 5] - (AgentCollHeading[2] - AgentCollHeading[5]))) / Mathf.Pow(CollisionDistance[2, 5], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[3, 5] - (AgentCollHeading[3] - AgentCollHeading[5]))) / Mathf.Pow(CollisionDistance[3, 5], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[4, 5] - (AgentCollHeading[4] - AgentCollHeading[5]))) / Mathf.Pow(CollisionDistance[4, 5], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[5, 6]) / Mathf.Pow(CollisionDistance[5, 6], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[5, 7]) / Mathf.Pow(CollisionDistance[5, 7], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[5, 8]) / Mathf.Pow(CollisionDistance[5, 8], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[5, 9]) / Mathf.Pow(CollisionDistance[5, 9], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[5, 10]) / Mathf.Pow(CollisionDistance[5, 10], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[5, 11]) / Mathf.Pow(CollisionDistance[5, 11], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[5, 12]) / Mathf.Pow(CollisionDistance[5, 12], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[5, 13]) / Mathf.Pow(CollisionDistance[5, 13], 2));/////

        CollisionRollChange[5] =
            -CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[0, 5] - (AgentCollHeading[0] - AgentCollHeading[5]))) / Mathf.Pow(CollisionDistance[0, 5], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[1, 5] - (AgentCollHeading[1] - AgentCollHeading[5]))) / Mathf.Pow(CollisionDistance[1, 5], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[2, 5] - (AgentCollHeading[2] - AgentCollHeading[5]))) / Mathf.Pow(CollisionDistance[2, 5], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[3, 5] - (AgentCollHeading[3] - AgentCollHeading[5]))) / Mathf.Pow(CollisionDistance[3, 5], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[4, 5] - (AgentCollHeading[4] - AgentCollHeading[5]))) / Mathf.Pow(CollisionDistance[4, 5], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[5, 6]) / Mathf.Pow(CollisionDistance[5, 6], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[5, 7]) / Mathf.Pow(CollisionDistance[5, 7], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[5, 8]) / Mathf.Pow(CollisionDistance[5, 8], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[5, 9]) / Mathf.Pow(CollisionDistance[5, 9], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[5, 10]) / Mathf.Pow(CollisionDistance[5, 10], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[5, 11]) / Mathf.Pow(CollisionDistance[5, 11], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[5, 12]) / Mathf.Pow(CollisionDistance[5, 12], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[5, 13]) / Mathf.Pow(CollisionDistance[5, 13], 2));/////

        ///////////////////////////////////////////////////////////////////////////////////////////////////

        CollisionPitchChange[6] =
            -CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[0, 6] - (AgentCollHeading[0] - AgentCollHeading[6]))) / Mathf.Pow(CollisionDistance[0, 6], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[1, 6] - (AgentCollHeading[1] - AgentCollHeading[6]))) / Mathf.Pow(CollisionDistance[1, 6], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[2, 6] - (AgentCollHeading[2] - AgentCollHeading[6]))) / Mathf.Pow(CollisionDistance[2, 6], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[3, 6] - (AgentCollHeading[3] - AgentCollHeading[6]))) / Mathf.Pow(CollisionDistance[3, 6], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[4, 6] - (AgentCollHeading[4] - AgentCollHeading[6]))) / Mathf.Pow(CollisionDistance[4, 6], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[5, 6] - (AgentCollHeading[5] - AgentCollHeading[6]))) / Mathf.Pow(CollisionDistance[5, 6], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[6, 7]) / Mathf.Pow(CollisionDistance[6, 7], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[6, 8]) / Mathf.Pow(CollisionDistance[6, 8], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[6, 9]) / Mathf.Pow(CollisionDistance[6, 9], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[6, 10]) / Mathf.Pow(CollisionDistance[6, 10], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[6, 11]) / Mathf.Pow(CollisionDistance[6, 11], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[6, 12]) / Mathf.Pow(CollisionDistance[6, 12], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[6, 13]) / Mathf.Pow(CollisionDistance[6, 13], 2));/////

        CollisionRollChange[6] =
            -CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[0, 6] - (AgentCollHeading[0] - AgentCollHeading[6]))) / Mathf.Pow(CollisionDistance[0, 6], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[1, 6] - (AgentCollHeading[1] - AgentCollHeading[6]))) / Mathf.Pow(CollisionDistance[1, 6], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[2, 6] - (AgentCollHeading[2] - AgentCollHeading[6]))) / Mathf.Pow(CollisionDistance[2, 6], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[3, 6] - (AgentCollHeading[3] - AgentCollHeading[6]))) / Mathf.Pow(CollisionDistance[3, 6], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[4, 6] - (AgentCollHeading[4] - AgentCollHeading[6]))) / Mathf.Pow(CollisionDistance[4, 6], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[5, 6] - (AgentCollHeading[5] - AgentCollHeading[6]))) / Mathf.Pow(CollisionDistance[5, 6], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[6, 7]) / Mathf.Pow(CollisionDistance[6, 7], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[6, 8]) / Mathf.Pow(CollisionDistance[6, 8], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[6, 9]) / Mathf.Pow(CollisionDistance[6, 9], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[6, 10]) / Mathf.Pow(CollisionDistance[6, 10], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[6, 11]) / Mathf.Pow(CollisionDistance[6, 11], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[6, 12]) / Mathf.Pow(CollisionDistance[6, 12], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[6, 13]) / Mathf.Pow(CollisionDistance[6, 13], 2));/////

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        CollisionPitchChange[7] =
            -CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[0, 7] - (AgentCollHeading[0] - AgentCollHeading[7]))) / Mathf.Pow(CollisionDistance[0, 7], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[1, 7] - (AgentCollHeading[1] - AgentCollHeading[7]))) / Mathf.Pow(CollisionDistance[1, 7], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[2, 7] - (AgentCollHeading[2] - AgentCollHeading[7]))) / Mathf.Pow(CollisionDistance[2, 7], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[3, 7] - (AgentCollHeading[3] - AgentCollHeading[7]))) / Mathf.Pow(CollisionDistance[3, 7], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[4, 7] - (AgentCollHeading[4] - AgentCollHeading[7]))) / Mathf.Pow(CollisionDistance[4, 7], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[5, 7] - (AgentCollHeading[5] - AgentCollHeading[7]))) / Mathf.Pow(CollisionDistance[5, 7], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[6, 7] - (AgentCollHeading[6] - AgentCollHeading[7]))) / Mathf.Pow(CollisionDistance[6, 7], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[7, 8]) / Mathf.Pow(CollisionDistance[7, 8], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[7, 9]) / Mathf.Pow(CollisionDistance[7, 9], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[7, 10]) / Mathf.Pow(CollisionDistance[7, 10], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[7, 11]) / Mathf.Pow(CollisionDistance[7, 11], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[7, 12]) / Mathf.Pow(CollisionDistance[7, 12], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[7, 13]) / Mathf.Pow(CollisionDistance[7, 13], 2));////

        CollisionRollChange[7] =
            -CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[0, 7] - (AgentCollHeading[0] - AgentCollHeading[7]))) / Mathf.Pow(CollisionDistance[0, 7], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[1, 7] - (AgentCollHeading[1] - AgentCollHeading[7]))) / Mathf.Pow(CollisionDistance[1, 7], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[2, 7] - (AgentCollHeading[2] - AgentCollHeading[7]))) / Mathf.Pow(CollisionDistance[2, 7], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[3, 7] - (AgentCollHeading[3] - AgentCollHeading[7]))) / Mathf.Pow(CollisionDistance[3, 7], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[4, 7] - (AgentCollHeading[4] - AgentCollHeading[7]))) / Mathf.Pow(CollisionDistance[4, 7], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[5, 7] - (AgentCollHeading[5] - AgentCollHeading[7]))) / Mathf.Pow(CollisionDistance[5, 7], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[6, 7] - (AgentCollHeading[6] - AgentCollHeading[7]))) / Mathf.Pow(CollisionDistance[6, 7], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[7, 8]) / Mathf.Pow(CollisionDistance[7, 8], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[7, 9]) / Mathf.Pow(CollisionDistance[7, 9], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[7, 10]) / Mathf.Pow(CollisionDistance[7, 10], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[7, 11]) / Mathf.Pow(CollisionDistance[7, 11], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[7, 12]) / Mathf.Pow(CollisionDistance[7, 12], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[7, 13]) / Mathf.Pow(CollisionDistance[7, 13], 2));////

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        CollisionPitchChange[8] =
            -CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[0, 8] - (AgentCollHeading[0] - AgentCollHeading[8]))) / Mathf.Pow(CollisionDistance[0, 8], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[1, 8] - (AgentCollHeading[1] - AgentCollHeading[8]))) / Mathf.Pow(CollisionDistance[1, 8], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[2, 8] - (AgentCollHeading[2] - AgentCollHeading[8]))) / Mathf.Pow(CollisionDistance[2, 8], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[3, 8] - (AgentCollHeading[3] - AgentCollHeading[8]))) / Mathf.Pow(CollisionDistance[3, 8], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[4, 8] - (AgentCollHeading[4] - AgentCollHeading[8]))) / Mathf.Pow(CollisionDistance[4, 8], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[5, 8] - (AgentCollHeading[5] - AgentCollHeading[8]))) / Mathf.Pow(CollisionDistance[5, 8], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[6, 8] - (AgentCollHeading[6] - AgentCollHeading[8]))) / Mathf.Pow(CollisionDistance[6, 8], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[7, 8] - (AgentCollHeading[7] - AgentCollHeading[8]))) / Mathf.Pow(CollisionDistance[7, 8], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[8, 9]) / Mathf.Pow(CollisionDistance[8, 9], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[8, 10]) / Mathf.Pow(CollisionDistance[8, 10], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[8, 11]) / Mathf.Pow(CollisionDistance[8, 11], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[8, 12]) / Mathf.Pow(CollisionDistance[8, 12], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[8, 13]) / Mathf.Pow(CollisionDistance[8, 13], 2));/////

        CollisionRollChange[8] =
            -CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[0, 8] - (AgentCollHeading[0] - AgentCollHeading[8]))) / Mathf.Pow(CollisionDistance[0, 8], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[1, 8] - (AgentCollHeading[1] - AgentCollHeading[8]))) / Mathf.Pow(CollisionDistance[1, 8], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[2, 8] - (AgentCollHeading[2] - AgentCollHeading[8]))) / Mathf.Pow(CollisionDistance[2, 8], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[3, 8] - (AgentCollHeading[3] - AgentCollHeading[8]))) / Mathf.Pow(CollisionDistance[3, 8], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[4, 8] - (AgentCollHeading[4] - AgentCollHeading[8]))) / Mathf.Pow(CollisionDistance[4, 8], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[5, 8] - (AgentCollHeading[5] - AgentCollHeading[8]))) / Mathf.Pow(CollisionDistance[5, 8], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[6, 8] - (AgentCollHeading[6] - AgentCollHeading[8]))) / Mathf.Pow(CollisionDistance[6, 8], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[7, 8] - (AgentCollHeading[7] - AgentCollHeading[8]))) / Mathf.Pow(CollisionDistance[7, 8], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[8, 9]) / Mathf.Pow(CollisionDistance[8, 9], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[8, 10]) / Mathf.Pow(CollisionDistance[8, 10], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[8, 11]) / Mathf.Pow(CollisionDistance[8, 11], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[8, 12]) / Mathf.Pow(CollisionDistance[8, 12], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[8, 13]) / Mathf.Pow(CollisionDistance[8, 13], 2));////

        ///////////////////////////////////////////////////////////////////////////////////////////////////

        CollisionPitchChange[9] =
            -CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[0, 9] - (AgentCollHeading[0] - AgentCollHeading[9]))) / Mathf.Pow(CollisionDistance[0, 9], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[1, 9] - (AgentCollHeading[1] - AgentCollHeading[9]))) / Mathf.Pow(CollisionDistance[1, 9], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[2, 9] - (AgentCollHeading[2] - AgentCollHeading[9]))) / Mathf.Pow(CollisionDistance[2, 9], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[3, 9] - (AgentCollHeading[3] - AgentCollHeading[9]))) / Mathf.Pow(CollisionDistance[3, 9], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[4, 9] - (AgentCollHeading[4] - AgentCollHeading[9]))) / Mathf.Pow(CollisionDistance[4, 9], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[5, 9] - (AgentCollHeading[5] - AgentCollHeading[9]))) / Mathf.Pow(CollisionDistance[5, 9], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[6, 9] - (AgentCollHeading[6] - AgentCollHeading[9]))) / Mathf.Pow(CollisionDistance[6, 9], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[7, 9] - (AgentCollHeading[7] - AgentCollHeading[9]))) / Mathf.Pow(CollisionDistance[7, 9], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[8, 9] - (AgentCollHeading[8] - AgentCollHeading[9]))) / Mathf.Pow(CollisionDistance[8, 9], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[9, 10]) / Mathf.Pow(CollisionDistance[9, 10], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[9, 11]) / Mathf.Pow(CollisionDistance[9, 11], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[9, 12]) / Mathf.Pow(CollisionDistance[9, 12], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[9, 13]) / Mathf.Pow(CollisionDistance[9, 13], 2));

        CollisionRollChange[9] =
            -CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[0, 9] - (AgentCollHeading[0] - AgentCollHeading[9]))) / Mathf.Pow(CollisionDistance[0, 9], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[1, 9] - (AgentCollHeading[1] - AgentCollHeading[9]))) / Mathf.Pow(CollisionDistance[1, 9], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[2, 9] - (AgentCollHeading[2] - AgentCollHeading[9]))) / Mathf.Pow(CollisionDistance[2, 9], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[3, 9] - (AgentCollHeading[3] - AgentCollHeading[9]))) / Mathf.Pow(CollisionDistance[3, 9], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[4, 9] - (AgentCollHeading[4] - AgentCollHeading[9]))) / Mathf.Pow(CollisionDistance[4, 9], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[5, 9] - (AgentCollHeading[5] - AgentCollHeading[9]))) / Mathf.Pow(CollisionDistance[5, 9], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[6, 9] - (AgentCollHeading[6] - AgentCollHeading[9]))) / Mathf.Pow(CollisionDistance[6, 9], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[7, 9] - (AgentCollHeading[7] - AgentCollHeading[9]))) / Mathf.Pow(CollisionDistance[7, 9], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[8, 9] - (AgentCollHeading[8] - AgentCollHeading[9]))) / Mathf.Pow(CollisionDistance[8, 9], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[9, 10]) / Mathf.Pow(CollisionDistance[9, 10], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[9, 11]) / Mathf.Pow(CollisionDistance[9, 11], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[9, 12]) / Mathf.Pow(CollisionDistance[9, 12], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[9, 13]) / Mathf.Pow(CollisionDistance[9, 13], 2));

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        CollisionPitchChange[10] =
            -CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[0, 10] - (AgentCollHeading[0] - AgentCollHeading[10]))) / Mathf.Pow(CollisionDistance[0, 10], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[1, 10] - (AgentCollHeading[1] - AgentCollHeading[10]))) / Mathf.Pow(CollisionDistance[1, 10], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[2, 10] - (AgentCollHeading[2] - AgentCollHeading[10]))) / Mathf.Pow(CollisionDistance[2, 10], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[3, 10] - (AgentCollHeading[3] - AgentCollHeading[10]))) / Mathf.Pow(CollisionDistance[3, 10], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[4, 10] - (AgentCollHeading[4] - AgentCollHeading[10]))) / Mathf.Pow(CollisionDistance[4, 10], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[5, 10] - (AgentCollHeading[5] - AgentCollHeading[10]))) / Mathf.Pow(CollisionDistance[5, 10], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[6, 10] - (AgentCollHeading[6] - AgentCollHeading[10]))) / Mathf.Pow(CollisionDistance[6, 10], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[7, 10] - (AgentCollHeading[7] - AgentCollHeading[10]))) / Mathf.Pow(CollisionDistance[7, 10], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[8, 10] - (AgentCollHeading[8] - AgentCollHeading[10]))) / Mathf.Pow(CollisionDistance[8, 10], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[9, 10] - (AgentCollHeading[9] - AgentCollHeading[10]))) / Mathf.Pow(CollisionDistance[9, 10], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[10, 11]) / Mathf.Pow(CollisionDistance[10, 11], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[10, 12]) / Mathf.Pow(CollisionDistance[10, 12], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[10, 13]) / Mathf.Pow(CollisionDistance[10, 13], 2));


        CollisionRollChange[10] =
            -CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[0, 10] - (AgentCollHeading[0] - AgentCollHeading[10]))) / Mathf.Pow(CollisionDistance[0, 10], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[1, 10] - (AgentCollHeading[1] - AgentCollHeading[10]))) / Mathf.Pow(CollisionDistance[1, 10], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[2, 10] - (AgentCollHeading[2] - AgentCollHeading[10]))) / Mathf.Pow(CollisionDistance[2, 10], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[3, 10] - (AgentCollHeading[3] - AgentCollHeading[10]))) / Mathf.Pow(CollisionDistance[3, 10], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[4, 10] - (AgentCollHeading[4] - AgentCollHeading[10]))) / Mathf.Pow(CollisionDistance[4, 10], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[5, 10] - (AgentCollHeading[5] - AgentCollHeading[10]))) / Mathf.Pow(CollisionDistance[5, 10], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[6, 10] - (AgentCollHeading[6] - AgentCollHeading[10]))) / Mathf.Pow(CollisionDistance[6, 10], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[7, 10] - (AgentCollHeading[7] - AgentCollHeading[10]))) / Mathf.Pow(CollisionDistance[7, 10], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[8, 10] - (AgentCollHeading[8] - AgentCollHeading[10]))) / Mathf.Pow(CollisionDistance[8, 10], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[9, 10] - (AgentCollHeading[9] - AgentCollHeading[10]))) / Mathf.Pow(CollisionDistance[9, 10], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[10, 11]) / Mathf.Pow(CollisionDistance[10, 11], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[10, 12]) / Mathf.Pow(CollisionDistance[10, 12], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[10, 13]) / Mathf.Pow(CollisionDistance[10, 13], 2));

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////


        CollisionPitchChange[11] =
            -CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[0, 11] - (AgentCollHeading[0] - AgentCollHeading[10]))) / Mathf.Pow(CollisionDistance[0, 11], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[1, 11] - (AgentCollHeading[1] - AgentCollHeading[10]))) / Mathf.Pow(CollisionDistance[1, 11], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[2, 11] - (AgentCollHeading[2] - AgentCollHeading[10]))) / Mathf.Pow(CollisionDistance[2, 11], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[3, 11] - (AgentCollHeading[3] - AgentCollHeading[10]))) / Mathf.Pow(CollisionDistance[3, 11], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[4, 11] - (AgentCollHeading[4] - AgentCollHeading[10]))) / Mathf.Pow(CollisionDistance[4, 11], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[5, 11] - (AgentCollHeading[5] - AgentCollHeading[10]))) / Mathf.Pow(CollisionDistance[5, 11], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[6, 11] - (AgentCollHeading[6] - AgentCollHeading[10]))) / Mathf.Pow(CollisionDistance[6, 11], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[7, 11] - (AgentCollHeading[7] - AgentCollHeading[10]))) / Mathf.Pow(CollisionDistance[7, 11], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[8, 11] - (AgentCollHeading[8] - AgentCollHeading[10]))) / Mathf.Pow(CollisionDistance[8, 11], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[9, 11] - (AgentCollHeading[9] - AgentCollHeading[10]))) / Mathf.Pow(CollisionDistance[9, 11], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[10, 11] - (AgentCollHeading[10] - AgentCollHeading[10]))) / Mathf.Pow(CollisionDistance[10, 11], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[11, 12]) / Mathf.Pow(CollisionDistance[11, 12], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[12, 13]) / Mathf.Pow(CollisionDistance[12, 13], 2));


        CollisionRollChange[11] =
            -CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[0, 11] - (AgentCollHeading[0] - AgentCollHeading[11]))) / Mathf.Pow(CollisionDistance[0, 11], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[1, 11] - (AgentCollHeading[1] - AgentCollHeading[11]))) / Mathf.Pow(CollisionDistance[1, 11], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[2, 11] - (AgentCollHeading[2] - AgentCollHeading[11]))) / Mathf.Pow(CollisionDistance[2, 11], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[3, 11] - (AgentCollHeading[3] - AgentCollHeading[11]))) / Mathf.Pow(CollisionDistance[3, 11], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[4, 11] - (AgentCollHeading[4] - AgentCollHeading[11]))) / Mathf.Pow(CollisionDistance[4, 11], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[5, 11] - (AgentCollHeading[5] - AgentCollHeading[11]))) / Mathf.Pow(CollisionDistance[5, 11], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[6, 11] - (AgentCollHeading[6] - AgentCollHeading[11]))) / Mathf.Pow(CollisionDistance[6, 11], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[7, 11] - (AgentCollHeading[7] - AgentCollHeading[11]))) / Mathf.Pow(CollisionDistance[7, 11], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[8, 11] - (AgentCollHeading[8] - AgentCollHeading[11]))) / Mathf.Pow(CollisionDistance[8, 11], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[9, 11] - (AgentCollHeading[9] - AgentCollHeading[11]))) / Mathf.Pow(CollisionDistance[9, 11], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[10, 11] - (AgentCollHeading[10] - AgentCollHeading[11]))) / Mathf.Pow(CollisionDistance[10, 11], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[11, 12]) / Mathf.Pow(CollisionDistance[11, 12], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[11, 13]) / Mathf.Pow(CollisionDistance[11, 13], 2));

        ///////////////////////////////////////////////////////////////////////////////////////////////////////

        CollisionPitchChange[12] =
            -CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[0, 12] - (AgentCollHeading[0] - AgentCollHeading[12]))) / Mathf.Pow(CollisionDistance[0, 12], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[1, 12] - (AgentCollHeading[1] - AgentCollHeading[12]))) / Mathf.Pow(CollisionDistance[1, 12], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[2, 12] - (AgentCollHeading[2] - AgentCollHeading[12]))) / Mathf.Pow(CollisionDistance[2, 12], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[3, 12] - (AgentCollHeading[3] - AgentCollHeading[12]))) / Mathf.Pow(CollisionDistance[3, 12], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[4, 12] - (AgentCollHeading[4] - AgentCollHeading[12]))) / Mathf.Pow(CollisionDistance[4, 12], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[5, 12] - (AgentCollHeading[5] - AgentCollHeading[12]))) / Mathf.Pow(CollisionDistance[5, 12], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[6, 12] - (AgentCollHeading[6] - AgentCollHeading[12]))) / Mathf.Pow(CollisionDistance[6, 12], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[7, 12] - (AgentCollHeading[7] - AgentCollHeading[12]))) / Mathf.Pow(CollisionDistance[7, 12], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[8, 12] - (AgentCollHeading[8] - AgentCollHeading[12]))) / Mathf.Pow(CollisionDistance[8, 12], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[9, 12] - (AgentCollHeading[9] - AgentCollHeading[12]))) / Mathf.Pow(CollisionDistance[9, 12], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[10, 12] - (AgentCollHeading[10] - AgentCollHeading[12]))) / Mathf.Pow(CollisionDistance[10, 12], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[11, 12] - (AgentCollHeading[11] - AgentCollHeading[12]))) / Mathf.Pow(CollisionDistance[11, 12], 2))
            + CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * CollisionHeading[12, 13]) / Mathf.Pow(CollisionDistance[12, 13], 2));


        CollisionRollChange[12] =
            -CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[0, 12] - (AgentCollHeading[0] - AgentCollHeading[12]))) / Mathf.Pow(CollisionDistance[0, 12], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[1, 12] - (AgentCollHeading[1] - AgentCollHeading[12]))) / Mathf.Pow(CollisionDistance[1, 12], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[2, 12] - (AgentCollHeading[2] - AgentCollHeading[12]))) / Mathf.Pow(CollisionDistance[2, 12], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[3, 12] - (AgentCollHeading[3] - AgentCollHeading[12]))) / Mathf.Pow(CollisionDistance[3, 12], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[4, 12] - (AgentCollHeading[4] - AgentCollHeading[12]))) / Mathf.Pow(CollisionDistance[4, 12], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[5, 12] - (AgentCollHeading[5] - AgentCollHeading[12]))) / Mathf.Pow(CollisionDistance[5, 12], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[6, 12] - (AgentCollHeading[6] - AgentCollHeading[12]))) / Mathf.Pow(CollisionDistance[6, 12], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[7, 12] - (AgentCollHeading[7] - AgentCollHeading[12]))) / Mathf.Pow(CollisionDistance[7, 12], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[8, 12] - (AgentCollHeading[8] - AgentCollHeading[12]))) / Mathf.Pow(CollisionDistance[8, 12], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[9, 12] - (AgentCollHeading[9] - AgentCollHeading[12]))) / Mathf.Pow(CollisionDistance[9, 12], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[10, 12] - (AgentCollHeading[10] - AgentCollHeading[12]))) / Mathf.Pow(CollisionDistance[10, 12], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[11, 12] - (AgentCollHeading[11] - AgentCollHeading[12]))) / Mathf.Pow(CollisionDistance[11, 12], 2))
            + CollisionAvoidCoeff * (-Mathf.Sin(Mathf.Deg2Rad * CollisionHeading[12, 13]) / Mathf.Pow(CollisionDistance[12, 13], 2));

        ///////////////////////////////////////////////////////////////////////////////////////////////////////

        CollisionPitchChange[13] =
            -CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[0, 13] - (AgentCollHeading[0] - AgentCollHeading[13]))) / Mathf.Pow(CollisionDistance[0, 13], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[1, 13] - (AgentCollHeading[1] - AgentCollHeading[13]))) / Mathf.Pow(CollisionDistance[1, 13], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[2, 13] - (AgentCollHeading[2] - AgentCollHeading[13]))) / Mathf.Pow(CollisionDistance[2, 13], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[3, 13] - (AgentCollHeading[3] - AgentCollHeading[13]))) / Mathf.Pow(CollisionDistance[3, 13], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[4, 13] - (AgentCollHeading[4] - AgentCollHeading[13]))) / Mathf.Pow(CollisionDistance[4, 13], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[5, 13] - (AgentCollHeading[5] - AgentCollHeading[13]))) / Mathf.Pow(CollisionDistance[5, 13], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[6, 13] - (AgentCollHeading[6] - AgentCollHeading[13]))) / Mathf.Pow(CollisionDistance[6, 13], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[7, 13] - (AgentCollHeading[7] - AgentCollHeading[13]))) / Mathf.Pow(CollisionDistance[7, 13], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[8, 13] - (AgentCollHeading[8] - AgentCollHeading[13]))) / Mathf.Pow(CollisionDistance[8, 13], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[9, 13] - (AgentCollHeading[9] - AgentCollHeading[13]))) / Mathf.Pow(CollisionDistance[9, 13], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[10, 13] - (AgentCollHeading[10] - AgentCollHeading[13]))) / Mathf.Pow(CollisionDistance[10, 13], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[11, 13] - (AgentCollHeading[11] - AgentCollHeading[13]))) / Mathf.Pow(CollisionDistance[11, 13], 2))
            - CollisionAvoidCoeff * (Mathf.Cos(Mathf.Deg2Rad * (CollisionHeading[12, 13] - (AgentCollHeading[12] - AgentCollHeading[13]))) / Mathf.Pow(CollisionDistance[12, 13], 2));


        CollisionRollChange[13] =
            -CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[0, 13] - (AgentCollHeading[0] - AgentCollHeading[13]))) / Mathf.Pow(CollisionDistance[0, 13], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[1, 13] - (AgentCollHeading[1] - AgentCollHeading[13]))) / Mathf.Pow(CollisionDistance[1, 13], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[2, 13] - (AgentCollHeading[2] - AgentCollHeading[13]))) / Mathf.Pow(CollisionDistance[2, 13], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[3, 13] - (AgentCollHeading[3] - AgentCollHeading[13]))) / Mathf.Pow(CollisionDistance[3, 13], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[4, 13] - (AgentCollHeading[4] - AgentCollHeading[13]))) / Mathf.Pow(CollisionDistance[4, 13], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[5, 13] - (AgentCollHeading[5] - AgentCollHeading[13]))) / Mathf.Pow(CollisionDistance[5, 13], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[6, 13] - (AgentCollHeading[6] - AgentCollHeading[13]))) / Mathf.Pow(CollisionDistance[6, 13], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[7, 13] - (AgentCollHeading[7] - AgentCollHeading[13]))) / Mathf.Pow(CollisionDistance[7, 13], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[8, 13] - (AgentCollHeading[8] - AgentCollHeading[13]))) / Mathf.Pow(CollisionDistance[8, 13], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[9, 13] - (AgentCollHeading[9] - AgentCollHeading[13]))) / Mathf.Pow(CollisionDistance[9, 13], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[10, 13] - (AgentCollHeading[10] - AgentCollHeading[13]))) / Mathf.Pow(CollisionDistance[10, 13], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[11, 13] - (AgentCollHeading[11] - AgentCollHeading[13]))) / Mathf.Pow(CollisionDistance[11, 13], 2))
            - CollisionAvoidCoeff * (Mathf.Sin(Mathf.Deg2Rad * (CollisionHeading[12, 13] - (AgentCollHeading[12] - AgentCollHeading[13]))) / Mathf.Pow(CollisionDistance[12, 13], 2));

    }
    /*
                if (Agent_Heading[8] < 0f) x = Agent_Heading[8] + 360f;
                else x = Agent_Heading[8];
                */

    //CollisionHeading[8, 9] = CollisionHeading[8, 9] - Agent_Heading[8];

    /*
    if (Agent_Heading[n] < 0f) CollisionHeading[n, k] -= Agent_Heading[n] + 360f;
    else CollisionHeading[n, k] -= Agent_Heading[n];

    */

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
