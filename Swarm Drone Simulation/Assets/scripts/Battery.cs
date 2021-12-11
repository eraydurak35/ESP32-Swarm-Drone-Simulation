using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battery : MonoBehaviour
{
    public float voltage;
    public float capacity = 750;

    public float intResistance = 0.043f;
    
    public float usedCapacity = 600;
    public float updateRate = 800;
    private float vSag = 0f;

    public void BatteryState()
    {

        usedCapacity += (GetComponent<Motors>().sumCurrent * 1000f) / (updateRate * 3600f);
        vSag = GetComponent<Motors>().sumCurrent * intResistance;

        if (usedCapacity < capacity * 8 / 10) voltage = ((-usedCapacity * 0.5f / (capacity * 8 / 10)) + 4.2f) - vSag;
        else voltage = (((-usedCapacity + 600) * 3.7f / (capacity * 2 / 10)) + 3.7f) -vSag;




    }

}
