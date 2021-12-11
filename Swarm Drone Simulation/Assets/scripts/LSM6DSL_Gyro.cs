using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LSM6DSL_Gyro : MonoBehaviour
{

    float xAxisReadValue;
    float xAxisLastValue;
    float xAxisDPS;
    [HideInInspector]
    public int xAxisOutput;

    float yAxisReadValue;
    float yAxisLastValue;
    float yAxisDPS;
    [HideInInspector]
    public int yAxisOutput;

    float zAxisReadValue;
    float zAxisLastValue;
    float zAxisDPS;
    [HideInInspector]
    public int zAxisOutput;

    public int noise;

    public float updateRate = 800;
    
    int maxOutput = 28571;

    // Max output 28571
    public void DPS125()
    {
        // X Axis calculations
        xAxisReadValue = UnityEditor.TransformUtils.GetInspectorRotation(transform).x;
        xAxisDPS = (xAxisReadValue - xAxisLastValue) * updateRate;
        xAxisLastValue = xAxisReadValue;

        if (Random.Range(-noise, noise) + ((xAxisDPS * maxOutput) / 125) > maxOutput) xAxisOutput = maxOutput;
        else if (Random.Range(-noise, noise) + ((xAxisDPS * maxOutput) / 125) < -maxOutput) xAxisOutput = -maxOutput;
        else xAxisOutput = Random.Range(-noise, noise) + (int)((xAxisDPS * maxOutput) / 125);

        // Y Axis calculations
        yAxisReadValue = UnityEditor.TransformUtils.GetInspectorRotation(transform).y;
        yAxisDPS = (yAxisReadValue - yAxisLastValue) * updateRate;
        yAxisLastValue = yAxisReadValue;

        if (Random.Range(-noise, noise ) + ((yAxisDPS * maxOutput) / 125) > maxOutput) yAxisOutput = maxOutput;
        else if (Random.Range(-noise, noise ) + ((yAxisDPS * maxOutput) / 125) < -maxOutput) yAxisOutput = -maxOutput;
        else yAxisOutput = Random.Range(-noise, noise ) + (int)((yAxisDPS * maxOutput) / 125);

        // Z Axis calculations
        zAxisReadValue = UnityEditor.TransformUtils.GetInspectorRotation(transform).z;
        zAxisDPS = (zAxisReadValue - zAxisLastValue) * updateRate;
        zAxisLastValue = zAxisReadValue;

        if (Random.Range(-noise, noise ) + ((zAxisDPS * maxOutput) / 125) > maxOutput) zAxisOutput = maxOutput;
        else if (Random.Range(-noise, noise ) + ((zAxisDPS * maxOutput) / 125) < -maxOutput) zAxisOutput = -maxOutput;
        else zAxisOutput = Random.Range(-noise, noise ) + (int)((zAxisDPS * maxOutput) / 125);
    }
    public void DPS250()
    {
        // X Axis calculations
        xAxisReadValue = UnityEditor.TransformUtils.GetInspectorRotation(transform).x;
        xAxisDPS = (xAxisReadValue - xAxisLastValue) * updateRate;
        xAxisLastValue = xAxisReadValue;

        if (Random.Range(-noise, noise ) + ((xAxisDPS * maxOutput) / 250) > maxOutput) xAxisOutput = maxOutput;
        else if (Random.Range(-noise, noise ) + ((xAxisDPS * maxOutput) / 250) < -maxOutput) xAxisOutput = -maxOutput;
        else xAxisOutput = Random.Range(-noise, noise ) + (int)((xAxisDPS * maxOutput) / 250);

        // Y Axis calculations
        yAxisReadValue = UnityEditor.TransformUtils.GetInspectorRotation(transform).y;
        yAxisDPS = (yAxisReadValue - yAxisLastValue) * updateRate;
        yAxisLastValue = yAxisReadValue;

        if (Random.Range(-noise, noise ) + ((yAxisDPS * maxOutput) / 250) > maxOutput) yAxisOutput = maxOutput;
        else if (Random.Range(-noise, noise ) + ((yAxisDPS * maxOutput) / 250) < -maxOutput) yAxisOutput = -maxOutput;
        else yAxisOutput = Random.Range(-noise, noise ) + (int)((yAxisDPS * maxOutput) / 250);

        // Z Axis calculations
        zAxisReadValue = UnityEditor.TransformUtils.GetInspectorRotation(transform).z;
        zAxisDPS = (zAxisReadValue - zAxisLastValue) * updateRate;
        zAxisLastValue = zAxisReadValue;

        if (Random.Range(-noise, noise ) + ((zAxisDPS * maxOutput) / 250) > maxOutput) zAxisOutput = maxOutput;
        else if (Random.Range(-noise, noise ) + ((zAxisDPS * maxOutput) / 250) < -maxOutput) zAxisOutput = -maxOutput;
        else zAxisOutput = Random.Range(-noise, noise ) + (int)((zAxisDPS * maxOutput) / 250);
    }
    public void DPS500()
    {
        // X Axis calculations
        xAxisReadValue = UnityEditor.TransformUtils.GetInspectorRotation(transform).x;
        xAxisDPS = (xAxisReadValue - xAxisLastValue) * updateRate;
        xAxisLastValue = xAxisReadValue;

        if (Random.Range(-noise, noise ) + ((xAxisDPS * maxOutput) / 500) > maxOutput) xAxisOutput = maxOutput;
        else if (Random.Range(-noise, noise ) + ((xAxisDPS * maxOutput) / 500) < -maxOutput) xAxisOutput = -maxOutput;
        else xAxisOutput = Random.Range(-noise, noise ) + (int)((xAxisDPS * maxOutput) / 500);

        // Y Axis calculations
        yAxisReadValue = UnityEditor.TransformUtils.GetInspectorRotation(transform).y;
        yAxisDPS = (yAxisReadValue - yAxisLastValue) * updateRate;
        yAxisLastValue = yAxisReadValue;

        if (Random.Range(-noise, noise ) + ((yAxisDPS * maxOutput) / 500) > maxOutput) yAxisOutput = maxOutput;
        else if (Random.Range(-noise, noise ) + ((yAxisDPS * maxOutput) / 500) < -maxOutput) yAxisOutput = -maxOutput;
        else yAxisOutput = Random.Range(-noise, noise ) + (int)((yAxisDPS * maxOutput) / 500);

        // Z Axis calculations
        zAxisReadValue = UnityEditor.TransformUtils.GetInspectorRotation(transform).z;
        zAxisDPS = (zAxisReadValue - zAxisLastValue) * updateRate;
        zAxisLastValue = zAxisReadValue;

        if (Random.Range(-noise, noise ) + ((zAxisDPS * maxOutput) / 500) > maxOutput) zAxisOutput = maxOutput;
        else if (Random.Range(-noise, noise ) + ((zAxisDPS * maxOutput) / 500) < -maxOutput) zAxisOutput = -maxOutput;
        else zAxisOutput = Random.Range(-noise, noise ) + (int)((zAxisDPS * maxOutput) / 500);
    }
    public void DPS1000()
    {
        // X Axis calculations
        xAxisReadValue = UnityEditor.TransformUtils.GetInspectorRotation(transform).x;
        xAxisDPS = (xAxisReadValue - xAxisLastValue) * updateRate;
        xAxisLastValue = xAxisReadValue;

        if (Random.Range(-noise, noise ) + ((xAxisDPS * maxOutput) / 1000) > maxOutput) xAxisOutput = maxOutput;
        else if (Random.Range(-noise, noise ) + ((xAxisDPS * maxOutput) / 1000) < -maxOutput) xAxisOutput = -maxOutput;
        else xAxisOutput = Random.Range(-noise, noise ) + (int)((xAxisDPS * maxOutput) / 1000);

        // Y Axis calculations
        yAxisReadValue = UnityEditor.TransformUtils.GetInspectorRotation(transform).y;
        yAxisDPS = (yAxisReadValue - yAxisLastValue) * updateRate;
        yAxisLastValue = yAxisReadValue;

        if (Random.Range(-noise, noise ) + ((yAxisDPS * maxOutput) / 1000) > maxOutput) yAxisOutput = maxOutput;
        else if (Random.Range(-noise, noise ) + ((yAxisDPS * maxOutput) / 1000) < -maxOutput) yAxisOutput = -maxOutput;
        else yAxisOutput = Random.Range(-noise, noise ) + (int)((yAxisDPS * maxOutput) / 1000);

        // Z Axis calculations
        zAxisReadValue = UnityEditor.TransformUtils.GetInspectorRotation(transform).z;
        zAxisDPS = (zAxisReadValue - zAxisLastValue) * updateRate;
        zAxisLastValue = zAxisReadValue;

        if (Random.Range(-noise, noise ) + ((zAxisDPS * maxOutput) / 1000) > maxOutput) zAxisOutput = maxOutput;
        else if (Random.Range(-noise, noise ) + ((zAxisDPS * maxOutput) / 1000) < -maxOutput) zAxisOutput = -maxOutput;
        else zAxisOutput = Random.Range(-noise, noise ) + (int)((zAxisDPS * maxOutput) / 1000);
    }
    public void DPS2000()
    {
        // X Axis calculations
        xAxisReadValue = UnityEditor.TransformUtils.GetInspectorRotation(transform).x;
        xAxisDPS = (xAxisReadValue - xAxisLastValue) * updateRate;
        xAxisLastValue = xAxisReadValue;

        if (Random.Range(-noise, noise ) + ((xAxisDPS * maxOutput) / 2000) > maxOutput) xAxisOutput = maxOutput;
        else if (Random.Range(-noise, noise ) + ((xAxisDPS * maxOutput) / 2000) < -maxOutput) xAxisOutput = -maxOutput;
        else xAxisOutput = Random.Range(-noise, noise ) + (int)((xAxisDPS * maxOutput) / 2000);

        // Y Axis calculations
        yAxisReadValue = UnityEditor.TransformUtils.GetInspectorRotation(transform).y;
        yAxisDPS = (yAxisReadValue - yAxisLastValue) * updateRate;
        yAxisLastValue = yAxisReadValue;

        if (Random.Range(-noise, noise ) + ((yAxisDPS * maxOutput) / 2000) > maxOutput) yAxisOutput = maxOutput;
        else if (Random.Range(-noise, noise ) + ((yAxisDPS * maxOutput) / 2000) < -maxOutput) yAxisOutput = -maxOutput;
        else yAxisOutput = Random.Range(-noise, noise ) + (int)((yAxisDPS * maxOutput) / 2000);

        // Z Axis calculations
        zAxisReadValue = UnityEditor.TransformUtils.GetInspectorRotation(transform).z;
        zAxisDPS = (zAxisReadValue - zAxisLastValue) * updateRate;
        zAxisLastValue = zAxisReadValue;

        if (Random.Range(-noise, noise ) + ((zAxisDPS * maxOutput) / 2000) > maxOutput) zAxisOutput = maxOutput;
        else if (Random.Range(-noise, noise ) + ((zAxisDPS * maxOutput) / 2000) < -maxOutput) zAxisOutput = -maxOutput;
        else zAxisOutput = Random.Range(-noise, noise ) + (int)((zAxisDPS * maxOutput) / 2000);
    }
}
