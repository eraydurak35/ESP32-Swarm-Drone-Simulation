using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentBattery : MonoBehaviour
{

    public float voltage;
    public float capacity = 750f;

    public float intResistance = 0.043f;

    public float usedCapacity = 0f;
    public float RemainBattPercent = 100f;
    public float updateRate = 200;
    private float vSag = 0f;

    public void BatteryState()
    {
        usedCapacity += (GetComponent<AgentMotors>().sumCurrent * 1000f) / (updateRate * 3600f);
        RemainBattPercent = ((capacity - usedCapacity) / capacity) * 100f;
        vSag = GetComponent<AgentMotors>().sumCurrent * intResistance;

        if (usedCapacity < capacity * 8f / 10f) voltage = ((-usedCapacity * 0.5f / (capacity * 8f / 10f)) + 4.2f) - vSag;
        else voltage = (((-usedCapacity + 600f) * 3.7f / (capacity * 2f / 10f)) + 3.7f) - vSag;
    }
}
