using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarSystemInfo
{
    
    public string systemName;
    public int systemID;
    public float posX;
    public float posY;
    public int generatedSystemSeed;
    public float systemDistanceScaleMod;
    public float systemAUScaleMod;
    public float AUtoSystemScale;

    public Generation_Solar_System.STARTEMPERATURES_TYPES mainBodyType;
    public SolarSystemSize systemSize;
    public SolarSystemType systemType;
    public float mainBodyUnitySizeScale;
    public float mainBodyPhysicalSize;
    public float mainBodyDensity;
    public float mainBodyTemperatureKelvin;



    public bool hasAsteroidBelt;
    public float baseSystemAngle; //for elliptical orbits along same line-ish


    public Generation_Solar_System.Star_Region_Range hotRange;
    public Generation_Solar_System.Star_Region_Range coldRange;
    public Generation_Solar_System.Star_Region_Range habRange;
    public Generation_Solar_System.Star_Region_Range asteroidBeltRange;


    public List<OrbitableInfo> orbitals = new List<OrbitableInfo>();
    public List<GameObject> orbitalGameobjects = new List<GameObject>();


    public float AUFarPlanet = 0.0f; //how far away the farthest planet is, for placing exits. set on gen.

    public int[] linkedSystemIDs;
    public float[] linkedSystemPositionsX;
    public float[] linkedSystemPositionsY;
    public string[] linkedSystemNames;
    public int numberActualLinks;

    public void SetCelestialBodyInfo(Generation_Solar_System.STARTEMPERATURES_TYPES inBodyType, SolarSystemSize inSize, SolarSystemType inSystemType, float inSizeScale, float inPhysicalSize, float inBodyDensity, float inTempmeratureKelvin, bool asteroidBelt, float inBaseAngle)
    {
        mainBodyType = inBodyType; //type mostly will help point to the sprite asset used
        systemSize = inSize;
        systemType = inSystemType;
        mainBodyUnitySizeScale = inSizeScale;
        mainBodyPhysicalSize = inPhysicalSize;
        mainBodyDensity = inBodyDensity;
        mainBodyTemperatureKelvin = inTempmeratureKelvin;
        baseSystemAngle = inBaseAngle;
    }

    public void SetRegionRangeValues(Generation_Solar_System.Star_Region_Range inHot, Generation_Solar_System.Star_Region_Range inHab, Generation_Solar_System.Star_Region_Range inCold, Generation_Solar_System.Star_Region_Range inAsteroid)
    {
        hotRange = inHot;
        habRange = inHab;
        coldRange = inCold;
        asteroidBeltRange = inAsteroid;
    }

    public void SetDistanceScale(float inDistanceScale, float inDistanceAUScale)
    {
        //Debug.Log("setting distance scale  " + inDistanceScale + " " + inDistanceAUScale);
        systemDistanceScaleMod = inDistanceScale;
        systemAUScaleMod = inDistanceAUScale;
    }

    public void AddOrbitalObject(OrbitableInfo inObject)
    {
        //Debug.Log("testing orbital object " + inObject.orbitalName + " " + inObject.GetInstanceID());
        //if (!orbitals.Contains(inObject))
        {
            //Debug.Log("adding new orbital object " + inObject.orbitalName);
            orbitals.Add(inObject);
        }
        if (inObject.distanceFromCenterAU > AUFarPlanet)
        {
            AUFarPlanet = inObject.distanceFromCenterAU;
        }
        //Debug.Log("au far planet set to " + AUFarPlanet + " for " + systemName);
    }
    
}
