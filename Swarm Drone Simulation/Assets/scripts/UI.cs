using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public Text RealSpeedText;
    public Text RealPitch;
    public Text EstPitch;
    public Text RealRoll;
    public Text EstRoll;
    public Text RealHeading;
    public Text EstHeading;
    public Text Current;
    public Text UsedCapacity;
    public Text BattVoltage;
    public Text BaroAltitude;
    public Text RealAltitude;
    public Text TOFAltitude;
    public Text AltitudeHold;

    public Text XaccReal;
    public Text YaccReal;
    public Text ZaccReal;
    public Text XaccEst;
    public Text YaccEst;
    public Text ZaccEst;

    public Text RealLocation;
    public Text EstLocation;
    public void updateUI()
    {
        GetComponent<EnvironmentalMeasurementsAndEffects>().Speed();
        GetComponent<EnvironmentalMeasurementsAndEffects>().RealAltitude();
        RealSpeedText.text = "Real Speed: " + GetComponent<EnvironmentalMeasurementsAndEffects>().speed.ToString("F2") + " m/sc";

        RealPitch.text = "Real Pitch: " + UnityEditor.TransformUtils.GetInspectorRotation(transform).x.ToString("F2") + "°";
        RealRoll.text = "Real Roll: " + UnityEditor.TransformUtils.GetInspectorRotation(transform).z.ToString("F2") + "°";

        //EstPitch.text = "Est. Pitch: " + GetComponent<ControlScript>().pitchGyro.ToString("F2") + "°";
        //EstRoll.text = "Est. Roll: " + GetComponent<ControlScript>().rollGyro.ToString("F2") + "°";

        Current.text = "Curr: " + GetComponent<Motors>().sumCurrent.ToString("F2")+ " Amps";


        if (GetComponent<Battery>().usedCapacity > 600) UsedCapacity.color = Color.red;
        else UsedCapacity.color = new Color(0.7529412f, 0.7529412f, 0.7529412f, 1);

        UsedCapacity.text = "Used Cap: " + GetComponent<Battery>().usedCapacity.ToString("F2") + " mAh";

        if (GetComponent<Battery>().voltage < 3.3) BattVoltage.color = Color.red;
        else BattVoltage.color = new Color(0.7529412f, 0.7529412f, 0.7529412f, 1);
        BattVoltage.text = "Voltage: " + GetComponent<Battery>().voltage.ToString("F2") + " V";

        //RealAltitude.text = "Real Altitude: " + GetComponent<EnvironmentalMeasurementsAndEffects>().realAltitude.ToString("F2") + " m";
        BaroAltitude.text = "Baro. Altitude: " + GetComponent<ControlScript>().barometerAltitude.ToString("F2") + " m";

        if (GetComponent<ControlScript>().altitudeHold)
        {
            AltitudeHold.text = "Altitude Hold: " + GetComponent<ControlScript>().altHoldSetPoint.ToString("F2") + " m";
            AltitudeHold.color = Color.green;
        }
        else
        {
            AltitudeHold.text = "Altitude Hold: Off";
            AltitudeHold.color = new Color(0.7529412f, 0.7529412f, 0.7529412f, 1);
        }
        /*
        XaccReal.text = "Xacc Real: " + GetComponent<LSM6DSL_Accelerometer>().xAccelerationGlobal.ToString("F2");
        YaccReal.text = "Yacc Real: " + GetComponent<LSM6DSL_Accelerometer>().yAccelerationGlobal.ToString("F2");
        ZaccReal.text = "Zacc Real: " + GetComponent<LSM6DSL_Accelerometer>().zAccelerationGlobal.ToString("F2");

        XaccEst.text = "  Xacc Est: " + ((GetComponent<BasicTranslate>().accx / 16393f) * 9.81f).ToString("F2");
        YaccEst.text = "  Yacc Est: " + ((GetComponent<L80REM37>().accGlobalizedYraw / 16393f) * 9.81f).ToString("F2");
        ZaccEst.text = "  Zacc Est: " + ((GetComponent<BasicTranslate>().accz / 16393f) * 9.81f).ToString("F2");
        */
        RealLocation.text = "Real Location: " + transform.localPosition;
        //EstLocation.text = "Est Location: (" + GetComponent<L80REM37>().meanLongtitude.ToString("F1")+ ", " 
        //    + transform.position.y.ToString("F1") + ", " + GetComponent<L80REM37>().meanLatitude.ToString("F1") +")";

        RealHeading.text = "Real Head.: " + transform.eulerAngles.y.ToString("F2");
        //EstHeading.text = "Est Head.: " + GetComponent<ControlScript>().headingGyro.ToString("F2");

    }
}
