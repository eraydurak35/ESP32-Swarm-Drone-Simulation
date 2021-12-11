using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public Text RealSpeedText;
    public Text RealPitch;
    public Text CalcPitch;
    public Text RealRoll;
    public Text CalcRoll;
    public Text Current;
    public Text UsedCapacity;
    public Text BattVoltage;
    public Text BaroAltitude;
    public Text RealAltitude;
    public Text TOFAltitude;

    public void updateUI()
    {
        GetComponent<EnvironmentalMeasurementsAndEffects>().Speed();
        GetComponent<EnvironmentalMeasurementsAndEffects>().RealAltitude();
        RealSpeedText.text = "Real Speed: " + GetComponent<EnvironmentalMeasurementsAndEffects>().speed.ToString("F2") + " m/sc";

        RealPitch.text = "Real Pitch: " + UnityEditor.TransformUtils.GetInspectorRotation(transform).x.ToString("F2") + "°";
        RealRoll.text = "Real Roll: " + UnityEditor.TransformUtils.GetInspectorRotation(transform).z.ToString("F2") + "°";

        CalcPitch.text = "Est. Pitch: " + GetComponent<ControlScript>().pitchGyro.ToString("F2") + "°";
        CalcRoll.text = "Est. Roll: " + GetComponent<ControlScript>().rollGyro.ToString("F2") + "°";

        Current.text = "Curr: " + GetComponent<Motors>().sumCurrent.ToString("F2")+ " Amps";


        if (GetComponent<Battery>().usedCapacity > 600) UsedCapacity.color = Color.red;
        else UsedCapacity.color = new Color(0.7529412f, 0.7529412f, 0.7529412f, 1);

        UsedCapacity.text = "Used Cap: " + GetComponent<Battery>().usedCapacity.ToString("F2") + " mAh";

        if (GetComponent<Battery>().voltage < 3.3) BattVoltage.color = Color.red;
        else BattVoltage.color = new Color(0.7529412f, 0.7529412f, 0.7529412f, 1);
        BattVoltage.text = "Voltage: " + GetComponent<Battery>().voltage.ToString("F2") + " V";

        RealAltitude.text = "Real Altitude: " + GetComponent<EnvironmentalMeasurementsAndEffects>().realAltitude.ToString("F2") + " m";
        BaroAltitude.text = "Baro. Altitude: " + GetComponent<ControlScript>().barometerAltitude.ToString("F2") + " m";
    }
}
