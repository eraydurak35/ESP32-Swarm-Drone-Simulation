using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BMP280 : MonoBehaviour
{
    private float altitude;
    public int noise;
    public float GetAltitude()
    {
        altitude = transform.position.y + (Random.Range(-noise, noise) / 1000f);
        return altitude;
    }
}
