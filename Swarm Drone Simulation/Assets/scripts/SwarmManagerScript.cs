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

    AgentControl[] AgentControl = new AgentControl[6];
    [SerializeField] GameObject[] Agents = new GameObject[6];

    public bool[] ActiveAgents = new bool[6];

    public GameObject CheckPointObject;
    GameObject checkpoint;
    private bool newCheckpoint = true;
    public float checkpointX;
    public float checkpointZ;
    public float checkpointY;
    [HideInInspector]
    public float[] Agent_X = new float[6];
    [HideInInspector]
    public float[] Agent_Y = new float[6];
    [HideInInspector]
    public float[] Agent_Z = new float[6];
    [HideInInspector]
    public float[] AgentProximity = new float[6];
    [HideInInspector]
    public float[] AgentTargetHeading = new float[6];
    private void Awake()
    {

        for (int i = 0; i < 6; i++)
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
    void Update()
    {

        for (int j = 0; j<6; j++)
        {
            Agent_X[j] = Agents[j].transform.position.x;
            Agent_Y[j] = Agents[j].transform.position.y;
            Agent_Z[j] = Agents[j].transform.position.z;
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
            

            
            for (int k = 0; k < 6; k++)
            {
                AgentProximity[k] = Mathf.Sqrt(Mathf.Pow((checkpointX - Agent_X[k]), 2) + Mathf.Pow((checkpointY - Agent_Y[k]), 2) + Mathf.Pow((checkpointZ - Agent_Z[k]), 2));
                AgentTargetHeading[k] = (Mathf.Atan2((checkpointX - Agent_X[k]), (Agent_Z[k] - checkpointZ)) * Mathf.Rad2Deg) * -1f;
                
                if (AgentProximity[k] < 2f)
                {
                    newCheckpoint = true;
                }
                
            }

            
            
        }
    }
}
