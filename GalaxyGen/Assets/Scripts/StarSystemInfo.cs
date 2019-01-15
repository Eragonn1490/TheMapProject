using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarSystemInfo : MonoBehaviour
{
    public Generation_Solar_System.STARTEMPERATURES_TYPES mainBodyType;
    public float mainBodyUnitySizeScale;
    public float mainBodyPhysicalSize;
    public float mainBodyDensity;
    public float mainBodyTemperatureKelvin;
    public bool hasAsteroidBelt;

    public Generation_Solar_System.STAR_REGION_RANGE hotRange;
    public Generation_Solar_System.STAR_REGION_RANGE coldRange;
    public Generation_Solar_System.STAR_REGION_RANGE habRange;
    public Generation_Solar_System.STAR_REGION_RANGE asteroidBeltRange;


    List<OrbitableInfo> orbitals;
    List<GameObject> orbitalGameobjects; 

    public void SetCelestialBodyInfo(Generation_Solar_System.STARTEMPERATURES_TYPES inBodyType, float inSizeScale, float inPhysicalSize, float inBodyDensity, float inTempmeratureKelvin, bool asteroidBelt)
    {
        mainBodyType = inBodyType; //type mostly will help point to the sprite asset used
        mainBodyUnitySizeScale = inSizeScale;
        mainBodyPhysicalSize = inPhysicalSize;
        mainBodyDensity = inBodyDensity;
        mainBodyTemperatureKelvin = inTempmeratureKelvin;
    }

    public void SetRegionRangeValues(Generation_Solar_System.STAR_REGION_RANGE inHot, Generation_Solar_System.STAR_REGION_RANGE inHab, Generation_Solar_System.STAR_REGION_RANGE inCold, Generation_Solar_System.STAR_REGION_RANGE inAsteroid)
    {
        hotRange = inHot;
        habRange = inHab;
        coldRange = inCold;
        asteroidBeltRange = inAsteroid;
    }

    public void AddOrbitalObject(OrbitableInfo inObject)
    {
        if (!orbitals.Contains(inObject))
        {
            orbitals.Add(inObject);
        }
    }
    
}
