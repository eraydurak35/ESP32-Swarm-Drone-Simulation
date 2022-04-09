using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentControl : MonoBehaviour
{

    SwarmManagerScript SwarmManager;
    [SerializeField] GameObject SwarmManagerObj;

    public GameObject LandingZone;

    public int AgentID;
    private bool Active = true;
    
    private float pitchGyro;
    
    private float rollGyro;
    
    private float lastPitchSensorValue;
    private float lastRollSensorValue;
    private float lastYawChangeRate;

    public float maxPitchAngle = 25f;
    public float maxRollAngle = 25f;
    public float yawRate = 100f;

    public float pitchP = 30f;
    public float pitchI = 5f;
    public float pitchD = 200f;

    public float rollP = 30f;
    public float rollI = 5f;
    public float rollD = 200f;

    public float yawP = 20f;
    public float yawI = 1f;
    public float yawD = 0f;

    private float tau = 0.02f;

    private float pitchSetpoint = 0f;
    private float rollSetpoint = 0f;
    [HideInInspector]
    public float yawSetRate = 0f;

    private float yawChangeRate;
    public float AltChangeRate;
    private float PrevAltitude;
    public float AltHoldSetRate;

    private float pitchError = 0f;
    private float rollError = 0f;
    private float yawError = 0f;
    private float pitchPrevError;
    private float rollPrevError;
    private float yawPrevError;
    public float AgentHeading;

    private int maxThrottle = 4000;
    [HideInInspector]
    public float LBThrottle;
    [HideInInspector]
    public float LTThrottle;
    [HideInInspector]
    public float RBThrottle;
    [HideInInspector]
    public float RTThrottle;



    private float pitch_P_out;
    private float pitch_I_out;
    private float pitch_D_out;

    private float roll_P_out;
    private float roll_I_out;
    private float roll_D_out;

    private float yaw_P_out;
    private float yaw_I_out;
    private float yaw_D_out;

    private float pitch_PID_out;
    private float roll_PID_out;
    private float yaw_PID_out;

    private float userThrottle = 300;

    private float maxPID = 1000;
    private float maxI = 100;
    private float maxD = 300;
    private float minThrottle = 100f;
    private float minUserThrottle = 500;
    private float sampleTime = 0.005f;

    [HideInInspector]
    public float barometerAltitude;
    private float prevBarometerAltitude;

    [HideInInspector]
    public float AltHoldSet;
    public float altHoldP = 280f;
    public float altHoldI = 100f;
    public float altHoldD = 6500;
    private float altHold_P_out;
    private float altHold_I_out;
    private float altHold_D_out;
    private float altHoldPID_out;
    private float altHoldError;
    private float altHoldPrevError;

    private float prevYaw;
    private float positionX;
    private float positionZ;
    private float prevPositionX;
    private float prevPositionZ;
    private float positionXError;
    private float positionZError;
    private float pitchPosHoldHorth;
    private float rollPosHoldHorth;
    [HideInInspector]
    public float PosHoldSetX;
    [HideInInspector]
    public float PosHoldSetZ;

    public float posHoldP = 100f;
    public float posHoldD = 30f;

    private float pitchPosHold_P_out;
    private float rollPosHold_P_out;
    private float pitchPosHold_D_out;
    private float rollPosHold_D_out;
    private float correctedPitchPosHoldAngle;
    private float correctedRollPosHoldAngle;

    public float TargetHeading;
    public float distanceToLZ;
    public float LandingDelay;



    // Start is called before the first frame update

    private void Awake()
    {
        SwarmManager = SwarmManagerObj.GetComponent<SwarmManagerScript>();
    }
    private void FixedUpdate()
    {
        SensorFusion();
        GetDataFromSwarmManager();
        PID();
        GetComponent<AgentMotors>().MotorsState();
    }

    void GetDataFromSwarmManager()
    {

        Active = SwarmManager.ActiveAgents[AgentID];


        if (Active)
        {
            LandingDelay = 5f;

            if (SwarmManager.TakeOff)
            {
                AltHoldSet = SwarmManager.takeOffHight;
                PosHoldSetX = LandingZone.transform.position.x;
                PosHoldSetZ = LandingZone.transform.position.z;

                if (!SwarmManager.takeOffSucces)
                {
                    for (int i = 0; i < 14; i++)
                    {
                        if (Mathf.Abs(AltHoldSet - SwarmManager.Agent_Y[i]) < 0.3f)
                        {
                            SwarmManager.AgentTakeOffSucces[i] = true;
                        }
                        for (int j = 0; j < 14; j++)
                        {
                            if (SwarmManager.AgentTakeOffSucces[j])
                            {
                                SwarmManager.takeoffcounter++;
                                if (SwarmManager.takeoffcounter == 14)
                                {
                                    SwarmManager.takeOffSucces = true;
                                }
                            }
                            else SwarmManager.takeoffcounter = 0;

                        }
                    }
                }
 
            }
            else if (SwarmManager.FormSquarePrizm)
            {
                SquarePrizmFormation();
            }
            else if (SwarmManager.FormPyramidSquarePrizm)
            {
                PyramidSquarePrizmFormation();
            }
            else if (SwarmManager.FormTriangle)
            {
                TriangleFormation();
            }
            else if (SwarmManager.FormTrianglePrizm)
            {
                TrianglePrizmFormation();
            }
            else if (SwarmManager.FormSquare)
            {
                SquareFormation();
            }
            else if (SwarmManager.FormHexagonPrizm)
            {
                HexagonPrizmFormation();
            }
            else if (SwarmManager.FormHexagon)
            {
                HexagonFormation();
            }
            if (SwarmManager.Form3DmissionTakeOff1)
            {
                Formation3DMissionTakeOff1();
            }
            if (SwarmManager.Form3DmissionTakeOff2)
            {
                Formation3DMissionTakeOff2();
            }
            if (SwarmManager.TaskSwitchTakeOff1)
            {
                TaskSwitchTakeOff1();
            }
            if (SwarmManager.TaskSwitchTakeOff2)
            {
                TaskSwitchTakeOff2();
            }


        }
        else Landing();

    }

    void Formation3DMissionTakeOff1()
    {
        /////////  TakeOff1 Logic for Square Prizm Formation
        for (int l = 0; l < 8; l++)
        {
            if (AgentID == l)
            {
                AltHoldSet = SwarmManager.takeOffHight;
                PosHoldSetX = LandingZone.transform.position.x;
                PosHoldSetZ = LandingZone.transform.position.z;
            } 
        }

        if (!SwarmManager.Form3DmissionTakeOff1Success)
        {
            for (int i = 0; i < 8; i++)
            {
                if (Mathf.Abs(AltHoldSet - SwarmManager.Agent_Y[i]) < 0.3f)
                {
                    SwarmManager.AgentTakeOffSucces[i] = true;
                }
                for (int j = 0; j < 8; j++)
                {
                    if (SwarmManager.AgentTakeOffSucces[j])
                    {
                        SwarmManager.takeoffcounter++;
                        if (SwarmManager.takeoffcounter == 8)
                        {
                            SwarmManager.Form3DmissionTakeOff1Success = true;
                        }
                    }
                    else SwarmManager.takeoffcounter = 0;

                }
            }
        }
    }
    void Formation3DMissionTakeOff2()
    {
        for (int l = 8; l < 12; l++)
        {
            if (AgentID == l)
            {
                AltHoldSet = SwarmManager.takeOffHight;
                PosHoldSetX = LandingZone.transform.position.x;
                PosHoldSetZ = LandingZone.transform.position.z;
            }
        }

        if (!SwarmManager.Form3DmissionTakeOff2Success)
        {
            for (int i = 8; i < 12; i++)
            {
                if (Mathf.Abs(AltHoldSet - SwarmManager.Agent_Y[i]) < 0.3f)
                {
                    SwarmManager.AgentTakeOffSucces[i] = true;
                }
                for (int j = 8; j < 12; j++)
                {
                    if (SwarmManager.AgentTakeOffSucces[j])
                    {
                        SwarmManager.takeoffcounter++;
                        if (SwarmManager.takeoffcounter == 4)
                        {
                            SwarmManager.Form3DmissionTakeOff2Success = true;
                        }
                    }
                    else SwarmManager.takeoffcounter = 0;

                }
            }
        }
    }

    void TaskSwitchTakeOff1()
    {
        for (int l = 0; l < 6; l++)
        {
            if (AgentID == l)
            {
                AltHoldSet = SwarmManager.takeOffHight;
                PosHoldSetX = LandingZone.transform.position.x;
                PosHoldSetZ = LandingZone.transform.position.z;
            }
        }

        if (!SwarmManager.TaskSwitchTakeOff1Succes)
        {
            for (int i = 0; i < 6; i++)
            {
                if (Mathf.Abs(AltHoldSet - SwarmManager.Agent_Y[i]) < 0.3f)
                {
                    SwarmManager.AgentTakeOffSucces[i] = true;
                }
                for (int j = 0; j < 6; j++)
                {
                    if (SwarmManager.AgentTakeOffSucces[j])
                    {
                        SwarmManager.takeoffcounter++;
                        if (SwarmManager.takeoffcounter == 6)
                        {
                            SwarmManager.TaskSwitchTakeOff1Succes = true;
                        }
                    }
                    else SwarmManager.takeoffcounter = 0;

                }
            }
        }

    }

    void TaskSwitchTakeOff2()
    {
        for (int l = 6; l < 8; l++)
        {
            if (AgentID == l)
            {
                AltHoldSet = SwarmManager.takeOffHight;
                PosHoldSetX = LandingZone.transform.position.x;
                PosHoldSetZ = LandingZone.transform.position.z;
            }
        }

        if (!SwarmManager.TaskSwitchTakeOff2Succes)
        {
            for (int i = 6; i < 8; i++)
            {
                if (Mathf.Abs(AltHoldSet - SwarmManager.Agent_Y[i]) < 0.3f)
                {
                    SwarmManager.AgentTakeOffSucces[i] = true;
                }
                for (int j = 6; j < 8; j++)
                {
                    if (SwarmManager.AgentTakeOffSucces[j])
                    {
                        SwarmManager.takeoffcounter++;
                        if (SwarmManager.takeoffcounter == 2)
                        {
                            SwarmManager.TaskSwitchTakeOff2Succes = true;
                        }
                    }
                    else SwarmManager.takeoffcounter = 0;

                }
            }
        }

    }


    void Landing()
    {
        PosHoldSetX = LandingZone.transform.position.x;
        PosHoldSetZ = LandingZone.transform.position.z;
        
        distanceToLZ = Mathf.Sqrt(Mathf.Pow((PosHoldSetX - transform.position.x), 2) + Mathf.Pow((PosHoldSetZ - transform.position.z), 2));

        if (distanceToLZ > 0.2f)
        {
            TargetHeading = (Mathf.Atan2((LandingZone.transform.position.x - transform.position.x), (transform.position.z - LandingZone.transform.position.z)) * Mathf.Rad2Deg) * -1f;
            AltHoldSet = 5f;
        }

        else if (distanceToLZ < 0.2f)
        {
            LandingDelay -= Time.deltaTime;
            TargetHeading = 0f;

            if (LandingDelay > 4f && LandingDelay < 5f)
            {
                AltHoldSet = 4f;
            }
            else if (LandingDelay > 2f && LandingDelay < 4f)
            {
                AltHoldSet = 3f;
            }
            else if (LandingDelay < 0)
            {
                AltHoldSet = 0f;
                SwarmManager.AgentLandedSucces[AgentID] = true;
            }
        }

    }

    void SquarePrizmFormation()
    {

        float[] Xposition = new float[8];
        float[] Zposition = new float[8];
        float[] Yposition = new float[8];
        float formationAverageX = 0f;
        float formationAverageY = 0f;
        float formationAverageZ = 0f;

        float hypotenus = Mathf.Sqrt((Mathf.Pow((SwarmManager.squarePrizmEdge / 2f), 2)) + (Mathf.Pow((SwarmManager.squarePrizmEdge / 2f), 2)));
        Xposition[0] = SwarmManager.formationTargetMiddleX + hypotenus * Mathf.Sin((SwarmManager.FormationHeading + 45f) * Mathf.Deg2Rad);
        Xposition[1] = SwarmManager.formationTargetMiddleX + hypotenus * Mathf.Sin((SwarmManager.FormationHeading + 135f) * Mathf.Deg2Rad);
        Xposition[2] = SwarmManager.formationTargetMiddleX + hypotenus * Mathf.Sin((SwarmManager.FormationHeading + 225f) * Mathf.Deg2Rad);
        Xposition[3] = SwarmManager.formationTargetMiddleX + hypotenus * Mathf.Sin((SwarmManager.FormationHeading + 315f) * Mathf.Deg2Rad);

        Zposition[0] = SwarmManager.formationTargetMiddleZ + hypotenus * Mathf.Cos((SwarmManager.FormationHeading + 45f) * Mathf.Deg2Rad);
        Zposition[1] = SwarmManager.formationTargetMiddleZ + hypotenus * Mathf.Cos((SwarmManager.FormationHeading + 135f) * Mathf.Deg2Rad);
        Zposition[2] = SwarmManager.formationTargetMiddleZ + hypotenus * Mathf.Cos((SwarmManager.FormationHeading + 225f) * Mathf.Deg2Rad);
        Zposition[3] = SwarmManager.formationTargetMiddleZ + hypotenus * Mathf.Cos((SwarmManager.FormationHeading + 315f) * Mathf.Deg2Rad);

        Yposition[0] = SwarmManager.formationTargetMiddleHeight + (SwarmManager.squarePrizmHeight / 2f);
        Yposition[1] = Yposition[0];
        Yposition[2] = Yposition[0];
        Yposition[3] = Yposition[0];


        if (SwarmManager.levelDelay > 0)
        {
            SwarmManager.levelDelay -= Time.deltaTime;
        }
        if (SwarmManager.levelDelay <= 0)
        {
            Xposition[4] = Xposition[0];
            Xposition[5] = Xposition[1];
            Xposition[6] = Xposition[2];
            Xposition[7] = Xposition[3];

            Zposition[4] = Zposition[0];
            Zposition[5] = Zposition[1];
            Zposition[6] = Zposition[2];
            Zposition[7] = Zposition[3];

            Yposition[4] = SwarmManager.formationTargetMiddleHeight - (SwarmManager.squarePrizmHeight / 2f);
            Yposition[5] = Yposition[4];
            Yposition[6] = Yposition[4];
            Yposition[7] = Yposition[4];
        }
        else
        {
            Xposition[4] = LandingZone.transform.position.x;
            Xposition[5] = LandingZone.transform.position.x;
            Xposition[6] = LandingZone.transform.position.x;
            Xposition[7] = LandingZone.transform.position.x;

            Zposition[4] = LandingZone.transform.position.z;
            Zposition[5] = LandingZone.transform.position.z;
            Zposition[6] = LandingZone.transform.position.z;
            Zposition[7] = LandingZone.transform.position.z;

            Yposition[4] = SwarmManager.takeOffHight;
            Yposition[5] = SwarmManager.takeOffHight;
            Yposition[6] = SwarmManager.takeOffHight;
            Yposition[7] = SwarmManager.takeOffHight;
        }

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 14; j++)
            {
                if (SwarmManager.ActiveAgents[j] && !SwarmManager.SquarePrizmInFormation[j] && !SwarmManager.SquarePrizmPositionFull[i])
                {
                    SwarmManager.SquarePrizmAssignedLocationNumber[j] = i;
                    SwarmManager.SquarePrizmInFormation[j] = true;
                    SwarmManager.SquarePrizmPositionFull[i] = true;
                    break;
                }
                else if (!SwarmManager.ActiveAgents[j] && SwarmManager.SquarePrizmInFormation[j])
                {
                    SwarmManager.SquarePrizmInFormation[j] = false;
                    SwarmManager.SquarePrizmPositionFull[SwarmManager.SquarePrizmAssignedLocationNumber[j]] = false;
                    SwarmManager.AgentFormSquarePrizmSucces[j] = false;
                }
            }
        }


        for (int c = 0; c < 14; c++)
        {
            if (SwarmManager.SquarePrizmInFormation[c])
            {
                formationAverageX += SwarmManager.Agent_X[c];
                formationAverageY += SwarmManager.Agent_Y[c];
                formationAverageZ += SwarmManager.Agent_Z[c];
            }
            if (c == 13)
            {
                SwarmManager.FormationMiddleX = formationAverageX / SwarmManager.SquarePrizmAgentRequired;
                SwarmManager.FormationMiddleY = formationAverageY / SwarmManager.SquarePrizmAgentRequired;
                SwarmManager.FormationMiddleZ = formationAverageZ / SwarmManager.SquarePrizmAgentRequired;
            }
        }



        if (AgentID == 0 && SwarmManager.SquarePrizmInFormation[0])
        {
            PosHoldSetX = Xposition[SwarmManager.SquarePrizmAssignedLocationNumber[0]];
            PosHoldSetZ = Zposition[SwarmManager.SquarePrizmAssignedLocationNumber[0]];
            AltHoldSet = Yposition[SwarmManager.SquarePrizmAssignedLocationNumber[0]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[0], SwarmManager.Agent_Y[0], SwarmManager.Agent_Z[0]),new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormSquarePrizmSucces[0] = true;
            }
        }
        if (AgentID == 1 && SwarmManager.SquarePrizmInFormation[1])
        {
            PosHoldSetX = Xposition[SwarmManager.SquarePrizmAssignedLocationNumber[1]];
            PosHoldSetZ = Zposition[SwarmManager.SquarePrizmAssignedLocationNumber[1]];
            AltHoldSet = Yposition[SwarmManager.SquarePrizmAssignedLocationNumber[1]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[1], SwarmManager.Agent_Y[1], SwarmManager.Agent_Z[1]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormSquarePrizmSucces[1] = true;
            }
        }
        if (AgentID == 2 && SwarmManager.SquarePrizmInFormation[2])
        {
            PosHoldSetX = Xposition[SwarmManager.SquarePrizmAssignedLocationNumber[2]];
            PosHoldSetZ = Zposition[SwarmManager.SquarePrizmAssignedLocationNumber[2]];
            AltHoldSet = Yposition[SwarmManager.SquarePrizmAssignedLocationNumber[2]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[2], SwarmManager.Agent_Y[2], SwarmManager.Agent_Z[2]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormSquarePrizmSucces[2] = true;
            }
        }
        if (AgentID == 3 && SwarmManager.SquarePrizmInFormation[3])
        {
            PosHoldSetX = Xposition[SwarmManager.SquarePrizmAssignedLocationNumber[3]];
            PosHoldSetZ = Zposition[SwarmManager.SquarePrizmAssignedLocationNumber[3]];
            AltHoldSet = Yposition[SwarmManager.SquarePrizmAssignedLocationNumber[3]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[3], SwarmManager.Agent_Y[3], SwarmManager.Agent_Z[3]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormSquarePrizmSucces[3] = true;
            }
        }
        if (AgentID == 4 && SwarmManager.SquarePrizmInFormation[4])
        {
            PosHoldSetX = Xposition[SwarmManager.SquarePrizmAssignedLocationNumber[4]];
            PosHoldSetZ = Zposition[SwarmManager.SquarePrizmAssignedLocationNumber[4]];
            AltHoldSet = Yposition[SwarmManager.SquarePrizmAssignedLocationNumber[4]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[4], SwarmManager.Agent_Y[4], SwarmManager.Agent_Z[4]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormSquarePrizmSucces[4] = true;
            }
        }
        if (AgentID == 5 && SwarmManager.SquarePrizmInFormation[5])
        {
            PosHoldSetX = Xposition[SwarmManager.SquarePrizmAssignedLocationNumber[5]];
            PosHoldSetZ = Zposition[SwarmManager.SquarePrizmAssignedLocationNumber[5]];
            AltHoldSet = Yposition[SwarmManager.SquarePrizmAssignedLocationNumber[5]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[5], SwarmManager.Agent_Y[5], SwarmManager.Agent_Z[5]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormSquarePrizmSucces[5] = true;
            }
        }
        if (AgentID == 6 && SwarmManager.SquarePrizmInFormation[6])
        {
            PosHoldSetX = Xposition[SwarmManager.SquarePrizmAssignedLocationNumber[6]];
            PosHoldSetZ = Zposition[SwarmManager.SquarePrizmAssignedLocationNumber[6]];
            AltHoldSet = Yposition[SwarmManager.SquarePrizmAssignedLocationNumber[6]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[6], SwarmManager.Agent_Y[6], SwarmManager.Agent_Z[6]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormSquarePrizmSucces[6] = true;
            }
        }
        if (AgentID == 7 && SwarmManager.SquarePrizmInFormation[7])
        {
            PosHoldSetX = Xposition[SwarmManager.SquarePrizmAssignedLocationNumber[7]];
            PosHoldSetZ = Zposition[SwarmManager.SquarePrizmAssignedLocationNumber[7]];
            AltHoldSet = Yposition[SwarmManager.SquarePrizmAssignedLocationNumber[7]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[7], SwarmManager.Agent_Y[7], SwarmManager.Agent_Z[7]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormSquarePrizmSucces[7] = true;
            }
        }
        if (AgentID == 8 && SwarmManager.SquarePrizmInFormation[8])
        {
            PosHoldSetX = Xposition[SwarmManager.SquarePrizmAssignedLocationNumber[8]];
            PosHoldSetZ = Zposition[SwarmManager.SquarePrizmAssignedLocationNumber[8]];
            AltHoldSet = Yposition[SwarmManager.SquarePrizmAssignedLocationNumber[8]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[8], SwarmManager.Agent_Y[8], SwarmManager.Agent_Z[8]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormSquarePrizmSucces[8] = true;
            }
        }
        if (AgentID == 9 && SwarmManager.SquarePrizmInFormation[9])
        {
            PosHoldSetX = Xposition[SwarmManager.SquarePrizmAssignedLocationNumber[9]];
            PosHoldSetZ = Zposition[SwarmManager.SquarePrizmAssignedLocationNumber[9]];
            AltHoldSet = Yposition[SwarmManager.SquarePrizmAssignedLocationNumber[9]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[9], SwarmManager.Agent_Y[9], SwarmManager.Agent_Z[9]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormSquarePrizmSucces[9] = true;
            }
        }
        if (AgentID == 10 && SwarmManager.SquarePrizmInFormation[10])
        {
            PosHoldSetX = Xposition[SwarmManager.SquarePrizmAssignedLocationNumber[10]];
            PosHoldSetZ = Zposition[SwarmManager.SquarePrizmAssignedLocationNumber[10]];
            AltHoldSet = Yposition[SwarmManager.SquarePrizmAssignedLocationNumber[10]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[10], SwarmManager.Agent_Y[10], SwarmManager.Agent_Z[10]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormSquarePrizmSucces[10] = true;
            }
        }
        if (AgentID == 11 && SwarmManager.SquarePrizmInFormation[11])
        {
            PosHoldSetX = Xposition[SwarmManager.SquarePrizmAssignedLocationNumber[11]];
            PosHoldSetZ = Zposition[SwarmManager.SquarePrizmAssignedLocationNumber[11]];
            AltHoldSet = Yposition[SwarmManager.SquarePrizmAssignedLocationNumber[11]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[11], SwarmManager.Agent_Y[11], SwarmManager.Agent_Z[11]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormSquarePrizmSucces[11] = true;
            }
        }
        if (AgentID == 12 && SwarmManager.SquarePrizmInFormation[12])
        {
            PosHoldSetX = Xposition[SwarmManager.SquarePrizmAssignedLocationNumber[12]];
            PosHoldSetZ = Zposition[SwarmManager.SquarePrizmAssignedLocationNumber[12]];
            AltHoldSet = Yposition[SwarmManager.SquarePrizmAssignedLocationNumber[12]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[12], SwarmManager.Agent_Y[12], SwarmManager.Agent_Z[12]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormSquarePrizmSucces[12] = true;
            }
        }
        if (AgentID == 13 && SwarmManager.SquarePrizmInFormation[13])
        {
            PosHoldSetX = Xposition[SwarmManager.SquarePrizmAssignedLocationNumber[13]];
            PosHoldSetZ = Zposition[SwarmManager.SquarePrizmAssignedLocationNumber[13]];
            AltHoldSet = Yposition[SwarmManager.SquarePrizmAssignedLocationNumber[13]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[13], SwarmManager.Agent_Y[13], SwarmManager.Agent_Z[13]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormSquarePrizmSucces[13] = true;
            }
        }

        for (int k = 0; k < 14; k++)
        {
            if (SwarmManager.AgentFormSquarePrizmSucces[k])
            {
                SwarmManager.squarePrizmCounter++;
            }
        }
        if (SwarmManager.squarePrizmCounter == SwarmManager.SquarePrizmAgentRequired)
        {
            SwarmManager.FormSquarePrizmSucces = true;
            SwarmManager.squarePrizmCounter = 0;
        }
        else
        {
            SwarmManager.FormSquarePrizmSucces = false;
            SwarmManager.squarePrizmCounter = 0;
        }
    }

    void PyramidSquarePrizmFormation()
    {

        float[] Xposition = new float[5];
        float[] Zposition = new float[5];
        float[] Yposition = new float[5];

        float formationAverageX = 0f;
        float formationAverageY = 0f;
        float formationAverageZ = 0f;


        Xposition[4] = SwarmManager.formationTargetMiddleX;

        Zposition[4] = SwarmManager.formationTargetMiddleZ;

        Yposition[4] = SwarmManager.formationTargetMiddleHeight + (SwarmManager.squarePrizmHeight / 2f);


        if (SwarmManager.levelDelay > 0)
        {
            SwarmManager.levelDelay -= Time.deltaTime;
        }
        if (SwarmManager.levelDelay <= 0)
        {
            float hypotenus = Mathf.Sqrt((Mathf.Pow((SwarmManager.pyramidSquarePrizmEdge / 2f), 2)) + (Mathf.Pow((SwarmManager.pyramidSquarePrizmEdge / 2f), 2)));
            Xposition[0] = SwarmManager.formationTargetMiddleX + hypotenus * Mathf.Sin((SwarmManager.FormationHeading + 45f) * Mathf.Deg2Rad);
            Xposition[1] = SwarmManager.formationTargetMiddleX + hypotenus * Mathf.Sin((SwarmManager.FormationHeading + 135f) * Mathf.Deg2Rad);
            Xposition[2] = SwarmManager.formationTargetMiddleX + hypotenus * Mathf.Sin((SwarmManager.FormationHeading + 225f) * Mathf.Deg2Rad);
            Xposition[3] = SwarmManager.formationTargetMiddleX + hypotenus * Mathf.Sin((SwarmManager.FormationHeading + 315f) * Mathf.Deg2Rad);

            Zposition[0] = SwarmManager.formationTargetMiddleZ + hypotenus * Mathf.Cos((SwarmManager.FormationHeading + 45f) * Mathf.Deg2Rad);
            Zposition[1] = SwarmManager.formationTargetMiddleZ + hypotenus * Mathf.Cos((SwarmManager.FormationHeading + 135f) * Mathf.Deg2Rad);
            Zposition[2] = SwarmManager.formationTargetMiddleZ + hypotenus * Mathf.Cos((SwarmManager.FormationHeading + 225f) * Mathf.Deg2Rad);
            Zposition[3] = SwarmManager.formationTargetMiddleZ + hypotenus * Mathf.Cos((SwarmManager.FormationHeading + 315f) * Mathf.Deg2Rad);

            Yposition[0] = SwarmManager.formationTargetMiddleHeight - (SwarmManager.squarePrizmHeight / 2f);
            Yposition[1] = Yposition[0];
            Yposition[2] = Yposition[0];
            Yposition[3] = Yposition[0];
        }
        else
        {
            Xposition[0] = LandingZone.transform.position.x;
            Xposition[1] = LandingZone.transform.position.x;
            Xposition[2] = LandingZone.transform.position.x;
            Xposition[3] = LandingZone.transform.position.x;

            Zposition[0] = LandingZone.transform.position.z;
            Zposition[1] = LandingZone.transform.position.z;
            Zposition[2] = LandingZone.transform.position.z;
            Zposition[3] = LandingZone.transform.position.z;

            Yposition[0] = SwarmManager.takeOffHight;
            Yposition[1] = SwarmManager.takeOffHight;
            Yposition[2] = SwarmManager.takeOffHight;
            Yposition[3] = SwarmManager.takeOffHight;
        }


        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 14; j++)
            {
                if (SwarmManager.ActiveAgents[j] && !SwarmManager.PyramidSquarePrizmInFormation[j] && !SwarmManager.PyramidSquarePrizmPositionFull[i])
                {
                    SwarmManager.PyramidSquarePrizmAssignedLocationNumber[j] = i;
                    SwarmManager.PyramidSquarePrizmInFormation[j] = true;
                    SwarmManager.PyramidSquarePrizmPositionFull[i] = true;
                    break;
                }
                else if (!SwarmManager.ActiveAgents[j] && SwarmManager.PyramidSquarePrizmInFormation[j])
                {
                    SwarmManager.PyramidSquarePrizmInFormation[j] = false;
                    SwarmManager.PyramidSquarePrizmPositionFull[SwarmManager.PyramidSquarePrizmAssignedLocationNumber[j]] = false;
                    SwarmManager.AgentFormPyramidSquarePrizmSucces[j] = false;
                }
            }
        }

        for (int c = 0; c < 14; c++)
        {
            if (SwarmManager.PyramidSquarePrizmInFormation[c])
            {
                formationAverageX += SwarmManager.Agent_X[c];
                formationAverageY += SwarmManager.Agent_Y[c];
                formationAverageZ += SwarmManager.Agent_Z[c];
            }
            if (c == 13)
            {
                SwarmManager.FormationMiddleX = formationAverageX / SwarmManager.PyramidSquarePrizmAgentRequired;
                SwarmManager.FormationMiddleY = formationAverageY / SwarmManager.PyramidSquarePrizmAgentRequired;
                SwarmManager.FormationMiddleZ = formationAverageZ / SwarmManager.PyramidSquarePrizmAgentRequired;
            }
        }

        if (AgentID == 0 && SwarmManager.PyramidSquarePrizmInFormation[0])
        {
            PosHoldSetX = Xposition[SwarmManager.PyramidSquarePrizmAssignedLocationNumber[0]];
            PosHoldSetZ = Zposition[SwarmManager.PyramidSquarePrizmAssignedLocationNumber[0]];
            AltHoldSet = Yposition[SwarmManager.PyramidSquarePrizmAssignedLocationNumber[0]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[0], SwarmManager.Agent_Y[0], SwarmManager.Agent_Z[0]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormPyramidSquarePrizmSucces[0] = true;
            }
        }
        if (AgentID == 1 && SwarmManager.PyramidSquarePrizmInFormation[1])
        {
            PosHoldSetX = Xposition[SwarmManager.PyramidSquarePrizmAssignedLocationNumber[1]];
            PosHoldSetZ = Zposition[SwarmManager.PyramidSquarePrizmAssignedLocationNumber[1]];
            AltHoldSet = Yposition[SwarmManager.PyramidSquarePrizmAssignedLocationNumber[1]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[1], SwarmManager.Agent_Y[1], SwarmManager.Agent_Z[1]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormPyramidSquarePrizmSucces[1] = true;
            }
        }
        if (AgentID == 2 && SwarmManager.PyramidSquarePrizmInFormation[2])
        {
            PosHoldSetX = Xposition[SwarmManager.PyramidSquarePrizmAssignedLocationNumber[2]];
            PosHoldSetZ = Zposition[SwarmManager.PyramidSquarePrizmAssignedLocationNumber[2]];
            AltHoldSet = Yposition[SwarmManager.PyramidSquarePrizmAssignedLocationNumber[2]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[2], SwarmManager.Agent_Y[2], SwarmManager.Agent_Z[2]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormPyramidSquarePrizmSucces[2] = true;
            }
        }
        if (AgentID == 3 && SwarmManager.PyramidSquarePrizmInFormation[3])
        {
            PosHoldSetX = Xposition[SwarmManager.PyramidSquarePrizmAssignedLocationNumber[3]];
            PosHoldSetZ = Zposition[SwarmManager.PyramidSquarePrizmAssignedLocationNumber[3]];
            AltHoldSet = Yposition[SwarmManager.PyramidSquarePrizmAssignedLocationNumber[3]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[3], SwarmManager.Agent_Y[3], SwarmManager.Agent_Z[3]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormPyramidSquarePrizmSucces[3] = true;
            }
        }
        if (AgentID == 4 && SwarmManager.PyramidSquarePrizmInFormation[4])
        {
            PosHoldSetX = Xposition[SwarmManager.PyramidSquarePrizmAssignedLocationNumber[4]];
            PosHoldSetZ = Zposition[SwarmManager.PyramidSquarePrizmAssignedLocationNumber[4]];
            AltHoldSet = Yposition[SwarmManager.PyramidSquarePrizmAssignedLocationNumber[4]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[4], SwarmManager.Agent_Y[4], SwarmManager.Agent_Z[4]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormPyramidSquarePrizmSucces[4] = true;
            }
        }
        if (AgentID == 5 && SwarmManager.PyramidSquarePrizmInFormation[5])
        {
            PosHoldSetX = Xposition[SwarmManager.PyramidSquarePrizmAssignedLocationNumber[5]];
            PosHoldSetZ = Zposition[SwarmManager.PyramidSquarePrizmAssignedLocationNumber[5]];
            AltHoldSet = Yposition[SwarmManager.PyramidSquarePrizmAssignedLocationNumber[5]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[5], SwarmManager.Agent_Y[5], SwarmManager.Agent_Z[5]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormPyramidSquarePrizmSucces[5] = true;
            }
        }
        if (AgentID == 6 && SwarmManager.PyramidSquarePrizmInFormation[6])
        {
            PosHoldSetX = Xposition[SwarmManager.PyramidSquarePrizmAssignedLocationNumber[6]];
            PosHoldSetZ = Zposition[SwarmManager.PyramidSquarePrizmAssignedLocationNumber[6]];
            AltHoldSet = Yposition[SwarmManager.PyramidSquarePrizmAssignedLocationNumber[6]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[6], SwarmManager.Agent_Y[6], SwarmManager.Agent_Z[6]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormPyramidSquarePrizmSucces[6] = true;
            }
        }
        if (AgentID == 7 && SwarmManager.PyramidSquarePrizmInFormation[7])
        {
            PosHoldSetX = Xposition[SwarmManager.PyramidSquarePrizmAssignedLocationNumber[7]];
            PosHoldSetZ = Zposition[SwarmManager.PyramidSquarePrizmAssignedLocationNumber[7]];
            AltHoldSet = Yposition[SwarmManager.PyramidSquarePrizmAssignedLocationNumber[7]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[7], SwarmManager.Agent_Y[7], SwarmManager.Agent_Z[7]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormPyramidSquarePrizmSucces[7] = true;
            }
        }
        if (AgentID == 8 && SwarmManager.PyramidSquarePrizmInFormation[8])
        {
            PosHoldSetX = Xposition[SwarmManager.PyramidSquarePrizmAssignedLocationNumber[8]];
            PosHoldSetZ = Zposition[SwarmManager.PyramidSquarePrizmAssignedLocationNumber[8]];
            AltHoldSet = Yposition[SwarmManager.PyramidSquarePrizmAssignedLocationNumber[8]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[8], SwarmManager.Agent_Y[8], SwarmManager.Agent_Z[8]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormPyramidSquarePrizmSucces[8] = true;
            }
        }
        if (AgentID == 9 && SwarmManager.PyramidSquarePrizmInFormation[9])
        {
            PosHoldSetX = Xposition[SwarmManager.PyramidSquarePrizmAssignedLocationNumber[9]];
            PosHoldSetZ = Zposition[SwarmManager.PyramidSquarePrizmAssignedLocationNumber[9]];
            AltHoldSet = Yposition[SwarmManager.PyramidSquarePrizmAssignedLocationNumber[9]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[9], SwarmManager.Agent_Y[9], SwarmManager.Agent_Z[9]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormPyramidSquarePrizmSucces[9] = true;
            }
        }
        if (AgentID == 10 && SwarmManager.PyramidSquarePrizmInFormation[10])
        {
            PosHoldSetX = Xposition[SwarmManager.PyramidSquarePrizmAssignedLocationNumber[10]];
            PosHoldSetZ = Zposition[SwarmManager.PyramidSquarePrizmAssignedLocationNumber[10]];
            AltHoldSet = Yposition[SwarmManager.PyramidSquarePrizmAssignedLocationNumber[10]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[10], SwarmManager.Agent_Y[10], SwarmManager.Agent_Z[10]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormPyramidSquarePrizmSucces[10] = true;
            }
        }
        if (AgentID == 11 && SwarmManager.PyramidSquarePrizmInFormation[11])
        {
            PosHoldSetX = Xposition[SwarmManager.PyramidSquarePrizmAssignedLocationNumber[11]];
            PosHoldSetZ = Zposition[SwarmManager.PyramidSquarePrizmAssignedLocationNumber[11]];
            AltHoldSet = Yposition[SwarmManager.PyramidSquarePrizmAssignedLocationNumber[11]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[11], SwarmManager.Agent_Y[11], SwarmManager.Agent_Z[11]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormPyramidSquarePrizmSucces[11] = true;
            }
        }
        if (AgentID == 12 && SwarmManager.PyramidSquarePrizmInFormation[12])
        {
            PosHoldSetX = Xposition[SwarmManager.PyramidSquarePrizmAssignedLocationNumber[12]];
            PosHoldSetZ = Zposition[SwarmManager.PyramidSquarePrizmAssignedLocationNumber[12]];
            AltHoldSet = Yposition[SwarmManager.PyramidSquarePrizmAssignedLocationNumber[12]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[12], SwarmManager.Agent_Y[12], SwarmManager.Agent_Z[12]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormPyramidSquarePrizmSucces[12] = true;
            }
        }
        if (AgentID == 13 && SwarmManager.PyramidSquarePrizmInFormation[13])
        {
            PosHoldSetX = Xposition[SwarmManager.PyramidSquarePrizmAssignedLocationNumber[13]];
            PosHoldSetZ = Zposition[SwarmManager.PyramidSquarePrizmAssignedLocationNumber[13]];
            AltHoldSet = Yposition[SwarmManager.PyramidSquarePrizmAssignedLocationNumber[13]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[13], SwarmManager.Agent_Y[13], SwarmManager.Agent_Z[13]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormPyramidSquarePrizmSucces[13] = true;
            }
        }

        for (int k = 0; k < 14; k++)
        {
            if (SwarmManager.AgentFormPyramidSquarePrizmSucces[k])
            {
                SwarmManager.pyramidSquarePrizmCounter++;
            }
        }
        if (SwarmManager.pyramidSquarePrizmCounter == SwarmManager.PyramidSquarePrizmAgentRequired)
        {
            SwarmManager.FormPyramidSquarePrizmSucces = true;
            SwarmManager.pyramidSquarePrizmCounter = 0;
        }
        else
        {
            SwarmManager.FormPyramidSquarePrizmSucces = false;
            SwarmManager.pyramidSquarePrizmCounter = 0;
        }
    }

    void TriangleFormation()
    {
        float[] Xposition = new float[3];
        float[] Zposition = new float[3];
        float[] Yposition = new float[3];
        float formationAverageX = 0f;
        float formationAverageY = 0f;
        float formationAverageZ = 0f;

        float traiangle2h = (((SwarmManager.triangleEdge / 2f) * 1.73205f) * 2f) / 3f;  // 1.73205f kök 3 deðeri
        Xposition[0] = SwarmManager.formationTargetMiddleX + traiangle2h * Mathf.Sin((SwarmManager.FormationHeading) * Mathf.Deg2Rad);
        Xposition[1] = SwarmManager.formationTargetMiddleX + traiangle2h * Mathf.Sin((SwarmManager.FormationHeading + 120f) * Mathf.Deg2Rad);
        Xposition[2] = SwarmManager.formationTargetMiddleX + traiangle2h * Mathf.Sin((SwarmManager.FormationHeading + 240f) * Mathf.Deg2Rad);


        Zposition[0] = SwarmManager.formationTargetMiddleZ + traiangle2h * Mathf.Cos((SwarmManager.FormationHeading) * Mathf.Deg2Rad);
        Zposition[1] = SwarmManager.formationTargetMiddleZ + traiangle2h * Mathf.Cos((SwarmManager.FormationHeading + 120f) * Mathf.Deg2Rad);
        Zposition[2] = SwarmManager.formationTargetMiddleZ + traiangle2h * Mathf.Cos((SwarmManager.FormationHeading + 240f) * Mathf.Deg2Rad);


        Yposition[0] = SwarmManager.formationTargetMiddleHeight;
        Yposition[1] = Yposition[0];
        Yposition[2] = Yposition[0];

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 14; j++)
            {
                if (SwarmManager.ActiveAgents[j] && !SwarmManager.TriangleInFormation[j] && !SwarmManager.TrianglePositionFull[i])
                {
                    SwarmManager.TriangleAssignedLocationNumber[j] = i;
                    SwarmManager.TriangleInFormation[j] = true;
                    SwarmManager.TrianglePositionFull[i] = true;
                    break;
                }
                else if (!SwarmManager.ActiveAgents[j] && SwarmManager.TriangleInFormation[j])
                {
                    SwarmManager.TriangleInFormation[j] = false;
                    SwarmManager.TrianglePositionFull[SwarmManager.TriangleAssignedLocationNumber[j]] = false;
                    SwarmManager.AgentFormTriangleSucces[j] = false;
                }
            }
        }

        for (int c = 0; c < 14; c++)
        {
            if (SwarmManager.TriangleInFormation[c])
            {
                formationAverageX += SwarmManager.Agent_X[c];
                formationAverageY += SwarmManager.Agent_Y[c];
                formationAverageZ += SwarmManager.Agent_Z[c];
            }
            if (c == 13)
            {
                SwarmManager.FormationMiddleX = formationAverageX / SwarmManager.TriangleAgentRequired;
                SwarmManager.FormationMiddleY = formationAverageY / SwarmManager.TriangleAgentRequired;
                SwarmManager.FormationMiddleZ = formationAverageZ / SwarmManager.TriangleAgentRequired;
            }
        }


        if (AgentID == 0 && SwarmManager.TriangleInFormation[0])
        {
            PosHoldSetX = Xposition[SwarmManager.TriangleAssignedLocationNumber[0]];
            PosHoldSetZ = Zposition[SwarmManager.TriangleAssignedLocationNumber[0]];
            AltHoldSet = Yposition[SwarmManager.TriangleAssignedLocationNumber[0]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[0], SwarmManager.Agent_Y[0], SwarmManager.Agent_Z[0]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormTriangleSucces[0] = true;
            }
        }
        if (AgentID == 1 && SwarmManager.TriangleInFormation[1])
        {
            PosHoldSetX = Xposition[SwarmManager.TriangleAssignedLocationNumber[1]];
            PosHoldSetZ = Zposition[SwarmManager.TriangleAssignedLocationNumber[1]];
            AltHoldSet = Yposition[SwarmManager.TriangleAssignedLocationNumber[1]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[1], SwarmManager.Agent_Y[1], SwarmManager.Agent_Z[1]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormTriangleSucces[1] = true;
            }
        }
        if (AgentID == 2 && SwarmManager.TriangleInFormation[2])
        {
            PosHoldSetX = Xposition[SwarmManager.TriangleAssignedLocationNumber[2]];
            PosHoldSetZ = Zposition[SwarmManager.TriangleAssignedLocationNumber[2]];
            AltHoldSet = Yposition[SwarmManager.TriangleAssignedLocationNumber[2]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[2], SwarmManager.Agent_Y[2], SwarmManager.Agent_Z[2]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormTriangleSucces[2] = true;
            }
        }
        if (AgentID == 3 && SwarmManager.TriangleInFormation[3])
        {
            PosHoldSetX = Xposition[SwarmManager.TriangleAssignedLocationNumber[3]];
            PosHoldSetZ = Zposition[SwarmManager.TriangleAssignedLocationNumber[3]];
            AltHoldSet = Yposition[SwarmManager.TriangleAssignedLocationNumber[3]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[3], SwarmManager.Agent_Y[3], SwarmManager.Agent_Z[3]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormTriangleSucces[3] = true;
            }
        }
        if (AgentID == 4 && SwarmManager.TriangleInFormation[4])
        {
            PosHoldSetX = Xposition[SwarmManager.TriangleAssignedLocationNumber[4]];
            PosHoldSetZ = Zposition[SwarmManager.TriangleAssignedLocationNumber[4]];
            AltHoldSet = Yposition[SwarmManager.TriangleAssignedLocationNumber[4]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[4], SwarmManager.Agent_Y[4], SwarmManager.Agent_Z[4]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormTriangleSucces[4] = true;
            }
        }
        if (AgentID == 5 && SwarmManager.TriangleInFormation[5])
        {
            PosHoldSetX = Xposition[SwarmManager.TriangleAssignedLocationNumber[5]];
            PosHoldSetZ = Zposition[SwarmManager.TriangleAssignedLocationNumber[5]];
            AltHoldSet = Yposition[SwarmManager.TriangleAssignedLocationNumber[5]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[5], SwarmManager.Agent_Y[5], SwarmManager.Agent_Z[5]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormTriangleSucces[5] = true;
            }
        }
        if (AgentID == 6 && SwarmManager.TriangleInFormation[6])
        {
            PosHoldSetX = Xposition[SwarmManager.TriangleAssignedLocationNumber[6]];
            PosHoldSetZ = Zposition[SwarmManager.TriangleAssignedLocationNumber[6]];
            AltHoldSet = Yposition[SwarmManager.TriangleAssignedLocationNumber[6]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[6], SwarmManager.Agent_Y[6], SwarmManager.Agent_Z[6]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormTriangleSucces[6] = true;
            }
        }
        if (AgentID == 7 && SwarmManager.TriangleInFormation[7])
        {
            PosHoldSetX = Xposition[SwarmManager.TriangleAssignedLocationNumber[7]];
            PosHoldSetZ = Zposition[SwarmManager.TriangleAssignedLocationNumber[7]];
            AltHoldSet = Yposition[SwarmManager.TriangleAssignedLocationNumber[7]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[7], SwarmManager.Agent_Y[7], SwarmManager.Agent_Z[7]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormTriangleSucces[7] = true;
            }
        }
        if (AgentID == 8 && SwarmManager.TriangleInFormation[8])
        {
            PosHoldSetX = Xposition[SwarmManager.TriangleAssignedLocationNumber[8]];
            PosHoldSetZ = Zposition[SwarmManager.TriangleAssignedLocationNumber[8]];
            AltHoldSet = Yposition[SwarmManager.TriangleAssignedLocationNumber[8]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[8], SwarmManager.Agent_Y[8], SwarmManager.Agent_Z[8]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormTriangleSucces[8] = true;
            }
        }
        if (AgentID == 9 && SwarmManager.TriangleInFormation[9])
        {
            PosHoldSetX = Xposition[SwarmManager.TriangleAssignedLocationNumber[9]];
            PosHoldSetZ = Zposition[SwarmManager.TriangleAssignedLocationNumber[9]];
            AltHoldSet = Yposition[SwarmManager.TriangleAssignedLocationNumber[9]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[9], SwarmManager.Agent_Y[9], SwarmManager.Agent_Z[9]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormTriangleSucces[9] = true;
            }
        }
        if (AgentID == 10 && SwarmManager.TriangleInFormation[10])
        {
            PosHoldSetX = Xposition[SwarmManager.TriangleAssignedLocationNumber[10]];
            PosHoldSetZ = Zposition[SwarmManager.TriangleAssignedLocationNumber[10]];
            AltHoldSet = Yposition[SwarmManager.TriangleAssignedLocationNumber[10]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[10], SwarmManager.Agent_Y[10], SwarmManager.Agent_Z[10]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormTriangleSucces[10] = true;
            }
        }
        if (AgentID == 11 && SwarmManager.TriangleInFormation[11])
        {
            PosHoldSetX = Xposition[SwarmManager.TriangleAssignedLocationNumber[11]];
            PosHoldSetZ = Zposition[SwarmManager.TriangleAssignedLocationNumber[11]];
            AltHoldSet = Yposition[SwarmManager.TriangleAssignedLocationNumber[11]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[11], SwarmManager.Agent_Y[11], SwarmManager.Agent_Z[11]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormTriangleSucces[11] = true;
            }
        }
        if (AgentID == 12 && SwarmManager.TriangleInFormation[12])
        {
            PosHoldSetX = Xposition[SwarmManager.TriangleAssignedLocationNumber[12]];
            PosHoldSetZ = Zposition[SwarmManager.TriangleAssignedLocationNumber[12]];
            AltHoldSet = Yposition[SwarmManager.TriangleAssignedLocationNumber[12]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[12], SwarmManager.Agent_Y[12], SwarmManager.Agent_Z[12]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormTriangleSucces[12] = true;
            }
        }
        if (AgentID == 13 && SwarmManager.TriangleInFormation[13])
        {
            PosHoldSetX = Xposition[SwarmManager.TriangleAssignedLocationNumber[13]];
            PosHoldSetZ = Zposition[SwarmManager.TriangleAssignedLocationNumber[13]];
            AltHoldSet = Yposition[SwarmManager.TriangleAssignedLocationNumber[13]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[13], SwarmManager.Agent_Y[13], SwarmManager.Agent_Z[13]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormTriangleSucces[13] = true;
            }
        }


        for (int k = 0; k < 14; k++)
        {
            if (SwarmManager.AgentFormTriangleSucces[k])
            {
                SwarmManager.triangleCounter++;
            }
        }
        if (SwarmManager.triangleCounter == SwarmManager.TriangleAgentRequired)
        {
            SwarmManager.FormTriangleSucces = true;
            SwarmManager.triangleCounter = 0;
        }
        else
        {
            SwarmManager.FormTriangleSucces = false;
            SwarmManager.triangleCounter = 0;
        }

    }
    
    void TrianglePrizmFormation()
    {
        float[] Xposition = new float[6];
        float[] Zposition = new float[6];
        float[] Yposition = new float[6];

        float formationAverageX = 0f;
        float formationAverageY = 0f;
        float formationAverageZ = 0f;

        float traiangle2h = (((SwarmManager.trianglePrizmEdge / 2f) * 1.73205f) * 2f) / 3f;  // 1.73205f kök 3 deðeri
        Xposition[0] = SwarmManager.formationTargetMiddleX + traiangle2h * Mathf.Sin((SwarmManager.FormationHeading) * Mathf.Deg2Rad);
        Xposition[1] = SwarmManager.formationTargetMiddleX + traiangle2h * Mathf.Sin((SwarmManager.FormationHeading + 120f) * Mathf.Deg2Rad);
        Xposition[2] = SwarmManager.formationTargetMiddleX + traiangle2h * Mathf.Sin((SwarmManager.FormationHeading + 240f) * Mathf.Deg2Rad);

        Zposition[0] = SwarmManager.formationTargetMiddleZ + traiangle2h * Mathf.Cos((SwarmManager.FormationHeading) * Mathf.Deg2Rad);
        Zposition[1] = SwarmManager.formationTargetMiddleZ + traiangle2h * Mathf.Cos((SwarmManager.FormationHeading + 120f) * Mathf.Deg2Rad);
        Zposition[2] = SwarmManager.formationTargetMiddleZ + traiangle2h * Mathf.Cos((SwarmManager.FormationHeading + 240f) * Mathf.Deg2Rad);

        Yposition[0] = SwarmManager.formationTargetMiddleHeight + (SwarmManager.trianglePrizmHeight / 2f);
        Yposition[1] = Yposition[0];
        Yposition[2] = Yposition[0];




        if (SwarmManager.levelDelay > 0)
        {
            SwarmManager.levelDelay -= Time.deltaTime;
        }
        if (SwarmManager.levelDelay <= 0)
        {
            Xposition[3] = Xposition[0];
            Xposition[4] = Xposition[1];
            Xposition[5] = Xposition[2];

            Zposition[3] = Zposition[0];
            Zposition[4] = Zposition[1];
            Zposition[5] = Zposition[2];

            Yposition[3] = SwarmManager.formationTargetMiddleHeight - (SwarmManager.trianglePrizmHeight / 2f);
            Yposition[4] = Yposition[3];
            Yposition[5] = Yposition[3];
        }
        else
        {
            Xposition[3] = LandingZone.transform.position.x;
            Xposition[4] = LandingZone.transform.position.x;
            Xposition[5] = LandingZone.transform.position.x;

            Zposition[3] = LandingZone.transform.position.z;
            Zposition[4] = LandingZone.transform.position.z;
            Zposition[5] = LandingZone.transform.position.z;

            Yposition[3] = SwarmManager.takeOffHight;
            Yposition[4] = SwarmManager.takeOffHight;
            Yposition[5] = SwarmManager.takeOffHight;
        }



        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 14; j++)
            {
                if (SwarmManager.ActiveAgents[j] && !SwarmManager.TrianglePrizmInFormation[j] && !SwarmManager.TrianglePrizmPositionFull[i])
                {
                    SwarmManager.TrianglePrizmAssignedLocationNumber[j] = i;
                    SwarmManager.TrianglePrizmInFormation[j] = true;
                    SwarmManager.TrianglePrizmPositionFull[i] = true;
                    break;
                }
                else if (!SwarmManager.ActiveAgents[j] && SwarmManager.TrianglePrizmInFormation[j])
                {
                    SwarmManager.TrianglePrizmInFormation[j] = false;
                    SwarmManager.TrianglePrizmPositionFull[SwarmManager.TrianglePrizmAssignedLocationNumber[j]] = false;
                    SwarmManager.AgentFormTrianglePrizmSucces[j] = false;
                }
            }
        }


        for (int c = 0; c < 14; c++)
        {
            if (SwarmManager.TrianglePrizmInFormation[c])
            {
                formationAverageX += SwarmManager.Agent_X[c];
                formationAverageY += SwarmManager.Agent_Y[c];
                formationAverageZ += SwarmManager.Agent_Z[c];
            }
            if (c == 13)
            {
                SwarmManager.FormationMiddleX = formationAverageX / SwarmManager.TrianglePrizmAgentRequired;
                SwarmManager.FormationMiddleY = formationAverageY / SwarmManager.TrianglePrizmAgentRequired;
                SwarmManager.FormationMiddleZ = formationAverageZ / SwarmManager.TrianglePrizmAgentRequired;
            }
        }


        if (AgentID == 0 && SwarmManager.TrianglePrizmInFormation[0])
        {
            PosHoldSetX = Xposition[SwarmManager.TrianglePrizmAssignedLocationNumber[0]];
            PosHoldSetZ = Zposition[SwarmManager.TrianglePrizmAssignedLocationNumber[0]];
            AltHoldSet = Yposition[SwarmManager.TrianglePrizmAssignedLocationNumber[0]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[0], SwarmManager.Agent_Y[0], SwarmManager.Agent_Z[0]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormTrianglePrizmSucces[0] = true;
            }
        }
        if (AgentID == 1 && SwarmManager.TrianglePrizmInFormation[1])
        {
            PosHoldSetX = Xposition[SwarmManager.TrianglePrizmAssignedLocationNumber[1]];
            PosHoldSetZ = Zposition[SwarmManager.TrianglePrizmAssignedLocationNumber[1]];
            AltHoldSet = Yposition[SwarmManager.TrianglePrizmAssignedLocationNumber[1]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[1], SwarmManager.Agent_Y[1], SwarmManager.Agent_Z[1]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormTrianglePrizmSucces[1] = true;
            }
        }
        if (AgentID == 2 && SwarmManager.TrianglePrizmInFormation[2])
        {
            PosHoldSetX = Xposition[SwarmManager.TrianglePrizmAssignedLocationNumber[2]];
            PosHoldSetZ = Zposition[SwarmManager.TrianglePrizmAssignedLocationNumber[2]];
            AltHoldSet = Yposition[SwarmManager.TrianglePrizmAssignedLocationNumber[2]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[2], SwarmManager.Agent_Y[2], SwarmManager.Agent_Z[2]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormTrianglePrizmSucces[2] = true;
            }
        }
        if (AgentID == 3 && SwarmManager.TrianglePrizmInFormation[3])
        {
            PosHoldSetX = Xposition[SwarmManager.TrianglePrizmAssignedLocationNumber[3]];
            PosHoldSetZ = Zposition[SwarmManager.TrianglePrizmAssignedLocationNumber[3]];
            AltHoldSet = Yposition[SwarmManager.TrianglePrizmAssignedLocationNumber[3]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[3], SwarmManager.Agent_Y[3], SwarmManager.Agent_Z[3]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormTrianglePrizmSucces[3] = true;
            }
        }
        if (AgentID == 4 && SwarmManager.TrianglePrizmInFormation[4])
        {
            PosHoldSetX = Xposition[SwarmManager.TrianglePrizmAssignedLocationNumber[4]];
            PosHoldSetZ = Zposition[SwarmManager.TrianglePrizmAssignedLocationNumber[4]];
            AltHoldSet = Yposition[SwarmManager.TrianglePrizmAssignedLocationNumber[4]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[4], SwarmManager.Agent_Y[4], SwarmManager.Agent_Z[4]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormTrianglePrizmSucces[4] = true;
            }
        }
        if (AgentID == 5 && SwarmManager.TrianglePrizmInFormation[5])
        {
            PosHoldSetX = Xposition[SwarmManager.TrianglePrizmAssignedLocationNumber[5]];
            PosHoldSetZ = Zposition[SwarmManager.TrianglePrizmAssignedLocationNumber[5]];
            AltHoldSet = Yposition[SwarmManager.TrianglePrizmAssignedLocationNumber[5]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[5], SwarmManager.Agent_Y[5], SwarmManager.Agent_Z[5]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormTrianglePrizmSucces[5] = true;
            }
        }
        if (AgentID == 6 && SwarmManager.TrianglePrizmInFormation[6])
        {
            PosHoldSetX = Xposition[SwarmManager.TrianglePrizmAssignedLocationNumber[6]];
            PosHoldSetZ = Zposition[SwarmManager.TrianglePrizmAssignedLocationNumber[6]];
            AltHoldSet = Yposition[SwarmManager.TrianglePrizmAssignedLocationNumber[6]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[6], SwarmManager.Agent_Y[6], SwarmManager.Agent_Z[6]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormTrianglePrizmSucces[6] = true;
            }
        }
        if (AgentID == 7 && SwarmManager.TrianglePrizmInFormation[7])
        {
            PosHoldSetX = Xposition[SwarmManager.TrianglePrizmAssignedLocationNumber[7]];
            PosHoldSetZ = Zposition[SwarmManager.TrianglePrizmAssignedLocationNumber[7]];
            AltHoldSet = Yposition[SwarmManager.TrianglePrizmAssignedLocationNumber[7]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[7], SwarmManager.Agent_Y[7], SwarmManager.Agent_Z[7]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormTrianglePrizmSucces[7] = true;
            }
        }
        if (AgentID == 8 && SwarmManager.TrianglePrizmInFormation[8])
        {
            PosHoldSetX = Xposition[SwarmManager.TrianglePrizmAssignedLocationNumber[8]];
            PosHoldSetZ = Zposition[SwarmManager.TrianglePrizmAssignedLocationNumber[8]];
            AltHoldSet = Yposition[SwarmManager.TrianglePrizmAssignedLocationNumber[8]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[8], SwarmManager.Agent_Y[8], SwarmManager.Agent_Z[8]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormTrianglePrizmSucces[8] = true;
            }
        }
        if (AgentID == 9 && SwarmManager.TrianglePrizmInFormation[9])
        {
            PosHoldSetX = Xposition[SwarmManager.TrianglePrizmAssignedLocationNumber[9]];
            PosHoldSetZ = Zposition[SwarmManager.TrianglePrizmAssignedLocationNumber[9]];
            AltHoldSet = Yposition[SwarmManager.TrianglePrizmAssignedLocationNumber[9]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[9], SwarmManager.Agent_Y[9], SwarmManager.Agent_Z[9]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormTrianglePrizmSucces[9] = true;
            }
        }
        if (AgentID == 10 && SwarmManager.TrianglePrizmInFormation[10])
        {
            PosHoldSetX = Xposition[SwarmManager.TrianglePrizmAssignedLocationNumber[10]];
            PosHoldSetZ = Zposition[SwarmManager.TrianglePrizmAssignedLocationNumber[10]];
            AltHoldSet = Yposition[SwarmManager.TrianglePrizmAssignedLocationNumber[10]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[10], SwarmManager.Agent_Y[10], SwarmManager.Agent_Z[10]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormTrianglePrizmSucces[10] = true;
            }
        }
        if (AgentID == 11 && SwarmManager.TrianglePrizmInFormation[11])
        {
            PosHoldSetX = Xposition[SwarmManager.TrianglePrizmAssignedLocationNumber[11]];
            PosHoldSetZ = Zposition[SwarmManager.TrianglePrizmAssignedLocationNumber[11]];
            AltHoldSet = Yposition[SwarmManager.TrianglePrizmAssignedLocationNumber[11]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[11], SwarmManager.Agent_Y[11], SwarmManager.Agent_Z[11]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormTrianglePrizmSucces[11] = true;
            }
        }
        if (AgentID == 12 && SwarmManager.TrianglePrizmInFormation[12])
        {
            PosHoldSetX = Xposition[SwarmManager.TrianglePrizmAssignedLocationNumber[12]];
            PosHoldSetZ = Zposition[SwarmManager.TrianglePrizmAssignedLocationNumber[12]];
            AltHoldSet = Yposition[SwarmManager.TrianglePrizmAssignedLocationNumber[12]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[12], SwarmManager.Agent_Y[12], SwarmManager.Agent_Z[12]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormTrianglePrizmSucces[12] = true;
            }
        }
        if (AgentID == 13 && SwarmManager.TrianglePrizmInFormation[13])
        {
            PosHoldSetX = Xposition[SwarmManager.TrianglePrizmAssignedLocationNumber[13]];
            PosHoldSetZ = Zposition[SwarmManager.TrianglePrizmAssignedLocationNumber[13]];
            AltHoldSet = Yposition[SwarmManager.TrianglePrizmAssignedLocationNumber[13]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[13], SwarmManager.Agent_Y[13], SwarmManager.Agent_Z[13]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormTrianglePrizmSucces[13] = true;
            }
        }


        for (int k = 0; k < 14; k++)
        {
            if (SwarmManager.AgentFormTrianglePrizmSucces[k])
            {
                SwarmManager.trianglePrizmCounter++;
            }
        }
        if (SwarmManager.trianglePrizmCounter == SwarmManager.TrianglePrizmAgentRequired)
        {
            SwarmManager.FormTrianglePrizmSucces = true;
            SwarmManager.trianglePrizmCounter = 0;
        }
        else
        {
            SwarmManager.FormTrianglePrizmSucces = false;
            SwarmManager.trianglePrizmCounter = 0;
        }

    }

    void SquareFormation()
    {
        float[] Xposition = new float[4];
        float[] Zposition = new float[4];
        float[] Yposition = new float[4];

        float formationAverageX = 0f;
        float formationAverageY = 0f;
        float formationAverageZ = 0f;

        float hypotenus = Mathf.Sqrt((Mathf.Pow((SwarmManager.squareEdge / 2f), 2)) + (Mathf.Pow((SwarmManager.squareEdge / 2f), 2)));
        Xposition[0] = SwarmManager.formationTargetMiddleX + hypotenus * Mathf.Sin((SwarmManager.FormationHeading + 45f) * Mathf.Deg2Rad);
        Xposition[1] = SwarmManager.formationTargetMiddleX + hypotenus * Mathf.Sin((SwarmManager.FormationHeading + 135f) * Mathf.Deg2Rad);
        Xposition[2] = SwarmManager.formationTargetMiddleX + hypotenus * Mathf.Sin((SwarmManager.FormationHeading + 225f) * Mathf.Deg2Rad);
        Xposition[3] = SwarmManager.formationTargetMiddleX + hypotenus * Mathf.Sin((SwarmManager.FormationHeading + 315f) * Mathf.Deg2Rad);

        Zposition[0] = SwarmManager.formationTargetMiddleZ + hypotenus * Mathf.Cos((SwarmManager.FormationHeading + 45f) * Mathf.Deg2Rad);
        Zposition[1] = SwarmManager.formationTargetMiddleZ + hypotenus * Mathf.Cos((SwarmManager.FormationHeading + 135f) * Mathf.Deg2Rad);
        Zposition[2] = SwarmManager.formationTargetMiddleZ + hypotenus * Mathf.Cos((SwarmManager.FormationHeading + 225f) * Mathf.Deg2Rad);
        Zposition[3] = SwarmManager.formationTargetMiddleZ + hypotenus * Mathf.Cos((SwarmManager.FormationHeading + 315f) * Mathf.Deg2Rad);

        Yposition[0] = SwarmManager.formationTargetMiddleHeight;
        Yposition[1] = Yposition[0];
        Yposition[2] = Yposition[0];
        Yposition[3] = Yposition[0];


        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 14; j++)
            {
                if (SwarmManager.ActiveAgents[j] && !SwarmManager.SquareInFormation[j] && !SwarmManager.SquarePositionFull[i])
                {
                    SwarmManager.SquareAssignedLocationNumber[j] = i;
                    SwarmManager.SquareInFormation[j] = true;
                    SwarmManager.SquarePositionFull[i] = true;
                    break;
                }
                else if (!SwarmManager.ActiveAgents[j] && SwarmManager.SquareInFormation[j])
                {
                    SwarmManager.SquareInFormation[j] = false;
                    SwarmManager.SquarePositionFull[SwarmManager.SquareAssignedLocationNumber[j]] = false;
                    SwarmManager.AgentFormSquareSucces[j] = false;
                }
            }
        }

        for (int c = 0; c < 14; c++)
        {
            if (SwarmManager.SquareInFormation[c])
            {
                formationAverageX += SwarmManager.Agent_X[c];
                formationAverageY += SwarmManager.Agent_Y[c];
                formationAverageZ += SwarmManager.Agent_Z[c];
            }
            if (c == 13)
            {
                SwarmManager.FormationMiddleX = formationAverageX / SwarmManager.SquareAgentRequired;
                SwarmManager.FormationMiddleY = formationAverageY / SwarmManager.SquareAgentRequired;
                SwarmManager.FormationMiddleZ = formationAverageZ / SwarmManager.SquareAgentRequired;
            }
        }

        if (AgentID == 0 && SwarmManager.SquareInFormation[0])
        {
            PosHoldSetX = Xposition[SwarmManager.SquareAssignedLocationNumber[0]];
            PosHoldSetZ = Zposition[SwarmManager.SquareAssignedLocationNumber[0]];
            AltHoldSet = Yposition[SwarmManager.SquareAssignedLocationNumber[0]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[0], SwarmManager.Agent_Y[0], SwarmManager.Agent_Z[0]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormSquareSucces[0] = true;
            }
        }
        if (AgentID == 1 && SwarmManager.SquareInFormation[1])
        {
            PosHoldSetX = Xposition[SwarmManager.SquareAssignedLocationNumber[1]];
            PosHoldSetZ = Zposition[SwarmManager.SquareAssignedLocationNumber[1]];
            AltHoldSet = Yposition[SwarmManager.SquareAssignedLocationNumber[1]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[1], SwarmManager.Agent_Y[1], SwarmManager.Agent_Z[1]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormSquareSucces[1] = true;
            }
        }
        if (AgentID == 2 && SwarmManager.SquareInFormation[2])
        {
            PosHoldSetX = Xposition[SwarmManager.SquareAssignedLocationNumber[2]];
            PosHoldSetZ = Zposition[SwarmManager.SquareAssignedLocationNumber[2]];
            AltHoldSet = Yposition[SwarmManager.SquareAssignedLocationNumber[2]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[2], SwarmManager.Agent_Y[2], SwarmManager.Agent_Z[2]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormSquareSucces[2] = true;
            }
        }
        if (AgentID == 3 && SwarmManager.SquareInFormation[3])
        {
            PosHoldSetX = Xposition[SwarmManager.SquareAssignedLocationNumber[3]];
            PosHoldSetZ = Zposition[SwarmManager.SquareAssignedLocationNumber[3]];
            AltHoldSet = Yposition[SwarmManager.SquareAssignedLocationNumber[3]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[3], SwarmManager.Agent_Y[3], SwarmManager.Agent_Z[3]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormSquareSucces[3] = true;
            }
        }
        if (AgentID == 4 && SwarmManager.SquareInFormation[4])
        {
            PosHoldSetX = Xposition[SwarmManager.SquareAssignedLocationNumber[4]];
            PosHoldSetZ = Zposition[SwarmManager.SquareAssignedLocationNumber[4]];
            AltHoldSet = Yposition[SwarmManager.SquareAssignedLocationNumber[4]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[4], SwarmManager.Agent_Y[4], SwarmManager.Agent_Z[4]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormSquareSucces[4] = true;
            }
        }
        if (AgentID == 5 && SwarmManager.SquareInFormation[5])
        {
            PosHoldSetX = Xposition[SwarmManager.SquareAssignedLocationNumber[5]];
            PosHoldSetZ = Zposition[SwarmManager.SquareAssignedLocationNumber[5]];
            AltHoldSet = Yposition[SwarmManager.SquareAssignedLocationNumber[5]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[5], SwarmManager.Agent_Y[5], SwarmManager.Agent_Z[5]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormSquareSucces[5] = true;
            }
        }
        if (AgentID == 6 && SwarmManager.SquareInFormation[6])
        {
            PosHoldSetX = Xposition[SwarmManager.SquareAssignedLocationNumber[6]];
            PosHoldSetZ = Zposition[SwarmManager.SquareAssignedLocationNumber[6]];
            AltHoldSet = Yposition[SwarmManager.SquareAssignedLocationNumber[6]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[6], SwarmManager.Agent_Y[6], SwarmManager.Agent_Z[6]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormSquareSucces[6] = true;
            }
        }
        if (AgentID == 7 && SwarmManager.SquareInFormation[7])
        {
            PosHoldSetX = Xposition[SwarmManager.SquareAssignedLocationNumber[7]];
            PosHoldSetZ = Zposition[SwarmManager.SquareAssignedLocationNumber[7]];
            AltHoldSet = Yposition[SwarmManager.SquareAssignedLocationNumber[7]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[7], SwarmManager.Agent_Y[7], SwarmManager.Agent_Z[7]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormSquareSucces[7] = true;
            }
        }
        if (AgentID == 8 && SwarmManager.SquareInFormation[8])
        {
            PosHoldSetX = Xposition[SwarmManager.SquareAssignedLocationNumber[8]];
            PosHoldSetZ = Zposition[SwarmManager.SquareAssignedLocationNumber[8]];
            AltHoldSet = Yposition[SwarmManager.SquareAssignedLocationNumber[8]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[8], SwarmManager.Agent_Y[8], SwarmManager.Agent_Z[8]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormSquareSucces[8] = true;
            }
        }
        if (AgentID == 9 && SwarmManager.SquareInFormation[9])
        {
            PosHoldSetX = Xposition[SwarmManager.SquareAssignedLocationNumber[9]];
            PosHoldSetZ = Zposition[SwarmManager.SquareAssignedLocationNumber[9]];
            AltHoldSet = Yposition[SwarmManager.SquareAssignedLocationNumber[9]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[9], SwarmManager.Agent_Y[9], SwarmManager.Agent_Z[9]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormSquareSucces[9] = true;
            }
        }
        if (AgentID == 10 && SwarmManager.SquareInFormation[10])
        {
            PosHoldSetX = Xposition[SwarmManager.SquareAssignedLocationNumber[10]];
            PosHoldSetZ = Zposition[SwarmManager.SquareAssignedLocationNumber[10]];
            AltHoldSet = Yposition[SwarmManager.SquareAssignedLocationNumber[10]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[10], SwarmManager.Agent_Y[10], SwarmManager.Agent_Z[10]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormSquareSucces[10] = true;
            }
        }
        if (AgentID == 11 && SwarmManager.SquareInFormation[11])
        {
            PosHoldSetX = Xposition[SwarmManager.SquareAssignedLocationNumber[11]];
            PosHoldSetZ = Zposition[SwarmManager.SquareAssignedLocationNumber[11]];
            AltHoldSet = Yposition[SwarmManager.SquareAssignedLocationNumber[11]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[11], SwarmManager.Agent_Y[11], SwarmManager.Agent_Z[11]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormSquareSucces[11] = true;
            }
        }
        if (AgentID == 12 && SwarmManager.SquareInFormation[12])
        {
            PosHoldSetX = Xposition[SwarmManager.SquareAssignedLocationNumber[12]];
            PosHoldSetZ = Zposition[SwarmManager.SquareAssignedLocationNumber[12]];
            AltHoldSet = Yposition[SwarmManager.SquareAssignedLocationNumber[12]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[12], SwarmManager.Agent_Y[12], SwarmManager.Agent_Z[12]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormSquareSucces[12] = true;
            }
        }
        if (AgentID == 13 && SwarmManager.SquareInFormation[13])
        {
            PosHoldSetX = Xposition[SwarmManager.SquareAssignedLocationNumber[13]];
            PosHoldSetZ = Zposition[SwarmManager.SquareAssignedLocationNumber[13]];
            AltHoldSet = Yposition[SwarmManager.SquareAssignedLocationNumber[13]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[13], SwarmManager.Agent_Y[13], SwarmManager.Agent_Z[13]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormSquareSucces[13] = true;
            }
        }

        for (int k = 0; k < 14; k++)
        {
            if (SwarmManager.AgentFormSquareSucces[k])
            {
                SwarmManager.squareCounter++;
            }
        }
        if (SwarmManager.squareCounter == SwarmManager.SquareAgentRequired)
        {
            SwarmManager.FormSquareSucces = true;
            SwarmManager.squareCounter = 0;
        }
        else
        {
            SwarmManager.FormSquareSucces = false;
            SwarmManager.squareCounter = 0;
        }
    }

    void HexagonPrizmFormation()
    {
        float[] Xposition = new float[12];
        float[] Zposition = new float[12];
        float[] Yposition = new float[12];

        float formationAverageX = 0f;
        float formationAverageY = 0f;
        float formationAverageZ = 0f;

        Xposition[0] = SwarmManager.formationTargetMiddleX + SwarmManager.hexagonPrizmEdge * Mathf.Sin((SwarmManager.FormationHeading + 30f) * Mathf.Deg2Rad);
        Xposition[1] = SwarmManager.formationTargetMiddleX + SwarmManager.hexagonPrizmEdge * Mathf.Sin((SwarmManager.FormationHeading + 90f) * Mathf.Deg2Rad);
        Xposition[2] = SwarmManager.formationTargetMiddleX + SwarmManager.hexagonPrizmEdge * Mathf.Sin((SwarmManager.FormationHeading + 150f) * Mathf.Deg2Rad);
        Xposition[3] = SwarmManager.formationTargetMiddleX + SwarmManager.hexagonPrizmEdge * Mathf.Sin((SwarmManager.FormationHeading + 210f) * Mathf.Deg2Rad);
        Xposition[4] = SwarmManager.formationTargetMiddleX + SwarmManager.hexagonPrizmEdge * Mathf.Sin((SwarmManager.FormationHeading + 270f) * Mathf.Deg2Rad);
        Xposition[5] = SwarmManager.formationTargetMiddleX + SwarmManager.hexagonPrizmEdge * Mathf.Sin((SwarmManager.FormationHeading + 330f) * Mathf.Deg2Rad);


        Zposition[0] = SwarmManager.formationTargetMiddleZ + SwarmManager.hexagonPrizmEdge * Mathf.Cos((SwarmManager.FormationHeading + 30f) * Mathf.Deg2Rad);
        Zposition[1] = SwarmManager.formationTargetMiddleZ + SwarmManager.hexagonPrizmEdge * Mathf.Cos((SwarmManager.FormationHeading + 90f) * Mathf.Deg2Rad);
        Zposition[2] = SwarmManager.formationTargetMiddleZ + SwarmManager.hexagonPrizmEdge * Mathf.Cos((SwarmManager.FormationHeading + 150f) * Mathf.Deg2Rad);
        Zposition[3] = SwarmManager.formationTargetMiddleZ + SwarmManager.hexagonPrizmEdge * Mathf.Cos((SwarmManager.FormationHeading + 210f) * Mathf.Deg2Rad);
        Zposition[4] = SwarmManager.formationTargetMiddleZ + SwarmManager.hexagonPrizmEdge * Mathf.Cos((SwarmManager.FormationHeading + 270f) * Mathf.Deg2Rad);
        Zposition[5] = SwarmManager.formationTargetMiddleZ + SwarmManager.hexagonPrizmEdge * Mathf.Cos((SwarmManager.FormationHeading + 330f) * Mathf.Deg2Rad);
       
             
        Yposition[0] = SwarmManager.formationTargetMiddleHeight + (SwarmManager.hexagonPrizmHeight / 2f);
        Yposition[1] = Yposition[0];
        Yposition[2] = Yposition[0];
        Yposition[3] = Yposition[0];
        Yposition[4] = Yposition[0];
        Yposition[5] = Yposition[0];
        


        if (SwarmManager.levelDelay > 0)
        {
            SwarmManager.levelDelay -= Time.deltaTime;
        }
        if (SwarmManager.levelDelay <= 0)
        {
            Xposition[6] = Xposition[0];
            Xposition[7] = Xposition[1];
            Xposition[8] = Xposition[2];
            Xposition[9] = Xposition[3];
            Xposition[10] = Xposition[4];
            Xposition[11] = Xposition[5];

            Zposition[6] = Zposition[0];
            Zposition[7] = Zposition[1];
            Zposition[8] = Zposition[2];
            Zposition[9] = Zposition[3];
            Zposition[10] = Zposition[4];
            Zposition[11] = Zposition[5];

            Yposition[6] = SwarmManager.formationTargetMiddleHeight - (SwarmManager.hexagonPrizmHeight / 2f);
            Yposition[7] = Yposition[6];
            Yposition[8] = Yposition[6];
            Yposition[9] = Yposition[6];
            Yposition[10] = Yposition[6];
            Yposition[11] = Yposition[6];
        }
        else
        {
            Xposition[6] = LandingZone.transform.position.x;
            Xposition[7] = LandingZone.transform.position.x;
            Xposition[8] = LandingZone.transform.position.x;
            Xposition[9] = LandingZone.transform.position.x;
            Xposition[10] = LandingZone.transform.position.x;
            Xposition[11] = LandingZone.transform.position.x;

            Zposition[6] = LandingZone.transform.position.z;
            Zposition[7] = LandingZone.transform.position.z;
            Zposition[8] = LandingZone.transform.position.z;
            Zposition[9] = LandingZone.transform.position.z;
            Zposition[10] = LandingZone.transform.position.z;
            Zposition[11] = LandingZone.transform.position.z;

            Yposition[6] = SwarmManager.takeOffHight;
            Yposition[7] = SwarmManager.takeOffHight;
            Yposition[8] = SwarmManager.takeOffHight;
            Yposition[9] = SwarmManager.takeOffHight;
            Yposition[10] = SwarmManager.takeOffHight;
            Yposition[11] = SwarmManager.takeOffHight;
        }



        for (int i = 0; i < 12; i++)
        {
            for (int j = 0; j < 14; j++)
            {
                if (SwarmManager.ActiveAgents[j] && !SwarmManager.HexagonPrizmInFormation[j] && !SwarmManager.HexagonPrizmPositionFull[i])
                {
                    SwarmManager.HexagonPrizmAssignedLocationNumber[j] = i;
                    SwarmManager.HexagonPrizmInFormation[j] = true;
                    SwarmManager.HexagonPrizmPositionFull[i] = true;
                    break;
                }
                else if (!SwarmManager.ActiveAgents[j] && SwarmManager.HexagonPrizmInFormation[j])
                {
                    SwarmManager.HexagonPrizmInFormation[j] = false;
                    SwarmManager.HexagonPrizmPositionFull[SwarmManager.HexagonPrizmAssignedLocationNumber[j]] = false;
                    SwarmManager.AgentFormHexagonPrizmSucces[j] = false;
                }
            }
        }

        for (int c = 0; c < 14; c++)
        {
            if (SwarmManager.HexagonPrizmInFormation[c])
            {
                formationAverageX += SwarmManager.Agent_X[c];
                formationAverageY += SwarmManager.Agent_Y[c];
                formationAverageZ += SwarmManager.Agent_Z[c];
            }
            if (c == 13)
            {
                SwarmManager.FormationMiddleX = formationAverageX / SwarmManager.HexagonPrizmAgentRequired;
                SwarmManager.FormationMiddleY = formationAverageY / SwarmManager.HexagonPrizmAgentRequired;
                SwarmManager.FormationMiddleZ = formationAverageZ / SwarmManager.HexagonPrizmAgentRequired;
            }
        }

        if (AgentID == 0 && SwarmManager.HexagonPrizmInFormation[0])
        {
            PosHoldSetX = Xposition[SwarmManager.HexagonPrizmAssignedLocationNumber[0]];
            PosHoldSetZ = Zposition[SwarmManager.HexagonPrizmAssignedLocationNumber[0]];
            AltHoldSet = Yposition[SwarmManager.HexagonPrizmAssignedLocationNumber[0]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[0], SwarmManager.Agent_Y[0], SwarmManager.Agent_Z[0]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormHexagonPrizmSucces[0] = true;
            }
        }
        if (AgentID == 1 && SwarmManager.HexagonPrizmInFormation[1])
        {
            PosHoldSetX = Xposition[SwarmManager.HexagonPrizmAssignedLocationNumber[1]];
            PosHoldSetZ = Zposition[SwarmManager.HexagonPrizmAssignedLocationNumber[1]];
            AltHoldSet = Yposition[SwarmManager.HexagonPrizmAssignedLocationNumber[1]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[1], SwarmManager.Agent_Y[1], SwarmManager.Agent_Z[1]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormHexagonPrizmSucces[1] = true;
            }
        }
        if (AgentID == 2 && SwarmManager.HexagonPrizmInFormation[2])
        {
            PosHoldSetX = Xposition[SwarmManager.HexagonPrizmAssignedLocationNumber[2]];
            PosHoldSetZ = Zposition[SwarmManager.HexagonPrizmAssignedLocationNumber[2]];
            AltHoldSet = Yposition[SwarmManager.HexagonPrizmAssignedLocationNumber[2]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[2], SwarmManager.Agent_Y[2], SwarmManager.Agent_Z[2]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormHexagonPrizmSucces[2] = true;
            }
        }
        if (AgentID == 3 && SwarmManager.HexagonPrizmInFormation[3])
        {
            PosHoldSetX = Xposition[SwarmManager.HexagonPrizmAssignedLocationNumber[3]];
            PosHoldSetZ = Zposition[SwarmManager.HexagonPrizmAssignedLocationNumber[3]];
            AltHoldSet = Yposition[SwarmManager.HexagonPrizmAssignedLocationNumber[3]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[3], SwarmManager.Agent_Y[3], SwarmManager.Agent_Z[3]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormHexagonPrizmSucces[3] = true;
            }
        }
        if (AgentID == 4 && SwarmManager.HexagonPrizmInFormation[4])
        {
            PosHoldSetX = Xposition[SwarmManager.HexagonPrizmAssignedLocationNumber[4]];
            PosHoldSetZ = Zposition[SwarmManager.HexagonPrizmAssignedLocationNumber[4]];
            AltHoldSet = Yposition[SwarmManager.HexagonPrizmAssignedLocationNumber[4]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[4], SwarmManager.Agent_Y[4], SwarmManager.Agent_Z[4]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormHexagonPrizmSucces[4] = true;
            }
        }
        if (AgentID == 5 && SwarmManager.HexagonPrizmInFormation[5])
        {
            PosHoldSetX = Xposition[SwarmManager.HexagonPrizmAssignedLocationNumber[5]];
            PosHoldSetZ = Zposition[SwarmManager.HexagonPrizmAssignedLocationNumber[5]];
            AltHoldSet = Yposition[SwarmManager.HexagonPrizmAssignedLocationNumber[5]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[5], SwarmManager.Agent_Y[5], SwarmManager.Agent_Z[5]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormHexagonPrizmSucces[5] = true;
            }
        }
        if (AgentID == 6 && SwarmManager.HexagonPrizmInFormation[6])
        {
            PosHoldSetX = Xposition[SwarmManager.HexagonPrizmAssignedLocationNumber[6]];
            PosHoldSetZ = Zposition[SwarmManager.HexagonPrizmAssignedLocationNumber[6]];
            AltHoldSet = Yposition[SwarmManager.HexagonPrizmAssignedLocationNumber[6]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[6], SwarmManager.Agent_Y[6], SwarmManager.Agent_Z[6]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormHexagonPrizmSucces[6] = true;
            }
        }
        if (AgentID == 7 && SwarmManager.HexagonPrizmInFormation[7])
        {
            PosHoldSetX = Xposition[SwarmManager.HexagonPrizmAssignedLocationNumber[7]];
            PosHoldSetZ = Zposition[SwarmManager.HexagonPrizmAssignedLocationNumber[7]];
            AltHoldSet = Yposition[SwarmManager.HexagonPrizmAssignedLocationNumber[7]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[7], SwarmManager.Agent_Y[7], SwarmManager.Agent_Z[7]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormHexagonPrizmSucces[7] = true;
            }
        }
        if (AgentID == 8 && SwarmManager.HexagonPrizmInFormation[8])
        {
            PosHoldSetX = Xposition[SwarmManager.HexagonPrizmAssignedLocationNumber[8]];
            PosHoldSetZ = Zposition[SwarmManager.HexagonPrizmAssignedLocationNumber[8]];
            AltHoldSet = Yposition[SwarmManager.HexagonPrizmAssignedLocationNumber[8]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[8], SwarmManager.Agent_Y[8], SwarmManager.Agent_Z[8]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormHexagonPrizmSucces[8] = true;
            }
        }
        if (AgentID == 9 && SwarmManager.HexagonPrizmInFormation[9])
        {
            PosHoldSetX = Xposition[SwarmManager.HexagonPrizmAssignedLocationNumber[9]];
            PosHoldSetZ = Zposition[SwarmManager.HexagonPrizmAssignedLocationNumber[9]];
            AltHoldSet = Yposition[SwarmManager.HexagonPrizmAssignedLocationNumber[9]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[9], SwarmManager.Agent_Y[9], SwarmManager.Agent_Z[9]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormHexagonPrizmSucces[9] = true;
            }
        }
        if (AgentID == 10 && SwarmManager.HexagonPrizmInFormation[10])
        {
            PosHoldSetX = Xposition[SwarmManager.HexagonPrizmAssignedLocationNumber[10]];
            PosHoldSetZ = Zposition[SwarmManager.HexagonPrizmAssignedLocationNumber[10]];
            AltHoldSet = Yposition[SwarmManager.HexagonPrizmAssignedLocationNumber[10]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[10], SwarmManager.Agent_Y[10], SwarmManager.Agent_Z[10]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormHexagonPrizmSucces[10] = true;
            }
        }
        if (AgentID == 11 && SwarmManager.HexagonPrizmInFormation[11])
        {
            PosHoldSetX = Xposition[SwarmManager.HexagonPrizmAssignedLocationNumber[11]];
            PosHoldSetZ = Zposition[SwarmManager.HexagonPrizmAssignedLocationNumber[11]];
            AltHoldSet = Yposition[SwarmManager.HexagonPrizmAssignedLocationNumber[11]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[11], SwarmManager.Agent_Y[11], SwarmManager.Agent_Z[11]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormHexagonPrizmSucces[11] = true;
            }
        }
        if (AgentID == 12 && SwarmManager.HexagonPrizmInFormation[12])
        {
            PosHoldSetX = Xposition[SwarmManager.HexagonPrizmAssignedLocationNumber[12]];
            PosHoldSetZ = Zposition[SwarmManager.HexagonPrizmAssignedLocationNumber[12]];
            AltHoldSet = Yposition[SwarmManager.HexagonPrizmAssignedLocationNumber[12]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[12], SwarmManager.Agent_Y[12], SwarmManager.Agent_Z[12]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormHexagonPrizmSucces[12] = true;
            }
        }
        if (AgentID == 13 && SwarmManager.HexagonPrizmInFormation[13])
        {
            PosHoldSetX = Xposition[SwarmManager.HexagonPrizmAssignedLocationNumber[13]];
            PosHoldSetZ = Zposition[SwarmManager.HexagonPrizmAssignedLocationNumber[13]];
            AltHoldSet = Yposition[SwarmManager.HexagonPrizmAssignedLocationNumber[13]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[13], SwarmManager.Agent_Y[13], SwarmManager.Agent_Z[13]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormHexagonPrizmSucces[13] = true;
            }
        }
        for (int k = 0; k < 14; k++)
        {
            if (SwarmManager.AgentFormHexagonPrizmSucces[k])
            {
                SwarmManager.hexagonPrizmCounter++;
            }
        }
        if (SwarmManager.hexagonPrizmCounter == SwarmManager.HexagonPrizmAgentRequired)
        {
            SwarmManager.FormHexagonPrizmSucces = true;
            SwarmManager.hexagonPrizmCounter = 0;
        }
        else
        {
            SwarmManager.FormHexagonPrizmSucces = false;
            SwarmManager.hexagonPrizmCounter = 0;
        }
    }

    void HexagonFormation()
    {
        float[] Xposition = new float[6];
        float[] Zposition = new float[6];
        float[] Yposition = new float[6];

        float formationAverageX = 0f;
        float formationAverageY = 0f;
        float formationAverageZ = 0f;

        Xposition[0] = SwarmManager.formationTargetMiddleX + SwarmManager.hexagonPrizmEdge * Mathf.Sin((SwarmManager.FormationHeading + 30f) * Mathf.Deg2Rad);
        Xposition[1] = SwarmManager.formationTargetMiddleX + SwarmManager.hexagonPrizmEdge * Mathf.Sin((SwarmManager.FormationHeading + 90f) * Mathf.Deg2Rad);
        Xposition[2] = SwarmManager.formationTargetMiddleX + SwarmManager.hexagonPrizmEdge * Mathf.Sin((SwarmManager.FormationHeading + 150f) * Mathf.Deg2Rad);
        Xposition[3] = SwarmManager.formationTargetMiddleX + SwarmManager.hexagonPrizmEdge * Mathf.Sin((SwarmManager.FormationHeading + 210f) * Mathf.Deg2Rad);
        Xposition[4] = SwarmManager.formationTargetMiddleX + SwarmManager.hexagonPrizmEdge * Mathf.Sin((SwarmManager.FormationHeading + 270f) * Mathf.Deg2Rad);
        Xposition[5] = SwarmManager.formationTargetMiddleX + SwarmManager.hexagonPrizmEdge * Mathf.Sin((SwarmManager.FormationHeading + 330f) * Mathf.Deg2Rad);

        Zposition[0] = SwarmManager.formationTargetMiddleZ + SwarmManager.hexagonPrizmEdge * Mathf.Cos((SwarmManager.FormationHeading + 30f) * Mathf.Deg2Rad);
        Zposition[1] = SwarmManager.formationTargetMiddleZ + SwarmManager.hexagonPrizmEdge * Mathf.Cos((SwarmManager.FormationHeading + 90f) * Mathf.Deg2Rad);
        Zposition[2] = SwarmManager.formationTargetMiddleZ + SwarmManager.hexagonPrizmEdge * Mathf.Cos((SwarmManager.FormationHeading + 150f) * Mathf.Deg2Rad);
        Zposition[3] = SwarmManager.formationTargetMiddleZ + SwarmManager.hexagonPrizmEdge * Mathf.Cos((SwarmManager.FormationHeading + 210f) * Mathf.Deg2Rad);
        Zposition[4] = SwarmManager.formationTargetMiddleZ + SwarmManager.hexagonPrizmEdge * Mathf.Cos((SwarmManager.FormationHeading + 270f) * Mathf.Deg2Rad);
        Zposition[5] = SwarmManager.formationTargetMiddleZ + SwarmManager.hexagonPrizmEdge * Mathf.Cos((SwarmManager.FormationHeading + 330f) * Mathf.Deg2Rad);

        Yposition[0] = SwarmManager.formationTargetMiddleHeight;
        Yposition[1] = Yposition[0];
        Yposition[2] = Yposition[0];
        Yposition[3] = Yposition[0];
        Yposition[4] = Yposition[0];
        Yposition[5] = Yposition[0];


        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 14; j++)
            {
                if (SwarmManager.ActiveAgents[j] && !SwarmManager.HexagonInFormation[j] && !SwarmManager.HexagonPositionFull[i])
                {
                    SwarmManager.HexagonAssignedLocationNumber[j] = i;
                    SwarmManager.HexagonInFormation[j] = true;
                    SwarmManager.HexagonPositionFull[i] = true;
                    break;
                }
                else if (!SwarmManager.ActiveAgents[j] && SwarmManager.HexagonInFormation[j])
                {
                    SwarmManager.HexagonInFormation[j] = false;
                    SwarmManager.HexagonPositionFull[SwarmManager.HexagonAssignedLocationNumber[j]] = false;
                    SwarmManager.AgentFormHexagonSucces[j] = false;
                }
            }
        }

        for (int c = 0; c < 14; c++)
        {
            if (SwarmManager.HexagonInFormation[c])
            {
                formationAverageX += SwarmManager.Agent_X[c];
                formationAverageY += SwarmManager.Agent_Y[c];
                formationAverageZ += SwarmManager.Agent_Z[c];
            }
            if (c == 13)
            {
                SwarmManager.FormationMiddleX = formationAverageX / SwarmManager.HexagonAgentRequired;
                SwarmManager.FormationMiddleY = formationAverageY / SwarmManager.HexagonAgentRequired;
                SwarmManager.FormationMiddleZ = formationAverageZ / SwarmManager.HexagonAgentRequired;
            }
        }

        if (AgentID == 0 && SwarmManager.HexagonInFormation[0])
        {
            PosHoldSetX = Xposition[SwarmManager.HexagonAssignedLocationNumber[0]];
            PosHoldSetZ = Zposition[SwarmManager.HexagonAssignedLocationNumber[0]];
            AltHoldSet = Yposition[SwarmManager.HexagonAssignedLocationNumber[0]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[0], SwarmManager.Agent_Y[0], SwarmManager.Agent_Z[0]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormHexagonSucces[0] = true;
            }
        }
        if (AgentID == 1 && SwarmManager.HexagonInFormation[1])
        {
            PosHoldSetX = Xposition[SwarmManager.HexagonAssignedLocationNumber[1]];
            PosHoldSetZ = Zposition[SwarmManager.HexagonAssignedLocationNumber[1]];
            AltHoldSet = Yposition[SwarmManager.HexagonAssignedLocationNumber[1]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[1], SwarmManager.Agent_Y[1], SwarmManager.Agent_Z[1]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormHexagonSucces[1] = true;
            }
        }
        if (AgentID == 2 && SwarmManager.HexagonInFormation[2])
        {
            PosHoldSetX = Xposition[SwarmManager.HexagonAssignedLocationNumber[2]];
            PosHoldSetZ = Zposition[SwarmManager.HexagonAssignedLocationNumber[2]];
            AltHoldSet = Yposition[SwarmManager.HexagonAssignedLocationNumber[2]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[2], SwarmManager.Agent_Y[2], SwarmManager.Agent_Z[2]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormHexagonSucces[2] = true;
            }
        }
        if (AgentID == 3 && SwarmManager.HexagonInFormation[3])
        {
            PosHoldSetX = Xposition[SwarmManager.HexagonAssignedLocationNumber[3]];
            PosHoldSetZ = Zposition[SwarmManager.HexagonAssignedLocationNumber[3]];
            AltHoldSet = Yposition[SwarmManager.HexagonAssignedLocationNumber[3]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[3], SwarmManager.Agent_Y[3], SwarmManager.Agent_Z[3]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormHexagonSucces[3] = true;
            }
        }
        if (AgentID == 4 && SwarmManager.HexagonInFormation[4])
        {
            PosHoldSetX = Xposition[SwarmManager.HexagonAssignedLocationNumber[4]];
            PosHoldSetZ = Zposition[SwarmManager.HexagonAssignedLocationNumber[4]];
            AltHoldSet = Yposition[SwarmManager.HexagonAssignedLocationNumber[4]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[4], SwarmManager.Agent_Y[4], SwarmManager.Agent_Z[4]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormHexagonSucces[4] = true;
            }
        }
        if (AgentID == 5 && SwarmManager.HexagonInFormation[5])
        {
            PosHoldSetX = Xposition[SwarmManager.HexagonAssignedLocationNumber[5]];
            PosHoldSetZ = Zposition[SwarmManager.HexagonAssignedLocationNumber[5]];
            AltHoldSet = Yposition[SwarmManager.HexagonAssignedLocationNumber[5]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[5], SwarmManager.Agent_Y[5], SwarmManager.Agent_Z[5]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormHexagonSucces[5] = true;
            }
        }
        if (AgentID == 6 && SwarmManager.HexagonInFormation[6])
        {
            PosHoldSetX = Xposition[SwarmManager.HexagonAssignedLocationNumber[6]];
            PosHoldSetZ = Zposition[SwarmManager.HexagonAssignedLocationNumber[6]];
            AltHoldSet = Yposition[SwarmManager.HexagonAssignedLocationNumber[6]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[6], SwarmManager.Agent_Y[6], SwarmManager.Agent_Z[6]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormHexagonSucces[6] = true;
            }
        }
        if (AgentID == 7 && SwarmManager.HexagonInFormation[7])
        {
            PosHoldSetX = Xposition[SwarmManager.HexagonAssignedLocationNumber[7]];
            PosHoldSetZ = Zposition[SwarmManager.HexagonAssignedLocationNumber[7]];
            AltHoldSet = Yposition[SwarmManager.HexagonAssignedLocationNumber[7]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[7], SwarmManager.Agent_Y[7], SwarmManager.Agent_Z[7]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormHexagonSucces[7] = true;
            }
        }
        if (AgentID == 8 && SwarmManager.HexagonInFormation[8])
        {
            PosHoldSetX = Xposition[SwarmManager.HexagonAssignedLocationNumber[8]];
            PosHoldSetZ = Zposition[SwarmManager.HexagonAssignedLocationNumber[8]];
            AltHoldSet = Yposition[SwarmManager.HexagonAssignedLocationNumber[8]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[8], SwarmManager.Agent_Y[8], SwarmManager.Agent_Z[8]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormHexagonSucces[8] = true;
            }
        }
        if (AgentID == 9 && SwarmManager.HexagonInFormation[9])
        {
            PosHoldSetX = Xposition[SwarmManager.HexagonAssignedLocationNumber[9]];
            PosHoldSetZ = Zposition[SwarmManager.HexagonAssignedLocationNumber[9]];
            AltHoldSet = Yposition[SwarmManager.HexagonAssignedLocationNumber[9]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[9], SwarmManager.Agent_Y[9], SwarmManager.Agent_Z[9]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormHexagonSucces[9] = true;
            }
        }
        if (AgentID == 10 && SwarmManager.HexagonInFormation[10])
        {
            PosHoldSetX = Xposition[SwarmManager.HexagonAssignedLocationNumber[10]];
            PosHoldSetZ = Zposition[SwarmManager.HexagonAssignedLocationNumber[10]];
            AltHoldSet = Yposition[SwarmManager.HexagonAssignedLocationNumber[10]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[10], SwarmManager.Agent_Y[10], SwarmManager.Agent_Z[10]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormHexagonSucces[10] = true;
            }
        }
        if (AgentID == 11 && SwarmManager.HexagonInFormation[11])
        {
            PosHoldSetX = Xposition[SwarmManager.HexagonAssignedLocationNumber[11]];
            PosHoldSetZ = Zposition[SwarmManager.HexagonAssignedLocationNumber[11]];
            AltHoldSet = Yposition[SwarmManager.HexagonAssignedLocationNumber[11]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[11], SwarmManager.Agent_Y[11], SwarmManager.Agent_Z[11]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormHexagonSucces[11] = true;
            }
        }
        if (AgentID == 12 && SwarmManager.HexagonInFormation[12])
        {
            PosHoldSetX = Xposition[SwarmManager.HexagonAssignedLocationNumber[12]];
            PosHoldSetZ = Zposition[SwarmManager.HexagonAssignedLocationNumber[12]];
            AltHoldSet = Yposition[SwarmManager.HexagonAssignedLocationNumber[12]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[12], SwarmManager.Agent_Y[12], SwarmManager.Agent_Z[12]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormHexagonSucces[12] = true;
            }
        }
        if (AgentID == 13 && SwarmManager.HexagonInFormation[13])
        {
            PosHoldSetX = Xposition[SwarmManager.HexagonAssignedLocationNumber[13]];
            PosHoldSetZ = Zposition[SwarmManager.HexagonAssignedLocationNumber[13]];
            AltHoldSet = Yposition[SwarmManager.HexagonAssignedLocationNumber[13]];

            if (Vector3.Distance(new Vector3(SwarmManager.Agent_X[13], SwarmManager.Agent_Y[13], SwarmManager.Agent_Z[13]), new Vector3(PosHoldSetX, AltHoldSet, PosHoldSetZ)) < 0.5f)
            {
                SwarmManager.AgentFormHexagonSucces[13] = true;
            }
        }

        for (int k = 0; k < 14; k++)
        {
            if (SwarmManager.AgentFormHexagonSucces[k])
            {
                SwarmManager.hexagonCounter++;
            }
        }
        if (SwarmManager.hexagonCounter == SwarmManager.HexagonAgentRequired)
        {
            SwarmManager.FormHexagonSucces = true;
            SwarmManager.hexagonCounter = 0;
        }
        else
        {
            SwarmManager.FormHexagonSucces = false;
            SwarmManager.hexagonCounter = 0;
        }


    }

    void PID()
    {

        //////////////////////////////////  HEADING CALCULATIONS  ////////////////////////
        ///
        if (TargetHeading > 180f) TargetHeading -= 360f;

        if (TargetHeading < -170f || TargetHeading > 170f)
        {
            if (TargetHeading < 0f) TargetHeading += 360f;

            if (AgentHeading < 0f) AgentHeading += 360f;
        }

        yawSetRate = (AgentHeading - TargetHeading) / 2f;
        if (yawSetRate > yawRate) yawSetRate = yawRate;
        else if (yawSetRate < -yawRate) yawSetRate = -yawRate;



        // Error calculations
        pitchError = pitchSetpoint - pitchGyro;
        rollError = rollSetpoint - rollGyro;
        yawError = yawChangeRate - yawSetRate;

        // P calculations
        pitch_P_out = pitchP * pitchError;
        roll_P_out = rollP * rollError;
        yaw_P_out = yawP * yawError;

        // I calculations
        pitch_I_out = pitch_I_out + 0.5f * pitchI * sampleTime * (pitchError + pitchPrevError);

        if (pitch_I_out > maxI) pitch_I_out = maxI;
        else if (pitch_I_out < -maxI) pitch_I_out = -maxI;

        roll_I_out = roll_I_out + 0.5f * rollI * sampleTime * (rollError + rollPrevError);

        if (roll_I_out > maxI) roll_I_out = maxI;
        else if (roll_I_out < -maxI) roll_I_out = -maxI;

        yaw_I_out = yaw_I_out + 0.5f * yawI * sampleTime * (yawError + yawPrevError);

        if (yaw_I_out > maxI) yaw_I_out = maxI;
        else if (yaw_I_out < -maxI) yaw_I_out = -maxI;

        // D calculations
        pitch_D_out = -(2.0f * pitchD * (pitchGyro - lastPitchSensorValue)
                        + (2.0f * tau - sampleTime) * pitch_D_out)
                        / (2.0f * tau + sampleTime);

        if (pitch_D_out > maxD) pitch_D_out = maxD;
        else if (pitch_D_out < -maxD) pitch_D_out = -maxD;

        roll_D_out = -(2.0f * rollD * (rollGyro - lastRollSensorValue)
                        + (2.0f * tau - sampleTime) * roll_D_out)
                        / (2.0f * tau + sampleTime);

        if (roll_D_out > maxD) roll_D_out = maxD;
        else if (roll_D_out < -maxD) roll_D_out = -maxD;

        yaw_D_out = -(2.0f * yawD * (yawChangeRate - lastYawChangeRate)
                        + (2.0f * tau - sampleTime) * yaw_D_out)
                        / (2.0f * tau + sampleTime);

        if (yaw_D_out > maxD) yaw_D_out = maxD;
        else if (yaw_D_out < -maxD) yaw_D_out = -maxD;

        lastPitchSensorValue = pitchGyro;
        lastRollSensorValue = rollGyro;
        lastYawChangeRate = yawChangeRate;

        pitchPrevError = pitchError;
        rollPrevError = rollError;
        yawPrevError = yawError;


        // Calculating the overall and limiting PID values
        pitch_PID_out = pitch_P_out + pitch_I_out + pitch_D_out;
        if (pitch_PID_out > maxPID) pitch_PID_out = maxPID;
        else if (pitch_PID_out < -maxPID) pitch_PID_out = -maxPID;

        roll_PID_out = roll_P_out + roll_I_out + roll_D_out;
        if (roll_PID_out > maxPID) roll_PID_out = maxPID;
        else if (roll_PID_out < -maxPID) roll_PID_out = -maxPID;

        yaw_PID_out = yaw_P_out + yaw_I_out + yaw_D_out;
        if (yaw_PID_out > maxPID) yaw_PID_out = maxPID;
        else if (yaw_PID_out < -maxPID) yaw_PID_out = -maxPID;


        ////////////////////////////////////////////////////////////////////////////////////////////////  ALTITUDE HOLD  //////////////////////////////////////

        AltHoldSetRate = ((AltHoldSet + SwarmManager.CollisionAltChange[AgentID]) - transform.position.y) / 4f; // 5f

        altHoldError = AltHoldSetRate - AltChangeRate;

        // Altitude hold P Calculation
        altHold_P_out = altHoldError * altHoldP;

        // Altitude hold I Calculation

        altHold_I_out = altHold_I_out + 0.5f * altHoldI * sampleTime * (altHoldError + altHoldPrevError);

        if (altHold_I_out > 3000f) altHold_I_out = 3000f;
        else if (altHold_I_out < 500f) altHold_I_out = 500f;


        // Altitude hold D Calculation
        altHold_D_out = -(2.0f * altHoldD * (AltChangeRate)
                    + (2.0f * tau - sampleTime) * altHold_D_out)
                    / (2.0f * tau + sampleTime);

        if (altHold_D_out > 1500f) altHold_D_out = 1500f;
        else if (altHold_D_out < -1500) altHold_D_out = -1500f;

        // Limiting Altitude Hold PID values
        altHoldPID_out = altHold_P_out + altHold_I_out + altHold_D_out;
        if (altHoldPID_out > 2000f) altHoldPID_out = 2000f;
        else if (altHoldPID_out < 0f) altHoldPID_out = 0f;

        userThrottle = altHoldPID_out + minUserThrottle;
        userThrottle += userThrottle * (Mathf.Sin(Mathf.Deg2Rad * Mathf.Abs(pitchGyro)) + Mathf.Sin(Mathf.Deg2Rad * Mathf.Abs(rollGyro)));

        altHoldPrevError = altHoldError;

        /////////////////////////////////////////////////////////////////////////////////////////////////  POSITION HOLD ////////////////////////////////////////////////


        positionXError = PosHoldSetX - positionX;
        positionZError = PosHoldSetZ - positionZ;

        rollPosHold_P_out = -positionXError * (posHoldP / 100);
        pitchPosHold_P_out = positionZError * (posHoldP / 100);

        pitchPosHold_D_out = -(2.0f * posHoldD * (positionZ - prevPositionZ)
                    + (2.0f * tau - sampleTime) * pitchPosHold_D_out)
                    / (2.0f * tau + sampleTime);

        rollPosHold_D_out = -(2.0f * posHoldD * (positionX - prevPositionX)
                    + (2.0f * tau - sampleTime) * rollPosHold_D_out)
                    / (2.0f * tau + sampleTime);


        pitchPosHoldHorth = pitchPosHold_P_out + pitchPosHold_D_out;
        rollPosHoldHorth = rollPosHold_P_out - rollPosHold_D_out;
        /*

        if (pitchPosHoldHorth > maxPitchAngle) pitchPosHoldHorth = maxPitchAngle;
        else if (pitchPosHoldHorth < -maxPitchAngle) pitchPosHoldHorth = -maxPitchAngle;

        if (rollPosHoldHorth > maxRollAngle) rollPosHoldHorth = maxRollAngle;
        else if (rollPosHoldHorth < -maxRollAngle) rollPosHoldHorth = -maxRollAngle;

        */
        correctedPitchPosHoldAngle = pitchPosHoldHorth * Mathf.Cos(Mathf.Deg2Rad * UnityEditor.TransformUtils.GetInspectorRotation(transform).y)
            + rollPosHoldHorth * Mathf.Cos(Mathf.Deg2Rad * (UnityEditor.TransformUtils.GetInspectorRotation(transform).y + 90f));

        correctedRollPosHoldAngle = rollPosHoldHorth * Mathf.Cos(Mathf.Deg2Rad * UnityEditor.TransformUtils.GetInspectorRotation(transform).y)
            + pitchPosHoldHorth * Mathf.Cos(Mathf.Deg2Rad * (UnityEditor.TransformUtils.GetInspectorRotation(transform).y - 90f));

        pitchSetpoint = correctedPitchPosHoldAngle + SwarmManager.CollisionPitchChange[AgentID];
        rollSetpoint = correctedRollPosHoldAngle + SwarmManager.CollisionRollChange[AgentID];

        if (pitchSetpoint > maxPitchAngle) pitchSetpoint = maxPitchAngle;
        else if (pitchSetpoint < -maxPitchAngle) pitchSetpoint = -maxPitchAngle;

        if (rollSetpoint > maxRollAngle) rollSetpoint = maxRollAngle;
        else if (rollSetpoint < -maxRollAngle) rollSetpoint = -maxRollAngle;

        prevPositionX = positionX;
        prevPositionZ = positionZ;




        ////////////////////////////////////////////////////////////////////////////////////////////  LIMITING OF SIGNALS  ///////////////////////////////////////////




        // Calculating and limiting all motor signals

        LBThrottle = userThrottle - pitch_PID_out + roll_PID_out + yaw_PID_out;

        if (LBThrottle > maxThrottle) LBThrottle = maxThrottle;
        else if (LBThrottle < minThrottle) LBThrottle = minThrottle;

        LTThrottle = userThrottle + pitch_PID_out + roll_PID_out - yaw_PID_out;

        if (LTThrottle > maxThrottle) LTThrottle = maxThrottle;
        else if (LTThrottle < minThrottle) LTThrottle = minThrottle;

        RBThrottle = userThrottle - pitch_PID_out - roll_PID_out - yaw_PID_out;

        if (RBThrottle > maxThrottle) RBThrottle = maxThrottle;
        else if (RBThrottle < minThrottle) RBThrottle = minThrottle;

        RTThrottle = userThrottle + pitch_PID_out - roll_PID_out + yaw_PID_out;

        if (RTThrottle > maxThrottle) RTThrottle = maxThrottle;
        else if (RTThrottle < minThrottle) RTThrottle = minThrottle;

    }

    public void SensorFusion()
    {
        pitchGyro = UnityEditor.TransformUtils.GetInspectorRotation(transform).x;
        rollGyro = UnityEditor.TransformUtils.GetInspectorRotation(transform).z;

        AgentHeading = transform.eulerAngles.y;
        
        if (AgentHeading > 180f)
        {
            AgentHeading -= 360f;
        }
        
        yawChangeRate = (prevYaw - UnityEditor.TransformUtils.GetInspectorRotation(transform).y) / sampleTime;
        prevYaw = UnityEditor.TransformUtils.GetInspectorRotation(transform).y;
        AltChangeRate = (transform.position.y - PrevAltitude) / sampleTime;
        PrevAltitude = transform.position.y;
        positionX = transform.position.x;
        positionZ = transform.position.z;
    }
}